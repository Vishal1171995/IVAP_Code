using Ivap.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Ivap.Areas.FileExplorer.Models
{
    public class FileMetaDataModel: BaseModel
    {
        public int? FileMetaID { get; set; }
        public string Description { get; set; }
        public string MetaData { get; set; }
        [Required(ErrorMessage = "Please Enter File type.")]
        public string FileTypeName { get; set; }
    }
}