using Ivap.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Ivap.Areas.Configuration.Models
{
    public class MenuModel : BaseModel
    {
        public int TID { get; set; }
        public int PID { get; set; }
        public string MenuName { get; set; }
        public int Order { get; set; }
        public string Roles { get; set; }
    }
}