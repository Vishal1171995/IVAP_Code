using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Ivap.Areas.InputProcessing.Models
{
    public class UploadInputModel
    {
        public int FileID { get; set; }
        public SelectList FileList { get; set; }
        public string System_Document { get; set; }
        public string Original_Document_Name { get; set; }
    }
}