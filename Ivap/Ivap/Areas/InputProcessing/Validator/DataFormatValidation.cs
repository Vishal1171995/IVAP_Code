using Ivap.Utils;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace Ivap.Areas.InputProcessing.Validator
{
    public class DataFormatValidation:BasicValidationBase 
    {

        //AMOUNT,DATETIME,MASTER,NUMBER,TEXT
        public override ValidationResponse Validate(BasicValidationBase ObjVal)
        {
            //Lookup Appropriate Validator from factory....
            var Obj = DataFormatValidationFactory.Create(ObjVal.DataType);
            return Obj.Validate(ObjVal);
        }
    }

    public class DataFormateValidationBase
    {
        public virtual ValidationResponse Validate(BasicValidationBase Obj)
        {
            ValidationResponse Res = new ValidationResponse();
            Res.IsSuccess = true;
            return Res;
        }
    }

    

    public class DateValidation: DataFormateValidationBase
    {
        public override  ValidationResponse Validate(BasicValidationBase ObjVal)
        {
            var ObjFac = DateFormatValidationFactory.Create(ObjVal.DateFormat);
            return ObjFac.Validate(ObjVal);
        }
    }

    public class NumberValidation : DataFormateValidationBase
    {
        public override ValidationResponse Validate(BasicValidationBase Obj)
        {
            ValidationResponse Res = new ValidationResponse();
            Res.IsSuccess = true;

            if(Obj.Value=="")
            {
                return Res;
            }

            if (!Regex.IsMatch(Obj.Value, @"[\d]([.][\d])?"))
            {
                Res.ValidationErrorType = "Data Format Validation";
                Res.IsSuccess = false;
                Res.Message = Obj.DisplayName + " must be numeric only.";
                Res.Data = Obj.DisplayName;
                return Res;
            }
            if (!(Convert.ToDecimal(Obj.Value) >= Obj.MinLength && Convert.ToDecimal(Obj.Value) <= Obj.MaxLength))
            {
                Res.ValidationErrorType = "Data Format Validation";
                Res.IsSuccess = false;
                Res.Message = Obj.DisplayName + " must be between " + Obj.MinLength + " and " + Obj.MaxLength + " only.";
                Res.Data = Obj.DisplayName;
                return Res;
            }
            return Res;
        }
    }

    public class AmountValidation : DataFormateValidationBase
    {
        public override ValidationResponse Validate(BasicValidationBase Obj)
        {
            ValidationResponse Res = new ValidationResponse();
            Res.IsSuccess = true;
            if (Obj.Value == "")
            {
                return Res;
            }
            if (!Regex.IsMatch(Obj.Value, @"[\d]([.][\d])?"))
            {
                Res.IsSuccess = false;
                Res.ValidationErrorType = "Data Format Validation";
                Res.Message = Obj.DisplayName+ " must be numeric only.";
                Res.Data = Obj.DisplayName;
                return Res;
            }
            if(!(Convert.ToDecimal(Obj.Value)>=Obj.MinLength && Convert.ToDecimal(Obj.Value) <= Obj.MaxLength))
            {
                Res.ValidationErrorType = "Data Format Validation";
                Res.IsSuccess = false;
                Res.Message = Obj.DisplayName + " must be between "+ Obj.MinLength + " and " + Obj.MaxLength+" only.";
                Res.Data = Obj.DisplayName;
                return Res;
            }
            return Res;
        }
    }

    public class TextValidation : DataFormateValidationBase
    {
        public override ValidationResponse Validate(BasicValidationBase Obj)
        {
            ValidationResponse Res = new ValidationResponse();
            
            Res.IsSuccess = true;
            if (Obj.Value == "")
            {
                return Res;
            }
            if (!(Obj.Value.Length>=Obj.MinLength && Obj.Value.Length <= Obj.MaxLength))
            {
                Res.IsSuccess = false;
                Res.Message = Obj.DisplayName + "character length must be between " + Obj.MinLength + " and " + Obj.MaxLength+" only.";
                Res.Data = Obj.DisplayName;
                Res.ValidationErrorType = "Data Format Validation";
                return Res;
            }
            return Res;
        }
    }


}