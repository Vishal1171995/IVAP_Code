using Ivap.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Ivap.Areas.CAPA.Models
{
    public class ConversationCapaModel : BaseModel
    {
       // public int? TID { set; get; }

       // [Required(ErrorMessage = "Required")]
        public string CAPAID { get; set; }

        //[Required(ErrorMessage = "Required")]
        public string ITEM_ID { get; set; }
        public string ITEM_NAME { get; set; }
       // [Required(ErrorMessage = "Required")]
        public string REMARK { get; set; }
        public string ATTACHMENT { get; set; }
        public string STATUS { get; set; }
        public string CLOSURE_DATE { get; set; }

        public string SYSTEM_ATTACHMENT { get; set; }


    }
}