using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace Ivap.Areas.InputProcessing.Validator.OtherValidation
{
    public class PanCardValidation: ExtraValidationsBase
    {
        public override ValidationResponse Validate(BasicValidationBase Obj)
        {
            ValidationResponse Res = new ValidationResponse();
            if (Obj.Value.Trim() == "")
            {
                Res.IsSuccess = true;
                return Res;
            }
            Regex regex = new Regex("^[a-zA-Z]{3}[p-P]{1}[a-zA-Z]{1}[0-9]{4}[a-zA-Z]{1}$");
            Match match = regex.Match(Obj.Value);
            Res.IsSuccess = false;
            Res.Message = Obj.DisplayName+" must be a valid Pancard number";
            if (match.Success)
            {
                Res.IsSuccess = true;
            }
            return Res;
        }
    }
}