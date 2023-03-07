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
    public class LWFModel: BaseModel
    {
        public int LWFID { get; set; }
        [Required(ErrorMessage = "Please Select State")]
        [Range(1, int.MaxValue, ErrorMessage = "Please Select State.")]
        public int? State_Id { get; set; }
        public int? Location_Id { get; set; }
        [Required(ErrorMessage = "Please select period flag.")]
        public string Period_Flag { get; set; }
        [Required(ErrorMessage = "Please select ded month.")]

        public int Ded_Month { get; set; }
        [Required(ErrorMessage = "Lwf employee required.")]
        public string Lwf_Employee { get; set; }
        [Required(ErrorMessage = "Lwf employer required.")]
        public string Lwf_Employer { get; set; }
        [Required(ErrorMessage = "Eff from date required.")]
        public string Eff_From_DT { get; set; }
        [Required(ErrorMessage = "Eff to date required.")]
        public string Eff_To_DT { get; set; }

        public string LWFID_TEXT{ get; set; }
        public string State_Id_TEXT { get; set; }
        public string Location_Id_TEXT { get; set; }
        public string Period_Flag_TEXT { get; set; }
        public string Ded_Month_TEXT { get; set; }
        public string Lwf_Employee_TEXT { get; set; }
        public string Lwf_Employer_TEXT { get; set; }
        public string Eff_From_DT_TEXT { get; set; }
        public string Eff_To_DT_TEXT { get; set; }
        public string Screen_Name { set; get; }
        public void SetDisplayName()
        {
            MasterMetaRepo ObjMetaRepo = new MasterMetaRepo(this.EID, "IVAP_MST_LWF", "ViewLwf");

            // this.Company_Id_TEXT = ObjMetaRepo.GetDisPlayName("COMP_ID");
            this.LWFID_TEXT = ObjMetaRepo.GetDisPlayName("LWFID");
            this.State_Id_TEXT = ObjMetaRepo.GetDisPlayName("State_Id");
            this.Location_Id_TEXT = ObjMetaRepo.GetDisPlayName("Location_Id");
            this.Period_Flag_TEXT = ObjMetaRepo.GetDisPlayName("Period_Flag");
            this.Ded_Month_TEXT = ObjMetaRepo.GetDisPlayName("Ded_Month");
            this.Lwf_Employee_TEXT = ObjMetaRepo.GetDisPlayName("Lwf_Employee");
            this.Lwf_Employer_TEXT = ObjMetaRepo.GetDisPlayName("Lwf_Employer");
            this.Eff_From_DT_TEXT = ObjMetaRepo.GetDisPlayName("Eff_From_DT");
            this.Eff_To_DT_TEXT = ObjMetaRepo.GetDisPlayName("Eff_To_DT");
            this.Screen_Name = ObjMetaRepo.Screen_Name;
        }




        // public string CreatedOn { get; set; }

        //  public int CreatedBy { get; set; }

        // public string UpdateOn { get; set; }

        // public string UpdatedBy { get; set; }

        //// public int IsActive { get; set; }

        // public string Action { get; set; }

        public SelectList StateList { get; set; }

        public SelectList DED_MONTHList { get; set; }
    }
    public enum Month
    {
        None,
        January,
        February,
        March,
        April,
        May,
        June,
        July,
        August,
        September,
        October,
        November,
        December,
    }


    public class LWFModelVW : BaseModel
    {
        public int LWFID { get; set; }

        [Required(ErrorMessage = "Please Select State")]
        [Range(1, int.MaxValue, ErrorMessage = "Please Select State.")]
        public int? State_Id { get; set; }
       
        public int? Location_Id { get; set; }
        [Required(ErrorMessage = "Please Select period flag")]
        public string Period_Flag { get; set; }
        [Required(ErrorMessage = "Please Select period flag")]
        [Range(1, int.MaxValue, ErrorMessage = "Please Select ded month.")]

        public int Ded_Month { get; set; }
        [Required(ErrorMessage = "Lwf employee required.")]
        public string Lwf_Employee { get; set; }
        [Required(ErrorMessage = "Lwf employer required.")]
        public string Lwf_Employer { get; set; }
        [Required(ErrorMessage = "Eff from Date required.")]
        public string Eff_From_DT { get; set; }
        [Required(ErrorMessage = "Eff to date required.")]
        public string Eff_To_DT { get; set; }
        public string DedMonthName { get; set; }
        public string StateName { get; set; }
        public string LocationName { get; set; }

    }
}