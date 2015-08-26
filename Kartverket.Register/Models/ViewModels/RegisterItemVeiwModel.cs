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
            historical = versionsItem.historical;
            suggestions = versionsItem.suggestions;            
        }

        public RegisterItemVeiwModel(VersionsItem versionsItem, string sorting)
        {
            currentVersion = versionsItem.currentVersion;
            historical = versionsItem.historical;
            suggestions = versionsItem.suggestions;


           switch (sorting)
           {
              case "Date":
                 historical = historical.OrderBy(s => s.dateSubmitted);
                 break;
              case "date_desc":
                 historical = historical.OrderByDescending(s => s.dateSubmitted);
                 break;
              default:
                 historical = historical.OrderBy(s => s.versionNumber);
                 break;
           }
        }

    }
}