using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OK_OnBoarding.Contracts
{
    public static class ApiRoute
    {
        public const string Version = "v1";
        public const string Root = "api";
        public const string Base = Root + "/" + Version;
        public static class Admin
        {
            public const string Get = Base + "/admins/{id}";
            public const string CreateAdmin = Base + "/admins/create";
            public const string Login = Base + "/admins/login";
            public const string ChangePassword = Base + "/admins/changepassword";
        }

        public static class Customer
        {
            public const string Signup = Base + "/customers/signup";
            public const string Login = Base + "/customers/login";
            public const string Get = Base + "/customers/{id}";
            public const string FacebookAuth = Base + "/customers/auth/fbauth";
            public const string GoogleAuth = Base + "/customers/auth/googleauth";
            public const string EnableCustomerCreation = Base + "/customers/enablecustomercreation";
        }

        public static class StoreOwner
        {
            public const string Signup = Base + "/storeowners/signup";
            public const string Login = Base + "/storeowners/login";
            public const string FacebookAuth = Base + "/storeowners/auth/fbauth";
            public const string GoogleAuth = Base + "/storeowners/auth/googleauth";
            public const string EnableStoreOwnerCreation = Base + "/storeowners/enablestoreownercreation";
        }

        public static class Deliveryman
        {
            public const string Signup = Base + "/deliverymen/signup";
            public const string Login = Base + "/deliverymen/login";
            public const string FacebookAuth = Base + "/deliverymen/auth/fbauth";
            public const string GoogleAuth = Base + "/deliverymen/auth/googleauth";
            public const string EnableDeliverymanCreation = Base + "/deliverymen/enabledeliverymancreation";
        }

        public static class SuperAdmin
        {
            public const string CreateSuperAdmin = Base + "/superadmin/create";
            public const string ChangePassword = Base + "/superadmin/changepassword";
            public const string Login = Base + "/superadmin/login";
            public const string CreateAdmin = Base + "/superadmin/createadmin";
            public const string DeactivateAdmin = Base + "/superadmin/deactivateadmin";
            public const string GetAllPrivileges = Base + "superadmin/getallprivileges";
            public const string GetAdminById = Base + "superadmin/getadminbyid";
            public const string GetAllAdmins = Base + "superadmin/getalladmins";
            public const string GetAllAdminsActivityLogs = Base + "superadmin/getalladminsactivitylogs";
            public const string GetSingleAdminAcitvityLogs = Base + "superadmin/getsingleadminsacitivitylogs";
        }
    }
}
