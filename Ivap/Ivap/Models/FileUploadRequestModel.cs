using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Ivap.Models
{
    public class FileUploadRequestModel:BaseModel
    {
        public string File_Name { set; get; }
    }

    public class FileUploadResponseModel 
    {
        public int SuccessCount { set; get; }

        public int FailCount { set; get; }

        public string ResultFile { set; get; }


        public bool IsSuccess { set; get; }

        public string Message { set; get; }

        public string Data { set; get; }
    }
}