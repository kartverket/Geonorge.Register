using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Microsoft.Owin;
using Moq;

namespace Helpers
{
    public class MvcMockHelper
    {
        public Mock<HttpContextBase> HttpContext { get; private set; }
        public Mock<HttpRequestBase> Request { get; private set; }
        public Mock<HttpResponseBase> Response { get; private set; }
        public Mock<HttpSessionStateBase> Session { get; private set; }
        public Mock<HttpServerUtilityBase> Server { get; private set; }
        public OwinContext OwinContext { get; private set; }

        public MvcMockHelper()
        {
            HttpContext = new Mock<HttpContextBase>();
            Request = new Mock<HttpRequestBase>();
            Response = new Mock<HttpResponseBase>();
            Session = new Mock<HttpSessionStateBase>();
            Server = new Mock<HttpServerUtilityBase>();

            HttpContext.Setup(ctx => ctx.Request).Returns(Request.Object);
            HttpContext.Setup(ctx => ctx.Request.Url).Returns(new Uri("http://TestUrl/"));
            HttpContext.Setup(ctx => ctx.Response).Returns(Response.Object);
            HttpContext.Setup(ctx => ctx.Session).Returns(Session.Object);
            HttpContext.SetupGet(x => x.Response.Cookies).Returns(new HttpCookieCollection());
            HttpContext.Setup(ctx => ctx.Server).Returns(Server.Object);
            HttpContext.Setup(ctx => ctx.Items).Returns(new Dictionary<string, object>());
        }

        public MvcMockHelper For(Controller controller)
        {
            ControllerContext context = new ControllerContext(new RequestContext(HttpContext.Object, new RouteData()), controller);
            controller.ControllerContext = context;

            var urlHelper = new Mock<UrlHelper>();
            urlHelper.Setup(x => x.Action(It.IsAny<string>(), It.IsAny<string>(), null, It.IsAny<string>())).Returns("");
            controller.Url = urlHelper.Object;

            OwinContext = new OwinContext();
            controller.HttpContext.Items["owin.Environment"] = OwinContext.Environment;

            return this;
        }

        public MvcMockHelper SetHttpMethodResult(string httpMethod)
        {
            Request
                .Setup(req => req.HttpMethod)
                .Returns(httpMethod);

            return this;
        }

        public MvcMockHelper SetupRequestUrl(string url)
        {
            if (url == null)
                throw new ArgumentNullException("url");

            if (!url.StartsWith("~/"))
                throw new ArgumentException("Sorry, we expect a virtual url starting with \"~/\".");

            var mock = Request;

            mock.Setup(req => req.QueryString)
                .Returns(GetQueryStringParameters(url));
            mock.Setup(req => req.AppRelativeCurrentExecutionFilePath)
                .Returns(GetUrlFileName(url));
            mock.Setup(req => req.PathInfo)
                .Returns(string.Empty);

            return this;
        }

        static string GetUrlFileName(string url)
        {
            if (url.Contains("?"))
                return url.Substring(0, url.IndexOf("?"));
            else
                return url;
        }

        static NameValueCollection GetQueryStringParameters(string url)
        {
            if (url.Contains("?"))
            {
                NameValueCollection parameters = new NameValueCollection();

                string[] parts = url.Split("?".ToCharArray());
                string[] keys = parts[1].Split("&".ToCharArray());

                foreach (string key in keys)
                {
                    string[] part = key.Split("=".ToCharArray());
                    parameters.Add(part[0], part[1]);
                }

                return parameters;
            }
            else
            {
                return null;
            }
        }
    }
}
