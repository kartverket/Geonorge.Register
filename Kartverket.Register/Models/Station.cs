using System.ComponentModel.DataAnnotations;

namespace Kartverket.Register.Models
{
    public class Station
    {
        [Key]
        public string Name { get; set; }
        public string Type { get; set; }
    }
}