using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kartverket.Register.Models
{
    public class Station
    {
        [Key]
        [Column(Order = 1)]
        public string StationName { get; set; }
        [Key]
        [Column(Order = 2)]
        public string StationType { get; set; }
        public string Description { get; set; }

    }
}