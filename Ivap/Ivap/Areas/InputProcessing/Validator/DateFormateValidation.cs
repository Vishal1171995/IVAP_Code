using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;

namespace Ivap.Areas.InputProcessing.Validator
{
        public class DateFormateValidationBase
        {
            public virtual ValidationResponse Validate(BasicValidationBase Obj)
            {
                ValidationResponse Res = new ValidationResponse();
                Res.IsSuccess = true;
                string[] tokens = Obj.Value.Split(' ');
                string Date = tokens[0].ToString();
                DateTime dateTime;
                if (!DateTime.TryParseExact(Date, Obj.DateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out dateTime))
                {
                    Res.ValidationErrorType = "Date Format Validation";
                    Res.IsSuccess = false;
                    Res.Message = Obj.DisplayName + " must be in valid date format as defined in entity.";
                    Res.Data = Obj.DisplayName;
                }
                return Res;
            }
        }


    public class DateFormatddmmyyforwordslash : DateFormateValidationBase
    {

        public override ValidationResponse Validate(BasicValidationBase Obj)
        {
            ValidationResponse Res = new ValidationResponse();
            Res.IsSuccess = true;
            if (Obj.Value == "")
            {
                return Res;
            }
            string[] tokens = Obj.Value.Split(' ');
            string[] Date = tokens[0].ToString().Split('/');
            try
            {
                DateTime dateTime;
                string Temp = new DateTime(Convert.ToInt32(Date[2]), Convert.ToInt32(Date[1]), Convert.ToInt32(Date[0])).ToShortDateString();
                if (!DateTime.TryParse(Temp, out dateTime))
                {
                    Res.ValidationErrorType = "Date Format Validation";
                    Res.IsSuccess = false;
                    Res.Message = Obj.DisplayName + " must be in valid date format as defined in entity.";
                    Res.Data = Obj.DisplayName;
                }
            }
            catch
            {
                Res.ValidationErrorType = "Date Format Validation";
                Res.IsSuccess = false;
                Res.Message = Obj.DisplayName + " must be in valid date format as defined in entity.";
                Res.Data = Obj.DisplayName;

            }
            return Res;
        }
    }

    public class DateFormatddmmyyyyforwordslash : DateFormateValidationBase
    {

        public override ValidationResponse Validate(BasicValidationBase Obj)
        {
            ValidationResponse Res = new ValidationResponse();
            Res.IsSuccess = true;
            if (Obj.Value == "")
            {
                return Res;
            }
            string[] tokens = Obj.Value.Split(' ');
            string[] Date = tokens[0].ToString().Split('/');
            try
            {
                DateTime dateTime;
                string Temp = new DateTime(Convert.ToInt32(Date[2]), Convert.ToInt32(Date[1]), Convert.ToInt32(Date[0])).ToShortDateString();
                if (!DateTime.TryParse(Temp, out dateTime))
                {
                    Res.ValidationErrorType = "Date Format Validation";
                    Res.IsSuccess = false;
                    Res.Message = Obj.DisplayName + " must be in valid date format as defined in entity.";
                    Res.Data = Obj.DisplayName;
                }
            }
            catch
            {
                Res.ValidationErrorType = "Date Format Validation";
                Res.IsSuccess = false;
                Res.Message = Obj.DisplayName + " must be in valid date format as defined in entity.";
                Res.Data = Obj.DisplayName;

            }
            return Res;
        }
    }

    public class DateFormatddmmyyyydash : DateFormateValidationBase
    {

        public override ValidationResponse Validate(BasicValidationBase Obj)
        {
            ValidationResponse Res = new ValidationResponse();
            Res.IsSuccess = true;
            if (Obj.Value == "")
            {
                return Res;
            }
            string[] tokens = Obj.Value.Split(' ');
            string[] Date = tokens[0].ToString().Split('-'); 
            try
            {
                DateTime dateTime;
                string Temp = new DateTime(Convert.ToInt32(Date[2]), Convert.ToInt32(Date[1]), Convert.ToInt32(Date[0])).ToShortDateString();
                if (!DateTime.TryParse(Temp, out dateTime))
                {
                    Res.ValidationErrorType = "Date Format Validation";
                    Res.IsSuccess = false;
                    Res.Message = Obj.DisplayName + " must be in valid date format as defined in entity.";
                    Res.Data = Obj.DisplayName;
                }
            }
            catch
            {
                Res.ValidationErrorType = "Date Format Validation";
                Res.IsSuccess = false;
                Res.Message = Obj.DisplayName + " must be in valid date format as defined in entity.";
                Res.Data = Obj.DisplayName;

            }
            return Res;
        }
    }


    public class DateFormatyyyymmdddash : DateFormateValidationBase
    {

        public override ValidationResponse Validate(BasicValidationBase Obj)
        {
            ValidationResponse Res = new ValidationResponse();
            Res.IsSuccess = true;
            if (Obj.Value == "")
            {
                return Res;
            }
            string[] tokens = Obj.Value.Split(' ');
            string[] Date = tokens[0].ToString().Split('-');
            try
            {
                DateTime dateTime;
                string Temp = new DateTime(Convert.ToInt32(Date[2]), Convert.ToInt32(Date[1]), Convert.ToInt32(Date[0])).ToShortDateString();
                if (!DateTime.TryParse(Temp, out dateTime))
                {
                    Res.ValidationErrorType = "Date Format Validation";
                    Res.IsSuccess = false;
                    Res.Message = Obj.DisplayName + " must be in valid date format as defined in entity.";
                    Res.Data = Obj.DisplayName;
                }
            }
            catch
            {
                Res.ValidationErrorType = "Date Format Validation";
                Res.IsSuccess = false;
                Res.Message = Obj.DisplayName + " must be in valid date format as defined in entity.";
                Res.Data = Obj.DisplayName;

            }
            return Res;
        }
    }

    public class DateFormatDD_MON_YYYY : DateFormateValidationBase
    {

        public override ValidationResponse Validate(BasicValidationBase Obj)
        {
            ValidationResponse Res = new ValidationResponse();
            Res.IsSuccess = true;
            if (Obj.Value == "")
            {
                return Res;
            }
            string[] tokens = Obj.Value.Split(' ');
            string[] Date = tokens[0].ToString().Split('-');
            try
            {
                DateTime dateTime;
                var Month = DateTime.ParseExact(Date[1].Trim(), "MMM", CultureInfo.CurrentCulture).Month;

                string Temp = new DateTime(Convert.ToInt32(Date[2]), Month, Convert.ToInt32(Date[0])).ToShortDateString();
                if (!DateTime.TryParse(Temp, out dateTime))
                {
                    Res.ValidationErrorType = "Date Format Validation";
                    Res.IsSuccess = false;
                    Res.Message = Obj.DisplayName + " must be in valid date format as defined in entity.";
                    Res.Data = Obj.DisplayName;
                }
            }
            catch(Exception Ex)
            {
                var Msg = Ex.Message;
                Res.ValidationErrorType = "Date Format Validation";
                Res.IsSuccess = false;
                Res.Message = Obj.DisplayName + " must be in valid date format as defined in entity.";
                Res.Data = Obj.DisplayName;

            }
            return Res;
        }
    }

    
}