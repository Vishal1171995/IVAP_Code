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
    public class ProcessModel: BaseModel
    {
        public int? TID { set; get; }

        [StringLength(10, MinimumLength = 1, ErrorMessage = "Maximum 10 Character Allowed.")]
        [Required(ErrorMessage = "Required")]
        [RegularExpression(@"^\S*$", ErrorMessage = "Spaces not allowed.")]
        public string PAY_PROC_CODE { set; get; }

        [Required(ErrorMessage = "Required")]
        [StringLength(10, MinimumLength = 1, ErrorMessage = "Maximum 10 Character Allowed.")]
        [RegularExpression(@"^\S*$", ErrorMessage = "Spaces not allowed.")]
        public string ERP_PROC_CODE { set; get; }
        [Required(ErrorMessage = "Required")]
        public string PROC_NAME { set; get; }
        public SelectList CompanyList { get; set; }

        public string PAY_PROC_CODE_Text
        {
            set;get;
        }

        public string PROC_NAME_Text { set; get; }

        public string ERP_PROC_CODE_Text { set; get; }

        public string Comp_ID_Text { set; get; }

        public string Screen_Name { set; get; }

        public string ISACTIVE_TEXT { get; set; }
        public void SetDisplayName()
        {
            MasterMetaRepo ObjMetaRepo = new MasterMetaRepo(this.EID, "IVAP_MST_PROCESS", "ProcessList");

            this.PAY_PROC_CODE_Text = ObjMetaRepo.GetDisPlayName("PAY_PROC_CODE");
            this.ERP_PROC_CODE_Text = ObjMetaRepo.GetDisPlayName("ERP_PROC_CODE");
            this.PROC_NAME_Text = ObjMetaRepo.GetDisPlayName("PROC_NAME");
            this.ISACTIVE_TEXT = ObjMetaRepo.GetDisPlayName("ISACTIVE");
            this.Screen_Name = ObjMetaRepo.Screen_Name;
        }
    }
}