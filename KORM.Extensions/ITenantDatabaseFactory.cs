using Kros.KORM;

namespace Sample.KormMultipleDatabases.KORM.Extensions
{
    public interface ITenantDatabaseFactory
    {
        IDatabase GetDatabase();
    }
}
