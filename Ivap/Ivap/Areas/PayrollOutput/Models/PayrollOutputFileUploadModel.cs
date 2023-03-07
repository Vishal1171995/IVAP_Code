using Ivap.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Ivap.Areas.PayrollOutput.Models
{
    public class PayrollOutputFileUploadModel : FileUploadRequestModel
    {
        public int FileID { set; get; }
        public string FileExtention { set; get; }
        public string FilePath { set; get; }

        public DateTime PayDate { get; set; }
    }
    public class PayrollOutputFileResponseModel : FileUploadResponseModel
    {
        public string BatchNumber { set; get; }
    }
}