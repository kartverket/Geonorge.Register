using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Kartverket.Register.Models.Api
{
    public class OrganizationV2
    {
        public string Name { get; set; }
        public string OrganizationNumber { get; set; }
        public string LogoFilename { get; set; }
        public string LogoLargeFilename { get; set; }
        public string ContactPerson { get; set; }
        public string Email { get; set; }
        public bool? NorgeDigitaltMember { get; set; }
        public int? AgreementYear { get; set; }
        public string AgreementDocumentUrl { get; set; }
        public string PriceFormDocument { get; set; }
        public string ShortName { get; set; }
        public string OrganizationType { get; set; }
        public string MunicipalityCode { get; set; }
        public string GeographicCenterX { get; set; }
        public string GeographicCenterY { get; set; }
        public string BoundingBoxNorth { get; set; }
        public string BoundingBoxSouth { get; set; }
        public string BoundingBoxEast { get; set; }
        public string BoundingBoxWest { get; set; }
        public string Status { get; set; }

        public static OrganizationV2 Convert(Models.Organization item)
        {
            return new OrganizationV2
            {
                Name = item.name,
                OrganizationNumber = item.number,
                LogoFilename = item.logoFilename,
                LogoLargeFilename = item.largeLogo,
                ContactPerson = item.contact,
                Email = item.epost,
                NorgeDigitaltMember = item.member,
                AgreementYear = item.agreementYear,
                AgreementDocumentUrl = item.agreementDocumentUrl,
                PriceFormDocument = item.priceFormDocument,
                ShortName = item.shortname,
                OrganizationType = item.OrganizationType ?? "regular",
                MunicipalityCode = item.MunicipalityCode,
                GeographicCenterX = item.GeographicCenterX,
                GeographicCenterY = item.GeographicCenterY,
                BoundingBoxNorth = item.BoundingBoxNorth,
                BoundingBoxSouth = item.BoundingBoxSouth,
                BoundingBoxEast = item.BoundingBoxEast,
                BoundingBoxWest = item.BoundingBoxWest,
                Status = item.statusId
            };
    }

        public static List<OrganizationV2> Convert(List<Models.Organization> items)
        {
            var models = new List<OrganizationV2>();
            foreach(var item in items)
            {
                models.Add(Convert(item));
            }

            return models;
        }
    }
}