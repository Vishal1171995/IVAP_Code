using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Ivap.Areas.CAPA.Models
{
    public class PreventiveDetailModel
    {
        public int PID { get; set; }
        public string Preventive_Action { get; set; }
        public string PreventiveAction_Text { get; set; }
        public string PreventiveAction_Owner { get; set; }

        public string Preventive_Email { get; set; }

        
    }
    public class PreventiveDetailModel_Update
    {
        public int TID { get; set; }
        public string Preventive_Action { get; set; }
        public string Action_Text { get; set; }
        public string Action_Owner { get; set; }
        public string Owner_Email { get; set; }


    }

    public class PreventiveRemarklModel
    {
        public int Preventive_CID { get; set; }
        public string Preventive_Item_Name { get; set; }
        public string Status { get; set; }
        public string Closure_Date { get; set; }
        public string Remark { get; set; }
        public string originalFileName { get; set; }
        public string TempFileName { get; set; }
    }
}