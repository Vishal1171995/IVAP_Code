using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Ivap.Areas.Master.ViewModel
{
    public class CreateUserVM
    {


        public int UID { set; get; }

        [Required(ErrorMessage = "Please enter UserID.")]
        [RegularExpression("^([A-Za-z0-9.\\d@_.]{5,25})|([a-zA-Z0-9_\\-\\.]+)@([a-zA-Z0-9_\\-\\.]+)\\.([a-zA-Z]{2,10})$ ", ErrorMessage = "Invalid UserID.")]
        public string UserID { set; get; }

        [Required(ErrorMessage = "Please enter first name.")]
        public string FirstName { set; get; }

        [Required(ErrorMessage = "Please enter last name.")]
        public string LastName { set; get; }

        [Required(ErrorMessage = "Please enter email.")]
        [EmailAddress(ErrorMessage = "Invalid Email Address.")]
        public string Email { set; get; }

        [Required(ErrorMessage = "Please select circle.")]
        public int EID { set; get; }

        public int Role { set; get; }

        [RegularExpression("^(\\+?\\d{1,4}[\\s-])?(?!0+\\s+,?$)\\d{10}\\s*,?$", ErrorMessage = "Invalid Mobile No")]
        public string MobileNo { set; get; }

        [Required(ErrorMessage = "Please enter  password")]
        [RegularExpression("^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d)(?=.*[$@$!%*?&])[A-Za-z\\d$@$!%*?&]{8,}", ErrorMessage = "Password must be minimum 8 characters long having  at least 1 Uppercase Alphabet, 1 Lowercase Alphabet, 1 Number and 1 Special Character.")]
        public string Password { set; get; }

        [Required(ErrorMessage = "Please enter confirm password")]
        [Compare("Password", ErrorMessage = "Password and Confirm Password do not match!.")]
        public string ConfirmPassword { get; set; }

        public int Created_By { set; get; }


    }
}