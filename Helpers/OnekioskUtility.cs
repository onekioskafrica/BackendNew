using OK_OnBoarding.Contracts.V1.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OK_OnBoarding.Helpers
{
    public class OnekioskUtility
    {
        public GenericResponse ValidateDiscountInputs(bool IsPercentageDiscount, bool IsAmountDiscount, bool IsSetPrice, decimal PercentageDiscount)
        {
            if (IsPercentageDiscount && IsAmountDiscount && IsSetPrice)
                return new GenericResponse { Status = false, Message = "IsPercentageDiscount, IsAmountDiscount and IsSetPrice cannot all be set to true or any both of these cannot be true." };
            if (IsPercentageDiscount && IsAmountDiscount)
                return new GenericResponse { Status = false, Message = "IsPercentageDiscount, IsAmountDiscount and IsSetPrice cannot all be set to true or any both of these cannot be true." };
            if (IsPercentageDiscount && IsSetPrice)
                return new GenericResponse { Status = false, Message = "IsPercentageDiscount, IsAmountDiscount and IsSetPrice cannot all be set to true or any both of these cannot be true." };
            if (IsAmountDiscount && IsSetPrice)
                return new GenericResponse { Status = false, Message = "IsPercentageDiscount, IsAmountDiscount and IsSetPrice cannot all be set to true or any both of these cannot be true." };

            if (IsPercentageDiscount && PercentageDiscount <= 0)
                return new GenericResponse { Status = false, Message = "PercentageDiscount must be greater than zero" };

            if (IsPercentageDiscount && PercentageDiscount > 100)
                return new GenericResponse { Status = false, Message = "PercentageDiscount must be less than zero" };

            return new GenericResponse { Status = true, Message = "Success" };
        }

    }
}
