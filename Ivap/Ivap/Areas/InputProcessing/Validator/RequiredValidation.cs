using Ivap.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Ivap.Areas.InputProcessing.Validator
{
    public class RequiredValidation:BasicValidationBase 
    {

        public override ValidationResponse Validate(BasicValidationBase ObjVal)
        {
            ValidationResponse Res = new ValidationResponse();
            Res.IsSuccess = true;
            try
            {
                if(ObjVal.Value.Trim()=="" && ObjVal.IsRequired=="1")
                {
                    Res.IsSuccess = false;
                    Res.Message = ObjVal.DisplayName + " required.";
                    Res.Data = ObjVal.DisplayName;
                    Res.ValidationErrorType = "Required Validation";
                }
                return Res;
            }
            catch
            {
                throw;
            }

        }
    }
}