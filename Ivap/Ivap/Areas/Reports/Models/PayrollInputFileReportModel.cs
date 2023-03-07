using Ivap.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Ivap.Areas.Reports.Models
{
    public class PayrollInputFileReportModel : BaseModel
    {
        public int File_ID { get; set; }
        public int SubFile_ID { get; set; }
        public DateTime PayDate { get; set; }
        public string File_Name { get; set; }
        public string File_Category { get; set; }
        public int Total_Pending_Record { get; set; }
        public int Total_Approved_Record { get; set; }

        public SelectList PayDateList { get; set; }
        public String SelectedPayDate { get; set; }
    }
}