using Ivap.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Ivap.Areas.Configuration.Models
{
    public class TransposeModel:BaseModel
    {
        public int? TID { get; set; }
        [Required(ErrorMessage = "Required")]
        public string Transpose_Field_Type { get; set; }
        public string Transpose_PMS_Input_File { get; set; }
        public string Transpose_Component_Name { get; set; }
        
        public string Transpose_File_Display_Name { get; set; }
        public string Transpose_Default_Value { get; set; }
        public bool Transpose_ISACTIVE { get; set; }
        public int? Display_order { get; set; }
        public int Transpose_FileID { get; set; }
        public string Component_Name { get; set; }
      
    }
}