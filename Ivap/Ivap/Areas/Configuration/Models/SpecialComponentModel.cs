using Ivap.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Ivap.Areas.Configuration.Models
{
    public class SpecialComponentModel : BaseModel
    {
        public int? TID { get; set; }
        [Required(ErrorMessage = "Required")]
        public string Special_Field_Type { get; set; }

        [Required(ErrorMessage = "Required")]
        public string Display_Name { get; set; }
        public string Default_Value { get; set; }

        public string LookUp_Name { get; set; }

        public string LookUp_Field_Value { get; set; }
        public int FileID { get; set; }
    }
}