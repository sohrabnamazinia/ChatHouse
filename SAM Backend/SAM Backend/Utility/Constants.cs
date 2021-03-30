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
        public const string PasswordAllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
        public const string UsernameAllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-_";
        public const string EmailFailedToConfirmedMessage = "Failed to confirm Email!";
        public const string UserNotFoundError = "User not found!";
        public const int MaxFailedAccessAttempts = 5;
        public const int DefaultLockoutTimeSpan = 15;
        public const bool RequireConfirmedEmail = true;
        public const int InterestCategoriesCount = 14;
        public const int OKStatuseCode = 200;



        public static bool IsAllowedUsername(string username)
        {
            char[] AllowedChars = UsernameAllowedUserNameCharacters.ToCharArray();
            foreach (char c in username)
            {
                if (!AllowedChars.Contains(c)) return false;
            }
            return true;
        }

    }

    
}
