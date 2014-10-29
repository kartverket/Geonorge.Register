using System.Data.Entity;

namespace Kartverket.Register.Models
{
    public class RegisterDbContext : DbContext
    {
        public RegisterDbContext() : base("RegisterDbContext") { }

        public DbSet<Organization> Organizations { get; set; }
    }
}