using System.Data.Entity;

namespace Kartverket.Register.Models
{
    public class RegisterDbContext : DbContext
    {
        public RegisterDbContext() : base("RegisterDbContext") { }

        // marking DbSet with virtual makes it testable
        public virtual DbSet<Organization> Organizations { get; set; }
    }
}