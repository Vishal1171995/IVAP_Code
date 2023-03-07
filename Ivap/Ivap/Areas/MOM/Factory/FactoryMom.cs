using Ivap.Areas.MOM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Ivap.Areas.MOM.Factory
{
    public static class FactoryMom
    {
        private static Dictionary<string, MomBase> objMomBase = 
            new Dictionary<string, MomBase>();
        public static MomBase Create(string typeMom)
        {
            if (objMomBase.Count == 0) {
                objMomBase.Add("PLANMOM", new PlanMom());
            }
            return objMomBase[typeMom];
        }
    }
}