using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Kartverket.Register.Models.ViewModels
{
    public class RegisterGrouped
    {
        public List<Group> Items { get; set; }

    }

    public class Group
    {
        public string Name { get; set; }
        public List<RegisterView> Items { get; set; }
    }

    [NotMapped]
    public class RegisterView : Register
    {
        public string ExternalUrl { get; set; }

        public RegisterView() { }
        public RegisterView(Register register)
        {
            this.systemId = register.systemId;
            this.name = register.name;
            this.description = register.description;
            this.Translations = register.Translations;
            this.parentRegisterId = register.parentRegisterId;
            this.seoname = register.seoname;
            if(register.parentRegister != null)
                this.parentRegister.seoname = register.parentRegister.seoname;
            if(register != null && register.owner != null)
                this.owner = new Organization { seoname = register.owner.seoname };

        }

        public override string GetObjectUrl()
        {
            if (!string.IsNullOrEmpty(ExternalUrl))
                return ExternalUrl;
            else
            { 
            return parentRegisterId == null
                ? "/" + seoname
                : "/" + parentRegister.seoname + "/" + seoname;
            }
        }
    }
}