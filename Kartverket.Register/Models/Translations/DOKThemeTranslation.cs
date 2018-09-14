using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Kartverket.Register.Models.Translations
{
    public class DOKThemeTranslation
    {
        public static readonly Dictionary<string, string> NorwegianToEnglish = new Dictionary<string, string>()
        {
            { "Annen", "Other" },
            { "Basis geodata", "Base maps" },
            { "Befolkning", "Population" },
            { "Energi", "Energy" },
            { "Flyfoto", "Aerial photography" },
            { "Forurensning", "Pollution" },
            { "Friluftsliv", "Outdoor activities" },
            { "Geologi", "Geology" },
            { "Høydedata", "Elevation" },
            { "Kulturminner", "Cultural heritage" },
            { "Kyst og fiskeri", "Marine activities" },
            { "Kyst/Fiskeri", "Marine activities" },
            { "Landbruk", "Agriculture" },
            { "Landskap", "Landscape" },
            { "Natur", "Nature" },
            { "Plan", "Planning" },
            { "Samferdsel", "Transportation" },
            { "Samfunnssikkerhet", "Crisis management" },
            { "Vær og klima", "Weather and climate" }
        };
    }
}