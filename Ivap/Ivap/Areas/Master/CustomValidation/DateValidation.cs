using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Ivap.Areas.Master.Models;

namespace Ivap.Areas.Master.CustomValidation
{
    public class DateValidation : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            MinWageModel objMin = (MinWageModel)validationContext.ObjectInstance;
            if (objMin.EFF_DT_FROM.Value.Date > objMin.EFF_DATE_TO.Value.Date)
            {
                return new ValidationResult("EFF_DT_FROM must be Less than EFF_DATE_TO date.");
            }
            else { return ValidationResult.Success; }
        }
    }
    public class DateValidationPtex : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            PTAXModel objMin = (PTAXModel)validationContext.ObjectInstance;
            if (objMin.EFF_FROM_DT > objMin.EFF_TO_DT)
            {
                return new ValidationResult("EFF_DT_FROM date must less than EFF_DATE_TO date.");
            }
            else { return ValidationResult.Success; }
        }
    }
}