using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Unity;

namespace Ivap.Areas.InputProcessing.Validator
{
    public class DateFormatValidationFactory
    {
        private static IUnityContainer ValidatorContainer = null;
        public static DateFormateValidationBase Create(String DataType)
        {
            if (ValidatorContainer == null)
            {
                ValidatorContainer = new UnityContainer();
                ValidatorContainer.RegisterType<DateFormateValidationBase, DateFormateValidationBase>("");
                ValidatorContainer.RegisterType<DateFormateValidationBase, DateFormatddmmyyforwordslash>("dd/mm/yy");
                ValidatorContainer.RegisterType<DateFormateValidationBase, DateFormatddmmyyyyforwordslash>("dd/mm/yyyy");
                ValidatorContainer.RegisterType<DateFormateValidationBase, DateFormatddmmyyyydash>("dd-mm-yyyy");
                ValidatorContainer.RegisterType<DateFormateValidationBase, DateFormatyyyymmdddash>("yyyy-mm-dd");
                ValidatorContainer.RegisterType<DateFormateValidationBase, DateFormatDD_MON_YYYY > ("DD-MON-YYYY");
            }

            return ValidatorContainer.Resolve<DateFormateValidationBase>(DataType);

        }
    }
}