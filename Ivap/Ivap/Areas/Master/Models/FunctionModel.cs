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
    public class FunctionModel : BaseModel
    {
        public int? TID { set; get; }

        [Required]

        public int ENTITY_ID { set; get; }
        [StringLength(10, MinimumLength = 1, ErrorMessage = "Maximum 10 Character Allowed.")]
        [RegularExpression(@"^\S*$", ErrorMessage = "Required")]
        [Required(ErrorMessage = "Required")]
        public string PAY_FUNC_CODE { set; get; }
        [Required(ErrorMessage = "Required")]
        [RegularExpression(@"^\S*$", ErrorMessage = "Required")]
        [StringLength(10, MinimumLength = 1, ErrorMessage = "Maximum 10 Character Allowed.")]
        public string ERP_FUNC_CODE { set; get; }
        [Required(ErrorMessage = "Required")]
        [StringLength(200, MinimumLength = 1, ErrorMessage = "Maximum 200 Character Allowed.")]
        public string FUNC_NAME { set; get; }

        public SelectList CompanyList { set; get; }

        public string ENTITY_ID_TEXT { set; get; }
        public string PAY_FUNC_CODE_TEXT { set; get; }
        public string ERP_FUNC_CODE_TEXT { set; get; }
        public string FUNC_NAME_TEXT { set; get; }
        public string Screen_Name { set; get; }
        public string ISACTIVE_TEXT { get; set; }
        public void SetDisplayName()
        {
            MasterMetaRepo ObjMetaRepo = new MasterMetaRepo(this.EID, "IVAP_MST_FUNCTION", "ViewFunction");

            //this.COMP_ID_TEXT = ObjMetaRepo.GetDisPlayName("COMP_ID");
            this.ENTITY_ID_TEXT = ObjMetaRepo.GetDisPlayName("ENTITY_ID");
            this.PAY_FUNC_CODE_TEXT = ObjMetaRepo.GetDisPlayName("PAY_FUNC_CODE");
            this.ERP_FUNC_CODE_TEXT = ObjMetaRepo.GetDisPlayName("ERP_FUNC_CODE");
            this.FUNC_NAME_TEXT = ObjMetaRepo.GetDisPlayName("FUNC_NAME");
            this.ISACTIVE_TEXT = ObjMetaRepo.GetDisPlayName("ISACTIVE");
            this.Screen_Name = ObjMetaRepo.Screen_Name;
        }
    }
}