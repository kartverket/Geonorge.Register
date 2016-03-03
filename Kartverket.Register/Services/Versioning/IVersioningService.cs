using Kartverket.Register.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kartverket.Register.Services.Versioning
{
    public interface IVersioningService
    {
        VersionsItem Versions(string registername, string parantRegister, string itemname);
        void DeleteVersionGroup(Guid versioningId);
    }
}
