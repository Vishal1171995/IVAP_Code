using Ivap.App_Start;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Optimization;

namespace Ivap.Areas.FileExplorer.AreaStart
{
    public class FileExplorerBundle
    {
        public static void RegisterFileExplorerBundles(BundleCollection Mbundles)
        {
            var FileMetaDatabundle = (new ScriptBundle("~/Scripts/FileMetaDataScript").Include(
             "~/Areas/FileExplorer/Scripts/FileMetaDataScript.js"
              ));
            FileMetaDatabundle.Orderer = new NonOrderingBundleOrderer();
            Mbundles.Add(FileMetaDatabundle);

            var FileExplorerbundle = (new ScriptBundle("~/Scripts/FileExplorerScript").Include(
             "~/Areas/FileExplorer/Scripts/FileExplorerScript.js"
              ));
            FileExplorerbundle.Orderer = new NonOrderingBundleOrderer();
            Mbundles.Add(FileExplorerbundle);
        }
    }
}