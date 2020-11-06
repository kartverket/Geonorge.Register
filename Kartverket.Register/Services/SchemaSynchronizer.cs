using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.Net;
using System.IO;
using System.Xml;
using FluentFTP;
using System.Threading.Tasks;
using Kartverket.Geonorge.Utilities.LogEntry;
using System.Security.Claims;
using Geonorge.AuthLib.Common;
using System.Web.Configuration;

namespace Kartverket.Register.Services
{
    public class SchemaSynchronizer
    {
        private ILogEntryService _logEntryService;

        public ILogEntryService LogEntryService
        {
            get
            {
                if (_logEntryService == null)
                    _logEntryService = new LogEntryService(WebConfigurationManager.AppSettings["LogApi"], WebConfigurationManager.AppSettings["LogApiKey"], new Kartverket.Geonorge.Utilities.Organization.HttpClientFactory());

                return _logEntryService;
            }
            set { _logEntryService = value; }
        }

        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        static string TargetNamespace = "http://skjema.geonorge.no/SOSI/produktspesifikasjon/";
        string SchemaRemoteUrl = WebConfigurationManager.AppSettings["SchemaRemoteUrl"];
        string SchemaRemoteUrlTest = WebConfigurationManager.AppSettings["SchemaRemoteUrlTest"];

        string SchemaFtpSite = WebConfigurationManager.AppSettings["SchemaFtpSite"];
        string SchemaUsernames = WebConfigurationManager.AppSettings["SchemaFtpUsernames"];
        string SchemaPasswords = WebConfigurationManager.AppSettings["SchemaFtpPasswords"];
        string SchemaFtpWorkingDirectory = WebConfigurationManager.AppSettings["SchemaFtpWorkingDirectory"]; 

        string SchemaFtpSiteTest = WebConfigurationManager.AppSettings["SchemaFtpSiteTest"];
        string SchemaUsernameTest = WebConfigurationManager.AppSettings["SchemaFtpUsernameTest"];
        string SchemaPasswordTest = WebConfigurationManager.AppSettings["SchemaFtpPasswordTest"];

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

        private bool UserHasAccess()
        {
            var user = ClaimsPrincipal.Current.GetUsername();
            string[] users = SchemaUsernames.Split(','); 

            if (users.ToList().Contains(user))
            {
                return true;
            }
            else
            {
                Log.Warn("User " + user + " does not have ftp rights");
                return false;
            }
        }

        public string Synchronize(string url)
        {
            if (UserHasAccess())
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

            Task.Run(() => LogEntryService.AddLogEntry(new LogEntry { ElementId = path + "/" + file.FileName, Operation = Operation.Added, User = ClaimsPrincipal.Current.GetUsername(), Description = "Ftp gml-skjema til test" }));

            return SchemaRemoteUrlTest + path + "/" + file.FileName;
        }

        private string UploadFileProd(string path, string filename)
        {
            try
            {
                User user = GetUser();

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
                target.Credentials = new NetworkCredential(user.Username, user.Password);

                target.Connect();

                if(!string.IsNullOrEmpty(SchemaFtpWorkingDirectory))
                    target.SetWorkingDirectory(SchemaFtpWorkingDirectory);

                FtpStatus status = target.Upload(stream, path + "/" + filename, FtpRemoteExists.Overwrite, true);

                if (status == FtpStatus.Failed)
                {
                    Log.Error("Kopiering av fil " + path + "/" + filename + " feilet");
                    throw new Exception("Kopiering mellom til ftp servere feilet");
                }

                stream.Close();
                source.Disconnect();
                target.Disconnect();
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                throw new Exception("Ftp skjema feilet");
            }

            Task.Run(() => LogEntryService.AddLogEntry(new LogEntry { ElementId = path + "/" + filename, Operation = Operation.Added, User = ClaimsPrincipal.Current.GetUsername(), Description = "Ftp gml-skjema til prod" }));

            return SchemaRemoteUrl + path + "/" + filename;
        }

        private User GetUser()
        {
            var user = ClaimsPrincipal.Current.GetUsername();
            string[] users = SchemaUsernames.Split(',');
            string[] passwords = SchemaPasswords.Split(',');

            for (int u = 0; u < users.Length; u++)
            {
                if(users[u] == user)
                {
                    return new User { Username = users[u], Password = passwords[u] };
                }
            }
          
            Log.Error("User " + user + " does not have ftp rights");
            throw new Exception("Permission denied");

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

    class User
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
