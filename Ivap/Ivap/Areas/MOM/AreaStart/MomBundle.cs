using Ivap.App_Start;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Optimization;

namespace Ivap.Areas.MOM.AreaStart
{
    public class MomBundle
    {
        public static void RegisterMomBundles(BundleCollection Mombundles)
        {
            var _Mombundles = (new ScriptBundle("~/Scripts/MomScript").Include(
            "~/Areas/MOM/Scripts/MOMScript.js"
             ));
            _Mombundles.Orderer = new NonOrderingBundleOrderer();
            Mombundles.Add(_Mombundles);
        }
    }
}