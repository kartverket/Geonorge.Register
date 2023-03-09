using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Kartverket.Register.Models
{
    public class DocumentFile
    {
        public string Url { get; set; }
        public string UrlEnglish { get; set; }
        public string UrlSchematron { get; set; }
        public string Filename { get; set; }
        public string Filename2AsciiDoc { get; set; }
        public string FilenameEnglish { get; set; }
    }
}