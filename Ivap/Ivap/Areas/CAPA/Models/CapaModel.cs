using Ivap.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Ivap.Areas.CAPA.Models
{
    public class CapaModel : BaseModel
    {
        public int? TID { set; get; }

        [Required(ErrorMessage = "Required")]
        public string Issue{get;set;}

        [Required(ErrorMessage = "Required")]
        public string Issue_Description { get; set; }
        [Required(ErrorMessage = "Required")]
        public string Customer_Impact { get; set; }
        [Required(ErrorMessage = "Required")]

        public string Sequence_of_events { get; set; }
        [Required(ErrorMessage = "Required")]
        public string Communication_process { get; set; }
        [Required(ErrorMessage = "Required")]
        public string Root_Cause { get; set; }
        public string CorrectiveAction_Detail { get; set; }
        public string CorrectiveAction_Text { get; set; }
        public string CorrectiveAction_Owner { get; set; }
        public string CorrectiveAction_Email { get; set; }
        public string PreventiveAction_Text { get; set; }
        public string PreventiveAction_Detail { get; set; }
        public string PreventiveAction_Owner { get; set; }

        public string Preventive_Action { get; set; }
        public string PreventiveAction_Email { get; set; }
        public string Corrective_Action { get; set; }

        public int? Corrective_ID { get; set; }
        public int? Preventive_ID { get; set; }
        public int? Capa_Update { get; set; }
        [Required(ErrorMessage = "Required")]
        public string Category { get; set; }
        [Required(ErrorMessage = "Required")]
        public string Finance_Type { get; set; }
        public string Finance_Amount { get; set; }
        [Required(ErrorMessage = "Required")]
        public string Impact_Value { get; set; }
        [Required(ErrorMessage = "Required")]   
        public string Stage { get; set; }
        [Required(ErrorMessage = "Required")]
        public string Incident_date { get; set; }


        public string CapaConversationCorrective { get; set; }

        public string CapaConversationPreventive { get; set; }

    }
}