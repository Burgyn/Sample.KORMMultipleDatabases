using Sample.KormMultipleDatabases.Domains;
using Kros.KORM;
using Kros.KORM.Metadata;

namespace Sample.KormMultipleDatabases.Infrastructure
{
    public class MasterDbConfiguration: DatabaseConfigurationBase
    {
        public override void OnModelCreating(ModelConfigurationBuilder modelBuilder)
        {
            modelBuilder
                .Entity<User>()
                .HasTableName("Users")
                .HasPrimaryKey(f => f.Id).AutoIncrement();
        }
    }
}
