using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace Ivap.Areas.InputProcessing.Validator.OtherValidation
{
    public class EmailValidation:ExtraValidationsBase
    {
        public override ValidationResponse Validate(BasicValidationBase Obj)
        {
            ValidationResponse Res = new ValidationResponse();
            if (Obj.Value.Trim() == "")
            {
                Res.IsSuccess = true;
                return Res;
            }
            //Regex regex = new Regex(Obj.Reg_Exp);
            Regex regex = new Regex(@"[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?$");
            Match match = regex.Match(Obj.Value);
            Res.IsSuccess = false;
            Res.Message = Obj.DisplayName + " must be a valid email.";
            if (match.Success)
            {
                Res.IsSuccess = true;
            }
            return Res;
        }
    }
}