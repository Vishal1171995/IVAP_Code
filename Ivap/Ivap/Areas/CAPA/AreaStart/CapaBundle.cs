using Ivap.App_Start;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Optimization;

namespace Ivap.Areas.CAPA.AreaStart
{
    public class CapaBundle
    {
        public static void RegisterCAPABundles(BundleCollection Mbundles)
        {
            var CapaReportbundle = (new ScriptBundle("~/Scripts/CapaReportJSScript").Include(
         "~/Areas/CAPA/Scripts/CapaReportJS.js"
          ));
            CapaReportbundle.Orderer = new NonOrderingBundleOrderer();
            Mbundles.Add(CapaReportbundle);

            var UpdateCapabundle = (new ScriptBundle("~/Scripts/UpdateCapaJSScript").Include(
       "~/Areas/CAPA/Scripts/UpdateCapaJS.js"
        ));
            UpdateCapabundle.Orderer = new NonOrderingBundleOrderer();
            Mbundles.Add(UpdateCapabundle);

            var ViewEditCapabundle = (new ScriptBundle("~/Scripts/ViewEditCapaJS").Include(
       "~/Areas/CAPA/Scripts/ViewEditCapaJS.js"
        ));
            ViewEditCapabundle.Orderer = new NonOrderingBundleOrderer();
            Mbundles.Add(ViewEditCapabundle);

        }
    }
}