using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Kartverket.Register.Models.ViewModels
{
    public class RegisterVeiwModel
    {
        public virtual Register register { get; set; }
        public virtual List<RegisterItem> registerItems { get; set; }
        //public virtual List<EPSG> epsgItems { get; set; }
        //public virtual List<Document> documentItems { get; set; }
        //public virtual List<Dataset> datasetItems { get; set; }
        //public virtual List<Organization> organizationItems { get; set; }
        //public virtual List<CodelistValue> codeItems { get; set; }


        public RegisterVeiwModel(FilterItems filterRegister)
        {
            register = filterRegister.register;
            registerItems = filterRegister.registerItems;
            //epsgItems = null;
            //documentItems = null;
            //datasetItems = null;
            //organizationItems = null;
            //codeItems = null;

            //if (register.containedItemClass == "EPSG")
            //{
            //    epsgItems = filterRegister.epsgItems;
            //}
            //if (register.containedItemClass == "Document")
            //{
            //    documentItems = filterRegister.documentItems;
            //}
            //if (register.containedItemClass == "Dataset")
            //{
            //    datasetItems = filterRegister.datasetItems;
            //}
            //if (register.containedItemClass == "Organization")
            //{
            //    organizationItems = filterRegister.organizationItems;
            //}
            //if (register.containedItemClass == "CodelistValue")
            //{
            //    codeItems = filterRegister.codeItems;
            //}
        }
        
        public RegisterVeiwModel(Register register)
        {
            this.register = register;
            registerItems = register.items.ToList();
            //epsgItems = null;
            //documentItems = null;
            //datasetItems = null;
            //organizationItems = null;
            //codeItems = null;

            //if (register.containedItemClass == "EPSG")
            //{
            //    List<EPSG> epsgList = new List<EPSG>();
            //    foreach (EPSG item in register.items){
            //        epsgList.Add(item);
            //    }
            //    epsgItems = epsgList;

            //}
            //if (register.containedItemClass == "Document")
            //{
            //    List<Document> docList = new List<Document>();
            //    foreach (Document item in register.items){
            //        docList.Add(item);
            //    }
            //    documentItems = docList;
            //}
            //if (register.containedItemClass == "Dataset")
            //{
            //    List<Dataset> datList = new List<Dataset>();
            //    foreach (Dataset item in register.items)
            //    {
            //        datList.Add(item);
            //    }
            //    datasetItems = datList;
            //}
            //if (register.containedItemClass == "Organization")
            //{
            //    List<Organization> orgList = new List<Organization>();
            //    foreach (Organization item in register.items)
            //    {
            //        orgList.Add(item);
            //    }
            //    organizationItems = orgList;
            //}
            //if (register.containedItemClass == "CodelistValue")
            //{
            //    List<CodelistValue> codeList = new List<CodelistValue>();
            //    foreach (CodelistValue item in register.items)
            //    {
            //        codeList.Add(item);
            //    }
            //    codeItems = codeList;
            //}        
        }

    }
}