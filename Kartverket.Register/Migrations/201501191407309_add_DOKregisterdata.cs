namespace Kartverket.Register.Migrations
{
    using Kartverket.Register.Models;
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class add_DOKregisterdata : DbMigration
    {
        public override void Up()
        {
           // RegisterDbContext db = new RegisterDbContext();
           // Register dokregister = new Register
           // {
           //     systemId = Guid.Parse("CD429E8B-2533-45D8-BCAA-86BC2CBDD0DD"),
           //     dateSubmitted = DateTime.Now,
           //     modified = DateTime.Now,
           //     name = "Det offentlige kartgrunnlaget",
           //     description = "Det offentlige kartgrunnlaget beskrives i plan- og bygningslovens paragraf 2-1 og kart- og planforskriften og skal være er en samling geografiske kvalitetsdata, såkalt offentlige autoritative data. Disse skal være valgt ut og tilrettelagt for å være et egnet kunnskapsgrunnlag for de mest vesentlige behovene som følger av plan- og bygningsloven.",
           //     containedItemClass = "Dataset",
           //     statusId = "Valid",
           //     seoname = "det-offentlige-kartgrunnlaget"
           // };

           // db.Registers.AddOrUpdate(
           //     dokregister
           //);
           // db.SaveChanges();
        }
        
        public override void Down()
        {
        }
    }
}
