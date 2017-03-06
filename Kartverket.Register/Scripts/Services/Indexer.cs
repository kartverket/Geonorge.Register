using Kartverket.Register.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Kartverket.Register.Services
{
    public interface Indexer
    {
        void Index(IEnumerable<RegisterIndexDoc> docs);
        void Index(RegisterIndexDoc doc);
        void DeleteIndex();
        void RemoveIndexDocument(string systemId);
    }
}