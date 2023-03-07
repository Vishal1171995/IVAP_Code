using Ivap.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Ivap.Areas.PayrollOutput.Models
{
    public class PayrollOutPutFileSetupModel : BaseModel
    {
        public int? FileID { get; set; }
        [Required(ErrorMessage = "File Type required.")]
        public string FILE_TYPE { get; set; }
        [Required(ErrorMessage = "CATEGORY required.")]
        public string CATEGORY { get; set; }
        [Required(ErrorMessage = "File name required.")]
        public string FILE_NAME { get; set; }
        [Required(ErrorMessage = "File desc required.")]
        public string FILE_DESC { get; set; }
        public string DUE_DATE { get; set; }
        public List<string> ENTITY_Component_ID { get; set; }



        public SelectList FILE_CATEGORY { get; set; }
    }

    public class PayrollOutPutFileDtlModel : BaseModel
    {
        [Required(ErrorMessage = "TID required.")]
        public int TID { get; set; }
        [Required(ErrorMessage = "Display order required.")]
        public int Display_Order { get; set; }
    }
}