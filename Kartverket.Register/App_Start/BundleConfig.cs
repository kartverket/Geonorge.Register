using System.Web.Optimization;

namespace Kartverket.Register
{
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new StyleBundle("~/Content/bower_components/kartverket-felleskomponenter/assets/css/styles").Include(
               "~/Content/bower_components/kartverket-felleskomponenter/assets/css/vendor.min.css",
               "~/Content/bower_components/kartverket-felleskomponenter/assets/css/vendorfonts.min.css",
               "~/Content/bower_components/kartverket-felleskomponenter/assets/css/all.min.css"
               ));

            bundles.Add(new StyleBundle("~/shared-partials-styles").Include(
               "~/dist/vendor.css",
               "~/dist/main.css"
               ));

            bundles.Add(new ScriptBundle("~/Content/bower_components/kartverket-felleskomponenter/assets/js/scripts").Include(
               "~/Content/bower_components/kartverket-felleskomponenter/assets/js/vendor.js",
               "~/Content/bower_components/kartverket-felleskomponenter/assets/js/main.js"
               ));

            bundles.Add(new ScriptBundle("~/shared-partials-scripts").Include(
               "~/dist/vendor.js",
               "~/dist/main.js"
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

            BundleTable.EnableOptimizations = false;

        }
    }
}