using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Ivap.Areas.Configuration.Models;

namespace Ivap.Areas.Configuration.CustomValidation
{
    public class EntityCompTypeValidation : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            EntityComponentModel objMin = (EntityComponentModel)validationContext.ObjectInstance;
           
            if (objMin.MIN_LENGTH == null && (objMin.COMPONENT_DATATYPE != "MASTER" && objMin.COMPONENT_DATATYPE != "DATETIME"))
            {
                return new ValidationResult("Min Length Required.");
            }
            else { return ValidationResult.Success; }
            //else if (objMin.COMPONENT_DATATYPE != "MASTER" && objMin.MIN_LENGTH < 1)
            //{
            //    return new ValidationResult("Min Length greater then 0.");
            //}

        }
    }


    public class EntityCompTypeMaxValidation : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            EntityComponentModel objMin = (EntityComponentModel)validationContext.ObjectInstance;
           
             if ((objMin.COMPONENT_DATATYPE != "MASTER" && objMin.COMPONENT_DATATYPE != "DATETIME") && objMin.MAX_LENGTH ==null)
            {
                return new ValidationResult("Max Length required.");
            }else if ((objMin.COMPONENT_DATATYPE != "MASTER" && objMin.COMPONENT_DATATYPE != "DATETIME") && objMin.MAX_LENGTH <= objMin.MIN_LENGTH)
            {
                return new ValidationResult("Max Length Greater then Min Length.");
            }
            else { return ValidationResult.Success; }
        }
    }

    public class EntityCompTypeTableNameValidation : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            EntityComponentModel objMin = (EntityComponentModel)validationContext.ObjectInstance;

            if (objMin.COMPONENT_DATATYPE == "MASTER" && objMin.Component_TableName?.Trim().Length==null)
            {
                return new ValidationResult("Component Table Name Required.");
            }
            else { return ValidationResult.Success; }
        }
    }
    public class EntityCompTypeColumnNameValidation : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            
            EntityComponentModel objMin = (EntityComponentModel)validationContext.ObjectInstance;
            int? a = objMin.Component_FieldName?.Length;
            if (objMin.COMPONENT_DATATYPE == "MASTER" && objMin.Component_FieldName?.Trim().Length ==null)
            {
                return new ValidationResult("Component Column Name Required.");
            }
            else { return ValidationResult.Success; }
        }
    }
}