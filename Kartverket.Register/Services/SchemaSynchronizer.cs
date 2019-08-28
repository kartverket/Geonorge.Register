using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.Net;
using System.IO;
using System.Xml;

namespace Kartverket.Register.Services
{
    public class SchemaSynchronizer
    {
        string UncPath = System.Web.Configuration.WebConfigurationManager.AppSettings["SchemaUNCpath"];
        static string TargetNamespace = "http://skjema.geonorge.no/SOSI/produktspesifikasjon/";
        string SchemaRemoteUrl = System.Web.Configuration.WebConfigurationManager.AppSettings["SchemaRemoteUrl"];

        public string Synchronize(HttpPostedFileBase file, string filename)
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
                    syncFile = UploadFile(file, path, filename);
                }
            }

            return syncFile;
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

        string UploadFile(HttpPostedFileBase file, string path, string filename)
        {
            MakeDir(path);

            file.SaveAs(UncPath + "\\" + path + "\\" + filename);
            
            return SchemaRemoteUrl + path + "/" + filename;
        }

        public void MakeDir(string path)
        {
            string[] subDirs = path.Split('/');


            string currentDir = UncPath;

            foreach (string subDir in subDirs)
            {
                try
                {
                    currentDir = currentDir + "\\" + subDir;
                    Directory.CreateDirectory(currentDir);
                }
                catch (Exception)
                {
                }
            }
        }

    }
}
