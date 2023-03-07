using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Ivap.Areas.FileExplorer.Models
{
    public class FileExplorerModel
    {
        public string text { get; set; }
        public int id { get; set; }
        public string type = "customer";
        public bool hasChildren { get; set; }
        public int EID { get; set; }
        public int ParentId { get; set; }
        public bool expanded { get; set; }
        public string spriteCssClass = "folder";
        public bool IsView { get; set; }
        public bool IsAll { get; set; }
    }
}