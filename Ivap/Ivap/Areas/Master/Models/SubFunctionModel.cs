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
    public class SubFunctionModel:BaseModel
    {
        public int SID { set; get; }
        [Required(ErrorMessage = "Required")]
        [Range(1, int.MaxValue, ErrorMessage = "Please select parrent function.")]
        public int PARENT_FUNC_ID { set; get; }

        [StringLength(10, MinimumLength = 1, ErrorMessage = "Maximum 10 Character Allowed.")]
        [Required(ErrorMessage = "Required")]
        [RegularExpression(@"^\S*$", ErrorMessage = "Spaces not allowed.")]
        public string PAY_SUB_FUNC_CODE { set; get; }

        [StringLength(10, MinimumLength = 1, ErrorMessage = "Maximum 10 Character Allowed.")]
        [Required(ErrorMessage = "Required")]
        [RegularExpression(@"^\S*$", ErrorMessage = "Spaces not allowed.")]
        public string ERP_SUB_FUNC_CODE { set; get; }

        [Required(ErrorMessage = "Required")]
        public string SUB_FUNC_NAME { set; get; }

        public SelectList FunctionList { get; set; }

        public string SID_TEXT { set; get; }
       
        public string ENTITY_ID_TEXT { set; get; }
        
        public string PARENT_FUNC_ID_TEXT { set; get; }

        public string PAY_SUB_FUNC_CODE_TEXT { set; get; }

        public string ERP_SUB_FUNC_CODE_TEXT { set; get; }
        public string SUB_FUNC_NAME_TEXT { set; get; }
        public string ISACTIVE_TEXT { set; get; }
        public string Screen_Name { set; get; }
        public void SetDisplayName()
        {
            MasterMetaRepo ObjMetaRepo = new MasterMetaRepo(this.EID, "IVAP_MST_SUB_FUNCTION", "ViewSubFunction");

            this.PARENT_FUNC_ID_TEXT = ObjMetaRepo.GetDisPlayName("PARENT_FUNC_ID");
            this.PAY_SUB_FUNC_CODE_TEXT = ObjMetaRepo.GetDisPlayName("PAY_SUB_FUNC_CODE");
            this.ERP_SUB_FUNC_CODE_TEXT = ObjMetaRepo.GetDisPlayName("ERP_SUB_FUNC_CODE");
            this.SUB_FUNC_NAME_TEXT = ObjMetaRepo.GetDisPlayName("SUB_FUNC_NAME");
            this.ENTITY_ID_TEXT = ObjMetaRepo.GetDisPlayName("ENTITY_ID");
            this.ISACTIVE_TEXT = ObjMetaRepo.GetDisPlayName("ISACTIVE");
            this.Screen_Name = ObjMetaRepo.Screen_Name;
        }

    }
    public class SubFunctionModelVW:BaseModel
    {
        public int SID { set; get; }
        [Required(ErrorMessage = "Required")]
        public string PARENT_FUNC_ID { set; get; }

        [StringLength(10, MinimumLength = 1, ErrorMessage = "Maximum 10 Character Allowed.")]
        [Required(ErrorMessage = "Required")]
        [RegularExpression(@"^\S*$", ErrorMessage = "Spaces not allowed.")]
        public string PAY_SUB_FUNC_CODE { set; get; }

        [StringLength(10, MinimumLength = 1, ErrorMessage = "Maximum 10 Character Allowed.")]
        [Required(ErrorMessage = "Required")]
        [RegularExpression(@"^\S*$", ErrorMessage = "Spaces not allowed.")]
        public string ERP_SUB_FUNC_CODE { set; get; }

        [Required(ErrorMessage = "Required")]
        public string SUB_FUNC_NAME { set; get; }
        public string Function_Name { get; set; }
    }
}
//TID	ENTITY_ID	PAY_SUB_FUNC_CODE	ERP_SUB_FUNC_CODE	SUB_FUNC_NAME	CREATED_ON	CREATED_BY	UPDATE_ON	UPDATED_BY	ISACTIVE	PARENT_FUNC_ID