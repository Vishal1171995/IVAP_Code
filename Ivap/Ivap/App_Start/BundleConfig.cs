using Ivap;
using Ivap.Areas.Master.AreaStart;
using System.Web;
using System.Web.Optimization;
using Ivap.Areas.Configuration.AreaStart;
using Ivap.Areas.FileExplorer.AreaStart;
using Ivap.Areas.MOM.AreaStart;
using Ivap.Areas.CAPA.AreaStart;

namespace Ivap.App_Start
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {

            var bundle = (new StyleBundle("~/Content/css/LoginCss")
                       .Include(
                                "~/Content/css/bootstrap.min.css",
                                "~/Content/css/CustomStyle.css",
                                "~/Content/css/TextBoxEffect.css"
                               //"~/Content/css/bootstrap.css",
                               //"~/Content/css/mui.min.css",
                               //"~/Content/css/style.css",
                               //"~/Content/css/font-awesome.min.css",
                               //"~/Content/css/jquery-ui.css",
                               // "~/Content/css/CommanStyle.css"
                           )
                       );
            bundle.Orderer = new NonOrderingBundleOrderer();
            bundles.Add(bundle);

            var CommanCssBundle = (new StyleBundle("~/Content/css/CommanCssBundle")
                       .Include(
                               "~/Content/css/bootstrap.min.css",
                               "~/Content/css/CustomStyle.css",
                               "~/Content/css/TextBoxEffect.css",
                               "~/Content/css/progress-tracker.css",
                               //"~/Content/css/timeline.css",
                               "~/Content/css/Kendu.css",
                               "~/Content/css/jquery-ui.css",
                               "~/Kendo/styles/kendo.common.min.css",
                               //"~/Kendo/styles/kendo.rtl.min.css",
                               "~/Kendo/styles/kendo.default.min.css"
                               //"~/Kendo/styles/kendo.dataviz.min.css",
                               //"~/Kendo/styles/kendo.dataviz.default.min.css"
                           )
                       );
            CommanCssBundle.Orderer = new NonOrderingBundleOrderer();
            bundles.Add(CommanCssBundle);
            //mui.min.js
            var LoginScript = (new ScriptBundle("~/Scripts/LoginScript")
                                .Include(
                                        "~/Scripts/jquery-3.2.1.min.js",
                                        "~/Scripts/bootstrap.min.js",
                                        "~/Scripts/jquery-ui-1.12.1.min.js",
                                        "~/Scripts/mui.min.js",
                                        "~/Scripts/jquery.unobtrusive*",
                                        "~/Scripts/jquery.validate*",
                                        "~/Scripts/Customscript/CommanUtills.js"
                                    )
                                );
            LoginScript.Orderer = new NonOrderingBundleOrderer();
            bundles.Add(LoginScript);

            var CommanScript = (new ScriptBundle("~/Scripts/CommanScript")
                              .Include(
                                     "~/Scripts/jquery-3.2.1.min.js",
                                     "~/Content/js/jquery.min.js",
                                     "~/Content/js/enscroll-0.6.2.min.js",
                                        "~/Scripts/bootstrap.min.js",
                                        "~/Scripts/jquery-ui-1.12.1.min.js",
                                        "~/Scripts/jquery.unobtrusive*",
                                        "~/Scripts/jquery.validate*",
                                        "~/Scripts/adminlte.min.js",
                                        "~/Kendo/js/kendo.all.min.js",
                                      "~/Kendo/js/kendo.window.min.js",
                                      "~/Kendo/content/shared/js/console.js",
                                      "~/Scripts/Customscript/CommanUtills.js",
                                      "~/Kendo/js/jszip.min.js",
                                      "~/Content/js/slick.js"

                                  )
                              );
            CommanScript.Orderer = new NonOrderingBundleOrderer();
            bundles.Add(CommanScript);
            MasterBundle.RegisterMasterBundles(bundles);
            ConfigurationBundle.RegisterConfigurationBundles(bundles);
            FileExplorerBundle.RegisterFileExplorerBundles(bundles);
            MomBundle.RegisterMomBundles(bundles);
            CapaBundle.RegisterCAPABundles(bundles);

        }
    }
}
