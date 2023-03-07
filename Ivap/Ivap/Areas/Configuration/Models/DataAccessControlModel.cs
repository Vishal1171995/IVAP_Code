using Ivap.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Ivap.Areas.Configuration.Models
{
    public class DataAccessControlModel:BaseModel
    {
        public int UserID { get; set; }
        public int UID { get; set; }
        public int TID { get; set; }
        public string Name { get; set; }
        public SelectList UserList { get; set; }
        public SelectList UserCopyRightList { get; set; }
        public SelectList MenuList { get; set; }
    }
}