using Kartverket.Register.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Kartverket.Register.Services.RegisterItem
{
    public class RegisterItemService : IRegisterItemService
    {
        private readonly RegisterDbContext _dbContext;

        public RegisterItemService(RegisterDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void SetNarrowerItems(List<Guid> narrowerList, CodelistValue codelistValue)
        {
            foreach (Guid narrowerId in narrowerList)
            {
                CodelistValue narrowerItem = _dbContext.CodelistValues.Find(narrowerId);
                codelistValue.narrowerItems.Add(narrowerItem);
                narrowerItem.broaderItemId = codelistValue.systemId;
                narrowerItem.modified = DateTime.Now;
                //_dbContext.Entry(narrowerItem).State = EntityState.Modified;
            }                       
            //_dbContext.SaveChanges();
        }

        public void SetBroaderItem(Guid broader, CodelistValue codelistValue)
        {
            codelistValue.broaderItemId = broader;
            _dbContext.SaveChanges();
            CodelistValue broaderItem = (CodelistValue)getItemById(broader);
            broaderItem.narrowerItems.Add(codelistValue);
        }

        private Kartverket.Register.Models.RegisterItem getItemById(Guid id)
        {
            var queryresult = from ri in _dbContext.RegisterItems
                              where ri.systemId == id
                              select ri;
            
            Kartverket.Register.Models.RegisterItem item = queryresult.FirstOrDefault();
            return item;            
        }
    }
}