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
        }

        public static class Customer
        {
            public const string Signup = Base + "/customers/signup";
            public const string Login = Base + "/customers/login";
            public const string Get = Base + "/customers/{id}";
            public const string FacebookAuth = Base + "/customers/auth/fbauth";
            public const string GoogleAuth = Base + "/customers/auth/googleauth";
        }

        public static class StoreOwner
        {
            public const string Signup = Base + "/storeowners/signup";
            public const string Login = Base + "/storeowners/login";
            public const string FacebookAuth = Base + "/storeowners/auth/fbauth";
            public const string GoogleAuth = Base + "/storeowners/auth/googleauth";
        }
    }
}
