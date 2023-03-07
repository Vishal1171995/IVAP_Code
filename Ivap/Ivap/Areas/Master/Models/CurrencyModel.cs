using Ivap.Areas.Master.Repository;
using Ivap.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Ivap.Areas.Master.Models
{
    public class CurrencyModel:BaseModel
    {
        public int CID { get; set; }
        [Required(ErrorMessage = "Required")]
        public string CURRENCY_CODE { set; get; }
        [Required(ErrorMessage = "Required")]
        public string CURRENCY_NAME { set; get; }

        public string CID_TEXT { get; set; }
      
        public string CURRENCY_CODE_TEXT { set; get; }
      
        public string CURRENCY_NAME_TEXT { set; get; }
        public string Screen_Name { set; get; }
        public void SetDisplayName()
        {
            MasterMetaRepo ObjMetaRepo = new MasterMetaRepo(this.EID, "IVAP_MST_CURRENCY", "ViewCurrency");

            //this.COMP_ID_TEXT = ObjMetaRepo.GetDisPlayName("COMP_ID");
            this.CID_TEXT = ObjMetaRepo.GetDisPlayName("TID");
            this.CURRENCY_CODE_TEXT = ObjMetaRepo.GetDisPlayName("CURRENCY_CODE");
            this.CURRENCY_NAME_TEXT = ObjMetaRepo.GetDisPlayName("CURRENCY_NAME");
            this.Screen_Name = ObjMetaRepo.Screen_Name;
        }
    }
}