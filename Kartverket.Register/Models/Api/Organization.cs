using System.Web.Configuration;

namespace Kartverket.Register.Models.Api
{
    public class Organization
    {
        public string Number { get; set; }

        public string Name { get; set; }

        public string LogoUrl { get; set; }

        public string LogoLargeUrl { get; set; }

        public string ShortName { get; set; }

        public void Convert(Models.Organization input)
        {
            Number = input.number;
            LogoUrl = WebConfigurationManager.AppSettings["RegistryUrl"] + "data/" + Models.Organization.DataDirectory + input.logoFilename;
            LogoLargeUrl = WebConfigurationManager.AppSettings["RegistryUrl"] + "data/" + Models.Organization.DataDirectory + input.largeLogo;
            ShortName = input.shortname;
            Name = input.name;
        }
    }
}