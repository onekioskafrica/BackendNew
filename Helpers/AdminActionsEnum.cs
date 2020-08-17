using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OK_OnBoarding.Helpers
{
    public enum AdminActionsEnum
    {
        LOGIN = 1,
        ADMIN_CREATE_ADMIN = 2,
        ADMIN_DEACTIVATE_ADMIN = 3,
        ADMIN_DEACTIVATE_STORE = 4,
        ADMIN_CHANGE_PASSWORD = 5,
        ADMIN_ACTIVATE_STORE = 6,
        ADMIN_ACTIVATE_DELIVERYMAN = 7,
        ADMIN_DEACTIVATE_DELIVERYMAN = 8,
        ADMIN_ACTIVATE_ADMIN = 9,
        ADMIN_ADDED_PRODUCT_CATEGORY = 10,
        ADMIN_ACTIVATE_PRODUCT = 11,
        ADMIN_DEACTIVATE_PRODUCT = 12,
        ADMIN_PUBLISHED_PRODUCT_REVIEW = 13,
        ADMIN_UNPUBLISH_PRODUCT_REVIEW = 14,
        ADMIN_PUBLISHED_STORE_REVIEW = 15,
        ADMIN_UNPUBLISHED_STORE_REVIEW = 16,
    }
}
