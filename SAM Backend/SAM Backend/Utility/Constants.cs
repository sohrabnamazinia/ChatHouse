using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SAM_Backend.Utility
{
    public static class Constants
    {
        public const string CORSPolicyName = "CORSPolicy";
        public const string TokenSignKey = "TokenSignKey";
        public const string RouteName = "RoutePattern";
        public const string RoutePattern = "api/{controller}/{action}/{id?}";
        public const string ConnectionStringKey = "AppDbConnection";
        public const int MaxFailedAccessAttempts = 5;
        public const int DefaultLockoutTimeSpan = 15;
        public const bool RequireConfirmedEmail = true;

    }
}
