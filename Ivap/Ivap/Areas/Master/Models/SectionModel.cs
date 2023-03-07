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
    public class SectionModel : BaseModel
    {
        public int SECTID { set; get; }
        [Required(ErrorMessage = "Required")]
        public int ENTITY_ID { set; get; }
        [StringLength(10, MinimumLength = 1, ErrorMessage = "Maximum 10 Character Allowed.")]
        [Required(ErrorMessage = "Required")]
        [RegularExpression(@"^\S*$", ErrorMessage = "Spaces not allowed.")]
        public string PAY_SECTION_CODE { set; get; }
        [StringLength(10, MinimumLength = 1, ErrorMessage = "Maximum 10 Character Allowed.")]
        [Required(ErrorMessage = "Required")]
        [RegularExpression(@"^\S*$", ErrorMessage = "Spaces not allowed.")]
        public string ERP_SECTION_CODE { set; get; }
        [Required(ErrorMessage = "Required")]
        public string SECTION_NAME { set; get; }

        public string ENTITY_NAME_TEXT { set; get; }
      
        public string PAY_SECTION_CODEE_TEXT { set; get; }
       
        public string ERP_SECTION_CODE_TEXT { set; get; }
      
        public string SECTION_NAME_TEXT { set; get; }
        public string ISACTIVE_TEXT { get; set; }
        public string Screen_Name { set; get; }
        public void SetDisplayName()
        {
            MasterMetaRepo ObjMetaRepo = new MasterMetaRepo(this.EID, "IVAP_MST_SECTION", "ViewSection");

            this.ENTITY_NAME_TEXT = ObjMetaRepo.GetDisPlayName("ENTITY_ID");
            this.ERP_SECTION_CODE_TEXT = ObjMetaRepo.GetDisPlayName("ERP_SECTION_CODE");
            this.PAY_SECTION_CODEE_TEXT = ObjMetaRepo.GetDisPlayName("PAY_SECTION_CODE");
            this.SECTION_NAME_TEXT = ObjMetaRepo.GetDisPlayName("SECTION_NAME");
            this.ISACTIVE_TEXT = ObjMetaRepo.GetDisPlayName("ISACTIVE");
            this.Screen_Name = ObjMetaRepo.Screen_Name;
        }

    }
}