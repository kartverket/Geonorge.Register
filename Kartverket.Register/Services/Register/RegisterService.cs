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

        public Kartverket.Register.Models.Register Filter(Kartverket.Register.Models.Register register, FilterParameters filter)
        {
            List<RegisterItem> registerItems = new List<RegisterItem>();

            if (register.containedItemClass == "EPSG")
            {   
                FilterEPSGkode(register, filter, registerItems);
            }
            else if (register.containedItemClass == "Document")
            {
                foreach (Document item in register.items)
                {
                    registerItems.Add(item);
                }
            }
            else if (register.containedItemClass == "Dataset")
            {
                foreach (Dataset item in register.items)
                {
                    registerItems.Add(item);
                }
            }
            else if (register.containedItemClass == "CodelistValue")
            {
                foreach (CodelistValue item in register.items)
                {
                    registerItems.Add(item);
                }
            }
            else if (register.containedItemClass == "Organization")
            {
                foreach (Organization item in register.items)
                {
                    registerItems.Add(item);
                }
            }



            return new Kartverket.Register.Models.Register
            {
                systemId = register.systemId,
                name = register.name,
                containedItemClass = register.containedItemClass,
                dateAccepted = register.dateAccepted,
                dateSubmitted = register.dateSubmitted,
                description = register.description,
                items = registerItems,
                manager = register.manager,
                managerId = register.managerId,
                modified = register.modified,
                owner = register.owner,
                ownerId = register.ownerId,
                parentRegister = register.parentRegister,
                parentRegisterId = register.parentRegisterId,
                seoname = register.seoname,
                status = register.status,
                statusId = register.statusId,
                subregisters = register.subregisters,
                replaces = register.replaces,
                targetNamespace = register.targetNamespace,
                versioning = register.versioning,
                versioningId = register.versioningId,
                versionNumber = register.versionNumber
            };
        }

        private void FilterEPSGkode(Kartverket.Register.Models.Register register, FilterParameters filter, List<RegisterItem> filterRegisterItems)
        {            
            string filterHorisontalt = filter.filterHorisontalt;
            string filterVertikalt = filter.filterVertikalt;
            string filterInspire = filter.InspireRequirement;
            string filterNational = filter.nationalRequirement;
            string filterNationalSea = filter.nationalSeaRequirement;
            
            foreach (EPSG item in register.items)
            {               
                if (filterHorisontalt == null)
                {
                    filterHorisontalt = item.horizontalReferenceSystem;
                }
                else
                {
                    if (!string.IsNullOrEmpty(item.horizontalReferenceSystem))
                    {
                        filterHorisontalt = item.horizontalReferenceSystem;
                    }
                    else
                    {
                        filterHorisontalt = "ikke angitt horisontalt referansesystem";
                    }
                }
                if (filterVertikalt == null)
                {
                    filterVertikalt = item.verticalReferenceSystem;
                }
                else
                {
                    if (!string.IsNullOrEmpty(item.verticalReferenceSystem))
                    {
                        filterVertikalt = item.verticalReferenceSystem;
                    }
                    else
                    {
                        filterVertikalt = "ikke angitt vertikalt referansesystem";
                    }
                }
                if (filterInspire == null)
                {
                    filterInspire = item.inspireRequirement.value;
                }
                if (filterNational == null)
                {
                    filterNational = item.nationalRequirement.value;
                }
                if (filterNationalSea == null)
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
                    filterRegisterItems.Add(epsgkode);
                }
                filterHorisontalt = filter.filterHorisontalt;
                filterVertikalt = filter.filterVertikalt;
                filterInspire = filter.InspireRequirement;
                filterNational = filter.nationalRequirement;
                filterNationalSea = filter.nationalSeaRequirement;
            }
        }
    }
}