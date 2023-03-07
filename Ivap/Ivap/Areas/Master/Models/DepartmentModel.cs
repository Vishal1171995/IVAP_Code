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
    public class DepartmentModel : BaseModel
    {
        public int DEPTID { get; set; }
        public int ENTITY_ID { get; set; }
        [StringLength(10, MinimumLength = 1, ErrorMessage = "Maximum 10 Character Allowed.")]
        [Required(ErrorMessage = "Required")]
        [RegularExpression(@"^\S*$", ErrorMessage = "Spaces not allowed.")]
        public string PAY_DEPT_CODE { get; set; }
        [StringLength(10, MinimumLength = 1, ErrorMessage = "Maximum 10 Character Allowed.")]
        [Required(ErrorMessage = "Required")]
        [RegularExpression(@"^\S*$", ErrorMessage = "Spaces not allowed.")]
        public string ERP_DEPT_CODE { get; set; }
        [Required(ErrorMessage = "Required")]
        public string DEPT_NAME { get; set; }
        public SelectList CompanyList { get; set; }

        public string ENTITY_ID_TEXT { get; set; }
        public string PAY_DEPT_CODE_TEXT { get; set; }
        public string ERP_DEPT_CODE_TEXT { get; set; }
        public string DEPT_NAME_TEXT { get; set; }
        public string Screen_Name { set; get; }
        public string ISACTIVE_TEXT { get; set; }
        public void SetDisplayName()
        {
            MasterMetaRepo ObjMetaRepo = new MasterMetaRepo(this.EID, "IVAP_MST_DEPARTMENT", "ViewDepartment");


            this.ENTITY_ID_TEXT = ObjMetaRepo.GetDisPlayName("ENTITY_ID");
            this.PAY_DEPT_CODE_TEXT = ObjMetaRepo.GetDisPlayName("PAY_DEPT_CODE");
            this.ERP_DEPT_CODE_TEXT = ObjMetaRepo.GetDisPlayName("ERP_DEPT_CODE");
            this.DEPT_NAME_TEXT = ObjMetaRepo.GetDisPlayName("DEPT_NAME");
            this.ISACTIVE_TEXT = ObjMetaRepo.GetDisPlayName("ISACTIVE");
            this.Screen_Name = ObjMetaRepo.Screen_Name;
        }
    }

    public class DepartmentVM : BaseModel
    {
        public int DEPTID { get; set; }
        [StringLength(10, MinimumLength = 1, ErrorMessage = "Maximum 10 Character Allowed.")]
        [Required(ErrorMessage = "Required")]
        [RegularExpression(@"^\S*$", ErrorMessage = "Spaces not allowed.")]
        public string PAY_DEPT_CODE { get; set; }
        [StringLength(10, MinimumLength = 1, ErrorMessage = "Maximum 10 Character Allowed.")]
        [Required(ErrorMessage = "Required")]
        [RegularExpression(@"^\S*$", ErrorMessage = "Spaces not allowed.")]
        public string ERP_DEPT_CODE { get; set; }
        [Required(ErrorMessage = "Required")]
        public string DEPT_NAME { get; set; }

    }

    public class GridDepartment
    {
        public int from { get; set; }
        public int To { get; set; }
        public string SortingStr { get; set; }
        public string FilterStr { get; set; }

    }
}