using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Ivap.Areas.Master.Models;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Ivap.Areas.Master.ViewModel
{
    public class EntityOnBoardVM: IValidatableObject
    {
        [Required]
        public EntityModel EntityModel { set; get; }
        [Required]
        public CreateUserVM EntityUser { set; get; }

        public SelectList StateList { get; set; }

        public SelectList CountryList { get; set; }

        public SelectList CurrencyList { get; set; }

        public SelectList Services_Availed_List { get; set; }

        public string ENTITY_Payperiod_Weekly_Fr { set; get; }
        public string ENTITY_Payperiod_Weekly_To { set; get; }
        public int ENTITY_Payperiod_Monthly_Fr { set; get; }
        public int ENTITY_Payperiod_Monthly_To { set; get; }
        public int ENTITY_Payperiod_Fortnightly_Fr1 { set; get; }
        public int ENTITY_Payperiod_Fortnightly_To1 { set; get; }
        public int ENTITY_Payperiod_Fortnightly_Fr2 { set; get; }
        public int ENTITY_Payperiod_Fortnightly_To2 { set; get; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
           
            if (EntityModel.ENTITY_Payperiod.ToUpper() == "MONTHLY")
            {
                //if (ENTITY_Payperiod_Monthly_Fr == 0)
                //{

                //    yield return
                //          new ValidationResult(errorMessage: "Required Monthly From",
                //                               memberNames: new[] { "ENTITY_Payperiod_Monthly_Fr" });

                //}
                //else if (ENTITY_Payperiod_Monthly_To == 0)
                //{
                //    yield return
                //          new ValidationResult(errorMessage: "Required Monthly To",
                //                               memberNames: new[] { "ENTITY_Payperiod_Monthly_To" });
                //}
                //else
                //{
                //    yield return ValidationResult.Success;
                //}
                yield return ValidationResult.Success;

            }
            else if (EntityModel.ENTITY_Payperiod.ToUpper() == "WEEKLY")
            {
                if (string.IsNullOrEmpty(ENTITY_Payperiod_Weekly_Fr))
                {
                    yield return
                           new ValidationResult(errorMessage: "Required Weekly From",
                                                memberNames: new[] { "ENTITY_Payperiod_Weekly_Fr" });
                }
                else if (string.IsNullOrEmpty(ENTITY_Payperiod_Weekly_To))
                {
                    yield return
                           new ValidationResult(errorMessage: "Required Weekly To",
                                                memberNames: new[] { "ENTITY_Payperiod_Weekly_To" });
                }


                else { yield return ValidationResult.Success; }
            }
            else if (EntityModel.ENTITY_Payperiod.ToUpper() == "FORTNIGHT")
            {
                if (ENTITY_Payperiod_Fortnightly_Fr1 == 0)
                {
                    yield return
                   new ValidationResult(errorMessage: "Required First Fortnight From",
                                       memberNames: new[] { "ENTITY_Payperiod_Fortnightly_Fr1" });
                }
                else if (ENTITY_Payperiod_Fortnightly_Fr2 == 0)
                {
                    yield return
                  new ValidationResult(errorMessage: "Required Second Fortnight From",
                                      memberNames: new[] { "ENTITY_Payperiod_Fortnightly_Fr2" });
                }
                else if (ENTITY_Payperiod_Fortnightly_To1 == 0)
                {
                    yield return
                  new ValidationResult(errorMessage: "Required First Fortnight To",
                                      memberNames: new[] { "ENTITY_Payperiod_Fortnightly_To1" });
                }
                else if (ENTITY_Payperiod_Fortnightly_To2 == 0)
                {
                    yield return
                   new ValidationResult(errorMessage: "Required Second Fortnight To",
                                       memberNames: new[] { "ENTITY_Payperiod_Fortnightly_To2" });
                }
                else
                {
                   yield return ValidationResult.Success;
                }

            }
            else
            {
                yield return ValidationResult.Success;
            }
        }

    }
   
}