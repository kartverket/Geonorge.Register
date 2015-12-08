using System.Web.Optimization;

namespace Kartverket.Register
{
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new StyleBundle("~/Content/css").Include(
                "~/Content/custom.css"));

            bundles.Add(new ScriptBundle("~/bundles/js").Include(
                "~/Scripts/visninger.js",
                "~/Scripts/tabs.js"));

            BundleTable.EnableOptimizations = true;
        }
    }
}