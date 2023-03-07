using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Unity;

namespace Ivap.DateConverter
{
    public class DateFormatConverterFactory
    {
        //DateConverterDD_MON_YYYY : DateConverterBase
        private static IUnityContainer ValidatorContainer = null;
        public static DateConverterBase Create(String DataType)
        {
            if (ValidatorContainer == null)
            {
                ValidatorContainer = new UnityContainer();
                ValidatorContainer.RegisterType<DateConverterBase, DateConverterBase>("");
                ValidatorContainer.RegisterType<DateConverterBase, DateConverterddmmyyforwordslash>("dd/mm/yy");
                ValidatorContainer.RegisterType<DateConverterBase, DateConverterddmmyyyyforwordslash>("dd/mm/yyyy");
                ValidatorContainer.RegisterType<DateConverterBase, DateConverterddmmyyyydash>("dd-mm-yyyy");
                ValidatorContainer.RegisterType<DateConverterBase, DateConverteryyyymmdddash>("yyyy-mm-dd");
                ValidatorContainer.RegisterType<DateConverterBase, DateConverterDD_MON_YYYY>("DD-MON-YYYY");
            }
            return ValidatorContainer.Resolve<DateConverterBase>(DataType);

        }
    }
}