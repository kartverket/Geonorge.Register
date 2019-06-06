using Kartverket.Register.Services;
using System;
using System.Web.Mvc;
using Geonorge.AuthLib.Common;

namespace Kartverket.Register.Controllers
{
    [HandleError]
    public class IndexController : BaseController
    {
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private readonly RegisterIndexer _indexer;

        public IndexController(RegisterIndexer indexer)
        {
            _indexer = indexer;
        }

        [Authorize(Roles = GeonorgeRoles.MetadataAdmin)]
        public ActionResult Index()
        {
            Log.Info("Run indexing of entire register.");
            DateTime start = DateTime.Now;
            
            _indexer.RunIndexing();

            DateTime stop = DateTime.Now;
            double seconds = stop.Subtract(start).TotalSeconds;
            Log.Info($"Indexing fininshed after {seconds} seconds.");

            return View();
        }

        [Authorize(Roles = GeonorgeRoles.MetadataAdmin)]
        [Route("IndexSingle/{systemId}")]
        public ActionResult IndexSingle(string systemId)
        {
            Log.Info("Run indexing of single register.");
            DateTime start = DateTime.Now;

            if (!string.IsNullOrWhiteSpace(systemId))
            {
                Log.Info("Running single indexing of register with systemID=" + systemId);
                _indexer.RunIndexingOn(systemId);
                
            }

            DateTime stop = DateTime.Now;
            double seconds = stop.Subtract(start).TotalSeconds;
            Log.Info($"Indexing fininshed after {seconds} seconds.");

            return View();
        }

        [Authorize(Roles = GeonorgeRoles.MetadataAdmin)]
        public ActionResult ReIndex()
        {
            Log.Info("Run reindexing of entire register.");
            DateTime start = DateTime.Now;

            _indexer.RunReIndexing();

            DateTime stop = DateTime.Now;
            double seconds = stop.Subtract(start).TotalSeconds;
            Log.Info($"Indexing fininshed after {seconds} seconds.");
       
            return View();
        }

        //[HttpPost]
        //[ValidateInput(false)] // allow posting of XML to go through
        //public ActionResult MetadataUpdated(string action, string uuid, string XMLFile)
        //{
        //    HttpStatusCode statusCode;
        //    try
        //    {
        //        Log.Info("Received notification of updated metadata: " + Request.HttpMethod + ", " + action + ", " + uuid);

        //        if (!string.IsNullOrWhiteSpace(uuid))
        //        {
        //            Log.Info("Running single indexing of metadata with uuid=" + uuid);

        //            _indexer.RunIndexingOn(uuid);

        //            statusCode = HttpStatusCode.Accepted;
        //        }
        //        else
        //        {
        //            Log.Warn("Not indexing metadata - uuid was empty");
        //            statusCode = HttpStatusCode.BadRequest;
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        Log.Error("Exception while indexing single metadata.", e);
        //        statusCode = HttpStatusCode.BadRequest;
        //    }
        //    return new HttpStatusCodeResult(statusCode);
        //}



        protected override void OnException(ExceptionContext filterContext)
        {
            Log.Error("Error", filterContext.Exception);
        }

    }
}