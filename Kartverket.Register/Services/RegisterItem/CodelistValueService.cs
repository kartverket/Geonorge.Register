using System;
using System.Collections.Generic;
using Kartverket.Register.Models;
using System.IO;
using System.Linq;
using System.Web;
using Kartverket.Register.Helpers;
using Kartverket.Register.Models.Translations;
using Kartverket.Register.Services.Register;

namespace Kartverket.Register.Services.RegisterItem
{
    public class CodelistValueService : ICodelistValueService
    {
        private readonly RegisterDbContext _dbContext;
        private readonly IUserService _userService;

        public CodelistValueService(RegisterDbContext dbContext)
        {
            _dbContext = dbContext;
            _userService = new UserService(_dbContext);
        }

        public CodelistValue NewCodelistValueFromImport(Models.Register register, string[] codelistValueImport)
        {
            //name, description, value
            //name, description, value, nameTranslated, descriptionTranslated
            var codelistValue = new CodelistValue();
            switch (codelistValueImport.Length)
            {
                case 7:
                    break;
                case 10:
                    codelistValue.Translations.Add(Translation(codelistValueImport, codelistValue));
                    break;
                //case 10:
                //    codelistValue.systemId = Guid.NewGuid();

                //    codelistValue.name = codelistValueImport[0];
                //    codelistValue.value = codelistValueImport[2];
                //    codelistValue.description = codelistValueImport[6];
                //    codelistValue.registerId = register.systemId;
                //    codelistValue.register = register;

                //    codelistValue.submitterId = _userService.GetUserOrganizationId();
                //    codelistValue.modified = DateTime.Now;
                //    codelistValue.dateSubmitted = DateTime.Now;
                //    codelistValue.statusId = "Submitted";
                //    codelistValue.seoname = RegisterUrls.MakeSeoFriendlyString(codelistValue.name, register.TransliterNorwegian);
                //    return codelistValue;
                default:
                    return null;
            }

            codelistValue.systemId = Guid.NewGuid();

            codelistValue.name = codelistValueImport[0];
            codelistValue.value = codelistValueImport[2];
            if(codelistValueImport.Length == 10)
                codelistValue.valueEnglish = codelistValueImport[5];
            codelistValue.description = codelistValueImport[1];
            codelistValue.registerId = register.systemId;
            codelistValue.register = register;

            codelistValue.submitterId = _userService.GetUserOrganizationId();
            codelistValue.modified = DateTime.Now;
            codelistValue.dateSubmitted = DateTime.Now;
            codelistValue.statusId = "Submitted";

            if(codelistValueImport.Length == 7) 
            { 
                codelistValue.statusId = GetStatus(codelistValueImport[3]);
                codelistValue.ValidFromDate = GetDate(codelistValueImport[4]);
                codelistValue.ValidToDate = GetDate(codelistValueImport[5]);
                codelistValue.externalId = codelistValueImport[6];
            }
            else if (codelistValueImport.Length == 10)
            {
                 codelistValue.statusId = GetStatus(codelistValueImport[6]);
                 codelistValue.ValidFromDate = GetDate(codelistValueImport[7]);
                 codelistValue.ValidToDate = GetDate(codelistValueImport[8]);
                 codelistValue.externalId = codelistValueImport[9];
            }

            codelistValue.seoname = RegisterUrls.MakeSeoFriendlyString(codelistValue.name, register.TransliterNorwegian);
            return codelistValue;
        }

        private DateTime? GetDate(string date)
        {
            if (!string.IsNullOrEmpty(date))
            {
                date = date.Replace(" 00.00.00", "");
                try {
                   DateTime? dateTime = DateTime.Parse(date);
                    return dateTime;
                }
                catch (Exception ex) { }
            }

            return null;
        }

        private string GetStatus(string status)
        {
            if (!string.IsNullOrEmpty(status)) {
                if (status.ToLower() == "gyldig")
                    return "Valid";
                else if (status.ToLower() == "tilbaketrukket")
                    return "Retired";
                else if (status.ToLower() == "utkast")
                    return "Draft";
            }
            return "Submitted";
        }

        private static CodelistValueTranslation Translation(string[] codeListValue, CodelistValue codelistValue)
        {
            return new CodelistValueTranslation
            {
                CultureName = "en",
                Name = codeListValue[3],
                Description = codeListValue[4],
                RegisterItemId = codelistValue.systemId
            };
           
        }
    }
}