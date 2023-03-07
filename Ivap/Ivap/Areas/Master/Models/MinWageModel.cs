using Ivap.Areas.Master.CustomValidation;
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
    public class MinWageModel:BaseModel
    {
        public int MinWageID { get; set; }
        [Required(ErrorMessage = "Required")]
        [Range(1, int.MaxValue, ErrorMessage = "Please select state.")]
        public int STATE_ID { get; set; }
        //[Required(ErrorMessage = "Please select location.")]
        //[Range(1, int.MaxValue, ErrorMessage = "Please select location.")]
        //public int LOCATION_ID { get; set; }
        [Required(ErrorMessage = "Required")]
        public string CATEGORY { get; set; }
        [Required(ErrorMessage = "Required")]
        public decimal? MIN_WAGE { get; set; }
        [Required(ErrorMessage = "Required")]
        public DateTime? EFF_DT_FROM { get; set; }
        [Required(ErrorMessage = "Required")]
        [DateValidation]
        public DateTime? EFF_DATE_TO { get; set; }
        public SelectList StateList { get; set; }
        public SelectList LocationList { get; set; }

        public string MinWageID_TEXT { get; set; }
        public string STATE_ID_TEXT { get; set; }
        public string CATEGORY_TEXT { get; set; }
        public string MIN_WAGE_TEXT { get; set; }
        public string EFF_DT_FROM_TEXT { get; set; }
        public string EFF_DATE_TO_TEXT { get; set; }

        public string Screen_Name { set; get; }
        public void SetDisplayName()
        {
            MasterMetaRepo ObjMetaRepo = new MasterMetaRepo(this.EID, "IVAP_MST_MINWAGE", "ViewMinWage");

            this.MinWageID_TEXT = ObjMetaRepo.GetDisPlayName("MinWageID");
            this.STATE_ID_TEXT = ObjMetaRepo.GetDisPlayName("STATE_ID");
            this.CATEGORY_TEXT = ObjMetaRepo.GetDisPlayName("CATEGORY");
            this.MIN_WAGE_TEXT = ObjMetaRepo.GetDisPlayName("MIN_WAGE");
            this.EFF_DT_FROM_TEXT = ObjMetaRepo.GetDisPlayName("EFF_DT_FROM");
            this.EFF_DATE_TO_TEXT = ObjMetaRepo.GetDisPlayName("EFF_DATE_TO");
            this.Screen_Name = ObjMetaRepo.Screen_Name;
        }
    }
    public class MinWageModelVM : BaseModel
    {
        public int MinWageID { get; set; }
        [Required(ErrorMessage = "Required")]
        public string STATE_ID { get; set; }
        [Required(ErrorMessage = "Required")]
        public string LOCATION_ID { get; set; }
        [Required(ErrorMessage = "Required")]
        public string CATEGORY { get; set; }
        [Required(ErrorMessage = "Required")]
        public decimal? MIN_WAGE { get; set; }
        [Required(ErrorMessage = "Required")]
        public string EFF_DT_FROM { get; set; }
        [Required(ErrorMessage = "Required")]
        public string EFF_DATE_TO { get; set; }
    }
}