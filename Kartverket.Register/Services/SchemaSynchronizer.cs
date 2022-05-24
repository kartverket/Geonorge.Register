using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.Net;
using System.IO;
using System.Xml;
using System.Threading.Tasks;
using Kartverket.Geonorge.Utilities.LogEntry;
using System.Security.Claims;
using Geonorge.AuthLib.Common;
using System.Web.Configuration;
using Renci.SshNet;
using Renci.SshNet.Sftp;
using Renci.SshNet.Common;

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
        static string TargetNamespaceTest = "http://skjema.test.geonorge.no/SOSI/produktspesifikasjon/";
        static string TargetNamespaceSecure = "https://skjema.geonorge.no/SOSI/produktspesifikasjon/";
        static string TargetNamespaceTestSecure = "https://skjema.test.geonorge.no/SOSI/produktspesifikasjon/";
        string SchemaRemoteUrl = WebConfigurationManager.AppSettings["SchemaRemoteUrl"];
        string SchemaRemoteUrlTest = WebConfigurationManager.AppSettings["SchemaRemoteUrlTest"];

        string SchemaFtpSite = WebConfigurationManager.AppSettings["SchemaFtpSite"];
        string SchemaUsernamesBaatMappings = WebConfigurationManager.AppSettings["SchemaUsernamesBaatMappings"];
        string SchemaUsernames = WebConfigurationManager.AppSettings["SchemaFtpUsernames"];
        string SchemaPasswords = WebConfigurationManager.AppSettings["SchemaFtpPasswords"];
        string SchemaFtpWorkingDirectory = WebConfigurationManager.AppSettings["SchemaFtpWorkingDirectory"]; 

        string SchemaFtpSiteTest = WebConfigurationManager.AppSettings["SchemaFtpSiteTest"];
        string SchemaUsernameTest = WebConfigurationManager.AppSettings["SchemaFtpUsernameTest"];
        string SchemaPasswordTest = WebConfigurationManager.AppSettings["SchemaFtpPasswordTest"];
        string SchemaFtpWorkingDirectoryTest = WebConfigurationManager.AppSettings["SchemaFtpWorkingDirectoryTest"];

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
                else 
                {
                    Log.Error("Ugyldig skjemaplassering:" + targetNamespace);
                    return "IllegalSchemaLocation";
                }
            }

            return syncFile;
        }

        private bool UserHasAccess()
        {
            var user = ClaimsPrincipal.Current.GetUsername();
            string[] users = SchemaUsernamesBaatMappings.Split(','); 

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
            path = path.Replace(TargetNamespaceTest, "");
            path = path.Replace(TargetNamespaceSecure, "");
            path = path.Replace(TargetNamespaceTestSecure, "");
            return path;
        }

        bool ValidTargetNamespace(XmlNode node)
        {
            return node != null && ((node.Value.Contains(TargetNamespace) || node.Value.Contains(TargetNamespaceTest))
                || (node.Value.Contains(TargetNamespaceSecure) || node.Value.Contains(TargetNamespaceTestSecure))) ;
        }

        string UploadFile(HttpPostedFileBase file, string path)
        {
            try
            {

            
                using (var sftp = new SftpClient(SchemaFtpSiteTest, SchemaUsernameTest, SchemaPasswordTest))
                {
                    sftp.Connect();

                    string[] subDirs = path.Split('/');

                    string currentDir = SchemaFtpWorkingDirectoryTest;

                    foreach (string subDir in subDirs)
                    {
                        currentDir = currentDir + "/" + subDir;
                        try { 
                        SftpFile folder = sftp.Get(currentDir);
                        }
                        catch(SftpPathNotFoundException ex)
                        {
                            sftp.CreateDirectory(currentDir);
                        }

                    }

                    var filePath = currentDir + "/" + file.FileName;
                    var fileStream = file.InputStream;
                    fileStream.Position = 0;

                    sftp.UploadFile(fileStream, filePath, true);
                    sftp.Disconnect();
                }
            }
            catch (Exception ex) {
                Log.Error("Sftp opplasting til skjema test feilet.", ex);
                throw new Exception("Sftp skjema test feilet");
            }

            Task.Run(() => LogEntryService.AddLogEntry(new LogEntry { ElementId = path + "/" + file.FileName, Operation = Operation.Added, User = ClaimsPrincipal.Current.GetUsername(), Description = "Ftp gml-skjema til test" }));

            return SchemaRemoteUrlTest + path + "/" + file.FileName;
        }

        private string UploadFileProd(string path, string filename)
        {
            try
            {
                User user = GetUser();

                MemoryStream stream = new MemoryStream();

                using (var sftp = new SftpClient(SchemaFtpSiteTest, SchemaUsernameTest, SchemaPasswordTest))
                {
                    sftp.Connect();

                    sftp.DownloadFile(SchemaFtpWorkingDirectoryTest + "/" + path + "/" + filename, stream);
                    sftp.Disconnect();
                }

                using (var sftp = new SftpClient(SchemaFtpSite, user.Username, user.Password))
                {
                    sftp.Connect();

                    string[] subDirs = path.Split('/');

                    string currentDir = SchemaFtpWorkingDirectory;

                    foreach (string subDir in subDirs)
                    {
                        currentDir = currentDir + "/" + subDir;
                        try
                        {
                            SftpFile folder = sftp.Get(currentDir);
                        }
                        catch (SftpPathNotFoundException ex)
                        {
                            sftp.CreateDirectory(currentDir);
                        }
                    }

                    stream.Position = 0;

                    var filePath = currentDir + "/" + filename;
                    sftp.UploadFile(stream, filePath, true);
                    sftp.Disconnect();
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                throw new Exception("Sftp skjema feilet");
            }

            Task.Run(() => LogEntryService.AddLogEntry(new LogEntry { ElementId = path + "/" + filename, Operation = Operation.Added, User = ClaimsPrincipal.Current.GetUsername(), Description = "Ftp gml-skjema til prod" }));

            return SchemaRemoteUrl + path + "/" + filename;
        }

        private User GetUser()
        {
            var user = ClaimsPrincipal.Current.GetUsername();
            string[] usersBaat = SchemaUsernamesBaatMappings.Split(',');
            string[] usersFtp = SchemaUsernames.Split(',');
            string[] passwords = SchemaPasswords.Split(',');

            for (int u = 0; u < usersBaat.Length; u++)
            {
                if(usersBaat[u] == user)
                {
                    return new User { Username = usersFtp[u], Password = passwords[u] };
                }
            }
          
            Log.Error("User " + user + " does not have sftp rights");
            throw new Exception("Permission denied");

        }

    }

    class User
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
