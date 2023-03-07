using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace Ivap.Areas.InputProcessing.Validator.OtherValidation
{
    public class RegExpValidation: ExtraValidationsBase
    {
        public override ValidationResponse Validate(BasicValidationBase Obj)
        {
            ValidationResponse Res = new ValidationResponse();
            Regex regex = new Regex(Obj.Reg_Exp);
            //Regex regex = new Regex("^[0-9]{12}$");
            Match match = regex.Match(Obj.Value);
            Res.IsSuccess = false;
            Res.Message = Obj.DisplayName + " must be in valid format";
            if (match.Success)
            {
                Res.IsSuccess = true;
            }
            return Res;
        }
    }
}