using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Configuration;
using ExpressiveAnnotations.Attributes;
using Kartverket.Register.Models.Translations;
using Resources;

namespace Kartverket.Register.Models.Api
{
    public class Organization
    {
        public string Number { get; set; }

        public string Name { get; set; }

        public string LogoUrl { get; set; }

        public string LogoLargeUrl { get; set; }

        public string ShortName { get; set; }

        public string Status { get; set; }

        public Contact ContactInformation { get; set; }

        public bool? Member { get; set; }


        public void Convert(Models.Organization input)
        {
            Number = input.number;
            LogoUrl = WebConfigurationManager.AppSettings["RegistryUrl"] + "data/" + Models.Organization.DataDirectory + input.logoFilename;
            LogoLargeUrl = WebConfigurationManager.AppSettings["RegistryUrl"] + "data/" + Models.Organization.DataDirectory + input.largeLogo;
            ShortName = input.shortname;
            Name = input.name;
            Status = input.statusId;
            ContactInformation = new Contact(input.contact, input.epost);
            Member = input.member.HasValue && input.member.Value == true ? true : false;
        }
    }

    public class Contact
    {
        public string Name { get; set; }
        public string Epost { get; set; }

        public Contact(string contact, string epost)
        {
            Name = contact;
            Epost = epost;
        }

        public Contact()
        {
        }
    }
}