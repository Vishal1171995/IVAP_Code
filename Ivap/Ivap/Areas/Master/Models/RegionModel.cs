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
    public class RegionModel:BaseModel
    {
        public int RegionID { get; set; }
        [StringLength(10, MinimumLength = 1, ErrorMessage = "Maximum 10 Character Allowed.")]
        [Required(ErrorMessage = "Required")]
        [RegularExpression(@"^\S*$", ErrorMessage = "Spaces not allowed.")]
        public string PAY_REGION_CODE { get; set; }
        [StringLength(10, MinimumLength = 1, ErrorMessage = "Maximum 10 Character Allowed.")]
        [Required(ErrorMessage = "Required")]
        [RegularExpression(@"^\S*$", ErrorMessage = "Spaces not allowed.")]
        public string ERP_REGION_CODE { get; set; }
        [Required(ErrorMessage = "Required")]
        public string REGION_NAME { get; set; }
        public SelectList CompList { get; set; }

        public string RegionID_TEXT { get; set; }
        public string PAY_REGION_CODE_TEXT { get; set; }
        public string ERP_REGION_CODE_TEXT { get; set; }
        public string REGION_NAME_TEXT { get; set; }
        public string Screen_Name { set; get; }
        public string ISACTIVE_TEXT { get; set; }
        public void SetDisplayName()
        {
            MasterMetaRepo ObjMetaRepo = new MasterMetaRepo(this.EID, "IVAP_MST_REGION", "ViewRegion");

            this.RegionID_TEXT = ObjMetaRepo.GetDisPlayName("RegionID");
            this.PAY_REGION_CODE_TEXT = ObjMetaRepo.GetDisPlayName("PAY_REGION_CODE");
            this.ERP_REGION_CODE_TEXT = ObjMetaRepo.GetDisPlayName("ERP_REGION_CODE");
            this.REGION_NAME_TEXT = ObjMetaRepo.GetDisPlayName("REGION_NAME");
            this.ISACTIVE_TEXT = ObjMetaRepo.GetDisPlayName("ISACTIVE");
            this.Screen_Name = ObjMetaRepo.Screen_Name;
        }
    }
    public class RegionModelVM : BaseModel
    {
        public int RegionID { get; set; }
        [StringLength(10, MinimumLength = 1, ErrorMessage = "Maximum 10 Character Allowed.")]
        [Required(ErrorMessage = "Required")]
        [RegularExpression(@"^\S*$", ErrorMessage = "Spaces not allowed.")]
        public string PAY_REGION_CODE { get; set; }
        [StringLength(10, MinimumLength = 1, ErrorMessage = "Maximum 10 Character Allowed.")]
        [Required(ErrorMessage = "Required")]
        [RegularExpression(@"^\S*$", ErrorMessage = "Spaces not allowed.")]
        public string ERP_REGION_CODE { get; set; }
        [Required(ErrorMessage = "Required")]
        public string REGION_NAME { get; set; }
    }
}