using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace Ivap.Areas.InputProcessing.Validator.OtherValidation
{
    public class UANValidation: ExtraValidationsBase
    {
        public override ValidationResponse Validate(BasicValidationBase Obj)
        {
            ValidationResponse Res = new ValidationResponse();
            if (Obj.Value.Trim() == "")
            {
                Res.IsSuccess = true;
                return Res;
            }
            Regex regex = new Regex("^[0-9]{12}$");
            Match match = regex.Match(Obj.Value);
            Res.IsSuccess = false;
            Res.Message = Obj.DisplayName + " must be a valid UAN number";
            if (match.Success)
            {
                Res.IsSuccess = true;
            }
            return Res;
        }
    }
}