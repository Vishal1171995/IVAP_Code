using Ivap.Areas.Master.Repository;
using Ivap.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Ivap.Areas.Master.Models
{
    public class StateModel:BaseModel
    {
        public int StateId { get; set; }
        public string Country_Name { get; set; }
        [Required(ErrorMessage = "Required")]
        [StringLength(10, ErrorMessage = "State Code can be min 3 and max 10 characters long.", MinimumLength = 3)]
        public string State_Code { get; set; }
        [Required(ErrorMessage = "Required")]
        [StringLength(100, ErrorMessage = "State Name can be  max 100 characters long.", MinimumLength = 3)]
        public string State_Name { get; set; }

        public string StateId_TEXT { get; set; }

        public string State_Code_TEXT { get; set; }
        public string State_Name_TEXT { get; set; }
        public string ISACTIVE_TEXT { get; set; }
        public string Screen_Name { set; get; }
        public void SetDisplayName()
        {
            MasterMetaRepo ObjMetaRepo = new MasterMetaRepo(this.EID, "IVAP_MST_STATE", "ViewState");

            this.StateId_TEXT = ObjMetaRepo.GetDisPlayName("StateId");
            this.State_Code_TEXT = ObjMetaRepo.GetDisPlayName("State_Code");
            this.State_Name_TEXT = ObjMetaRepo.GetDisPlayName("State_Name");
            this.ISACTIVE_TEXT = ObjMetaRepo.GetDisPlayName("IsActive");
            this.Screen_Name = ObjMetaRepo.Screen_Name;
        }

    }
}