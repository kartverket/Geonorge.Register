using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Kartverket.Register.Models
{
    public class FilterItems
    {
        public virtual Register register { get; set; }
        public virtual List<RegisterItem> registerItems { get; set; }
        public virtual List<EPSG> epsgItems { get; set; }
        public virtual List<Document> documentItems { get; set; }
        public virtual List<Dataset> datasetItems { get; set; }
        public virtual List<Organization> organizationItems { get; set; }
        public virtual List<CodelistValue> codeItems { get; set; }

        public FilterItems()
        {

        }

        public FilterItems(FilterItems otherResult)
        {
            register = otherResult.register;
            registerItems = otherResult.registerItems;
            epsgItems = otherResult.epsgItems;
            documentItems = otherResult.documentItems;
            datasetItems = otherResult.datasetItems;
            organizationItems = otherResult.organizationItems;
            codeItems = otherResult.codeItems;
        }    
    }
}