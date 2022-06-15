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
using Kartverket.Register.Models;

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
        string SchemaUsername = WebConfigurationManager.AppSettings["SchemaFtpUsername"];
        string SchemaPassword = WebConfigurationManager.AppSettings["SchemaFtpPassword"];
        string SchemaFtpWorkingDirectory = WebConfigurationManager.AppSettings["SchemaFtpWorkingDirectory"]; 

        string SchemaFtpSiteTest = WebConfigurationManager.AppSettings["SchemaFtpSiteTest"];
        string SchemaUsernameTest = WebConfigurationManager.AppSettings["SchemaFtpUsernameTest"];
        string SchemaPasswordTest = WebConfigurationManager.AppSettings["SchemaFtpPasswordTest"];
        string SchemaFtpWorkingDirectoryTest = WebConfigurationManager.AppSettings["SchemaFtpWorkingDirectoryTest"];

        public string Synchronize(HttpPostedFileBase file, HttpPostedFileBase schematronfile = null)
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
                    syncFile = UploadFile(file, path, schematronfile);
                }
                else 
                {
                    Log.Error("Ugyldig skjemaplassering:" + targetNamespace);
                    return "IllegalSchemaLocation";
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

        string UploadFile(HttpPostedFileBase file, string path, HttpPostedFileBase schematronfile = null)
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

                    if(schematronfile != null) 
                    {
                        filePath = currentDir + "/" + schematronfile.FileName;
                        fileStream = schematronfile.InputStream;
                        fileStream.Position = 0;

                        sftp.UploadFile(fileStream, filePath, true);
                    }

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
                MemoryStream streamSchematron = new MemoryStream();
                bool schematronFile;

                using (var sftp = new SftpClient(SchemaFtpSiteTest, SchemaUsernameTest, SchemaPasswordTest))
                {
                    var filePath = SchemaFtpWorkingDirectoryTest + "/" + path + "/" + filename;
                    sftp.Connect();

                    sftp.DownloadFile(filePath, stream);

                    filePath = filePath.Replace(".xsd", ".sch");

                    schematronFile = sftp.Exists(filePath);

                    if (schematronFile) {
                        sftp.DownloadFile(filePath, streamSchematron);
                    }

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

                    if (schematronFile) 
                    {
                        streamSchematron.Position = 0;
                        filePath = currentDir + "/" + filename.Replace(".xsd", ".sch");
                        sftp.UploadFile(streamSchematron, filePath, true);
                    }

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
           return new User { Username = SchemaUsername, Password = SchemaPassword };
        
        }

        internal void RemoveFiles(Document document)
        {
            try
            {
                var filePath = new Uri(document.documentUrl).AbsolutePath.Replace("/SOSI/produktspesifikasjon","");

                if (document.documentUrl.Contains(SchemaRemoteUrl) || document.documentUrl.Contains(SchemaRemoteUrlTest))
                {
                    using (var sftp = new SftpClient(SchemaFtpSiteTest, SchemaUsernameTest, SchemaPasswordTest))
                    {
                        sftp.Connect();

                        sftp.DeleteFile(SchemaFtpWorkingDirectoryTest + filePath);

                        bool schematronFile = sftp.Exists(SchemaFtpWorkingDirectoryTest + filePath.Replace(".xsd", ".sch"));

                        if (schematronFile)
                        {
                            sftp.DeleteFile(SchemaFtpWorkingDirectoryTest + filePath.Replace(".xsd", ".sch"));
                        }

                        sftp.Disconnect();
                    }
                }

                if (document.documentUrl.Contains(SchemaRemoteUrl))
                {
                    User user = GetUser();

                    using (var sftp = new SftpClient(SchemaFtpSite, user.Username, user.Password))
                    {
                        sftp.Connect();

                        sftp.DeleteFile(SchemaFtpWorkingDirectory + filePath);

                        bool schematronFile = sftp.Exists(SchemaFtpWorkingDirectory + filePath.Replace(".xsd",".sch"));

                        if (schematronFile)
                        {
                            sftp.DeleteFile(SchemaFtpWorkingDirectory + filePath.Replace(".xsd", ".sch"));
                        }

                        sftp.Disconnect();
                    }

                }
            }
            catch (Exception ex) {
                Log.Error("Error delete xsd file", ex);
            }
        }
    }

    class User
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
