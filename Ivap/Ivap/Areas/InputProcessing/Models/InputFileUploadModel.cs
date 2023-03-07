using Ivap.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Ivap.Areas.InputProcessing.Models
{
    public class InputFileUploadModel: FileUploadRequestModel
    {
        public int FileID { set; get; }

        public string FileExtention { set; get; }
        public string FilePath { set; get; }


    }

    public class InputFileResponseModel : FileUploadResponseModel
    {
        public string BatchNumber { set; get; }
    }
}