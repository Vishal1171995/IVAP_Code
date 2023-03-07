using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Ivap.Areas.MOM.Models
{
    public class MOMGrid
    {
        public int UniqueID { get; set; }
        public int Sr { get; set; }
        public string Title { get; set; }
        public string Desc { get; set; }
        public string Start_Date { get; set; }
        public string End_Date { get; set; }
        public string BackColor { get; set; }
        public string borderColor { get; set; }
    }
}