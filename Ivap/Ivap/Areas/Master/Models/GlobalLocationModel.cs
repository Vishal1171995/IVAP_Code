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
    public class GlobalLocationModel:BaseModel
    {
        public int TID { get; set; }
        //LOC_CODE,LOC_NAME,STATE_ID,ISMETRO,CREATED_ON,CREATED_BY,ISACTIVE
        [Required(ErrorMessage = "Required")]
        public string LOC_CODE { get; set; }
        [Required(ErrorMessage = "Required")]
        public string LOC_NAME { get; set; }
        [Required(ErrorMessage = "Required")]
        [Range(1, int.MaxValue, ErrorMessage = "Please select state.")]
        public int STATE_ID { get; set; }

        public int from { get; set; }

        public int Circle { get; set; }
        public int To { get; set; }
        public string SortingStr { get; set; }
        public string FilterStr { get; set; }

        public SelectList StateList { get; set; }
        public bool ISMETRO { get; set; }
        public string TID_TEXT { get; set; }
        public string LOC_CODE_TEXT { get; set; }
        public string LOC_NAME_TEXT { get; set; }
        public string STATE_ID_TEXT { get; set; }
        public string ISMETRO_TEXT { get; set; }
        public string Screen_Name { set; get; }
   }
    public class GridGLocation
    {
        public int from { get; set; }
        public int To { get; set; }
        public string SortingStr { get; set; }
        public string FilterStr { get; set; }
    }
}