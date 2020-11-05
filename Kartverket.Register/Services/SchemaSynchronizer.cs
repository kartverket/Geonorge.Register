using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.Net;
using System.IO;
using System.Xml;
using FluentFTP;

namespace Kartverket.Register.Services
{
    public class SchemaSynchronizer
    {
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        string UncPath = System.Web.Configuration.WebConfigurationManager.AppSettings["SchemaUNCpath"];
        string UncPathTest = System.Web.Configuration.WebConfigurationManager.AppSettings["SchemaUNCpathTest"];
        static string TargetNamespace = "http://skjema.geonorge.no/SOSI/produktspesifikasjon/";
        string SchemaRemoteUrl = System.Web.Configuration.WebConfigurationManager.AppSettings["SchemaRemoteUrl"];
        string SchemaRemoteUrlTest = System.Web.Configuration.WebConfigurationManager.AppSettings["SchemaRemoteUrlTest"];

        string SchemaFtpSite = System.Web.Configuration.WebConfigurationManager.AppSettings["SchemaFtpSite"];
        string SchemaUsername = System.Web.Configuration.WebConfigurationManager.AppSettings["SchemaUsername"];
        string SchemaPassword = System.Web.Configuration.WebConfigurationManager.AppSettings["SchemaPassword"];

        string SchemaFtpSiteTest = System.Web.Configuration.WebConfigurationManager.AppSettings["SchemaFtpSiteTest"];
        string SchemaUsernameTest = System.Web.Configuration.WebConfigurationManager.AppSettings["SchemaUsernameTest"];
        string SchemaPasswordTest = System.Web.Configuration.WebConfigurationManager.AppSettings["SchemaPasswordTest"];

        public string Synchronize(HttpPostedFileBase file)
        {
            string syncFile = "";
            if (file != null && file.ContentLength > 0 && (file.ContentType == "text/xml" || file.ContentType == "application/xml"))
            {
                var document = new XmlDocument();
                document.Load(file.InputStream);
                var attributes = document.DocumentElement.Attributes;
                var targetNamespace = attributes.GetNamedItem("targetNamespace");
                if (ValidTargetNamespace(targetNamespace))
                {
                    string path = GetFilePath(targetNamespace.Value);
                    syncFile = UploadFile(file, path);
                }
            }

            return syncFile;
        }

        public string Synchronize(string url)
        {
            var uri = new Uri(url);
            var filename = uri.Segments.Last();
            string mainPath = "/SOSI/produktspesifikasjon/";
            int ix = url.IndexOf(mainPath);

            if (ix != -1)
            {
                string path = url.Substring(ix + mainPath.Length, url.Length - 1 - (ix + mainPath.Length + filename.Length));
                url = UploadFileProd(path, filename);
            }

            return url;
        }

        private string GetFilePath(string path)
        {
            path = path.Replace(TargetNamespace, "");
            return path;
        }

        bool ValidTargetNamespace(XmlNode node)
        {
            return node != null && node.Value.Contains(TargetNamespace);
        }

        string UploadFile(HttpPostedFileBase file, string path)
        {
            try
            {
                Uri uri = new Uri(SchemaFtpSiteTest);
                FtpClient client = new FtpClient(uri.Scheme + Uri.SchemeDelimiter + uri.Host);
                client.OnLogEvent = OnFTPLogEvent;
                client.Port = uri.Port;
                client.Credentials = new NetworkCredential(SchemaUsernameTest, SchemaPasswordTest);

                client.Connect();

                FtpStatus status = client.Upload(file.InputStream, path + "/" + file.FileName, FtpRemoteExists.Overwrite, true);

                if (status == FtpStatus.Failed) {
                    Log.Error("Opplasting av fil " + path + "/" + file.FileName + " feilet");
                    throw new Exception("Opplasting til ftp server feilet");
                }

                client.Disconnect();
            }
            catch (Exception ex) {
                Log.Error(ex);
                throw new Exception("Ftp skjema feilet");
            }

            return SchemaRemoteUrlTest + path + "/" + file.FileName;
        }

        private string UploadFileProd(string path, string filename)
        {
            try
            {
                Uri uri = new Uri(SchemaFtpSiteTest);
                FtpClient source = new FtpClient(uri.Scheme + Uri.SchemeDelimiter + uri.Host);
                source.OnLogEvent = OnFTPLogEvent;
                source.Port = uri.Port;
                source.Credentials = new NetworkCredential(SchemaUsernameTest, SchemaPasswordTest);

                source.Connect();

                MemoryStream stream = new MemoryStream();

                source.Download(stream, path + "/" + filename);


                uri = new Uri(SchemaFtpSite);
                FtpClient target = new FtpClient(uri.Scheme + Uri.SchemeDelimiter + uri.Host);
                target.OnLogEvent = OnFTPLogEvent;
                target.Port = uri.Port;
                target.Credentials = new NetworkCredential(SchemaUsername, SchemaPassword);

                target.Connect();

                FtpStatus status = target.Upload(stream, path + "/" + filename, FtpRemoteExists.Overwrite, true);

                if (status == FtpStatus.Failed)
                {
                    Log.Error("Kopiering av fil " + path + "/" + filename + " feilet");
                    throw new Exception("Kopiering mellom til ftp servere feilet");
                }

                source.Disconnect();
                //target.Disconnect();
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                throw new Exception("Ftp skjema feilet");
            }


            return SchemaRemoteUrl + path + "/" + filename;
        }

        private static void OnFTPLogEvent(FtpTraceLevel ftpTraceLevel, string logMessage)
        {
            switch (ftpTraceLevel)
            {
                case FtpTraceLevel.Error:
                    Log.Error(logMessage);
                    break;
                case FtpTraceLevel.Verbose:
                    Log.Debug(logMessage);
                    break;
                case FtpTraceLevel.Warn:
                    Log.Warn(logMessage);
                    break;
                case FtpTraceLevel.Info:
                default:
                    Log.Info(logMessage);
                    break;
            }
        }

    }
}
