using Kartverket.Register.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Kartverket.Register.Services
{
    public interface IndexDocumentCreator
    {
        List<RegisterIndexDoc> CreateIndexDocs(IEnumerable<object> searchResultItems);
        RegisterIndexDoc CreateIndexDoc(SearchResultItem register);
    }
}