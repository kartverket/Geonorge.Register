﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Resources;

namespace Kartverket.Register.Models.Translations
{
    public abstract class Translation<T> where T : Translation<T>, new()
    {

        public Guid Id { get; set; }
        [Display(Name = "Navn_Engelsk", ResourceType = typeof(Shared))]
        public string Name { get; set; }
        [Display(Name = "Beskrivelse_Engelsk", ResourceType = typeof(Shared))]
        public string Description { get; set; }
        public string CultureName { get; set; }

        protected Translation()
        {
            Id = Guid.NewGuid();
        }
    }
}