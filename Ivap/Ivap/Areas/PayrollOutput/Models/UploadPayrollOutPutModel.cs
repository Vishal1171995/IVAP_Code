using Ivap.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Ivap.Areas.PayrollOutput.Models
{
    public class UploadPayrollOutPutModel: BaseModel
    {
        public int FileID { get; set; }
        public SelectList FileList { get; set; }
        public string System_Document { get; set; }
        public string Original_Document_Name { get; set; }
        [DisplayFormat(DataFormatString = "{0:MMM-yyyy}")]
        public DateTime PayDate { get; set; }
    }
}