using System.Web;
using System.Web.Optimization;

namespace TestMVC
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                      "~/Scripts/bootstrap.min.js",
                      "~/Scripts/respond.js",

                      "~/Scripts/jquery-1.10.2.min.js",
                      "~/Scripts/jquery.validate.js",
                      "~/Scripts/jquery.validate.unobtrusive.js",
                      "~/Scripts/jqueryDataTable.js",
                      "~/Scripts/jqueryConfirm.js"
                     
                      ));
            bundles.Add(new StyleBundle("~/bundles/css").Include(
                "~/Content/jqueryDataTables.css",
                "~/Content/jquery-confirm.css",
                "~/fonts/font-awesome-4.7.0/css/font-awesome.min.css"
               
                ));
            
            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/site.css"));



           
        }
    }
}
