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
    public class DivisionModel:BaseModel
    {
        public int DiviID { get; set; }
        [StringLength(10, MinimumLength = 1, ErrorMessage = "Maximum 10 Character Allowed.")]
        [Required(ErrorMessage = "Required")]
        [RegularExpression(@"^\S*$", ErrorMessage = "Spaces not allowed.")]
        public string PAY_DIVI_CODE { get; set; }
        [StringLength(10, MinimumLength = 1, ErrorMessage = "Maximum 10 Character Allowed.")]
        [Required(ErrorMessage = "Required")]
        [RegularExpression(@"^\S*$", ErrorMessage = "Spaces not allowed.")]
        public string ERP_DIVI_CODE { get; set; }
        [Required(ErrorMessage = "Required")]
        public string DIVI_NAME { get; set; }
        public SelectList CompList { get; set; }

        public string DiviID_TEXT { get; set; }

        public string PAY_DIVI_CODE_TEXT { get; set; }
        public string ERP_DIVI_CODE_TEXT { get; set; }
        public string DIVI_NAME_TEXT { get; set; }
        public string Screen_Name { set; get; }
        public string ISACTIVE_TEXT { get; set; }
        public void SetDisplayName()
        {
            MasterMetaRepo ObjMetaRepo = new MasterMetaRepo(this.EID, "IVAP_MST_DIVISiON", "Division");

            this.DiviID_TEXT = ObjMetaRepo.GetDisPlayName("DiviID");
            this.PAY_DIVI_CODE_TEXT = ObjMetaRepo.GetDisPlayName("PAY_DIVI_CODE");
            this.ERP_DIVI_CODE_TEXT = ObjMetaRepo.GetDisPlayName("ERP_DIVI_CODE");
            this.DIVI_NAME_TEXT = ObjMetaRepo.GetDisPlayName("DIVI_NAME");
            this.ISACTIVE_TEXT = ObjMetaRepo.GetDisPlayName("ISACTIVE");
            this.Screen_Name = ObjMetaRepo.Screen_Name;
        }
    }
    public class DivisionModelVM : BaseModel
    {
        public int DiviID { get; set; }
        [StringLength(10, MinimumLength = 1, ErrorMessage = "Maximum 10 Character Allowed.")]
        [Required(ErrorMessage = "Required")]
        [RegularExpression(@"^\S*$", ErrorMessage = "Spaces not allowed.")]
        public string PAY_DIVI_CODE { get; set; }
        [StringLength(10, MinimumLength = 1, ErrorMessage = "Maximum 10 Character Allowed.")]
        [Required(ErrorMessage = "Required")]
        [RegularExpression(@"^\S*$", ErrorMessage = "Spaces not allowed.")]
        public string ERP_DIVI_CODE { get; set; }
        [Required(ErrorMessage = "Required")]
        public string DIVI_NAME { get; set; }
        public SelectList CompList { get; set; }
    }
}