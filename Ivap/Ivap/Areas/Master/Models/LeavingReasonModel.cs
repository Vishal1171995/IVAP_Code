using Ivap.Areas.Master.Repository;
using Ivap.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Ivap.Areas.Master.Models
{
    public class LeavingReasonModel : BaseModel
    {
        //ENTITY_ID,PAY_LEAVING_CODE,ERP_LEAVING_CODE,[VOL/NON_VOL],REASON
        public int TID { get; set; }
        [Required(ErrorMessage = "Required")]
        public int ENTITY_ID { get; set; }
        [StringLength(10, MinimumLength = 1, ErrorMessage = "Maximum 10 Character Allowed.")]
        [Required(ErrorMessage = "Required")]
        [RegularExpression(@"^\S*$", ErrorMessage = "Spaces not allowed.")]
        public string PAY_LEAVING_CODE { get; set; }
        [StringLength(10, MinimumLength = 1, ErrorMessage = "Maximum 10 Character Allowed.")]
        [Required(ErrorMessage = "Required")]
        [RegularExpression(@"^\S*$", ErrorMessage = "Spaces not allowed.")]
        public string ERP_LEAVING_CODE { get; set; }
        [Required(ErrorMessage = "Required")]
        public string VOL { get; set; }
        [Required(ErrorMessage = "Required")]
       // public string VOLNON_VOL {get;set;}
        public string REASON { get; set; }
        public string TID_TEXT { get; set; }
        public string ENTITY_ID_TEXT { get; set; }
        public string PAY_LEAVING_CODE_TEXT { get; set; }
        public string ERP_LEAVING_CODE_TEXT { get; set; }
        public string VOL_TEXT { get; set; }
        public string REASON_TEXT { get; set; }
        public string Screen_Name { set; get; }
        public string ISACTIVE_TEXT { get; set; }
        public void SetDisplayName()
        {
            MasterMetaRepo ObjMetaRepo = new MasterMetaRepo(this.EID, "IVAP_MST_LEAVING_REASON", "ViewLeavingReason");

            // this.Company_Id_TEXT = ObjMetaRepo.GetDisPlayName("COMP_ID");
            this.TID_TEXT = ObjMetaRepo.GetDisPlayName("TID");
            this.ENTITY_ID_TEXT = ObjMetaRepo.GetDisPlayName("ENTITY_ID");
            this.PAY_LEAVING_CODE_TEXT = ObjMetaRepo.GetDisPlayName("PAY_LEAVING_CODE");
            this.ERP_LEAVING_CODE_TEXT = ObjMetaRepo.GetDisPlayName("ERP_LEAVING_CODE");
            this.VOL_TEXT = ObjMetaRepo.GetDisPlayName("VOL/NON_VOL");
            this.REASON_TEXT = ObjMetaRepo.GetDisPlayName("REASON");
            this.ISACTIVE_TEXT = ObjMetaRepo.GetDisPlayName("ISACTIVE");
            this.Screen_Name = ObjMetaRepo.Screen_Name;
        }
    }
}