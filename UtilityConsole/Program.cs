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

            Dictionary<string, Municipality> municipalitiesByCode = service.GetMunicipalities();
            Console.WriteLine(municipalitiesByCode.Keys.Count + " municipalities");

            string fileName = "c:\\dev\\MunicipalityCodeToOrganizationNumber.cs";
            var fs = new FileStream(fileName, FileMode.CreateNew);
            using (StreamWriter file = new StreamWriter(fs))
            {
                file.WriteLine("Dictionary<string, string> municipalityFromCodeToOrganizationNumber = new Dictionary<string, string>() {");

                foreach (string key in municipalitiesByCode.Keys)
                {
                    file.WriteLine("{\"" + key + "\",\"" + municipalitiesByCode[key].OrganizationNumber + "\"},");
                }

                file.WriteLine("};");
            }
        }
    }
}
