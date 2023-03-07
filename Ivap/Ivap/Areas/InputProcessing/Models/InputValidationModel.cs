using Ivap.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Ivap.Areas.InputProcessing.Models
{
    public class InputValidationModel
    {
        public int ReqValFailCount { set; get; }
        public int MasterValFailCount { set; get; }
        public int DataFormateValFailCount { set; get; }
        public int DateFormateValFailCount { set; get; }
        public int SuccessCount { set; get; }
        public int TotalCount { set; get; }
    }
    public class InputValidationVM
    {
        public double ReqValFailCount { set; get; }
        public double MasterValFailCount { set; get; }
        public double DataFormateValFailCount { set; get; }
        public double DateFormateValFailCount { set; get; }
        public double SuccessCount { set; get; }
        public int TotalCount { set; get; }

        public double MasterAccessVoilationCount { set; get; }
    }
}