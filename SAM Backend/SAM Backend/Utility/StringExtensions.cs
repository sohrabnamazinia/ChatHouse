using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SAM_Backend.Utility
{
    public static class StringExtensions
    {
        public static bool IsNullOrEmpty(this string s)
        {
            return s == null || s.Length == 0;
        }

        public static bool IsAllowedUsername(this string username)
        {
            char[] AllowedChars = Constants.UsernameAllowedUserNameCharacters.ToCharArray();
            foreach (char c in username)
            {
                if (!AllowedChars.Contains(c)) return false;
            }
            return true;
        }
    }
}
