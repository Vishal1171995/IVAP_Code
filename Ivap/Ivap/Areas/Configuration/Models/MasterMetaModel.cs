using Ivap.Areas.Master.Repository;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Ivap.Areas.Configuration.Models
{
    public class MasterMetaModel
    {
            public int TID { get; set; }
            public string SCREEN_NAME { get; set; }
            [Required(ErrorMessage = "Display Name required.")]
            public string DISPLAY_NAME { get; set; }
            // [Required(ErrorMessage = "Display Order required.")]
            public int DISPLAY_ORDER { get; set; }
            public int ENTITY_ID { get; set; }

            public SelectList SCREEN_NAMEList { get; set; }

        
    }
}