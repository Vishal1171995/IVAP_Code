using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Ivap.ViewModel
{
    public class LoginVM
    {

        
        [Required(ErrorMessage = "Please enter your user name.")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Please enter password.")]
        [DisplayName("Password")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Please enter entity code.")]
        public string EntityCode { get; set; }
    }

    public class ForgetPasswordVM
    {
        [Required(ErrorMessage = "Please enter your user name.")]
        public string UserName { get; set; }
    }

}