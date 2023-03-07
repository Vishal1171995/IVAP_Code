using Ivap.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Ivap.Areas.Master.Models
{
    public class EntityModel : BaseModel
    {
        public int? TID { set; get; }
        public string ENTITY_CODE { set; get; }
        [StringLength(200, MinimumLength = 3, ErrorMessage = "Required")]
        [Required(ErrorMessage = "Required")]
        public string ENTITY_NAME { set; get; }
        [StringLength(500, MinimumLength = 3, ErrorMessage = "Minimum 3 Character is required.")]
        [Required(ErrorMessage = "Required")]
        public string ENTITY_ADDR1 { set; get; }
        public string ENTITY_ADDR2 { set; get; }
       // [Required(ErrorMessage = "Required")]
        public int ENTITY_CITY { set; get; }
        [Required(ErrorMessage = "Required")]
        [Range(1, int.MaxValue, ErrorMessage = "Required")]
        public int ENTITY_STATE { set; get; }
        [Required(ErrorMessage = "Required")]
        [Range(1, int.MaxValue, ErrorMessage = "Required")]
        public int ENTITY_Country { set; get; }
        [Required(ErrorMessage = "Required")]
        [Range(1, int.MaxValue, ErrorMessage = "Required")]
        public int ENTITY_Currency { set; get; }
        [Required(ErrorMessage = "Required")]
        public string DATE_FORMAT { set; get; }

        
        [Required(ErrorMessage = "Required")]
        public string ENTITY_Payperiod { set; get; }
        [RegularExpression(@"^\d{6}(-\d{4})?$", ErrorMessage = "Please Enter Valid Postal Code.")]
        [Required(ErrorMessage = "Required")]
        public string ENTITY_PIN { set; get; }
        [RegularExpression(@"[a-zA-Z][a-zA-Z0-9]*$", ErrorMessage = "Only alphanumeric allowed")]
        [Required(ErrorMessage = "Required")]
        public string DOMAIN_NAME { set; get; }
        public SelectList StateList { get; set; }

        public SelectList CityList { get; set; }
        [Required(ErrorMessage = "Required")]
        public string ENTITY_Services_Availed { set; get; }
       




    }
   

}




