using Ivap.App_Start;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Optimization;

namespace Ivap.Areas.Master.AreaStart
{
    public class MasterBundle
    {
        public static void RegisterMasterBundles(BundleCollection Mbundles)
        {
            var Bankbundle = (new ScriptBundle("~/Scripts/BankScript").Include(
             "~/Areas/Master/Scripts/BankScript.js"
              ));
            Bankbundle.Orderer = new NonOrderingBundleOrderer();
            Mbundles.Add(Bankbundle);

            var Classbundle = (new ScriptBundle("~/Scripts/ClassScript").Include(
            "~/Areas/Master/Scripts/ClassScript.js"
             ));
            Classbundle.Orderer = new NonOrderingBundleOrderer();
            Mbundles.Add(Classbundle);

            var Companybundle = (new ScriptBundle("~/Scripts/CompanyScript").Include(
            "~/Areas/Master/Scripts/CompanyScript.js"
             ));
            Companybundle.Orderer = new NonOrderingBundleOrderer();
            Mbundles.Add(Companybundle);

            var CostCenterbundle = (new ScriptBundle("~/Scripts/CostCenterScript").Include(
            "~/Areas/Master/Scripts/CostCenterScript.js"
             ));
            CostCenterbundle.Orderer = new NonOrderingBundleOrderer();
            Mbundles.Add(CostCenterbundle);

            var Currencybundle = (new ScriptBundle("~/Scripts/CurrencyScript").Include(
            "~/Areas/Master/Scripts/CurrencyScript.js"
             ));
            Currencybundle.Orderer = new NonOrderingBundleOrderer();
            Mbundles.Add(Currencybundle);


            var Departmentbundle = (new ScriptBundle("~/Scripts/DepartmentScript").Include(
            "~/Areas/Master/Scripts/DepartmentScript.js"
             ));
            Departmentbundle.Orderer = new NonOrderingBundleOrderer();
            Mbundles.Add(Departmentbundle);

            var Designationbundle = (new ScriptBundle("~/Scripts/DesignationScript").Include(
            "~/Areas/Master/Scripts/DesignationScript.js"
             ));
            Designationbundle.Orderer = new NonOrderingBundleOrderer();
            Mbundles.Add(Designationbundle);

            var Divisionbundle = (new ScriptBundle("~/Scripts/DivisionScript").Include(
            "~/Areas/Master/Scripts/DivisionScript.js"
             ));
            Divisionbundle.Orderer = new NonOrderingBundleOrderer();
            Mbundles.Add(Divisionbundle);

            var Entitybundle = (new ScriptBundle("~/Scripts/EntityScript").Include(
            "~/Areas/Master/Scripts/EntityScript.js"
             ));
            Entitybundle.Orderer = new NonOrderingBundleOrderer();
            Mbundles.Add(Entitybundle);

            var Functionbundle = (new ScriptBundle("~/Scripts/FunctionScript").Include(
           "~/Areas/Master/Scripts/FunctionScript.js"
            ));
            Functionbundle.Orderer = new NonOrderingBundleOrderer();
            Mbundles.Add(Functionbundle);

            var GlobalLocationbundle = (new ScriptBundle("~/Scripts/GlobalLocationScript").Include(
           "~/Areas/Master/Scripts/GlobalLocationScript.js"
            ));
            GlobalLocationbundle.Orderer = new NonOrderingBundleOrderer();
            Mbundles.Add(GlobalLocationbundle);

            var Gradebundle = (new ScriptBundle("~/Scripts/GradeScript").Include(
           "~/Areas/Master/Scripts/GradeScript.js"
            ));
            Gradebundle.Orderer = new NonOrderingBundleOrderer();
            Mbundles.Add(Gradebundle);

            var LeavingReasonbundle = (new ScriptBundle("~/Scripts/LeavingReasonScript").Include(
           "~/Areas/Master/Scripts/LeavingReasonScript.js"
            ));
            LeavingReasonbundle.Orderer = new NonOrderingBundleOrderer();
            Mbundles.Add(LeavingReasonbundle);

            var Levelbundle = (new ScriptBundle("~/Scripts/LevelScript").Include(
           "~/Areas/Master/Scripts/LevelScript.js"
            ));
            Levelbundle.Orderer = new NonOrderingBundleOrderer();
            Mbundles.Add(Levelbundle);

            var Locationbundle = (new ScriptBundle("~/Scripts/LocationScript").Include(
           "~/Areas/Master/Scripts/LocationScript.js"
            ));
            Locationbundle.Orderer = new NonOrderingBundleOrderer();
            Mbundles.Add(Locationbundle);

            var LWFbundle = (new ScriptBundle("~/Scripts/LWFScript").Include(
           "~/Areas/Master/Scripts/LWFScript.js"
            ));
            LWFbundle.Orderer = new NonOrderingBundleOrderer();
            Mbundles.Add(LWFbundle);

            var MinWagebundle = (new ScriptBundle("~/Scripts/MinWageScript").Include(
           "~/Areas/Master/Scripts/MinWageScript.js"
            ));
            MinWagebundle.Orderer = new NonOrderingBundleOrderer();
            Mbundles.Add(MinWagebundle);

            var Plantbundle = (new ScriptBundle("~/Scripts/PlantScript").Include(
           "~/Areas/Master/Scripts/PlantScript.js"
            ));
            Plantbundle.Orderer = new NonOrderingBundleOrderer();
            Mbundles.Add(Plantbundle);


            var Processbundle = (new ScriptBundle("~/Scripts/ProcessScript").Include(
         "~/Areas/Master/Scripts/ProcessScript.js"
          ));
            Processbundle.Orderer = new NonOrderingBundleOrderer();
            Mbundles.Add(Processbundle);

            var Ptaxbundle = (new ScriptBundle("~/Scripts/PtaxScript").Include(
         "~/Areas/Master/Scripts/PtaxScript.js"
          ));
            Ptaxbundle.Orderer = new NonOrderingBundleOrderer();
            Mbundles.Add(Ptaxbundle);

            var Regionbundle = (new ScriptBundle("~/Scripts/RegionScript").Include(
         "~/Areas/Master/Scripts/RegionScript.js"
          ));
            Regionbundle.Orderer = new NonOrderingBundleOrderer();
            Mbundles.Add(Regionbundle);

            var Rolebundle = (new ScriptBundle("~/Scripts/RoleScript").Include(
         "~/Areas/Master/Scripts/RoleScript.js"
          ));
            Rolebundle.Orderer = new NonOrderingBundleOrderer();
            Mbundles.Add(Rolebundle);

            var Sectionbundle = (new ScriptBundle("~/Scripts/SectionScript").Include(
         "~/Areas/Master/Scripts/SectionScript.js"
          ));
            Sectionbundle.Orderer = new NonOrderingBundleOrderer();
            Mbundles.Add(Sectionbundle);

            var Statebundle = (new ScriptBundle("~/Scripts/StateScript").Include(
       "~/Areas/Master/Scripts/StateScript.js"
        ));
            Statebundle.Orderer = new NonOrderingBundleOrderer();
            Mbundles.Add(Statebundle);

            var SubFunctionbundle = (new ScriptBundle("~/Scripts/SubFunctionScript").Include(
       "~/Areas/Master/Scripts/SubFunctionScript.js"
        ));
            SubFunctionbundle.Orderer = new NonOrderingBundleOrderer();
            Mbundles.Add(SubFunctionbundle);

            var Typebundle = (new ScriptBundle("~/Scripts/TypeScript").Include(
       "~/Areas/Master/Scripts/TypeScript.js"
        ));
            Typebundle.Orderer = new NonOrderingBundleOrderer();
            Mbundles.Add(Typebundle);

            var Userbundle = (new ScriptBundle("~/Scripts/UserScript").Include(
       "~/Areas/Master/Scripts/UserScript.js"
        ));
            Userbundle.Orderer = new NonOrderingBundleOrderer();
            Mbundles.Add(Userbundle);
        }
    }
}