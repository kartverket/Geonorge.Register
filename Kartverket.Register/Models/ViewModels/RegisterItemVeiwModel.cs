using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Kartverket.Register.Models.ViewModels
{
    public class RegisterItemVeiwModel
    {
        public virtual RegisterItem currentVersion { get; set; }
        public virtual List<RegisterItem> historical { get; set; }
        public virtual List<RegisterItem> suggestions { get; set; }

        public RegisterItemVeiwModel(VersionsItem versionsItem)
        {
            currentVersion = versionsItem.currentVersion;
            historical = versionsItem.historical;
            suggestions = versionsItem.suggestions;            
        }

    }
}