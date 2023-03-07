using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Ivap.Areas.Configuration.Models;

namespace Ivap.Areas.Master.CustomValidation
{
    public class CompTypeValidation : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            GlobalComponentModel objMin = (GlobalComponentModel)validationContext.ObjectInstance;
            bool s = (objMin.MIN_LENGTH == null && objMin.COMPONENT_TYPE != "DEDUCTION") || objMin.MIN_LENGTH < 1;
            if (objMin.MIN_LENGTH == null && objMin.COMPONENT_TYPE != "MASTER")
            {
                return new ValidationResult("Min Length Required.");
            }else if (objMin.COMPONENT_TYPE != "MASTER" && objMin.MIN_LENGTH < 0)
            {
                return new ValidationResult("Min Length greater or equal to 0.");
            }
            else { return ValidationResult.Success; }
        }
    }


    public class CompTypeMaxValidation : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            GlobalComponentModel objMin = (GlobalComponentModel)validationContext.ObjectInstance;
           
             if (objMin.COMPONENT_TYPE != "MASTER" && objMin.MAX_LENGTH ==null)
            {
                return new ValidationResult("Max Length required.");
            }else if (objMin.COMPONENT_TYPE != "MASTER" && objMin.MAX_LENGTH <= objMin.MIN_LENGTH)
            {
                return new ValidationResult("Max Length Greater then Min Length.");
            }
            else { return ValidationResult.Success; }
        }
    }

    public class CompTypeTableNameValidation : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            GlobalComponentModel objMin = (GlobalComponentModel)validationContext.ObjectInstance;

            if (objMin.COMPONENT_TYPE == "MASTER" && objMin.Component_TableName.Trim()==null)
            {
                return new ValidationResult("Required.");
            }
            else { return ValidationResult.Success; }
        }
    }
    public class CompTypeColumnNameValidation : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            GlobalComponentModel objMin = (GlobalComponentModel)validationContext.ObjectInstance;

            if (objMin.COMPONENT_TYPE == "MASTER" && objMin.Component_FieldName.Trim() == null)
            {
                return new ValidationResult("Required.");
            }
            else { return ValidationResult.Success; }
        }
    }
}