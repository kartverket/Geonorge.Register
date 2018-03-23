using Kartverket.Register.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Kartverket.Register.Models.Translations
{
    public class AlertTypes
    {
        public Dictionary<Alert, List<Translated>> Items;

        public AlertTypes()
        {
            CreateAlertTypes();
        }

        public KeyValuePair<Alert, List<Translated>> GetAlertType(string type)
        {
            var alertType = Items.Where(k => k.Key.Key.Equals(type)).FirstOrDefault();
            return alertType;
        }

        public Dictionary<string, string> GetAlertTypes()
        {
            Dictionary<string, string> alertTypes = new Dictionary<string, string>();
            foreach(var item in Items)
            {
                if (CultureHelper.IsNorwegian())
                    alertTypes.Add(item.Key.Key, item.Key.Value);
                else
                {
                    var translated = item.Value.Where(c => c.Culture == CultureHelper.GetCurrentCulture()).FirstOrDefault();
                    if (translated != null )
                        alertTypes.Add(item.Key.Key, translated.AlertType);
                    else
                        alertTypes.Add(item.Key.Key, item.Key.Value);
                }
            }

            return alertTypes;
        }

        public void CreateAlertTypes()
        {
            Alert ChangeUrl = new Alert { Key = "ChangedUrl", Value ="Endret Url" };
            Translated ChangedUrlTranslated = new Translated { AlertType = "Changed Url", Culture = "en" };
            List<Translated> ChangedUrlTranslatedList = new List<Translated>();
            ChangedUrlTranslatedList.Add(ChangedUrlTranslated);

            Alert ChangeDataQuality = new Alert { Key = "ChangedDataQuality", Value = "Endret datakvalitet" };
            Translated ChangeDataQualityTranslated = new Translated { AlertType = "Changed data quality", Culture = "en" };
            List<Translated> ChangeDataQualityTranslatedList = new List<Translated>();
            ChangeDataQualityTranslatedList.Add(ChangeDataQualityTranslated);

            Alert ChangedDataStructure = new Alert { Key = "ChangedDatastructure", Value = "Endret datastruktur" };
            Translated ChangedDataStructureTranslated = new Translated { AlertType = "Changed data structure", Culture = "en" };
            List<Translated> ChangedDataStructureTranslatedList = new List<Translated>();
            ChangedDataStructureTranslatedList.Add(ChangedDataStructureTranslated);

            Alert NewService = new Alert { Key = "NewService", Value = "Ny tjeneste" };
            Translated NewServiceTranslated = new Translated { AlertType = "New service", Culture = "en" };
            List<Translated> NewServiceTranslatedList = new List<Translated>();
            NewServiceTranslatedList.Add(NewServiceTranslated);

            Alert RemovedService = new Alert { Key = "RemovedService", Value = "Fjernet tjeneste" };
            Translated RemovedServiceTranslated = new Translated { AlertType = "Removed service", Culture = "en" };
            List<Translated> RemovedServiceTranslatedList = new List<Translated>();
            RemovedServiceTranslatedList.Add(RemovedServiceTranslated);

            Alert ChangedDataContent = new Alert { Key = "ChangedDataContent", Value = "Endret datainnhold" };
            Translated ChangedDataContentTranslated = new Translated { AlertType = "Changed data content", Culture = "en" };
            List<Translated> ChangedDataContentTranslatedList = new List<Translated>();
            ChangedDataContentTranslatedList.Add(ChangedDataContentTranslated);

            Alert ChangedCodeList = new Alert { Key = "ChangedCodelist", Value = "Endret kodelister" };
            Translated ChangedCodeListTranslated = new Translated { AlertType = "Changed codelists", Culture = "en" };
            List<Translated> ChangedCodeListTranslatedList = new List<Translated>();
            ChangedCodeListTranslatedList.Add(ChangedCodeListTranslated);

            Items = new Dictionary<Alert, List<Translated>>();

            Items.Add(ChangeUrl, ChangedUrlTranslatedList);
            Items.Add(ChangeDataQuality, ChangeDataQualityTranslatedList);
            Items.Add(ChangedDataStructure, ChangedDataStructureTranslatedList);
            Items.Add(NewService, NewServiceTranslatedList);
            Items.Add(RemovedService, RemovedServiceTranslatedList);
            Items.Add(ChangedDataContent, ChangedDataContentTranslatedList);
            Items.Add(ChangedCodeList, ChangedCodeListTranslatedList);

        }

        public class Alert
        {
            public string Key;
            public string Value;

        }

        public class Translated
        {
            public string AlertType;
            public string Culture;
        }
    }

}