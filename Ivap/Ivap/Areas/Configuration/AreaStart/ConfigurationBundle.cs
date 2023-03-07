using Ivap.App_Start;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Optimization;

namespace Ivap.Areas.Configuration.AreaStart
{
    public class ConfigurationBundle
    {
        public static void RegisterConfigurationBundles(BundleCollection Mbundles)
        {
            var CalendarSetuptbundle = (new ScriptBundle("~/Scripts/CalendarSetupScript").Include(
           "~/Areas/Configuration/Scripts/CalendarSetupScript.js"
            ));
            CalendarSetuptbundle.Orderer = new NonOrderingBundleOrderer();
            Mbundles.Add(CalendarSetuptbundle);

            var DataAccessControlbundle = (new ScriptBundle("~/Scripts/DataAccessControlScript").Include(
           "~/Areas/Configuration/Scripts/DataAccessControlScript.js"
            ));
            DataAccessControlbundle.Orderer = new NonOrderingBundleOrderer();
            Mbundles.Add(DataAccessControlbundle);

            var EntityComponentbundle = (new ScriptBundle("~/Scripts/EntityComponentScript").Include(
           "~/Areas/Configuration/Scripts/EntityComponentScript.js"
            ));
            EntityComponentbundle.Orderer = new NonOrderingBundleOrderer();
            Mbundles.Add(EntityComponentbundle);

            var FileSetupbundle = (new ScriptBundle("~/Scripts/FileSetupScript").Include(
           "~/Areas/Configuration/Scripts/FileSetupScript.js"
            ));
            FileSetupbundle.Orderer = new NonOrderingBundleOrderer();
            Mbundles.Add(FileSetupbundle);

            var GlobalComponentbundle = (new ScriptBundle("~/Scripts/GlobalComponentScript").Include(
           "~/Areas/Configuration/Scripts/GlobalComponentScript.js"
            ));
            GlobalComponentbundle.Orderer = new NonOrderingBundleOrderer();
            Mbundles.Add(GlobalComponentbundle);

            var MasterMetabundle = (new ScriptBundle("~/Scripts/MasterMetaScript").Include(
           "~/Areas/Configuration/Scripts/MasterMetaScript.js"
            ));
            MasterMetabundle.Orderer = new NonOrderingBundleOrderer();
            Mbundles.Add(MasterMetabundle);

            var Menubundle = (new ScriptBundle("~/Scripts/MenuScript").Include(
           "~/Areas/Configuration/Scripts/MenuScript.js"
            ));
            Menubundle.Orderer = new NonOrderingBundleOrderer();
            Mbundles.Add(Menubundle);

            var MonthClosebundle = (new ScriptBundle("~/Scripts/MonthCloseScript").Include(
           "~/Areas/Configuration/Scripts/MonthCloseScript.js"
            ));
            MonthClosebundle.Orderer = new NonOrderingBundleOrderer();
            Mbundles.Add(MonthClosebundle);

            var WorkFlowSettingbundle = (new ScriptBundle("~/Scripts/WorkFlowSettingScript").Include(
         "~/Areas/Configuration/Scripts/WorkFlowSettingScript.js"
          ));
            WorkFlowSettingbundle.Orderer = new NonOrderingBundleOrderer();
            Mbundles.Add(WorkFlowSettingbundle);

        }

     }
}