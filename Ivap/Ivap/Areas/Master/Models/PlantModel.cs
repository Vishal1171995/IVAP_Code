using Ivap.Areas.Master.Repository;
using Ivap.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Ivap.Areas.Master.Models
{
    public class PlantModel:BaseModel
    {
        public int PlantID { get; set; }
        [StringLength(10, MinimumLength = 1, ErrorMessage = "Maximum 10 Character Allowed.")]
        [Required(ErrorMessage = "Required")]
        [RegularExpression(@"^\S*$", ErrorMessage = "Spaces not allowed.")]
        public string PAY_PLANT_CODE { get; set; }
        [StringLength(10, MinimumLength = 1, ErrorMessage = "Maximum 10 Character Allowed.")]
        [Required(ErrorMessage = "Required")]
        [RegularExpression(@"^\S*$", ErrorMessage = "Spaces not allowed.")]
        public string ERP_PLANT_CODE { get; set; }
        [Required(ErrorMessage = "Required")]
        public string PLANT_NAME { get; set; }
        public string PlantID_TEXT { get; set; }
        public string PAY_PLANT_CODE_TEXT { get; set; }
        public string ERP_PLANT_CODE_TEXT { get; set; }
        public string PLANT_NAME_TEXT { get; set; }
        public string Screen_Name { set; get; }
        public string ISACTIVE_TEXT { get; set; }
        public void SetDisplayName()
        {
            MasterMetaRepo ObjMetaRepo = new MasterMetaRepo(this.EID, "IVAP_MST_PLANT", "ViewPlant");
            this.Screen_Name = ObjMetaRepo.Screen_Name;
            this.PlantID_TEXT = ObjMetaRepo.GetDisPlayName("PlantID");
            this.PAY_PLANT_CODE_TEXT = ObjMetaRepo.GetDisPlayName("PAY_PLANT_CODE");
            this.ERP_PLANT_CODE_TEXT = ObjMetaRepo.GetDisPlayName("ERP_PLANT_CODE");
            this.PLANT_NAME_TEXT = ObjMetaRepo.GetDisPlayName("PLANT_NAME");
            this.ISACTIVE_TEXT = ObjMetaRepo.GetDisPlayName("ISACTIVE");
        }
    }
}