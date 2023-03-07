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
    public class Location:BaseModel
    {
        [Required(ErrorMessage = "Required")]
        public int? Location_Id { set; get; }
        [StringLength(10, MinimumLength = 1, ErrorMessage = "Maximum 10 Character Allowed.")]
        [Required(ErrorMessage = "Required")]
        [RegularExpression(@"^\S*$", ErrorMessage = "Spaces not allowed.")]
        public string Pay_Loc_Code { set; get; }
        [StringLength(10, MinimumLength = 1, ErrorMessage = "Maximum 10 Character Allowed.")]
        [Required(ErrorMessage = "Required")]
        [RegularExpression(@"^\S*$", ErrorMessage = "Spaces not allowed.")]
        public string Erp_Loc_Code { set; get; }
        [Required(ErrorMessage = "Required")]
        [StringLength(100, ErrorMessage = "Location Name can be  max 100 characters long.")]
        public string Location_Name { set; get; }
        [Required(ErrorMessage = "Required")]
        [Range(1, int.MaxValue, ErrorMessage = "Please select state.")]
        public int State_Id { set; get; }
        public string Parent_Location_Name { set; get; }
        public bool Is_Metro { set; get; }
        [Required(ErrorMessage = "Required")]
        [Range(1, int.MaxValue, ErrorMessage = "Select Location Name")]
        public int PARENT_LOC_ID { get; set; }
        public SelectList StateList { get; set; }
        public SelectList CompanyList { get; set; }
        public SelectList LocNameList { get; set; }
        public string Pay_Loc_Code_TEXT { set; get; }
        public string Erp_Loc_Code_TEXT { set; get; }
        public string Location_Name_TEXT { set; get; }
        public string State_Id_TEXT { set; get; }
        public string Company_Id_TEXT { get; set; }
        public string Is_Metro_TEXT { set; get; }
        public string ISACTIVE_TEXT { get; set; }
        public string PARENT_LOC_ID_TEXT { get; set; }
        public string Screen_Name { set; get; }
        public void SetDisplayName()
        {
            MasterMetaRepo ObjMetaRepo = new MasterMetaRepo(this.EID, "IVAP_MST_LOCATION", "ViewLocation");
            this.Pay_Loc_Code_TEXT = ObjMetaRepo.GetDisPlayName("Pay_Loc_Code");
            this.Erp_Loc_Code_TEXT = ObjMetaRepo.GetDisPlayName("Erp_Loc_Code");
            this.Location_Name_TEXT = ObjMetaRepo.GetDisPlayName("Loc_Name");
            this.State_Id_TEXT = ObjMetaRepo.GetDisPlayName("State_Id");
            this.Is_Metro_TEXT = ObjMetaRepo.GetDisPlayName("IsMetro");
            this.ISACTIVE_TEXT = ObjMetaRepo.GetDisPlayName("ISACTIVE");
            this.PARENT_LOC_ID_TEXT = ObjMetaRepo.GetDisPlayName("PARENT_LOC_ID");
            this.Screen_Name = ObjMetaRepo.Screen_Name;
        }

    }
    public class LocationModelVM:BaseModel
    {
        public int Location_Id { set; get; }
        [StringLength(10, MinimumLength = 1, ErrorMessage = "Maximum 10 Character Allowed.")]
        [Required(ErrorMessage = "Required")]
        [RegularExpression(@"^\S*$", ErrorMessage = "Spaces not allowed.")]
        public string Pay_Loc_Code { set; get; }
        [StringLength(10, MinimumLength = 1, ErrorMessage = "Maximum 10 Character Allowed.")]
        [Required(ErrorMessage = "Required")]
        [RegularExpression(@"^\S*$", ErrorMessage = "Spaces not allowed.")]
        public string Erp_Loc_Code { set; get; }
        [Required(ErrorMessage = "Required")]
        public string Location_Name { set; get; }
        [Required(ErrorMessage = "Required")]
        public string Parent_Location_Name { set; get; }
        public bool Is_Metro { set; get; }
        public string State_Id { set; get; }
        public string StateList { get; set; }
        public string LocNameList { get; set; }
        public string PARENT_LOC_ID { get; set; }
    }
}