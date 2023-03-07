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
    public class DesignationModel: BaseModel
    {
        public int DSGID { get; set; }
        [StringLength(10, MinimumLength = 1, ErrorMessage = "Maximum 10 Character Allowed.")]
        [Required(ErrorMessage = "Required")]
        [RegularExpression(@"^\S*$", ErrorMessage = "Spaces not allowed.")]
        public string PAY_DSG_CODE { get; set; }
        [StringLength(10, MinimumLength = 1, ErrorMessage = "Maximum 10 Character Allowed.")]
        [Required(ErrorMessage = "Required")]
        [RegularExpression(@"^\S*$", ErrorMessage = "Spaces not allowed.")]
        public string ERP_DSG_CODE { get; set; }
        [Required(ErrorMessage = "Required")]
        public string DSG_NAME { get; set; }

        public string DSGID_TEXT { get; set; }
        public string PAY_DSG_CODE_TEXT { get; set; }
        public string ERP_DSG_CODE_TEXT { get; set; }
        public string DSG_NAME_TEXT { get; set; }
        public string ISACTIVE_TEXT { get; set; }
        public string Screen_Name { set; get; }
        public void SetDisplayName()
        {
            MasterMetaRepo ObjMetaRepo = new MasterMetaRepo(this.EID, "IVAP_MST_DESIGNATION", "ViewDesignation");

           
            this.DSGID_TEXT = ObjMetaRepo.GetDisPlayName("DSGID");
            this.PAY_DSG_CODE_TEXT = ObjMetaRepo.GetDisPlayName("PAY_DSG_CODE");
            this.ERP_DSG_CODE_TEXT = ObjMetaRepo.GetDisPlayName("ERP_DSG_CODE");
            this.DSG_NAME_TEXT = ObjMetaRepo.GetDisPlayName("DSG_NAME");
            this.ISACTIVE_TEXT = ObjMetaRepo.GetDisPlayName("ISACTIVE");
            this.Screen_Name = ObjMetaRepo.Screen_Name;
        }

    }
    public class DesignationModelVM : BaseModel
    {
        public int DSGID { get; set; }
        [StringLength(10, MinimumLength = 1, ErrorMessage = "Maximum 10 Character Allowed.")]
        [Required(ErrorMessage = "Required")]
        [RegularExpression(@"^\S*$", ErrorMessage = "Spaces not allowed.")]
        public string PAY_DSG_CODE { get; set; }
        [StringLength(10, MinimumLength = 1, ErrorMessage = "Maximum 10 Character Allowed.")]
        [Required(ErrorMessage = "Required")]
        [RegularExpression(@"^\S*$", ErrorMessage = "Spaces not allowed.")]
        public string ERP_DSG_CODE { get; set; }
        [Required(ErrorMessage = "Required")]
        public string DSG_NAME { get; set; }
        //[Required(ErrorMessage = "Please select grade.")]
        //public string GRADE_ID { get; set; }
        //public int PARENT_DSG { get; set; }
        //public string JOB_DESC { get; set; }
        //public int MANPOWER_BUDGET { get; set; }

    }
}