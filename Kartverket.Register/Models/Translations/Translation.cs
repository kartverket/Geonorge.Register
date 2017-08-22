using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Kartverket.Register.Models.Translations
{
    public abstract class Translation<T> where T : Translation<T>, new()
    {

        public Guid Id { get; set; }

        public string CultureName { get; set; }

        protected Translation()
        {
            Id = Guid.NewGuid();
        }
    }
}