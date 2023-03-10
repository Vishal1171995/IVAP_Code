using Ivap.Controllers;
using Ivap.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;


namespace Ivap.Areas.MOM.Models
{
    public class CreateMOM: BaseModel
    {
      
        public int MID { get; set; }
        [Required(ErrorMessage = "Please enter meeting date")]
        public DateTime MeetingHeld { get; set; }
        [Required(ErrorMessage = "Please enter Meeting Attendees")]
        [StringLength(2000, MinimumLength = 0)]
        public string Meeting_Attendees { get; set; }
        [Required(ErrorMessage = "Please enter address")]
        [StringLength(2000, MinimumLength = 0)]
        public string Address { get; set; }

        [Required(ErrorMessage = "Please enter Agenda of Meeting")]
        [StringLength(2000, MinimumLength = 0)]
        public string Agenda { get; set; }

        //[Required(ErrorMessage = "Kindly Add Minutes of Detail")]
        public string MinutesDetail { get; set; }
        public string Status { get; set; }
    }
}