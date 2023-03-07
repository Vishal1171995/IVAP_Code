using Ivap.Areas.InputProcessing.Validator.OtherValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Unity;

namespace Ivap.Areas.InputProcessing.Validator
{
    public class DataFormatValidationFactory
    {
        private static IUnityContainer ValidatorContainer = null;
        public static DataFormateValidationBase Create(String DataType)
        {
            if (ValidatorContainer == null)
            {
                ValidatorContainer = new UnityContainer();
                ValidatorContainer.RegisterType<DataFormateValidationBase, DataFormateValidationBase>("");
                ValidatorContainer.RegisterType<DataFormateValidationBase, DataFormateValidationBase>("MASTER");
                ValidatorContainer.RegisterType<DataFormateValidationBase, AmountValidation>("AMOUNT");
                ValidatorContainer.RegisterType<DataFormateValidationBase, DateValidation>("DATETIME");
                ValidatorContainer.RegisterType<DataFormateValidationBase, NumberValidation>("NUMBER");
                ValidatorContainer.RegisterType<DataFormateValidationBase, TextValidation>("TEXT");
                //TEXT

            }
            return ValidatorContainer.Resolve<DataFormateValidationBase>(DataType);

        }
    }
}