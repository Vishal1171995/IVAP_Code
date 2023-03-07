using Ivap.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Ivap.Areas.InputProcessing.Validator
{
    public class ValidationResponse:Response
    {
        public string ValidationErrorType { set; get; }
    }
}