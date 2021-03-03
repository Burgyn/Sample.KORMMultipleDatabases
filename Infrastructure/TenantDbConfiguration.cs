using Sample.KormMultipleDatabases.Domains;
using Kros.KORM;
using Kros.KORM.Metadata;

namespace Sample.KormMultipleDatabases.Infrastructure
{
    public class TenantDbConfiguration : DatabaseConfigurationBase
    {
        public override void OnModelCreating(ModelConfigurationBuilder modelBuilder)
        {
            modelBuilder
                .Entity<Work>()
                .HasTableName("Works")
                .HasPrimaryKey(f => f.Id).AutoIncrement();
        }
    }
}
