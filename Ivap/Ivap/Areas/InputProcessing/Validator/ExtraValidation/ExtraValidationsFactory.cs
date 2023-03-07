using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Unity;

namespace Ivap.Areas.InputProcessing.Validator.OtherValidation
{
    public class ExtraValidationsFactory
    {
        private static IUnityContainer ValidatorContainer = null;
        public static ExtraValidationsBase Create(String ValidationType)
        {
            if (ValidatorContainer == null)
            {
                ValidatorContainer = new UnityContainer();
                ValidatorContainer.RegisterType<ExtraValidationsBase, ExtraValidationsBase>("");
                ValidatorContainer.RegisterType<ExtraValidationsBase, ExtraValidationsBase>("EXPRESSION");
                ValidatorContainer.RegisterType<ExtraValidationsBase, PanCardValidation>("PAN CARD");
                ValidatorContainer.RegisterType<ExtraValidationsBase, UANValidation>("UAN(PF)");
                ValidatorContainer.RegisterType<ExtraValidationsBase, RegExpValidation>("REGULAR EXPRESSION");
                ValidatorContainer.RegisterType<ExtraValidationsBase, AadharCardValidation>("AADHAR NO");
                ValidatorContainer.RegisterType<ExtraValidationsBase, EmailValidation>("EMAIL");
            }
            return ValidatorContainer.Resolve<ExtraValidationsBase>(ValidationType);
        }
    }
}