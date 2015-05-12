using Kartverket.Register.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Kartverket.Register.Services.Versioning
{
    public class RegisterService : IRegisterService
    {
        private readonly RegisterDbContext _dbContext;

        public RegisterService(RegisterDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public FilterItems Filter(Kartverket.Register.Models.Register register, FilterParameters filter)
        {
            string filterHorisontalt = filter.filterHorisontalt;
            string filterVertikalt = filter.filterVertikalt;
            string filterInspire = filter.InspireRequirement;
            string filterNational = filter.nationalRequirement;
            string filterNationalSea = filter.nationalSeaRequirement;

            List<RegisterItem> filterEpsg = new List<RegisterItem>();
            foreach (EPSG item in register.items)
            {
                if (filterHorisontalt == null)
                {
                    filterHorisontalt = item.horizontalReferenceSystem;
                }
                if (filterVertikalt == null)
                {
                    filterVertikalt = item.verticalReferenceSystem;
                }
                if (filterInspire == null)
                {
                    filterInspire  = item.inspireRequirement.value;
                }
                if (filterNational == null)
                {
                    filterNational = item.nationalRequirement.value;
                }
                if (filterNationalSea  == null)
                {
                    filterNationalSea = item.nationalRequirement.value;
                }

                var queryResult = from e in _dbContext.EPSGs
                                  where e.horizontalReferenceSystem == filterHorisontalt
                                  && e.verticalReferenceSystem == filterVertikalt
                                  && e.inspireRequirement.value == filterInspire
                                  && e.nationalRequirement.value == filterNational
                                  && e.nationalSeasRequirement.value == filterNationalSea
                                  && e.systemId == item.systemId
                                  select e;
                
                if (queryResult.Count() > 0)
                {
                    Kartverket.Register.Models.EPSG epsgkode = queryResult.First();
                    filterEpsg.Add(epsgkode);
                }
                filterHorisontalt = filter.filterHorisontalt;
                filterVertikalt = filter.filterVertikalt;
                filterInspire = filter.InspireRequirement;
                filterNational = filter.nationalRequirement;
                filterNationalSea = filter.nationalSeaRequirement;
            }

            return new FilterItems
            {
                register = register,
                registerItems = filterEpsg,
                documentItems = null,
                datasetItems = null,
                organizationItems = null,
                codeItems = null
            };
        }
    }
}