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
    public class GradeModel : BaseModel
    {
        public int? TID { set; get; }
        [Required]
        public int ENTITY_ID { set; get; }
        [Required(ErrorMessage = "Required")]
        [StringLength(10, MinimumLength = 1, ErrorMessage = "Maximum 10 Character Allowed.")]
        public string PAY_GRADE_CODE { set; get; }//varchar(10) Unchecked
        [Required(ErrorMessage = "Required")]
        [StringLength(10, MinimumLength = 1, ErrorMessage = "Maximum 10 Character Allowed.")]
        public string ERP_GRADE_CODE { set; get; }//varchar(10) Unchecked
        [Required(ErrorMessage = "Required")]
        [StringLength(100, MinimumLength = 1, ErrorMessage = "Maximum 100 Character Allowed.")]
        public string GARDE_NAME { set; get; }//varchar(100)    Unchecked
        [Required(ErrorMessage = "Required")]
        [Range(1, int.MaxValue, ErrorMessage = "Required")]
        public decimal? GRADE_SCALE_FROM { set; get; }//numeric(10, 2)  Unchecked
        [Required(ErrorMessage = "Required")]
        [Range(1, int.MaxValue, ErrorMessage = "Required")]
        public decimal? GRADE_SCALE_TO { set; get; }//numeric(10, 2)  Unchecked
        [Required(ErrorMessage = "Required")]
        [Range(1, int.MaxValue, ErrorMessage = "Required")]
        public decimal? GRADE_MIDPOINT { set; get; }//numeric(10, 2)  Unchecked
        [Required(ErrorMessage = "Required")]
        public int APPR_MONTH { set; get; }//int Unchecked
        public decimal? APPR_NO { set; get; }//numeric //(10, 2)  Unchecked
        [Required(ErrorMessage = "Required")]
        public decimal? Prob_Period { set; get; } ///numeric(10, 2)  Checked
        [Required(ErrorMessage = "Required")]
        public int Appr_Period_Prob { set; get; }//int Unchecked
        [Required]
        public int Appr_Period_Conf { set; get; }//int Unchecked
        public SelectList CompanyList { get; set; }
        public string ENTITY_ID_TEXT { set; get; }
        public string PAY_GRADE_CODE_TEXT { set; get; }
        public string ERP_GRADE_CODE_TEXT { set; get; }
        public string GARDE_NAME_TEXT { set; get; }
        public string GRADE_SCALE_FROM_TEXT { set; get; }
        public string GRADE_SCALE_TO_TEXT { set; get; }
        public string GRADE_MIDPOINT_TEXT { set; get; }
        public string APPR_MONTH_TEXT { set; get; }
        public string APPR_NO_TEXT { set; get; }
        public string Prob_Period_TEXT { set; get; }
        public string Appr_Period_Prob_TEXT { set; get; }
        public string Appr_Period_Conf_TEXT { set; get; }
        public string ISACTIVE_TEXT { get; set; }
        public string Screen_Name { set; get; }
        public void SetDisplayName()
        {
            MasterMetaRepo ObjMetaRepo = new MasterMetaRepo(this.EID, "IVAP_MST_GRADE", "GradeList");

            this.ENTITY_ID_TEXT = ObjMetaRepo.GetDisPlayName("ENTITY_ID");
            this.PAY_GRADE_CODE_TEXT = ObjMetaRepo.GetDisPlayName("PAY_GRADE_CODE");
            this.ERP_GRADE_CODE_TEXT = ObjMetaRepo.GetDisPlayName("ERP_GRADE_CODE");
            this.GARDE_NAME_TEXT = ObjMetaRepo.GetDisPlayName("GARDE_NAME");
            this.GRADE_SCALE_FROM_TEXT = ObjMetaRepo.GetDisPlayName("GRADE_SCALE_FROM");

            this.GRADE_SCALE_TO_TEXT = ObjMetaRepo.GetDisPlayName("GRADE_SCALE_TO");
            this.GRADE_MIDPOINT_TEXT = ObjMetaRepo.GetDisPlayName("GRADE_MIDPOINT");
            this.APPR_MONTH_TEXT = ObjMetaRepo.GetDisPlayName("APPR_MONTH");
            this.APPR_NO_TEXT = ObjMetaRepo.GetDisPlayName("APPR_NO");

            this.Prob_Period_TEXT = ObjMetaRepo.GetDisPlayName("Prob_Period");
            this.Appr_Period_Prob_TEXT = ObjMetaRepo.GetDisPlayName("Appr_Period_Prob");
            this.Appr_Period_Conf_TEXT = ObjMetaRepo.GetDisPlayName("Appr_Period_Conf");
            this.ISACTIVE_TEXT = ObjMetaRepo.GetDisPlayName("ISACTIVE");
            this.Screen_Name = ObjMetaRepo.Screen_Name;
        }
    }
}