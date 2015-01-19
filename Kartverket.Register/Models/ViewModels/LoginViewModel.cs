using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Kartverket.Register.Models
{
    public class LoginViewModel
    {
        [Required]
        [Display(Name = "Brukernavn")]
        public string Username { get; set; }

        [Required]
        [Display(Name = "Passord")]
        public string Password { get; set; }
    }
}