using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Kartverket.Register.Resources;

namespace Kartverket.Register.Models
{
    public class GeoDataCollection
    {
        [Key]
        public Guid systemId { get; set; }
        [Display(Name = "Title", ResourceType = typeof(GeodataCollection))]
        public string Title { get; set; }
        [Display(Name = "Link", ResourceType = typeof(GeodataCollection))]
        public string Link { get; set; }
        [Display(Name = "Purpose", ResourceType = typeof(GeodataCollection))]
        public string Purpose { get; set; }
        [Display(Name = "Organization", ResourceType = typeof(GeodataCollection))]
        public Organization Organization { get; set; }
    }
}