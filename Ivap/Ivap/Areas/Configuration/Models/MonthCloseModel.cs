using Ivap.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Ivap.Areas.Configuration.Models
{
    public class MonthCloseModel:BaseModel
    {
        public int? TID { get; set; }
        [Required(ErrorMessage = "Please select month.")]
        public int Month { get; set; }
        [Required(ErrorMessage = "Please select year.")]
        public int Year { get; set; }
        [Required(ErrorMessage = "Please enter open date.")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime OpenDate { get; set; }
        [Required(ErrorMessage = "Please enter close date.")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime CloseDate { get; set; }
        public string Status { get; set; }
    }
}