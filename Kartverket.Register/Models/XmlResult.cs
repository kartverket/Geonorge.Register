using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Xml.Serialization;

namespace Kartverket.Register.Models
{
    public class XmlResult : ActionResult
    {
        private object _objectToSerialize;

        public XmlResult(object objectToSerialize)
        {
            _objectToSerialize = objectToSerialize;
        }

        public override void ExecuteResult(ControllerContext context)
        {
            if (_objectToSerialize != null)
            {
                var xs = new XmlSerializer(_objectToSerialize.GetType());
                context.HttpContext.Response.ContentType = "text/xml";
                xs.Serialize(context.HttpContext.Response.Output, _objectToSerialize);
            }
        }

    }
}