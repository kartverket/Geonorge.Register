using System.Web.Optimization;

namespace Kartverket.Register
{
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {


            bundles.Add(new StyleBundle("~/node_modules/geonorge-base/assets/css/styles").Include(
                "~/node_modules/geonorge-base/assets/css/vendor.min.css",
                "~/node_modules/geonorge-base/assets/css/vendorfonts.min.css",
                "~/node_modules/geonorge-base/assets/css/all.min.css"
            ));

            bundles.Add(new StyleBundle("~/shared-partials-styles").Include(
           "~/dist/vendor.css",
           "~/dist/main.css"
           ));

            bundles.Add(new ScriptBundle("~/shared-partials-scripts").Include(
               "~/dist/vendor.js",
               "~/dist/main.js"
              
           ));

            bundles.Add(new ScriptBundle("~/node_modules/geonorge-base/assets/js/scripts").Include(
               "~/node_modules/geonorge-base/assets/js/vendor.min.js",
               "~/node_modules/geonorge-base/assets/js/main.min.js",
                "~/node_modules/vue/dist/vue.min.js",
                "~/node_modules/chart.js/dist/chart.min.js"
           ));


            bundles.Add(new StyleBundle("~/Content/local-styles").Include(
                "~/Content/leaflet.css",
                "~/Content/custom.css"));

            bundles.Add(new ScriptBundle("~/Scripts/local-scripts").Include(
                "~/Scripts/jquery.validate.date.js",
                "~/Scripts/jquery.validate.min.js",
                "~/Scripts/visninger.js",
                "~/Scripts/tabs.js",
                "~/Scripts/leaflet.js",
                "~/Scripts/L.TileLayer.Kartverket.min.js",
                "~/Scripts/proj4-compressed.js",
                "~/node_modules/tablesorter/dist/js/jquery.tablesorter.min.js",
                "~/Scripts/geonorge-register.js",
                "~/Scripts/proj4leaflet.js"));

            BundleTable.EnableOptimizations = true;

        }
    }
}