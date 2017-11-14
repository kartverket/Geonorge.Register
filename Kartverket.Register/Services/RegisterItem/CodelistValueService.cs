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
            //name, value, description
            //name, value, description, nameTranslated, descriptionTranslated
            var codelistValue = new CodelistValue();
            switch (codelistValueImport.Length)
            {
                case 3:
                    break;
                case 5:
                    codelistValue.Translations.Add(Translation(codelistValueImport, codelistValue));
                    break;
                default:
                    return null;
            }

            codelistValue.systemId = Guid.NewGuid();

            codelistValue.name = codelistValueImport[0];
            codelistValue.value = codelistValueImport[1];
            codelistValue.description = codelistValueImport[2];
            codelistValue.registerId = register.systemId;
            codelistValue.register = register;

            if (string.IsNullOrWhiteSpace(codelistValue.value)) return null;
            codelistValue.submitterId = _userService.GetUserOrganizationId();
            codelistValue.modified = DateTime.Now;
            codelistValue.dateSubmitted = DateTime.Now;
            codelistValue.statusId = "Submitted";
            codelistValue.seoname = RegisterUrls.MakeSeoFriendlyString(codelistValue.name);
            return codelistValue;
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