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
    public class CompanyModel:BaseModel
    {
        public int CompID { get; set; }
        [Required(ErrorMessage = "Required")]
        public string COMP_CODE { get; set; }
        [Required(ErrorMessage = "Required")]
        public string COMP_NAME { get; set; }
        [Required(ErrorMessage = "Required")]
        public string COMP_ADDR1 { get; set; }
        public string COMP_ADDR2 { get; set; }
        [Required(ErrorMessage = "Required")]
        public string COMP_CITY { get; set; }
        [Required(ErrorMessage = "Required")]
        [Range(1, int.MaxValue, ErrorMessage = "Required")]
        public int COMP_STATE { get; set; }
        [Required(ErrorMessage = "Required")]
        [RegularExpression("^\\d{6,6}$", ErrorMessage = "Invalid")]
        public string COMP_PIN { get; set; }
        [Required(ErrorMessage = "Required")]
        [Range(1, int.MaxValue, ErrorMessage = "Required")]
        public int COMP_CLASS { get; set; }
        [Required(ErrorMessage = "Required")]
        //[RegularExpression("^[A-Z]{5}[0-9]{4}[A-Z]{1}$", ErrorMessage = "Invalid PAN.")]
        [StringLength(10, ErrorMessage = "Pan no can be 10 characters long.", MinimumLength = 10)]
        [RegularExpression(@"[A-Z]{5}\d{4}[A-Z]{1}", ErrorMessage = "* Invalid PAN Number")]
        public string COMP_PANNO { get; set; }

        [Required(ErrorMessage = "Required")]
        [RegularExpression("^[A-Z]{4}[0-9]{5}[A-Z]{1}$", ErrorMessage = "Invalid Tan.")]
        [StringLength(10, ErrorMessage = "tan no can be 10 characters long.", MinimumLength = 10)]
        public string COMP_TANNO { get; set; }
        [Required(ErrorMessage = "Required")]
        public string COMP_TDSCIRCLE { get; set; }
        public string SIGN_FNAME { get; set; }
        public string SIGN_LNAME { get; set; }
        public string SIGN_FATHER_NAME { get; set; }
        public string SIGN_ADDR1 { get; set; }
        public string SIGN_ADDR2 { get; set; }
        public string SIGN_CITY { get; set; }
        public string SIGN_DSG { get; set; }
        public int SIGN_STATE { get; set; }
        [RegularExpression("^\\d{6,6}$", ErrorMessage = "Invalid")]
        public string SIGN_PIN { get; set; }
        public string SIGN_PLACE { get; set; }
        public string SIGN_DATE { get; set; }
        [Required(ErrorMessage = "Required")]
        [Range(1, int.MaxValue, ErrorMessage = "Required")]
        public int RETIRE_AGE { get; set; }
        [Required]
        public bool EMP_CODE_GEN { get; set; }

        public string EMP_CODE_PREFIX { get; set; }
        [Required(ErrorMessage = "Required")]
        [Range(1, int.MaxValue, ErrorMessage = "Required")]
        public int EMP_CODE_LEN { get; set; }
        //[Required(ErrorMessage = "Please enter comp logo.")]
        public string Comp_Logo { get; set; }
        //[Required(ErrorMessage = "Required")]
        public string COMP_URL { get; set; }

        public SelectList StateList { get; set; }
        public SelectList ClassList { get; set; }
        public SelectList EntityList { get; set; }

        //TEXT_NAME
        public string EID_TEXT { get; set; }
        public string CompID_TEXT { get; set; }
        public string COMP_CODE_TEXT { get; set; }
        public string COMP_NAME_TEXT { get; set; }
        public string COMP_ADDR1_TEXT { get; set; }
        public string COMP_ADDR2_TEXT { get; set; }
        public string COMP_CITY_TEXT { get; set; }
        public string COMP_STATE_TEXT { get; set; }
        public string COMP_PIN_TEXT { get; set; }
        public string COMP_CLASS_TEXT { get; set; }
        public string COMP_PANNO_TEXT { get; set; }
        public string COMP_TANNO_TEXT { get; set; }
        public string COMP_TDSCIRCLE_TEXT { get; set; }
        public string SIGN_FNAME_TEXT { get; set; }
        public string SIGN_LNAME_TEXT { get; set; }
        public string SIGN_FATHER_NAME_TEXT { get; set; }
        public string SIGN_ADDR1_TEXT { get; set; }
        public string SIGN_ADDR2_TEXT { get; set; }
        public string SIGN_CITY_TEXT { get; set; }
        public string SIGN_DSG_TEXT { get; set; }
        public string SIGN_STATE_TEXT { get; set; }
        public string SIGN_PIN_TEXT { get; set; }
        public string SIGN_PLACE_TEXT { get; set; }
        public string SIGN_DATE_TEXT { get; set; }
        public string RETIRE_AGE_TEXT { get; set; }
        public string EMP_CODE_GEN_TEXT { get; set; }
        public string EMP_CODE_PREFIX_TEXT { get; set; }
        public string EMP_CODE_LEN_TEXT { get; set; }
        public string Comp_Logo_TEXT { get; set; }
        public string COMP_URL_TEXT { get; set; }

        public string Screen_Name { set; get; }
        public string ISACTIVE_TEXT { get; set; }
        public void SetDisplayName()
        {
            MasterMetaRepo ObjMetaRepo = new MasterMetaRepo(this.EID, "IVAP_MST_COMPANY", "ViewCompany");

            this.EID_TEXT = ObjMetaRepo.GetDisPlayName("EID");
            this.COMP_CODE_TEXT = ObjMetaRepo.GetDisPlayName("COMP_CODE");
            this.COMP_NAME_TEXT = ObjMetaRepo.GetDisPlayName("COMP_NAME");
            this.COMP_ADDR1_TEXT = ObjMetaRepo.GetDisPlayName("COMP_ADDR1");
            this.COMP_ADDR2_TEXT = ObjMetaRepo.GetDisPlayName("COMP_ADDR2");
            this.COMP_CITY_TEXT = ObjMetaRepo.GetDisPlayName("COMP_CITY");

            this.COMP_STATE_TEXT = ObjMetaRepo.GetDisPlayName("COMP_STATE");
            this.COMP_PIN_TEXT = ObjMetaRepo.GetDisPlayName("COMP_PIN");
            this.COMP_CLASS_TEXT = ObjMetaRepo.GetDisPlayName("COMP_CLASS");
            this.COMP_PANNO_TEXT = ObjMetaRepo.GetDisPlayName("COMP_PANNO");
            this.COMP_TANNO_TEXT = ObjMetaRepo.GetDisPlayName("COMP_TANNO");
            this.COMP_TDSCIRCLE_TEXT = ObjMetaRepo.GetDisPlayName("COMP_TDSCIRCLE");
            this.SIGN_FNAME_TEXT = ObjMetaRepo.GetDisPlayName("SIGN_FNAME");
            this.SIGN_LNAME_TEXT = ObjMetaRepo.GetDisPlayName("SIGN_LNAME");
            this.SIGN_FATHER_NAME_TEXT = ObjMetaRepo.GetDisPlayName("SIGN_FATHER_NAME");
            this.SIGN_ADDR1_TEXT = ObjMetaRepo.GetDisPlayName("SIGN_ADDR1");
            this.SIGN_ADDR2_TEXT = ObjMetaRepo.GetDisPlayName("SIGN_ADDR2");
            this.SIGN_CITY_TEXT = ObjMetaRepo.GetDisPlayName("SIGN_CITY");
            this.SIGN_DSG_TEXT = ObjMetaRepo.GetDisPlayName("SIGN_DSG");

            this.SIGN_STATE_TEXT = ObjMetaRepo.GetDisPlayName("SIGN_STATE");
            this.SIGN_PIN_TEXT = ObjMetaRepo.GetDisPlayName("SIGN_PIN");
            this.SIGN_PLACE_TEXT = ObjMetaRepo.GetDisPlayName("SIGN_PLACE");
            this.SIGN_DATE_TEXT = ObjMetaRepo.GetDisPlayName("SIGN_DATE");
            this.RETIRE_AGE_TEXT = ObjMetaRepo.GetDisPlayName("RETIRE_AGE");
            this.EMP_CODE_GEN_TEXT = ObjMetaRepo.GetDisPlayName("EMP_CODE_GEN");
            this.EMP_CODE_PREFIX_TEXT = ObjMetaRepo.GetDisPlayName("EMP_CODE_PREFIX");
            this.EMP_CODE_LEN_TEXT = ObjMetaRepo.GetDisPlayName("EMP_CODE_LEN");
            this.Comp_Logo_TEXT = ObjMetaRepo.GetDisPlayName("Comp_Logo");
            this.COMP_URL_TEXT = ObjMetaRepo.GetDisPlayName("COMP_URL");
            // this. = ObjMetaRepo.GetDisPlayName("UPDATED_ON");ISACTIVE_TEXT
            this.ISACTIVE_TEXT = ObjMetaRepo.GetDisPlayName("ISACTIVE");
            this.Screen_Name = ObjMetaRepo.Screen_Name;
        }

    }
    public class CompanyModelVM:BaseModel
    {
        public int CompID { get; set; }
        [Required(ErrorMessage = "Required")]
        public string COMP_CODE { get; set; }
        [Required(ErrorMessage = "Required")]
        public string COMP_NAME { get; set; }
        [Required(ErrorMessage = "Required")]
        public string COMP_ADDR1 { get; set; }
        public string COMP_ADDR2 { get; set; }
        [Required(ErrorMessage = "Required")]
        public string COMP_CITY { get; set; }
        [Required(ErrorMessage = "Required")]
        public string COMP_STATE { get; set; }
        [Required(ErrorMessage = "Required")]
        public string COMP_PIN { get; set; }
        [Required(ErrorMessage = "Required")]
        public string COMP_CLASS { get; set; }
        [Required(ErrorMessage = "Required")]
        public string COMP_PANNO { get; set; }
        [Required(ErrorMessage = "Required")]
        public string COMP_TANNO { get; set; }
        [Required(ErrorMessage = "Required")]
        public string COMP_TDSCIRCLE { get; set; }
        public string SIGN_FNAME { get; set; }
        public string SIGN_LNAME { get; set; }
        public string SIGN_FATHER_NAME { get; set; }
        public string SIGN_ADDR1 { get; set; }
        public string SIGN_ADDR2 { get; set; }
        public string SIGN_CITY { get; set; }
        public string SIGN_DSG { get; set; }
        public string SIGN_STATE { get; set; }
        public string SIGN_PIN { get; set; }
        public string SIGN_PLACE { get; set; }
        public string SIGN_DATE { get; set; }
        [Required(ErrorMessage = "Required")]
        [Range(1, int.MaxValue, ErrorMessage = "Required")]
        public int RETIRE_AGE { get; set; }
        [Required]
        public bool EMP_CODE_GEN { get; set; }
        [Required(ErrorMessage = "Required")]
        public string EMP_CODE_PREFIX { get; set; }
        [Required(ErrorMessage = "Required")]
        [Range(1, int.MaxValue, ErrorMessage = "Required")]
        public int EMP_CODE_LEN { get; set; }
        public string Comp_Logo { get; set; }
        //[Required(ErrorMessage = "Required")]
        public string COMP_URL { get; set; }

        //TEXT_NAME
        public string EID_TEXT { get; set; }
        public string CompID_TEXT { get; set; }
        public string COMP_CODE_TEXT { get; set; }
        public string COMP_NAME_TEXT { get; set; }
        public string COMP_ADDR1_TEXT { get; set; }
        public string COMP_ADDR2_TEXT { get; set; }
        public string COMP_CITY_TEXT { get; set; }
        public string COMP_STATE_TEXT { get; set; }
        public string COMP_PIN_TEXT { get; set; }
        public string COMP_CLASS_TEXT { get; set; }
        public string COMP_PANNO_TEXT { get; set; }
        public string COMP_TANNO_TEXT { get; set; }
        public string COMP_TDSCIRCLE_TEXT { get; set; }
        public string SIGN_FNAME_TEXT { get; set; }
        public string SIGN_LNAME_TEXT { get; set; }
        public string SIGN_FATHER_NAME_TEXT { get; set; }
        public string SIGN_ADDR1_TEXT { get; set; }
        public string SIGN_ADDR2_TEXT { get; set; }
        public string SIGN_CITY_TEXT { get; set; }
        public string SIGN_DSG_TEXT { get; set; }
        public string SIGN_STATE_TEXT { get; set; }
        public string SIGN_PIN_TEXT { get; set; }
        public string SIGN_PLACE_TEXT { get; set; }
        public string SIGN_DATE_TEXT { get; set; }
        public string RETIRE_AGE_TEXT { get; set; }
        public string EMP_CODE_GEN_TEXT { get; set; }
        public string EMP_CODE_PREFIX_TEXT { get; set; }
        public string EMP_CODE_LEN_TEXT { get; set; }
        public string Comp_Logo_TEXT { get; set; }
        public string COMP_URL_TEXT { get; set; }
        public string Screen_Name { set; get; }
        public string ISACTIVE_TEXT { get; set; }
        public void SetDisplayName()
        {
            MasterMetaRepo ObjMetaRepo = new MasterMetaRepo(this.EID, "IVAP_MST_COMPANY", "ViewCompany");

            this.EID_TEXT = ObjMetaRepo.GetDisPlayName("EID");
            this.COMP_CODE_TEXT = ObjMetaRepo.GetDisPlayName("COMP_CODE");
            this.COMP_NAME_TEXT = ObjMetaRepo.GetDisPlayName("COMP_NAME");
            this.COMP_ADDR1_TEXT = ObjMetaRepo.GetDisPlayName("COMP_ADDR1");
            this.COMP_ADDR2_TEXT = ObjMetaRepo.GetDisPlayName("COMP_ADDR2");
            this.COMP_CITY_TEXT = ObjMetaRepo.GetDisPlayName("COMP_CITY");

            this.COMP_STATE_TEXT = ObjMetaRepo.GetDisPlayName("COMP_STATE");
            this.COMP_PIN_TEXT = ObjMetaRepo.GetDisPlayName("COMP_PIN");
            this.COMP_CLASS_TEXT = ObjMetaRepo.GetDisPlayName("COMP_CLASS");
            this.COMP_PANNO_TEXT = ObjMetaRepo.GetDisPlayName("COMP_PANNO");
            this.COMP_TANNO_TEXT = ObjMetaRepo.GetDisPlayName("COMP_TANNO");
            this.COMP_TDSCIRCLE_TEXT = ObjMetaRepo.GetDisPlayName("COMP_TDSCIRCLE");
            this.SIGN_FNAME_TEXT = ObjMetaRepo.GetDisPlayName("SIGN_FNAME");
            this.SIGN_LNAME_TEXT = ObjMetaRepo.GetDisPlayName("SIGN_LNAME");
            this.SIGN_FATHER_NAME_TEXT = ObjMetaRepo.GetDisPlayName("SIGN_FATHER_NAME");
            this.SIGN_ADDR1_TEXT = ObjMetaRepo.GetDisPlayName("SIGN_ADDR1");
            this.SIGN_ADDR2_TEXT = ObjMetaRepo.GetDisPlayName("SIGN_ADDR2");
            this.SIGN_CITY_TEXT = ObjMetaRepo.GetDisPlayName("SIGN_CITY");
            this.SIGN_DSG_TEXT = ObjMetaRepo.GetDisPlayName("SIGN_DSG");

            this.SIGN_STATE_TEXT = ObjMetaRepo.GetDisPlayName("SIGN_STATE");
            this.SIGN_PIN_TEXT = ObjMetaRepo.GetDisPlayName("SIGN_PIN");
            this.SIGN_PLACE_TEXT = ObjMetaRepo.GetDisPlayName("SIGN_PLACE");
            this.SIGN_DATE_TEXT = ObjMetaRepo.GetDisPlayName("SIGN_DATE");
            this.RETIRE_AGE_TEXT = ObjMetaRepo.GetDisPlayName("RETIRE_AGE");
            this.EMP_CODE_GEN_TEXT = ObjMetaRepo.GetDisPlayName("EMP_CODE_GEN");
            this.EMP_CODE_PREFIX_TEXT = ObjMetaRepo.GetDisPlayName("EMP_CODE_PREFIX");
            this.EMP_CODE_LEN_TEXT = ObjMetaRepo.GetDisPlayName("EMP_CODE_LEN");
            this.Comp_Logo_TEXT = ObjMetaRepo.GetDisPlayName("Comp_Logo");
            this.COMP_URL_TEXT = ObjMetaRepo.GetDisPlayName("COMP_URL");
            // this. = ObjMetaRepo.GetDisPlayName("UPDATED_ON");ISACTIVE_TEXT
            this.ISACTIVE_TEXT = ObjMetaRepo.GetDisPlayName("ISACTIVE");
            this.Screen_Name = ObjMetaRepo.Screen_Name;
        }
    }
}