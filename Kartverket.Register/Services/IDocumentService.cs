using Kartverket.Register.Models;
using System.Collections.Generic;
using System.Web;

namespace Kartverket.Register.Services
{
    public interface IDocumentService
    {
        Document UpdateDocument(Document originalDocument, Document document, HttpPostedFileBase documentfile, HttpPostedFileBase thumbnail, bool retired, bool sosi);
    }
}
