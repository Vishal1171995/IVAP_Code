using Ivap.Areas.Master.Repository;
using Ivap.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Ivap.Areas.Master.Models
{
    public class LevelModel: BaseModel
    {
        public int LevelID { get; set; }
        [StringLength(10, MinimumLength = 1, ErrorMessage = "Maximum 10 Character Allowed.")]
        [Required(ErrorMessage = "Required")]
        [RegularExpression(@"^\S*$", ErrorMessage = "Spaces not allowed.")]
        public string PAY_LEVEL_CODE { get; set; }
        [StringLength(10, MinimumLength = 1, ErrorMessage = "Maximum 10 Character Allowed.")]
        [Required(ErrorMessage = "Required")]
        [RegularExpression(@"^\S*$", ErrorMessage = "Spaces not allowed.")]
        public string ERP_LEVEL_CODE { get; set; }
        [Required(ErrorMessage = "Required")]
        public string LEVEL_NAME { get; set; }
        public string LevelID_TEXT { get; set; }
        public string PAY_LEVEL_CODE_TEXT { get; set; }
        public string ERP_LEVEL_CODE_TEXT { get; set; }
        public string LEVEL_NAME_TEXT { get; set; }
        public string Screen_Name { set; get; }
        public string ISACTIVE_TEXT { get; set; }
        public void SetDisplayName()
        {
            MasterMetaRepo ObjMetaRepo = new MasterMetaRepo(this.EID, "IVAP_MST_LEVEL", "ViewLevel");

            this.LevelID_TEXT = ObjMetaRepo.GetDisPlayName("LevelID");
            this.PAY_LEVEL_CODE_TEXT = ObjMetaRepo.GetDisPlayName("PAY_LEVEL_CODE");
            this.ERP_LEVEL_CODE_TEXT = ObjMetaRepo.GetDisPlayName("ERP_LEVEL_CODE");
            this.LEVEL_NAME_TEXT = ObjMetaRepo.GetDisPlayName("LEVEL_NAME");
            this.ISACTIVE_TEXT = ObjMetaRepo.GetDisPlayName("ISACTIVE");
            this.Screen_Name = ObjMetaRepo.Screen_Name;
        }
    }
}