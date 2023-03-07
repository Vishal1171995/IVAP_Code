using Ivap.Areas.Master.Repository;
using Ivap.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Ivap.Areas.Master.Models
{
    public class ServicesAvailedModel : BaseModel
    {
        public int SID { get; set; }
        public string SERVICE_NAME { set; get; }
    }
}