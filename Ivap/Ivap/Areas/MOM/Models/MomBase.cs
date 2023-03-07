using Ivap.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Ivap.Areas.MOM.Models
{
    public class MomBase : BaseModel
    {
        public string MinutesDetail { get; set; }
        public int MID { get; set; }
        [Required(ErrorMessage = "Please enter meeting date")]
        public DateTime MeetingHeld { get; set; }
        [Required(ErrorMessage = "Please enter Meeting Attendees")]
        public string Meeting_Attendees { get; set; }
        [Required(ErrorMessage = "Please enter address")]
        public string Address { get; set; }

        [Required(ErrorMessage = "Please enter Agenda of Meeting")]
        public string Agenda { get; set; }

        //public string Discuss_Points { get; set; }

        //public string Responsibilities_Holders { get; set; }

        //public string ClosurePoints_Discussed { get; set; }

        public string Status { get; set; }

        //public bool CanPrint { get; set; }

        //public int CurrUser { get; set; }

        //public int CurrStatus { get; set; }

        public string filter { get; set; }
        public virtual void Validate()
        {
            throw new Exception("Not Implement Method");
        }
    }
}