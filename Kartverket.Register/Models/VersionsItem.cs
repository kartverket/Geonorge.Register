using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Kartverket.Register.Models
{
    public class VersionsItem
    {
        public virtual RegisterItem currentVersion { get; set; }
        public virtual List<Kartverket.Register.Models.RegisterItem> historical { get; set; }
        public virtual List<Kartverket.Register.Models.RegisterItem> suggestions { get; set; }

        public VersionsItem()
        {

        }

        public VersionsItem(VersionsItem otherResult)
        {
            currentVersion = otherResult.currentVersion;
            historical = otherResult.historical;
            suggestions = otherResult.suggestions;
        }    
    }
}