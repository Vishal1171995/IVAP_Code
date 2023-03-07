using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Ivap.Areas.InputProcessing.Validator.OtherValidation
{
    public class ExtraValidationsBase
    {
        public virtual ValidationResponse Validate(BasicValidationBase Obj)
        {
            ValidationResponse Res = new ValidationResponse();
            Res.IsSuccess = true;
            return Res;
        }
    }
}