using System.ComponentModel.DataAnnotations;

namespace Kartverket.Register.Models
{
    public class Department
    {
        [Key]
        public string value { get; set; }
        public string description { get; set; }
    }
}