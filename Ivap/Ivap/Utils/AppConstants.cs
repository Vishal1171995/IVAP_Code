using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Ivap.Utils
{
    /// <summary>
    /// 
    /// </summary>
    public static class AppConstants
    {

        public struct Message
        {
            public const string InvalidUser = "Invalid user name or password.";
            public const string PasswordExpired = "Your password has been expired.Please update your password.";
        }

        public struct GDFilter
        {
            public const string Equal = "eq";
            public const string NoEqual = "neq";
            public const string StartWith = "startswith";
            public const string contains = "contains";
            public const string Doesnotcontain = "doesnotcontain";
            public const string EndsWith = "endswith";
            public const string Gte = "gte";
            public const string Gt = "gt";
            public const string Lte = "lte";
            public const string Lt = "lt";
        }

        /// <summary>
        /// 
        /// </summary>
    }
}