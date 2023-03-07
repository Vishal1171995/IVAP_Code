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
    public class RoleModel:BaseModel
    {
        public int? RoleID { set; get; }
        //[Required]
        //[Range(1, int.MaxValue, ErrorMessage = "Please select circle state.")]
        //public int EntityID { get; set; }

        [Required(ErrorMessage = "Required")]
        [StringLength(100, ErrorMessage = "Role Name can be min 4 and  max 100 characters long.", MinimumLength = 4)]
        public string RoleName { set; get; }

        [Required(ErrorMessage = "Required")]
        public string RoleType { set; get; }

        [Required(ErrorMessage = "Required")]
        public bool IsActive { set; get; }

        [Required(ErrorMessage = "Required")]
        public int CreatedBy { set; get; }

        public SelectList EntityList { get; set; }


        public string RoleID_TEXT { set; get; }
        public string RoleName_TEXT { set; get; }
        public string RoleType_TEXT { set; get; }
        public string Screen_Name { set; get; }
        public void SetDisplayName()
        {
            MasterMetaRepo ObjMetaRepo = new MasterMetaRepo(this.EID, "IVAP_MST_USERROLE", "ViewRoles");

           // this.RoleID_TEXT = ObjMetaRepo.GetDisPlayName("COMP_ID");
            this.RoleName_TEXT = ObjMetaRepo.GetDisPlayName("ROLENAME");
            this.RoleType_TEXT = ObjMetaRepo.GetDisPlayName("ROLETYPE");
          //  this.Location_Name_TEXT = ObjMetaRepo.GetDisPlayName("Loc_Name");
            
            this.Screen_Name = ObjMetaRepo.Screen_Name;
        }


    }

    public class GridRoles
    {
        public int from { get; set; }
        public int To { get; set; }
        public string SortingStr { get; set; }
        public string FilterStr { get; set; }

    }
}