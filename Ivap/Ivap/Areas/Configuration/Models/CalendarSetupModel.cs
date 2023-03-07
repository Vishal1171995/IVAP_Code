using Ivap.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Ivap.Areas.Configuration.Models
{
    public class CalendarSetupModel:BaseModel
    {
        public int? TID { get; set; }
        public string CALENDAR_NAME { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "Please select calendar type")]
        public int CALENDAR_TYPE { get; set; }
        [Required(ErrorMessage = "Please select file type.")]
        public string FILE_TYPE { get; set; }
        [Required(ErrorMessage = "Required")]
        public string DESCRIPTION { get; set; }
        [Required(ErrorMessage = "Please enter pay date.")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? PAY_DATE { get; set; }

        [Required(ErrorMessage = "Please enter due date.")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? DUE_DATE { get; set; }
        [Required(ErrorMessage = "Please enter remainder day.")]
        public int REMAINDER_DAYS { get; set; }
        [Required(ErrorMessage = "Please select event.")]
        public string EVENT { get; set; }

        public string ActivityCategory { get; set; }
        //[Required(ErrorMessage = "Please select frequency.")]
        public string FREQUENCY { get; set; }

        public SelectList CalendarTypeList { get; set; }
    }
}