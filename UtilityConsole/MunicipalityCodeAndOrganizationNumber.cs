using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace UtilityConsole
{
    public class MunicipalityCodeAndOrganizationNumber
    {
        private readonly List<Municipality> _allMunicipalities = new List<Municipality>(); 

        public void FetchData(string url)
        {
            var rawJson = DownloadData(url);

            JObject jsonData = JObject.Parse(rawJson);

            var municipalities = from m in jsonData["data"]
                                    select new Municipality((string)m["navn"], (string)m["forretningsadresse"]["kommunenummer"], (string)m["organisasjonsnummer"]);

            _allMunicipalities.AddRange(municipalities);

            foreach (Municipality municipality in municipalities)
            {
                Console.WriteLine(municipality.Name + " [" + municipality.Code + "] - " + municipality.OrganizationNumber);
            }

            var links = from link in jsonData["links"] select new {rel = (string) link["rel"], href = (string) link["href"]};

            foreach (var link in links)
            {
                Console.WriteLine(link.rel + ": " + link.href);
                if (link.rel.Equals("next"))
                {
                    Console.WriteLine("-- Going to next page");
                    FetchData(link.href);
                }
            }

            
        }

        public List<Municipality> GetMunicipalities()
        {
            return _allMunicipalities;
        }


        private static string DownloadData(string url)
        {
            var httpClient = new HttpClient();
            Task<string> downloadTask = httpClient.GetStringAsync(url);
            string rawJson = downloadTask.Result;
            return rawJson;
        }
    }


    public class Municipality
    {
        public Municipality(string name, string code, string organizationNumber)
        {
            Name = name;
            Code = code;
            OrganizationNumber = organizationNumber;
        }

        public string Name { get; set; }
        public string Code { get; set; }
        public string OrganizationNumber { get; set; }
    }
}
