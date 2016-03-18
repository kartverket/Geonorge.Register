using System.Web.Optimization;

namespace Kartverket.Register
{
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            

            bundles.Add(new StyleBundle("~/Content/bower-styles").Include(
                "~/Content/bower_components/kartverket-felleskomponenter/assets/css/vendor.min.css",
                "~/Content/bower_components/kartverket-felleskomponenter/assets/css/main.min.css"
            ));

            bundles.Add(new ScriptBundle("~/bundles/bower-scripts").Include(
               "~/Content/bower_components/kartverket-felleskomponenter/assets/js/vendor.min.js",
               "~/Content/bower_components/kartverket-felleskomponenter/assets/js/main.js"
           ));


            bundles.Add(new StyleBundle("~/Content/local-styles").Include(
                "~/Content/leaflet.css",
                "~/Content/statusIcons.css",
                "~/Content/custom.css"));

            bundles.Add(new ScriptBundle("~/bundles/local-scripts").Include(
                "~/Scripts/jquery.validate.date.js",
                "~/Scripts/jquery.validate.min.js",
                "~/Scripts/visninger.js",
                "~/Scripts/tabs.js",
                "~/Scripts/leaflet.js",
                "~/Scripts/L.TileLayer.Kartverket.min.js",
                "~/Scripts/proj4-compressed.js",
                "~/Scripts/tablesorter/jquery.tablesorter.combined.js",
                "~/Scripts/geonorge-register.js",
                "~/Scripts/proj4leaflet.js"));
        }        
    }
}