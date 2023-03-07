using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Ivap.ViewModel
{
    public class UserVM
    {
        [Required]
        [RegularExpression("^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d)(?=.*[$@$!%*?&])[A-Za-z\\d$@$!%*?&]{8,}", ErrorMessage = "Password must be minimum 8 characters long having  at least 1 Uppercase Alphabet, 1 Lowercase Alphabet, 1 Number and 1 Special Character.")]
        public string Password { set; get; }

        [Required]
        [Compare("Password", ErrorMessage = "Password and Confirm Password do not match!.")]
        public string ConfirmPassword { set; get; }

        public string PassLinkKey { set; get; }

        public int UID { set; get; }

    }

    public class UpdateProfileVM
    {
        [Required(ErrorMessage = "Please enter first name.")]
        [StringLength(100, ErrorMessage = "First Name can be min 4 and  max 50 characters long.", MinimumLength = 4)]
        public string FirstName { set; get; }

        [Required(ErrorMessage = "Please enter last name.")]
        [StringLength(100, ErrorMessage = "Last Name can be min 4 and  max 50 characters long.", MinimumLength = 4)]
        public string LastName { set; get; }

        public int UID { set; get; }

        [Required(ErrorMessage = "Please enter email.")]
        [EmailAddress(ErrorMessage = "Invalid Email Address.")]
        public string Email { set; get; }

        [Required(ErrorMessage = "Please enter Mobile number.")]

        [RegularExpression("^(\\+?\\d{1,4}[\\s-])?(?!0+\\s+,?$)\\d{10}\\s*,?$", ErrorMessage = "Invalid Mobile No")]
        public string MobileNo { set; get; }


        public string ProfilePic { set; get; }



    }
}