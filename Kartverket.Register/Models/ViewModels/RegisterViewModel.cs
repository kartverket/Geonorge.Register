//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Web;

//namespace Kartverket.Register.Models.ViewModels
//{
//    public class RegisterViewModel
//    {
//        public List<RegisterViewData> Items { get; set; }
//        string culture;

//        public RegisterViewModel(List<Register> list, string culture)
//        {
//            this.culture = culture;
//            Items = CreateFromList(list);
//        }

//        private List<RegisterViewData> CreateFromList(List<Register> list)
//        {
//            var items = new List<RegisterViewData>();
//            foreach (var item in list)
//            {
//                var data = new RegisterViewData();
//                data.Description = GetDescriptionLocale(item, culture);
//                data.Name = GetNameLocale(item, culture);
//                data.Owner = item.owner != null ? item.owner.name : "";
//                data.SystemId = item.systemId.ToString();
//                data.ParentRegisterId = item.parentRegisterId.HasValue ? item.parentRegisterId.Value.ToString() : "";
//                data.Seoname = item.seoname;

//                items.Add(data);
//            }

//            return items;
//        }

//        private string GetNameLocale(Models.Register item, string cultureName)
//        {
//            var name = item.Translations[cultureName].Name;
//            if (string.IsNullOrEmpty(name))
//                name = item.Translations[cultureName.Substring(0, 2)].Name;
//            if (string.IsNullOrEmpty(name))
//                name = item.name;

//            return name;
//        }
//        private string GetDescriptionLocale(Models.Register item, string cultureName)
//        {
//            var description = item.Translations[cultureName].Description;
//            if (string.IsNullOrEmpty(description))
//                description = item.Translations[cultureName.Substring(0, 2)].Description;
//            if (string.IsNullOrEmpty(description))
//                description = item.description;

//            return description;
//        }

//        public class RegisterViewData
//        {
//            public string SystemId { get; set; }
//            public string Owner { get; set; }
//            public string Name { get; set; }
//            public string Description { get; set; }
//            public string ParentRegisterId { get; set; }
//            public string Seoname { get; set; }

//        }
//    }
//}
