using AutoMapper;
using GeoCoordinatePortable;
using Microsoft.EntityFrameworkCore;
using OK_OnBoarding.Contracts.V1.Requests;
using OK_OnBoarding.Contracts.V1.Responses;
using OK_OnBoarding.Data;
using OK_OnBoarding.Domains;
using OK_OnBoarding.Entities;
using OK_OnBoarding.Helpers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;

namespace OK_OnBoarding.Services
{
    public class CartService : ICartService
    {
        private readonly DataContext _dataContext;
        private readonly IMapper _mapper;
        private readonly AppSettings _appSettings;

        public CartService(DataContext dataContext, IMapper mapper, AppSettings appSettings)
        {
            _dataContext = dataContext;
            _mapper = mapper;
            _appSettings = appSettings;
        }

        public async Task<GenericResponse> AddCartItemAsync(AddCartItemRequest request)
        {
            var cartExist = await _dataContext.Carts.Include(c => c.CartItems).FirstOrDefaultAsync(c => c.Id == request.CartId && c.SessionId == request.SessionId);
            if (cartExist == null)
                return new GenericResponse { Status = false, Message = "Invalid Cart" };

            var productExist = await _dataContext.Products.Include(p => p.ProductPricing).FirstOrDefaultAsync(p => p.Id == request.ProductId);
            if (productExist == null)
                return new GenericResponse { Status = false, Message = "Invalid Product" };
            if (productExist.StoreId != request.StoreId)
                return new GenericResponse { Status = false, Message = "Product doesn't belong to store." };
            if (productExist.InStock <= 0)
                return new GenericResponse { Status = false, Message = "Product is out of stock." };
            if (request.Quantity > productExist.InStock)
                return new GenericResponse { Status = false, Message = $"{productExist.InStock} units are left in stock." };

            // Check if product is already in cart
            var productIdsInCart = cartExist.CartItems.Select(c => c.ProductId).ToArray<Guid>();
            if (productIdsInCart.Contains(request.ProductId))
            {
                var updateCartResponse = await UpdateCartItemInCart(request.CartId, request.ProductId, request.Quantity);
                return updateCartResponse;
            }

            var response = await AddNewCartItemToCart(request, productExist);
            if (!response.Status)
                return response;

            return new GenericResponse { Status = true, Message = "Item added successfully.", Data = response.Data };
        }

        public async Task<GenericResponse> UpdateCartItemInCart(int cartId, Guid productId, int quantity)
        {
            var cartItemExist = await _dataContext.CartItems.FirstOrDefaultAsync(c => c.CartId == cartId && c.ProductId == productId);

            cartItemExist.Quantity = quantity;

            cartItemExist.Total = cartItemExist.Quantity * cartItemExist.Price;
            cartItemExist.UpdatedAt = DateTime.Now;
            cartItemExist.IsActive = true;

            _dataContext.Entry(cartItemExist).State = EntityState.Modified;
            var updated = 0;
            try
            {
                updated = await _dataContext.SaveChangesAsync();
            }
            catch (Exception)
            {
                return new GenericResponse { Status = false, Message = "Error Occurred." };
            }
            if (updated <= 0)
                return new GenericResponse { Status = false, Message = "Couldn't add item to cart." };

            return new GenericResponse { Status = true, Message = "Item added successfully.", Data = cartItemExist };
        }

        public async Task<GenericResponse> AddNewCartItemToCart(AddCartItemRequest request, Product product)
        {
            var newCartItem = new CartItem
            {
                ProductId = request.ProductId,
                StoreId = request.StoreId,
                CartId = request.CartId,
                SKU = product.ProductPricing.SellerSku,
                Quantity = request.Quantity,
                IsActive = true,
                CreatedAt = DateTime.Now
            };
            if (product.ProductPricing.IsSalePriceSet == true && (DateTime.Now >= product.ProductPricing.SaleStartDate || DateTime.Now <= product.ProductPricing.SaleEndDate))
            {
                newCartItem.Price = product.ProductPricing.SalePrice;
            }
            else
            {
                newCartItem.Price = product.ProductPricing.Price;
            }
            newCartItem.Total = newCartItem.Quantity * newCartItem.Price;
            await _dataContext.CartItems.AddAsync(newCartItem);
            var added = 0;
            try
            {
                added = await _dataContext.SaveChangesAsync();
            }
            catch (Exception)
            {
                return new GenericResponse { Status = false, Message = "Error Occurred." };
            }
            if (added <= 0)
                return new GenericResponse { Status = false, Message = "Couldn't add item to cart." };

            return new GenericResponse { Status = true, Data = newCartItem };
        }

        public async Task<GenericResponse> CheckoutAsync(CheckoutRequest request)
        {
            var cartExist = await _dataContext.Carts.Include(c => c.CartItems).FirstOrDefaultAsync(c => c.Id == request.CartId && c.SessionId == request.SessionId);
            if (cartExist == null)
                return new GenericResponse { Status = false, Message = "Invalid Cart" };
            if (cartExist.Status == CartStatusEnum.ABANDONED.ToString() || cartExist.Status == CartStatusEnum.COMPLETE.ToString() || cartExist.Status == CartStatusEnum.PAID.ToString())
                return new GenericResponse { Status = false, Message = "Cannot checkout this cart." };
            
            var cartItemsInsideCart = cartExist.CartItems.Where(c => c.IsActive).ToList();
            if (cartItemsInsideCart.Count == 0)
                return new GenericResponse { Status = false, Message = "Cart is empty" };

            var totalPrice = cartItemsInsideCart.Sum(c => c.Total);

            var locationForDelivery = new Location { Latitude = request.Latitude, Longitude = request.Longitude };
            CheckoutResponse checkoutResponse = null;
            //Tax ???
            //Shipping
            var undiscountedShipping = await CalculateUndiscountedShipping(cartItemsInsideCart, locationForDelivery);
            var totalDiscount = await ComputeTotalDiscount(cartItemsInsideCart, undiscountedShipping, request.CouponCode);
            if (cartExist.Status == CartStatusEnum.CHECKOUT.ToString())
            {
                var modifyOrderResponse = await ModifyOrder(request, totalPrice, undiscountedShipping, totalDiscount, cartItemsInsideCart, totalDiscount.DiscountsPerStoreList);
                if (!modifyOrderResponse.Status)
                    return modifyOrderResponse;
                checkoutResponse = (CheckoutResponse) modifyOrderResponse.Data;
            }
            else
            {
                var createOrderResponse = await CreateOrder(cartExist, totalPrice, undiscountedShipping, request, totalDiscount, cartItemsInsideCart, totalDiscount.DiscountsPerStoreList);
                if (!createOrderResponse.Status)
                    return createOrderResponse;
                checkoutResponse = (CheckoutResponse)createOrderResponse.Data;
            }

            checkoutResponse.CartItems = cartItemsInsideCart;

            return new GenericResponse { Status = true, Message = "Checkout Successful", Data = checkoutResponse };
        }

        public async Task<GenericResponse> CreateCartAsync(Guid CustomerId, string SessionId)
        {
            var customerExist = await _dataContext.Customers.FirstOrDefaultAsync(c => c.CustomerId == CustomerId);
            if (customerExist == null)
                return new GenericResponse { Status = false, Message = "Invalid Customer" };
            if (!customerExist.IsVerified)
                return new GenericResponse { Status = false, Message = "Customer is not verified." };

            var cartExists = await _dataContext.Carts.FirstOrDefaultAsync(c => c.CustomerId == CustomerId && c.SessionId == SessionId);
            if (cartExists != null)
                return new GenericResponse { Status = false, Message = "Cart already exists." };

            var newCart = new Cart {
                CustomerId = CustomerId,
                SessionId = SessionId,
                Token = SessionId,
                Status = CartStatusEnum.NEW.ToString(),
                CreatedAt = DateTime.Now
            };
            await _dataContext.Carts.AddAsync(newCart);
            var created = 0;
            try
            {
                created = await _dataContext.SaveChangesAsync();
            }
            catch (Exception)
            {
                return new GenericResponse { Status = false, Message = "Error Occurred." };
            }
            if (created <= 0)
                return new GenericResponse { Status = false, Message = "Couldn't create cart." };

            newCart.Customer = null;

            return new GenericResponse { Status = true, Message = "Cart Created", Data = newCart };
        }

        public async Task<GenericResponse> GetCartAsync(int cartId)
        {
            var cartExist = await _dataContext.Carts.FirstOrDefaultAsync(c => c.Id == cartId);
            if (cartExist == null)
                return new GenericResponse { Status = false, Message = "Invalid Cart Id" };
            var cartItems = await _dataContext.CartItems.Include(c => c.Product).Include(c => c.Store).Where(c => c.CartId == cartId && c.IsActive).ToListAsync();
            cartExist.CartItems = cartItems;
            foreach(var cartItem in cartItems)
            {
                cartItem.Product.Store = null;
                cartItem.Store.CartItems = null;
                cartItem.Store.StoreReviews = null;
                cartItem.Store.Products = null;
            }
            return new GenericResponse { Status = true, Message = "Success", Data = cartExist };
        }

        public async Task<GenericResponse> ClearCartAsync(int cartId)
        {
            var cartExist = await _dataContext.Carts.FirstOrDefaultAsync(c => c.Id == cartId);
            if (cartExist == null)
                return new GenericResponse { Status = false, Message = "Invalid Cart Id" };
            if(cartExist.Status == CartStatusEnum.NEW.ToString())
            {
                _dataContext.Carts.Remove(cartExist);
                var cartItems = await _dataContext.CartItems.Where(c => c.CartId == cartId).ToListAsync<CartItem>();
                _dataContext.CartItems.RemoveRange(cartItems);

                var deleted = 0;
                try
                {
                    deleted = await _dataContext.SaveChangesAsync();
                }
                catch (Exception)
                {
                    return new GenericResponse { Status = false, Message = "Error Occurred" };
                }
                if (deleted <= 0)
                    return new GenericResponse { Status = false, Message = "Couldn't Clear Cart" };

                return new GenericResponse { Status = true, Message = "Cart Cleared." };
            }
            else if(cartExist.Status == CartStatusEnum.CHECKOUT.ToString())
            {
                var cartItems = await _dataContext.CartItems.Where(c => c.CartId == cartId).ToListAsync<CartItem>();
                _dataContext.CartItems.RemoveRange(cartItems);

                var orderAssociatedWithCheckout = await _dataContext.Orders.FirstOrDefaultAsync(o => o.SessionId == cartExist.SessionId);
                if(orderAssociatedWithCheckout != null)
                {
                    _dataContext.Orders.Remove(orderAssociatedWithCheckout);
                }
                var paymentAssociatedWithOrder = await _dataContext.Payments.Where(p => p.SessionId == cartExist.SessionId).ToListAsync<Payment>();
                _dataContext.Payments.RemoveRange(paymentAssociatedWithOrder);

                var deleted = 0;
                try
                {
                    deleted = await _dataContext.SaveChangesAsync();
                }
                catch (Exception)
                {
                    return new GenericResponse { Status = false, Message = "Error Occurred" };
                }
                if (deleted <= 0)
                    return new GenericResponse { Status = false, Message = "Couldn't Clear Cart" };

                return new GenericResponse { Status = true, Message = "Cart Cleared." };
            }
            else
            {
                return new GenericResponse { Status = false, Message = "Can't clear this cart." };
            }
          
        }

        public async Task<GenericResponse> RemoveCartItemAsync(int cartItemId)
        {
            var cartItemExist = await _dataContext.CartItems.FirstOrDefaultAsync(c => c.Id == cartItemId);
            if (cartItemExist == null)
                return new GenericResponse { Status = false, Message = "Invalid Cart Item Id" };

            cartItemExist.IsActive = false;
            _dataContext.Entry(cartItemExist).State = EntityState.Modified;
            var updated = 0;
            try
            {
                updated = await _dataContext.SaveChangesAsync();
            }
            catch (Exception)
            {
                return new GenericResponse { Status = false, Message = "Error Occurred." };
            }
            if (updated <= 0)
                return new GenericResponse { Status = false, Message = "Unable to remove item." };

            return new GenericResponse { Status = true, Message = "Item removed successfully." };
        }

        public async Task<GenericResponse> GetOrderAsync(string sessionId)
        {
            Order orderExist = null;
            try
            {
                orderExist = await _dataContext.Orders.FirstOrDefaultAsync(o => o.SessionId == sessionId);
            }
            catch (Exception)
            {
                return new GenericResponse { Status = false, Message = "Error Occurred." };
            }
            
            if (orderExist == null)
                return new GenericResponse { Status = false, Message = "Invalid Order Id" };

            return new GenericResponse { Status = true, Message = "Success", Data = orderExist };
        }

        public async Task<List<Order>> GetAllCustomerOrdersAsync(Guid CustomerId, PaginationFilter paginationFilter = null)
        {
            List<Order> allCustomerOrders = null;
            if(paginationFilter == null)
            {
                allCustomerOrders = await _dataContext.Orders.Where(o => o.CustomerId == CustomerId).ToListAsync<Order>();
            }
            else
            {
                var skip = (paginationFilter.PageNumber - 1) * paginationFilter.PageSize;
                allCustomerOrders = await _dataContext.Orders.Where(o => o.CustomerId == CustomerId).Skip(skip).Take(paginationFilter.PageSize).ToListAsync<Order>();
            }
            return allCustomerOrders;
        }

        public async Task<List<Cart>> GetAllCustomerCartsAsync(Guid CustomerId, PaginationFilter paginationFilter = null)
        {
            List<Cart> allCustomerCarts = null;
            if (paginationFilter == null)
            {
                allCustomerCarts = await _dataContext.Carts.Where(c => c.CustomerId == CustomerId).ToListAsync<Cart>();
            }
            else
            {
                var skip = (paginationFilter.PageNumber - 1) * paginationFilter.PageSize;
                allCustomerCarts = await _dataContext.Carts.Where(c => c.CustomerId == CustomerId).Skip(skip).Take(paginationFilter.PageSize).ToListAsync<Cart>();
            }
            return allCustomerCarts;
        }


        #region HELPERS

        public async Task<GenericResponse> CreateOrder(Cart cartExist, decimal totalPrice, decimal undiscountedShipping, CheckoutRequest request, TotalDiscount totalDiscount, List<CartItem> cartItemsInsideCart, List<StoreDiscountDetail> storeDiscountDetails)
        {
            var order = new Order
            {
                CustomerId = cartExist.CustomerId,
                SessionId = cartExist.SessionId,
                Token = cartExist.Token,
                Status = OrderStatusEnum.NEW.ToString(),
                SubTotal = totalPrice,
                Shipping = undiscountedShipping,
                Total = undiscountedShipping + totalPrice,
                Promo = request.CouponCode,
                StoreOwnerShippingDiscount = totalDiscount.TotalStoreOwnerShippingDiscount,
                StoreOwnerPriceDiscount = totalDiscount.TotalStoreOwnerPriceDiscount,
                OnekioskPriceDiscount = totalDiscount.TotalOnekioskPriceDiscount,
                OnekioskShippingDiscount = totalDiscount.TotalOnekioskShippingDiscount,
                ShippingDiscount = totalDiscount.TotalShippingDiscount,
                PriceDiscount = totalDiscount.TotalPriceDiscount,
                Discount = totalDiscount.TotalPriceDiscount + totalDiscount.TotalShippingDiscount,
                GrandTotal = totalPrice - (totalDiscount.TotalShippingDiscount + totalDiscount.TotalPriceDiscount),
                Line1 = request.Line1,
                City = request.City,
                State = request.State,
                Country = request.Country,
                CreatedAt = DateTime.Now,
            };

            await _dataContext.Orders.AddAsync(order);

            var payments = new List<Payment>();
            foreach (var cartItem in cartItemsInsideCart)
            {
                var storeDiscountOnItem = storeDiscountDetails.FirstOrDefault(s => s.CartItemId == cartItem.Id);
                var payment = new Payment()
                {
                    CartItemId = cartItem.Id,
                    SessionId = order.SessionId,
                    StoreId = cartItem.StoreId,
                    Total = cartItem.Total,
                    StoreDiscountOnPrice = storeDiscountOnItem.DiscountOnPrice,
                    StoreDiscountOnShipping = storeDiscountOnItem.DiscountOnShipping,
                    IsSettled = false,
                    AmountPaidToOneKiosk = 0.00M,
                    AmountPaidToStore = 0.00M
                };
                payments.Add(payment);
            }
            await _dataContext.Payments.AddRangeAsync(payments);

            cartExist.Status = CartStatusEnum.CHECKOUT.ToString();
            cartExist.UpdatedAt = DateTime.Now;

            _dataContext.Entry(cartExist).State = EntityState.Modified;

            var created = 0;
            try
            {
                created = await _dataContext.SaveChangesAsync();
            }
            catch (Exception)
            {
                return new GenericResponse { Status = false, Message = "Error Occurred." };
            }
            if (created <= 0)
                return new GenericResponse { Status = false, Message = "Couldn't Checkout." };

            var checkoutResponse = _mapper.Map<CheckoutResponse>(order);
            return new GenericResponse { Status = true, Message = "Order Created", Data = checkoutResponse };
        }

        public async Task<GenericResponse> ModifyOrder(CheckoutRequest request, decimal totalPrice, decimal undiscountedShipping, TotalDiscount totalDiscount, List<CartItem> cartItemsInsideCart, List<StoreDiscountDetail> storeDiscountDetails)
        {
            var orderExist = await _dataContext.Orders.FirstOrDefaultAsync(o => o.SessionId == request.SessionId);
            orderExist.SubTotal = totalPrice;
            orderExist.Shipping = undiscountedShipping;
            orderExist.Total = undiscountedShipping + totalPrice;
            orderExist.Promo = request.CouponCode;
            orderExist.StoreOwnerShippingDiscount = totalDiscount.TotalStoreOwnerShippingDiscount;
            orderExist.StoreOwnerPriceDiscount = totalDiscount.TotalStoreOwnerPriceDiscount;
            orderExist.OnekioskPriceDiscount = totalDiscount.TotalOnekioskPriceDiscount;
            orderExist.OnekioskShippingDiscount = totalDiscount.TotalOnekioskShippingDiscount;
            orderExist.ShippingDiscount = totalDiscount.TotalShippingDiscount;
            orderExist.PriceDiscount = totalDiscount.TotalPriceDiscount;
            orderExist.Discount = totalDiscount.TotalPriceDiscount + totalDiscount.TotalShippingDiscount;
            orderExist.GrandTotal = totalPrice - (totalDiscount.TotalShippingDiscount + totalDiscount.TotalPriceDiscount);
            orderExist.Line1 = request.Line1;
            orderExist.City = request.City;
            orderExist.State = request.State;
            orderExist.Country = request.Country;

            _dataContext.Entry(orderExist).State = EntityState.Modified;

            var paymentExist = await _dataContext.Payments.Where(p => p.SessionId == request.SessionId).ToListAsync<Payment>();
            _dataContext.Payments.RemoveRange(paymentExist);


            var payments = new List<Payment>();
            foreach (var cartItem in cartItemsInsideCart)
            {
                var storeDiscountOnItem = storeDiscountDetails.FirstOrDefault(s => s.CartItemId == cartItem.Id);
                var payment = new Payment()
                {
                    CartItemId = cartItem.Id,
                    SessionId = orderExist.SessionId,
                    StoreId = cartItem.StoreId,
                    Total = cartItem.Total,
                    StoreDiscountOnPrice = storeDiscountOnItem.DiscountOnPrice,
                    StoreDiscountOnShipping = storeDiscountOnItem.DiscountOnShipping,
                    IsSettled = false,
                    AmountPaidToOneKiosk = 0.00M,
                    AmountPaidToStore = 0.00M
                };
                payments.Add(payment);
            }
            await _dataContext.Payments.AddRangeAsync(payments);

            var modified = 0;
            try
            {
                modified = await _dataContext.SaveChangesAsync();
            }
            catch (Exception)
            {
                return new GenericResponse { Status = false, Message = "Error Occurred." };
            }
            if (modified <= 0)
                return new GenericResponse { Status = false, Message = "Couldn't Checkout." };

            var checkoutResponse = _mapper.Map<CheckoutResponse>(orderExist);

            return new GenericResponse { Status = true, Message = "Modified", Data = checkoutResponse };
        }

        public async Task<TotalDiscount> ComputeTotalDiscount(List<CartItem> cartItems, decimal undiscountedShipping, string couponCode)
        {
            var StoreDiscounts = new List<StoreDiscountDetail>();

            var totalStoreOwnerConfiguredShippingDiscount = 0.00M;
            var totalStoreOwnerConfiguredPriceDiscount = 0.00M;

            var totalAdminConfiguredShippingDiscount = 0.00M;
            var totalAdminConfiguredPriceDiscount = 0.00M;

            foreach (var cartItem in cartItems)
            {
                var storeOwnerDiscountResponse = await GetDiscountConfiguredByStoreOwner(couponCode, cartItem, undiscountedShipping);
                totalStoreOwnerConfiguredShippingDiscount = totalStoreOwnerConfiguredShippingDiscount + storeOwnerDiscountResponse.DiscountOnShipping;
                totalStoreOwnerConfiguredPriceDiscount = totalStoreOwnerConfiguredPriceDiscount + storeOwnerDiscountResponse.DiscountOnPrice;

                var storeDiscount = new StoreDiscountDetail
                {
                    CartItemId = cartItem.Id,
                    StoreId = cartItem.StoreId,
                    DiscountOnPrice = totalStoreOwnerConfiguredPriceDiscount,
                    DiscountOnShipping = totalStoreOwnerConfiguredShippingDiscount
                };
                StoreDiscounts.Add(storeDiscount);

                var adminConfiguredDiscountResponse = await GetDiscountConfiguredByAdmin(cartItem, undiscountedShipping);
                totalAdminConfiguredShippingDiscount = totalAdminConfiguredShippingDiscount + adminConfiguredDiscountResponse.DiscountOnShipping;
                totalAdminConfiguredPriceDiscount = totalAdminConfiguredPriceDiscount + adminConfiguredDiscountResponse.DiscountOnPrice;
            }

            var totalShippingDiscount = totalStoreOwnerConfiguredShippingDiscount + totalAdminConfiguredShippingDiscount;
            var totalPriceDiscount = totalStoreOwnerConfiguredPriceDiscount + totalAdminConfiguredPriceDiscount;

            return new TotalDiscount
            {
                TotalShippingDiscount = totalShippingDiscount,
                TotalPriceDiscount = totalPriceDiscount,
                TotalOnekioskShippingDiscount = totalAdminConfiguredShippingDiscount,
                TotalOnekioskPriceDiscount = totalAdminConfiguredPriceDiscount,
                TotalStoreOwnerShippingDiscount = totalStoreOwnerConfiguredShippingDiscount,
                TotalStoreOwnerPriceDiscount = totalStoreOwnerConfiguredPriceDiscount,
                DiscountsPerStoreList = StoreDiscounts
            };
        }

        public async Task<decimal> CalculateUndiscountedShipping(List<CartItem> cartItems, Location location)
        {
            var theIDsOfStoresPurchasedFrom = cartItems.Select(c => c.StoreId).Distinct().ToList<Guid>();
            var pickupStoreID = theIDsOfStoresPurchasedFrom.ElementAt(0);
            /*
             * Get the latitude and longitude of the pickupStore.
             * Calculate the distance between the pickupStore and parameters - latitude and longitude.
             * 
             */
            var pickupStore = await _dataContext.StoresBusinessInformation.FirstOrDefaultAsync(s => s.StoreId == pickupStoreID);
            var shippingPerKMInKobo = _appSettings.ShippingPerKmInKobo;
            var distanceInKM = 0.0;
            if (pickupStore != null)
            {
                var latitude = pickupStore.Latitude;
                var longitude = pickupStore.Longitude;

                var purchaseLocation = new GeoCoordinate(location.Latitude, location.Longitude);
                var pickupLocation = new GeoCoordinate(latitude, longitude);

                var distanceInMetres = purchaseLocation.GetDistanceTo(pickupLocation);
                distanceInKM = distanceInMetres / 1000;
            }
            var shippingBasedOnLocation = (Convert.ToDecimal(distanceInKM) * shippingPerKMInKobo)/100; //in Naira.
            var extraShippingForBuyingFromMultipleStores = _appSettings.ExtraShippingForBuyingFromMultipleStores;

            var numberOfStoresPurchasedFrom = theIDsOfStoresPurchasedFrom.Count;
            var numberOfExtraShippingIncurred = numberOfStoresPurchasedFrom - 1;

            var totalExtraShipping = numberOfExtraShippingIncurred * extraShippingForBuyingFromMultipleStores;

            return shippingBasedOnLocation + totalExtraShipping;
        }

        public async Task<DiscountResponse> GetDiscountConfiguredByStoreOwner(string couponCode, CartItem cartItem, decimal shipping)
        {
            var discountOnShipping = 0.00M;
            var discountOnPrice = 0.00M;
            var todayDate = DateTime.Now;
            var couponExist = await _dataContext.Coupons.FirstOrDefaultAsync(c => c.Code == couponCode && c.IsActive && todayDate >= c.StartDate && todayDate <= c.EndDate);

            var priceOfProduct = cartItem.Total;

            //Invalid Coupon
            if (couponExist == null)
                return new DiscountResponse { DiscountOnShipping = discountOnShipping, DiscountOnPrice = discountOnPrice };

            if (couponExist.IsSlotSet && couponExist.SlotUsed > couponExist.AllocatedSlot)
                return new DiscountResponse { DiscountOnShipping = discountOnShipping, DiscountOnPrice = discountOnPrice };

            var product = await _dataContext.Products.Include(p => p.ProductPricing).FirstOrDefaultAsync(p => p.Id == cartItem.ProductId);

            if (couponExist.IsForAllProducts && product.StoreId == couponExist.StoreId)
            {
                if (couponExist.IsForShipping)
                {
                    discountOnShipping = discountOnShipping + ComputeDiscountOnShipping(couponExist, shipping);
                }
                if (couponExist.IsForPrice)
                {
                    discountOnPrice = discountOnPrice + ComputeDiscountOnPrice(couponExist, priceOfProduct);
                }
                // Increment SlotUsed in the case of AllocatedSlot Coupon
                if (couponExist.IsSlotSet)
                {
                    await IncrementSlotUsed(couponExist);
                }
            }
            if (couponExist.IsForCategory && product.StoreId == couponExist.StoreId)
            {
                if (product.ProductCategories.Select(pc => pc.CategoryId).Contains(couponExist.CategoryId))
                {
                    if (couponExist.IsForShipping)
                    {
                        discountOnShipping = discountOnShipping + ComputeDiscountOnShipping(couponExist, shipping);
                    }
                    if (couponExist.IsForPrice)
                    {
                        discountOnPrice = discountOnPrice + ComputeDiscountOnPrice(couponExist, priceOfProduct);
                    }
                    // Increment SlotUsed in the case of AllocatedSlot Coupon
                    if (couponExist.IsSlotSet)
                    {
                        await IncrementSlotUsed(couponExist);
                    }
                }
            }
            if (couponExist.IsForProduct & product.StoreId == couponExist.StoreId)
            {
                if (product.Id == couponExist.ProductId)
                {
                    if (couponExist.IsForShipping)
                    {
                        discountOnShipping = discountOnShipping + ComputeDiscountOnShipping(couponExist, shipping);
                    }
                    if (couponExist.IsForPrice)
                    {
                        discountOnPrice = discountOnPrice + ComputeDiscountOnPrice(couponExist, priceOfProduct);
                    }
                    // Increment SlotUsed in the case of AllocatedSlot Coupon
                    if (couponExist.IsSlotSet)
                    {
                        await IncrementSlotUsed(couponExist);
                    }
                }
            }

            return new DiscountResponse { DiscountOnPrice = discountOnPrice, DiscountOnShipping = discountOnShipping };
        }

        public async Task<DiscountResponse> GetDiscountConfiguredByAdmin(CartItem cartItem, decimal shipping)
        {
            var discountOnShipping = 0.00M;
            var discountOnPrice = 0.00M;
            var todayDate = DateTime.Now;
            var couponsExist = await _dataContext.Coupons.Where(c => c.IsAdminConfigured && c.IsActive && todayDate >= c.StartDate && todayDate <= c.EndDate).ToListAsync<Coupon>();

            //No Discount
            if (couponsExist == null)
                return new DiscountResponse { DiscountOnShipping = discountOnShipping, DiscountOnPrice = discountOnPrice };

            //Remove Coupons with exceeded Allocated Slot
            RemoveCouponWithExceededAllocatedSlots(couponsExist);

            //After removing coupons with exceeded Allocated Slot, list of coupons might be empty.
            if (couponsExist.Count == 0)
                return new DiscountResponse { DiscountOnShipping = discountOnShipping, DiscountOnPrice = discountOnPrice };

            var priceOfProduct = cartItem.Total;

            var product = await _dataContext.Products.Include(p => p.ProductPricing).FirstOrDefaultAsync(p => p.Id == cartItem.ProductId);

            foreach (var coupon in couponsExist)
            {
                if (coupon.IsForAllProducts)
                {
                    if (coupon.IsForShipping)
                    {
                        discountOnShipping = discountOnShipping + ComputeDiscountOnShipping(coupon, shipping);
                    }
                    if (coupon.IsForPrice)
                    {
                        discountOnPrice = discountOnPrice + ComputeDiscountOnPrice(coupon, priceOfProduct);
                    }
                    // Increment SlotUsed in the case of AllocatedSlot Coupon
                    if (coupon.IsSlotSet)
                    {
                        await IncrementSlotUsed(coupon);
                    }
                }
                if (coupon.IsForCategory)
                {
                    if (product.ProductCategories.Select(pc => pc.CategoryId).Contains(coupon.CategoryId))
                    {
                        if (coupon.IsForShipping)
                        {
                            discountOnShipping = discountOnShipping + ComputeDiscountOnShipping(coupon, shipping);
                        }
                        if (coupon.IsForPrice)
                        {
                            discountOnPrice = discountOnPrice + ComputeDiscountOnPrice(coupon, priceOfProduct);
                        }
                        // Increment SlotUsed in the case of AllocatedSlot Coupon
                        if (coupon.IsSlotSet)
                        {
                            await IncrementSlotUsed(coupon);
                        }
                    }
                }
                if (coupon.IsForProduct)
                {
                    if (product.Id == coupon.ProductId)
                    {
                        if (coupon.IsForShipping)
                        {
                            discountOnShipping = discountOnShipping + ComputeDiscountOnShipping(coupon, shipping);
                        }
                        if (coupon.IsForPrice)
                        {
                            discountOnPrice = discountOnPrice + ComputeDiscountOnPrice(coupon, priceOfProduct);
                        }
                        // Increment SlotUsed in the case of AllocatedSlot Coupon
                        if (coupon.IsSlotSet)
                        {
                            await IncrementSlotUsed(coupon);
                        }
                    }
                }
            }
            return new DiscountResponse { DiscountOnPrice = discountOnPrice, DiscountOnShipping = discountOnShipping };
        }

        public decimal ComputeDiscountOnShipping(Coupon coupon, decimal shipping)
        {
            var discountOnShipping = 0.00M;
            if (coupon.IsPercentageDiscount)
            {
                var percentageDiscount = (coupon.PercentageDiscount * shipping) / 100;
                discountOnShipping = percentageDiscount;
            }
            if (coupon.IsAmountDiscount)
            {
                discountOnShipping = shipping >= coupon.AmountDiscount ? coupon.AmountDiscount : shipping;
            }
            if (coupon.IsSetPrice)
            {
                discountOnShipping = shipping >= coupon.SetPrice ? coupon.SetPrice : shipping;
            }
            return discountOnShipping;
        }

        public decimal ComputeDiscountOnPrice(Coupon coupon, decimal priceOfProduct)
        {
            var discountOnPrice = 0.00M;
            if (coupon.IsPercentageDiscount)
            {
                var percentageDiscount = (coupon.PercentageDiscount * priceOfProduct) / 100;
                discountOnPrice = percentageDiscount;
            }
            if (coupon.IsAmountDiscount)
            {
                discountOnPrice = priceOfProduct >= coupon.AmountDiscount ? coupon.AmountDiscount : priceOfProduct;
            }
            if (coupon.IsSetPrice)
            {
                discountOnPrice = priceOfProduct >= coupon.SetPrice ? coupon.SetPrice : priceOfProduct;
            }
            return discountOnPrice;
        }

        public void RemoveCouponWithExceededAllocatedSlots(List<Coupon> couponsExist)
        {
            var numberOfCoupons = couponsExist.Count;
            for (int i = 0; i < numberOfCoupons; i++)
            {
                if (couponsExist[i].IsSlotSet && (couponsExist[i].SlotUsed > couponsExist[i].AllocatedSlot))
                    couponsExist.RemoveAt(i);
            }
        }

        public async Task<int> IncrementSlotUsed(Coupon coupon)
        {
            coupon.SlotUsed = coupon.SlotUsed + 1;
            _dataContext.Entry(coupon).State = EntityState.Modified;
            var updated = 0;
            try
            {
                updated = await _dataContext.SaveChangesAsync();
            }
            catch (Exception)
            {
                return 0;
            }
            return updated;
        }

        #endregion
    }
}
