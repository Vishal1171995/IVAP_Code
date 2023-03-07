﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Ivap.Areas.CAPA.Models
{
    public class CorrectiveDetailModel
    {
        public int CID { get; set; }
        public string Corrective_Action { get; set; }
        public string CorrectiveAction_Text { get; set; }
        public string CorrectiveAction_Owner { get; set; }
        public string Corrective_Email { get; set; }  
    }
    public class CorrectiveDetailModel_Update
    {
        public int TID { get; set; }
        public string Corrective_Action { get; set; }
        public string Action_Text { get; set; }
        public string Action_Owner { get; set; }
        public string Owner_Email { get; set; }
    }

    public class CorrectiveRemarklModel
    {
        public int Corrective_CID { get; set; }
        public string Corrective_Item_Name { get; set; }
        public string Status { get; set; }
        public string Closure_Date { get; set; }
        public string Remark { get; set; }
        public string originalFileName { get; set; }
        public string TempFileName { get; set; }
    }
}