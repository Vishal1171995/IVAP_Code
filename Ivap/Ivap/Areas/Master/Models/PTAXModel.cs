using Ivap.Areas.Master.CustomValidation;

using Ivap.Areas.Master.Repository;
using Ivap.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Ivap.Areas.Master.Models
{
    public class PTAXModel: BaseModel
    {
        public int PTAXID { get; set; }
        [Required(ErrorMessage = "Please Select State")]
        [Range(1, int.MaxValue, ErrorMessage = "Please Select State.")]
        public int STATE_ID { get; set; }
        [Required(ErrorMessage = "Please Select Period Flag")]
        public string PERIOD_FLAG { get; set; }

        //[Required(ErrorMessage = "Please Select Month")]
        //[Range(1, int.MaxValue, ErrorMessage = "Please Select Month.")]
        public int DED_MONTH { get; set; }
        [Required(ErrorMessage = "Please Select Month From")]
        [Range(1, int.MaxValue, ErrorMessage = "Please Select Month From.")]
        public int YTD_MONTH_FROM { get; set; }

        [Required(ErrorMessage = "Please Select Month To")]
        [Range(1, int.MaxValue, ErrorMessage = "Please Select Month To.")]
        public int YTD_MONTH_TO { get; set; }

        //[Required(ErrorMessage = "Salary From required")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:c}")]
        [Range(0.0001, int.MaxValue, ErrorMessage = "Salary From required")]
        public decimal PT_SAL_FROM { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:c}")]
        // [Required(ErrorMessage = "Salary To required")]
        [Range(0.0001, int.MaxValue, ErrorMessage = "Salary To required")]
        public decimal PT_SAL_TO { get; set; }
        [Required(ErrorMessage = "Please select gender")]
        public string GENDER { get; set; }

       // [Required(ErrorMessage = "PTax is required")]
        [Range(0.0001, int.MaxValue, ErrorMessage = "PTax is required")]
        public decimal PTAX { get; set; }
        [Required(ErrorMessage = "EFF From Date is required")]
        public DateTime EFF_FROM_DT { get; set; }
        [Required(ErrorMessage = "EFF To Date is required")]
        [DateValidationPtex]
        public DateTime EFF_TO_DT { get; set; }
        public SelectList StateList { get; set; }
        public SelectList DED_MONTHList { get; set; }
        public SelectList YTD_MONTH_FROMList { get; set; }
        public SelectList YTD_MONTH_TOList { get; set; }
        public string PTAXID_TEXT { get; set; }
        public string STATE_ID_TEXT { get; set; }
        public string PERIOD_FLAG_TEXT { get; set; }
        public string DED_MONTH_TEXT { get; set; }
        public string YTD_MONTH_FROM_TEXT { get; set; }
        public string YTD_MONTH_TO_TEXT { get; set; }
        public string PT_SAL_FROM_TEXT { get; set; }
        public string PT_SAL_TO_TEXT { get; set; }
        public string GENDER_TEXT { get; set; }
        public string PTAX_TEXT { get; set; }
        public string EFF_FROM_DT_TEXT { get; set; }
        public string EFF_TO_DT_TEXT { get; set; }
        public string Screen_Name { set; get; }
        public void SetDisplayName()
        {
            MasterMetaRepo ObjMetaRepo = new MasterMetaRepo(this.EID, "IVAP_MST_PTAX", "ViewPTAX");

            this.PTAXID_TEXT = ObjMetaRepo.GetDisPlayName("PTAXID");
            this.STATE_ID_TEXT = ObjMetaRepo.GetDisPlayName("STATE_ID");
            this.PERIOD_FLAG_TEXT = ObjMetaRepo.GetDisPlayName("PERIOD_FLAG");
            this.DED_MONTH_TEXT = ObjMetaRepo.GetDisPlayName("DED_MONTH");
            this.YTD_MONTH_FROM_TEXT = ObjMetaRepo.GetDisPlayName("YTD_MONTH_FROM");
            this.YTD_MONTH_TO_TEXT = ObjMetaRepo.GetDisPlayName("YTD_MONTH_TO");
            this.PT_SAL_FROM_TEXT = ObjMetaRepo.GetDisPlayName("PT_SAL_FROM");
            this.PT_SAL_TO_TEXT = ObjMetaRepo.GetDisPlayName("PT_SAL_TO");
            this.GENDER_TEXT = ObjMetaRepo.GetDisPlayName("GENDER");
            this.PTAX_TEXT = ObjMetaRepo.GetDisPlayName("PTAX");
            this.EFF_FROM_DT_TEXT = ObjMetaRepo.GetDisPlayName("EFF_FROM_DT");
            this.EFF_TO_DT_TEXT = ObjMetaRepo.GetDisPlayName("EFF_TO_DT");
            this.Screen_Name = ObjMetaRepo.Screen_Name;
        }
    }

    public class PtaxModelVW : BaseModel
    {
        public int PTAXID { get; set; }
        public int STATE_ID { get; set; }
        [Required(ErrorMessage = "Please Select Period Flag")]
        public string PERIOD_FLAG { get; set; }

        [Required(ErrorMessage = "Please Select Month")]
        public int DED_MONTH { get; set; }
        [Required(ErrorMessage = "Please Select Month From")]
        public int YTD_MONTH_FROM { get; set; }
        [Range(0.0001, int.MaxValue, ErrorMessage = "Salary To required")]
        public decimal PT_SAL_TO { get; set; }

        [Required(ErrorMessage = "Please Select Month To")]
        public int YTD_MONTH_TO { get; set; }

        [Range(0.0001, int.MaxValue, ErrorMessage = "Salary From required")]
        public decimal PT_SAL_FROM { get; set; }

        [Required(ErrorMessage = "Please select gender")]
        public string GENDER { get; set; }

        [Range(0.0001, int.MaxValue, ErrorMessage = "PTax is required")]
        public decimal PTAX { get; set; }
        [Required(ErrorMessage = "EFF From Date is required")]
        public DateTime EFF_FROM_DT { get; set; }
        [Required(ErrorMessage = "EFF To Date is required")]
        //[DateValidation]
        public DateTime EFF_TO_DT { get; set; }
        public string StateName { get; set; }
        public string DED_MONTH_ { get; set; }
        public string YTD_MONTH_FROM_ { get; set; }
        public string YTD_MONTH_TO_ { get; set; }
    }
}