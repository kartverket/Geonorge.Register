using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace UtilityConsole
{
    public class Program
    {
        public static void Main(string[] args)
        {

            string url = "http://data.brreg.no/enhetsregisteret/enhet.json?page=0&size=100&$filter=organisasjonsform%20eq%20%27KOMM%27";

            var service = new MunicipalityCodeAndOrganizationNumber();
            service.FetchData(url);

            List<Municipality> municipalities = service.GetMunicipalities();
            Console.WriteLine(municipalities.Count + " municipalities");

            CreateFileWithDictionaryOfMunicipalitiesCodeToOrganizationNumber(municipalities);
            CreateFileWithDictionaryOfMunicipalitiesOrganizationNumberToCode(municipalities);
        }

        private static void CreateFileWithDictionaryOfMunicipalitiesOrganizationNumberToCode(List<Municipality> municipalities)
        {
            string fileName = "c:\\dev\\MunicipalityOrganizationNumberToCode.cs";
            var fs = new FileStream(fileName, FileMode.Create);
            using (StreamWriter file = new StreamWriter(fs))
            {
                file.WriteLine("Dictionary<string, string> municipalityFromOrganizationNumberToCode = new Dictionary<string, string>() {");

                foreach (Municipality municipality in municipalities)
                {
                    file.WriteLine("{\"" + municipality.OrganizationNumber + "\",\"" + municipality.Code + "\"},");
                }

                file.WriteLine("};");
            }
        }
        
        private static void CreateFileWithDictionaryOfMunicipalitiesCodeToOrganizationNumber(List<Municipality> municipalities)
        {
            string fileName = "c:\\dev\\MunicipalityCodeToOrganizationNumber.cs";
            var fs = new FileStream(fileName, FileMode.Create);
            using (StreamWriter file = new StreamWriter(fs))
            {
                file.WriteLine("Dictionary<string, string> municipalityFromCodeToOrganizationNumber = new Dictionary<string, string>() {");

                foreach (Municipality municipality in municipalities)
                {
                    file.WriteLine("{\"" + municipality.Code + "\",\"" + municipality.OrganizationNumber + "\"},");
                }

                file.WriteLine("};");
            }
        }
    }
}
