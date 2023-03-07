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
    public class TypeModel : BaseModel
    {
        public int? TID { set; get; }

        [Required(ErrorMessage = "Required")]
        public string TYPE_NAME { set; get; }

        [StringLength(10, MinimumLength = 1, ErrorMessage = "Maximum 10 Character Allowed.")]
        [Required(ErrorMessage = "Required")]
        [RegularExpression(@"^\S*$", ErrorMessage = "Spaces not allowed.")]
        public string PAY_TYPE_CODE { set; get; }

        [StringLength(10, MinimumLength = 1, ErrorMessage = "Maximum 10 Character Allowed.")]
        [Required(ErrorMessage = "Required")]
        [RegularExpression(@"^\S*$", ErrorMessage = "Spaces not allowed.")]
        public string ERP_TYPE_CODE { set; get; }
        public string TID_TEXT { set; get; }
        public string TYPE_NAME_TEXT { set; get; }
        public string PAY_TYPE_CODE_TEXT { set; get; }
        public string ERP_TYPE_CODE_TEXT { set; get; }
        public string Screen_Name { set; get; }

        public string ISACTIVE_TEXT { get; set; }
        public void SetDisplayName()
        {
            MasterMetaRepo ObjMetaRepo = new MasterMetaRepo(this.EID, "IVAP_MST_TYPE", "TypeList");

            this.TID_TEXT = ObjMetaRepo.GetDisPlayName("TID");
            this.TYPE_NAME_TEXT = ObjMetaRepo.GetDisPlayName("TYPE_NAME");
            this.PAY_TYPE_CODE_TEXT = ObjMetaRepo.GetDisPlayName("PAY_TYPE_CODE");
            this.ERP_TYPE_CODE_TEXT = ObjMetaRepo.GetDisPlayName("ERP_TYPE_CODE");
            this.ISACTIVE_TEXT = ObjMetaRepo.GetDisPlayName("ISACTIVE");
            this.Screen_Name = ObjMetaRepo.Screen_Name;
        }

    }
}