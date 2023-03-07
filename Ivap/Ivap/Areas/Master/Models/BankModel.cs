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
    public class BankModel : BaseModel
    {
        public int BANKID { get; set; }
       
        public string BANK_NAME { get; set; }
       
        public string BANK_ADDR { get; set; }

       
        public string BANK_PIN { get; set; }
      
        public string BANK_PHONE { get; set; }

        //[Required(ErrorMessage = "Please Select Company Name.")]
        //[Range(1, int.MaxValue, ErrorMessage = "Please Select Company Name.")]

        //[Required(ErrorMessage = "Required")]
        //[Range(1, int.MaxValue, ErrorMessage = "Required")]
        public int BANK_STATE { get; set; }


        public string BANK_CODE { get; set; }
        [Required(ErrorMessage = "Required")]
        public string BANK_CITY { get; set; }

        [StringLength(20, MinimumLength = 1, ErrorMessage = "Maximum 10 Character Allowed.")]
        [Required(ErrorMessage = "Required")]
        [RegularExpression(@"^\S*$", ErrorMessage = "Spaces not allowed.")]
        public string PAY_BANK_CODE { get; set; }
        [StringLength(20, MinimumLength = 1, ErrorMessage = "Maximum 10 Character Allowed.")]
        [Required(ErrorMessage = "Required")]
        [RegularExpression(@"^\S*$", ErrorMessage = "Spaces not allowed.")]
        public string ERP_BANK_CODE { get; set; }
        //[Required(ErrorMessage = "Required")]
        public string IFSC_Code { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "Required")]
        public int GLOBAL_BANK_ID { get; set; }

        public SelectList CompanyList { get; set; }
        public SelectList BankStateList { get; set; }
        public SelectList BankNameList { get; set; }
        public string GLOBAL_BANK_NAME_TEXT { get; set; }
        public string BANKID_TEXT { get; set; }
        public string BANK_NAME_TEXT { get; set; }
        public string BANK_ADDR_TEXT { get; set; }
        public string BANK_PIN_TEXT { get; set; }
        public string BANK_PHONE_TEXT { get; set; }
        public string ENTITY_ID_TEXT { get; set; }
        public string BANK_STATE_TEXT { get; set; }
        public string BANK_CODE_TEXT { get; set; }
        public string BANK_CITY_TEXT { get; set; }
        public string IFSC_Code_TEXT { get; set; }
      
        public string ERP_BANK_CODE_TEXT { get; set; }
        public string PAY_BANK_CODE_TEXT { get; set; }
        public string ISACTIVE_TEXT { get; set; }
        public string Screen_Name { set; get; }
        public void SetDisplayName()
        {
            MasterMetaRepo ObjMetaRepo = new MasterMetaRepo(this.EID, "IVAP_MST_BANK", "ViewBank");
            this.GLOBAL_BANK_NAME_TEXT = ObjMetaRepo.GetDisPlayName("GLOBAL_BANK_NAME");
            this.BANKID_TEXT = ObjMetaRepo.GetDisPlayName("BANKID");
            this.BANK_NAME_TEXT = ObjMetaRepo.GetDisPlayName("BANK_NAME");
            this.BANK_ADDR_TEXT = ObjMetaRepo.GetDisPlayName("BANK_ADDR");
            this.BANK_PIN_TEXT = ObjMetaRepo.GetDisPlayName("BANK_PIN");
            this.BANK_PHONE_TEXT = ObjMetaRepo.GetDisPlayName("BANK_PHONE");
            this.ENTITY_ID_TEXT = ObjMetaRepo.GetDisPlayName("ENTITY_ID");
            this.BANK_STATE_TEXT = ObjMetaRepo.GetDisPlayName("BANK_STATE");
            this.BANK_CODE_TEXT = ObjMetaRepo.GetDisPlayName("BANK_CODE");
            this.BANK_CITY_TEXT = ObjMetaRepo.GetDisPlayName("BANK_CITY");
            this.IFSC_Code_TEXT = ObjMetaRepo.GetDisPlayName("IFSC");
            this.ERP_BANK_CODE_TEXT = ObjMetaRepo.GetDisPlayName("ERP_BANK_CODE");
            this.PAY_BANK_CODE_TEXT = ObjMetaRepo.GetDisPlayName("PAY_BANK_CODE");
            this.ISACTIVE_TEXT = ObjMetaRepo.GetDisPlayName("ISACTIVE");
            // this.ENTITY_ID_TEXT = ObjMetaRepo.GetDisPlayName("ENTITY_ID");
            this.Screen_Name = ObjMetaRepo.Screen_Name;
        }
    }


    public class BankVM : BaseModel
    {
        public int BANKID { get; set; }
        [Required(ErrorMessage = "Required")]
        public string BANK_NAME { get; set; }
        [Required(ErrorMessage = "Required")]
        public string Global_Bank_Name { get; set; }
       
        public string BANK_ADDR { get; set; }

      
        public string BANK_PIN { get; set; }
      
        public string BANK_PHONE { get; set; }

        //[Required(ErrorMessage = "Please Select Company Name.")]
        //public string COMP_Name { get; set; }

        //[Required(ErrorMessage = "Required")]
        public string BANK_STATE { get; set; }


        public string BANK_CODE { get; set; }
        [Required(ErrorMessage = "Required")]
        public string BANK_CITY { get; set; }

        [StringLength(20, MinimumLength = 1, ErrorMessage = "Maximum 10 Character Allowed.")]
        [Required(ErrorMessage = "Required")]
        [RegularExpression(@"^\S*$", ErrorMessage = "Spaces not allowed.")]
        public string PAY_BANK_CODE { get; set; }
        [StringLength(20, MinimumLength = 1, ErrorMessage = "Maximum 10 Character Allowed.")]
        [Required(ErrorMessage = "Required")]
        [RegularExpression(@"^\S*$", ErrorMessage = "Spaces not allowed.")]
        public string ERP_BANK_CODE { get; set; }
        //[Required(ErrorMessage = "Required")]
        public string IFSC_Code { get; set; }

       
        public int GLOBAL_BANK_ID { get; set; }
        public string GLOBAL_BANK_NAME_TEXT { get; set; }
        public string BANKID_TEXT { get; set; }
        public string BANK_NAME_TEXT { get; set; }
        public string BANK_ADDR_TEXT { get; set; }
        public string BANK_PIN_TEXT { get; set; }
        public string BANK_PHONE_TEXT { get; set; }
        public string ENTITY_ID_TEXT { get; set; }
        public string BANK_STATE_TEXT { get; set; }
        public string BANK_CODE_TEXT { get; set; }
        public string BANK_CITY_TEXT { get; set; }
        public string IFSC_Code_TEXT { get; set; }
        public string ERP_BANK_CODE_TEXT { get; set; }
        public string PAY_BANK_CODE_TEXT { get; set; }
        public string ISACTIVE_TEXT { get; set; }
        public string Screen_Name { set; get; }

        public void SetDisplayName()
        {
            MasterMetaRepo ObjMetaRepo = new MasterMetaRepo(this.EID, "IVAP_MST_BANK", "ViewBank");
            this.GLOBAL_BANK_NAME_TEXT = ObjMetaRepo.GetDisPlayName("GLOBAL_BANK_NAME");
            this.BANKID_TEXT = ObjMetaRepo.GetDisPlayName("BANKID");
            this.BANK_NAME_TEXT = ObjMetaRepo.GetDisPlayName("BANK_NAME");
            this.BANK_ADDR_TEXT = ObjMetaRepo.GetDisPlayName("BANK_ADDR");
            this.BANK_PIN_TEXT = ObjMetaRepo.GetDisPlayName("BANK_PIN");
            this.BANK_PHONE_TEXT = ObjMetaRepo.GetDisPlayName("BANK_PHONE");
            this.ENTITY_ID_TEXT = ObjMetaRepo.GetDisPlayName("ENTITY_ID");
            this.BANK_STATE_TEXT = ObjMetaRepo.GetDisPlayName("BANK_STATE");
            this.BANK_CODE_TEXT = ObjMetaRepo.GetDisPlayName("BANK_CODE");
            this.BANK_CITY_TEXT = ObjMetaRepo.GetDisPlayName("BANK_CITY");
            this.IFSC_Code_TEXT = ObjMetaRepo.GetDisPlayName("IFSC");
            this.ERP_BANK_CODE_TEXT = ObjMetaRepo.GetDisPlayName("ERP_BANK_CODE");
            this.PAY_BANK_CODE_TEXT = ObjMetaRepo.GetDisPlayName("PAY_BANK_CODE");
            this.ISACTIVE_TEXT = ObjMetaRepo.GetDisPlayName("ISACTIVE");
            // this.ENTITY_ID_TEXT = ObjMetaRepo.GetDisPlayName("ENTITY_ID");
            this.Screen_Name = ObjMetaRepo.Screen_Name;
        }

    }

    public class GridBank
    {
        public int from { get; set; }
        public int To { get; set; }
        public string SortingStr { get; set; }
        public string FilterStr { get; set; }

    }
}