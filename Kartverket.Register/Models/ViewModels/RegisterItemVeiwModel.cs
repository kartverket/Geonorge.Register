using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Kartverket.Register.Models.ViewModels
{
    public class RegisterItemVeiwModel
    {
        public virtual RegisterItem currentVersion { get; set; }
        public virtual IEnumerable<RegisterItem> historical { get; set; }
        public virtual IEnumerable<RegisterItem> suggestions { get; set; }

        public RegisterItemVeiwModel(VersionsItem versionsItem)
        {
            currentVersion = versionsItem.currentVersion;
            historical = versionsItem.historical.OrderByDescending(s => s.dateSubmitted);
            suggestions = versionsItem.suggestions.OrderByDescending(s => s.dateSubmitted);            
        }
    }
}