using System;
using System.Collections.Generic;
using System.IO;
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
        public const string RoomNotFound = "Room not found!";
        public const string InterestsRoomFormatError = "Interests list is not in a valid format for a Room";
        public const string MinIOBucketUsers = "users";
        public const string MinIOHostAddress = "45.82.139.208:9000";
        public const string ChatRoomHubRoute = "/Hubs/ChatRoom";
        public static readonly string SMTPGoogleDomain = "smtp.gmail.com";
        public static readonly string ProjectEmail = "GoNuts.ChatHouse@gmail.com";
        public static readonly string ConfirmAccountRegisterationViewPath = Path.DirectorySeparatorChar.ToString() + "Templates" + Path.DirectorySeparatorChar.ToString() + "EmailTemplate" + Path.DirectorySeparatorChar.ToString() + "EmailConfirmation.html";
        public static readonly string ProjectSender = "ChatHouse Team";
        public static readonly string ProjectReciever = "User";
        public static readonly string EmailConfirmationSubject = "Confirm your Email";
        public static readonly int SMTPPort = 587;
        public static readonly byte[] png = new byte[] { 137, 80, 78, 71 };
        public static readonly byte[] tiff = new byte[] { 73, 73, 42 };
        public static readonly byte[] tiff2 = new byte[] { 77, 77, 42 };
        public static readonly byte[] jpeg = new byte[] { 255, 216, 255, 224 };
        public static readonly byte[] jpeg2 = new byte[] { 255, 216, 255, 225 };
        public static readonly byte[] jpg = new byte[] { 255, 216, 255, 219 };
        public static readonly string jfif;
        public static readonly string tif;
        public const int MaxUserImageSizeByte = 5000000;
        public const int MaxFailedAccessAttempts = 5;
        public const int DefaultLockoutTimeSpan = 15;
        public const bool RequireConfirmedEmail = true;
        public const int InterestCategoriesCount = 14;
        public const int OKStatuseCode = 200;
        public const int PresignedGetObjectExpirationPeriod = 60 * 60 * 24 * 7;
        public const double RoomDefaultExpirationPeriodInHours = 6;
    }


}
