using Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Kartverket.Register.Models.Translations
{
    public class DatasetTranslation : Translation<DatasetTranslation>
    {
        public Guid RegisterItemId { get; set; }
        [Display(Name = "DOK_Delivery_Theme", ResourceType = typeof(DataSet))]
        public string ThemeGroupId { get; set; }

        public DatasetTranslation()
        {
            Id = Guid.NewGuid();
        }
    }
}
