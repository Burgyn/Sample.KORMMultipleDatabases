using Sample.KormMultipleDatabases.Infrastructure;
using Kros.Extensions;
using Kros.KORM;
using Kros.KORM.Extensions.Asp;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Concurrent;
using System.Reflection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System.Collections.Generic;

namespace Sample.KormMultipleDatabases.KORM.Extensions
{
    public class TenantDatabaseFactory : ITenantDatabaseFactory
    {
        private static readonly ConcurrentDictionary<string, Builder> _builders = new ConcurrentDictionary<string, Builder>();
        private readonly KormConnectionSettings _connectionSettings;
        private readonly IServiceCollection _services;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public TenantDatabaseFactory(
            KormConnectionSettings connectionSettings,
            IServiceCollection services,
            IHttpContextAccessor httpContextAccessor)
        {
            _connectionSettings = connectionSettings;
            _services = services;
            _httpContextAccessor = httpContextAccessor;
        }

        public IDatabase GetDatabase()
        {
            string name = GetTenantName();
            Builder builder = _builders.GetOrAdd(name, _ => new Builder(_connectionSettings, _services).Migrate(name));

            return builder.Build(name);
        }

        private string GetTenantName()
        {
            var route = _httpContextAccessor.HttpContext.GetRouteData();

            return route.Values.GetValueOrDefault("tenant").ToString();
        }

        private class Builder
        {
            private readonly KormConnectionSettings _connectionSettings;
            private readonly IServiceCollection _services;

            public Builder(KormConnectionSettings connectionSettings, IServiceCollection services)
            {
                _connectionSettings = connectionSettings;
                _services = services;
            }

            public Builder Migrate(string name)
            {
                new KormBuilder(_services, new KormConnectionSettings()
                {
                    ConnectionString = _connectionSettings.GetConnectionString(name),
                    AutoMigrate = true
                }).AddKormMigrations(o =>
                {
                    o.AddAssemblyScriptsProvider(Assembly.GetEntryAssembly(), "Sample.KormMultipleDatabases.SqlScripts.TenantDb");
                })
                .Migrate();

                return this;
            }

            public IDatabase Build(string name)
                => Database.Builder
                .UseDatabaseConfiguration<TenantDbConfiguration>()
                .UseConnection(_connectionSettings.GetConnectionString(name))
                .Build();
        }
    }

    public static class KormConnectionSettingsExtension
    {
        public static string GetConnectionString(this KormConnectionSettings value, string name)
            => value.ConnectionString.Format(name);
    }
}
