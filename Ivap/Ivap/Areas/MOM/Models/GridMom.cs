using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Ivap.Areas.MOM.Models
{
    public class GridMom
    {
        public int from { get; set; }

        public int Circle { get; set; }
        public int To { get; set; }
        public string SortingStr { get; set; }
        public string FilterStr { get; set; }
    }
}