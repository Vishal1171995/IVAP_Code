using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Ivap.Models;
using Ivap.Areas.Master.Repository;

namespace Ivap.Areas.Master.Models
{
    public class CostCentreModel : BaseModel
    {
        public int CostCenterID { get; set; }
        public int ENTITY_ID { get; set; }
        [StringLength(10, MinimumLength = 1, ErrorMessage = "Maximum 10 Character Allowed.")]
        [Required(ErrorMessage = "Required")]
        [RegularExpression(@"^\S*$", ErrorMessage = "Spaces not allowed.")]
        public string PAY_COST_CODE { get; set; }
        [StringLength(10, MinimumLength = 1, ErrorMessage = "Maximum 10 Character Allowed.")]
        [Required(ErrorMessage = "Required")]
        [RegularExpression(@"^\S*$", ErrorMessage = "Spaces not allowed.")]
        public string ERP_COST_CODE { get; set; }
        [Required(ErrorMessage ="Please Enter Cost Name.")]
        public string COST_NAME { get; set; }

        public SelectList CompanyList { get; set; }

        public string ENTITY_ID_TEXT { get; set; }
        public string PAY_COST_CODE_TEXT { get; set; }
        public string ERP_COST_CODE_TEXT { get; set; }
        public string COST_NAME_TEXT { get; set; }
        public string ISACTIVE_TEXT { get; set; }
        public string Screen_Name { set; get; }
        public void SetDisplayName()
        {
            MasterMetaRepo ObjMetaRepo = new MasterMetaRepo(this.EID, "IVAP_MST_COSTCENTRE", "ViewCostCentre");

            //this.COMP_ID_TEXT = ObjMetaRepo.GetDisPlayName("COMP_ID");
            this.ENTITY_ID_TEXT = ObjMetaRepo.GetDisPlayName("ENTITY_ID");
            this.PAY_COST_CODE_TEXT = ObjMetaRepo.GetDisPlayName("PAY_COST_CODE");
            this.ERP_COST_CODE_TEXT = ObjMetaRepo.GetDisPlayName("ERP_COST_CODE");
            this.COST_NAME_TEXT = ObjMetaRepo.GetDisPlayName("COST_NAME");
            this.ISACTIVE_TEXT = ObjMetaRepo.GetDisPlayName("ISACTIVE");
            this.Screen_Name = ObjMetaRepo.Screen_Name;
        }
    }


    public class CostCentreVM : BaseModel
    {
        public int CostCenterID { get; set; }
        //[Required(ErrorMessage = "Please Select Company Name.")]
        //public string COMP_NAME { get; set; }
        [StringLength(10, MinimumLength = 1, ErrorMessage = "Maximum 10 Character Allowed.")]
        [Required(ErrorMessage = "Required")]
        [RegularExpression(@"^\S*$", ErrorMessage = "Spaces not allowed.")]
        public string PAY_COST_CODE { get; set; }
        [StringLength(10, MinimumLength = 1, ErrorMessage = "Maximum 10 Character Allowed.")]
        [Required(ErrorMessage = "Required")]
        [RegularExpression(@"^\S*$", ErrorMessage = "Spaces not allowed.")]
        public string ERP_COST_CODE { get; set; }
        [Required(ErrorMessage = "Please Enter Cost Name.")]
        public string COST_NAME { get; set; }

    }

    public class GridCostCentre
    {
        public int from { get; set; }
        public int To { get; set; }
        public string SortingStr { get; set; }
        public string FilterStr { get; set; }

    }
}