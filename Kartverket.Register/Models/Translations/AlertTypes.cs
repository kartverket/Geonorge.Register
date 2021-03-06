﻿using Kartverket.Register.Helpers;
using Kartverket.Register.Services.Register;
using Kartverket.Register.Services.RegisterItem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Kartverket.Register.Models.Translations
{
    public class AlertTypes
    {
        public Dictionary<Alert, List<Translated>> Items;
        private IRegisterService _registerService;

        public AlertTypes(IRegisterService registerService, string category)
        {
            _registerService = registerService;
            CreateAlertTypes(category);
        }

        public KeyValuePair<Alert, List<Translated>> GetAlertType(string type)
        {
            var alertType = Items.Where(k => k.Key.Value.Equals(type)).FirstOrDefault();
            return alertType;
        }

        public Dictionary<string, string> GetAlertTypes()
        {
            Dictionary<string, string> alertTypes = new Dictionary<string, string>();
            foreach(var item in Items)
            {
                if (CultureHelper.IsNorwegian())
                    alertTypes.Add(item.Key.Value, item.Key.Value);
                else
                {
                    var translated = item.Value.Where(c => c.Culture == CultureHelper.GetCurrentCulture()).FirstOrDefault();
                    if (translated != null )
                        alertTypes.Add(item.Key.Value, translated.AlertType);
                    else
                        alertTypes.Add(item.Key.Value, item.Key.Value);
                }
            }

            return alertTypes;
        }

        public void CreateAlertTypes(string category)
        {
            Items = new Dictionary<Alert, List<Translated>>();

            Register alertTypes;

            if (category == Constants.AlertCategoryDataset)
                alertTypes = _registerService.GetRegister("Metadata kodelister", "Datasettvarsel");
            else if (category == Constants.AlertCategoryOperation)
                alertTypes = _registerService.GetRegister("Metadata kodelister", "Driftsmeldinger");
            else
                alertTypes = _registerService.GetRegister("Metadata kodelister", "Tjenestevarsel");

            foreach (var alertType in alertTypes.items.Cast<CodelistValue>())
            {
                if(alertType.value != null)
                {
                    var translationEnglish = alertType.Translations.Where(t => t.CultureName == Culture.EnglishCode).FirstOrDefault();
                    string nameTranslated = translationEnglish != null ? translationEnglish.Name : alertType.name;
                    Translated translation = new Translated { AlertType = nameTranslated, Culture = Culture.EnglishCode };
                    List<Translated> translations = new List<Translated>();
                    translations.Add(translation);
                    Items.Add( new Alert { Key = alertType.value, Value = alertType.name }, translations) ;
                }
            }


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