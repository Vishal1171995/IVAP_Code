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
    public class ClassModel:BaseModel
    {
        public int CID { set; get; }
        [Required]
        public int ENTITY_ID { set; get; }
        [StringLength(10, MinimumLength = 1, ErrorMessage = "Maximum 10 Character Allowed.")]
        [Required(ErrorMessage = "Required")]
        [RegularExpression(@"^\S*$", ErrorMessage = "Spaces not allowed.")]
        public string PAY_CLASS_CODE { set; get; }
        [StringLength(10, MinimumLength = 1, ErrorMessage = "Maximum 10 Character Allowed.")]
        [Required(ErrorMessage = "Required")]
        [RegularExpression(@"^\S*$", ErrorMessage = "Spaces not allowed.")]
        public string ERP_CLASS_CODE { set; get; }
        [Required(ErrorMessage = "Required")]
        public string CLASS_NAME { set; get; }

        public SelectList CompanyList { get; set; }
        public string COMP_ID_TEXT { set; get; }
      
        public string PAY_CLASS_CODE_TEXT { set; get; }
       
        public string ERP_CLASS_CODE_TEXT { set; get; }
      
        public string CLASS_NAME_TEXT { set; get; }
        public string ISACTIVE_TEXT { get; set; }
        public string Screen_Name { set; get; }
       // public string ISACTIVE_TEXT { get; set; }
        public void SetDisplayName()
        {
            MasterMetaRepo ObjMetaRepo = new MasterMetaRepo(this.EID, "IVAP_MST_CLASS", "ViewClass");

            //this.COMP_ID_TEXT = ObjMetaRepo.GetDisPlayName("COMP_ID");
            this.ERP_CLASS_CODE_TEXT = ObjMetaRepo.GetDisPlayName("ERP_CLASS_CODE");
            this.PAY_CLASS_CODE_TEXT = ObjMetaRepo.GetDisPlayName("PAY_CLASS_CODE");
            this.CLASS_NAME_TEXT = ObjMetaRepo.GetDisPlayName("CLASS_NAME");
            this.ISACTIVE_TEXT = ObjMetaRepo.GetDisPlayName("ISACTIVE");
            this.Screen_Name = ObjMetaRepo.Screen_Name;
        }

    }
}