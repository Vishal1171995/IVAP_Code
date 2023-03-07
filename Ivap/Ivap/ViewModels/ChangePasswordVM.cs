using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Ivap.ViewModel
{
    public class ChangePasswordVM
    {
        [Required(ErrorMessage = "Please enter your user name")]
        public string UserName { get; set; }

        [Required(ErrorMessage ="Please enter current password")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Please enter new password")]
        [RegularExpression("^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d)(?=.*[$@$!%*?&])[A-Za-z\\d$@$!%*?&]{8,}", ErrorMessage = "Password must be minimum 8 characters long having  at least 1 Uppercase Alphabet, 1 Lowercase Alphabet, 1 Number and 1 Special Character.")]
        public string NewPassword { get; set; }
        [Required(ErrorMessage = "Please enter confirm password")]
        [Compare("NewPassword",ErrorMessage = "Password and Confirm Password do not match!.")]
        public string ConfirmPassword { get; set; }

    }

    public class ResetPasswordVM
    {
        [Required(ErrorMessage="Please enter your User ID")]
        [RegularExpression(@"^[a-zA-Z0-9\@#._]+$",ErrorMessage="Invalid USerID")]
        public string UserID { set; get; }

        [Required(ErrorMessage = "Please enter your OTP")]
        public int  OTP { set; get; }

        public string CurrentPassword { set; get; }

        [Required(ErrorMessage = "Please enter new password")]
        [RegularExpression("^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d)(?=.*[$@$!%*?&])[A-Za-z\\d$@$!%*?&]{8,}", ErrorMessage = "Password must be minimum 8 characters long having  at least 1 Uppercase Alphabet, 1 Lowercase Alphabet, 1 Number and 1 Special Character.")]
        public string NewPassword { get; set; }
        [Required(ErrorMessage = "Please enter confirm password")]
        [Compare("NewPassword", ErrorMessage = "Password and Confirm Password do not match!.")]
        public string ConfirmPassword { get; set; }

    }

    
        public class PasswordOTPVM
        {
            [Required(ErrorMessage = "UserName Required.")]
            [RegularExpression(@"^[a-zA-Z0-9\@#._]+$", ErrorMessage = "Invalid USerID")]
            public string UserName { set; get; }
        }

        public class PasswordValidateOTPVM
        {
            [Required(ErrorMessage = "UserName Required.")]
            public string UserName { set; get; }

            [Required(ErrorMessage = "OTP Required.")]
            public int OTP { set; get; }
        }
}