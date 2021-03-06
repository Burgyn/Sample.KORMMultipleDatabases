﻿using System.Reflection;
using Sample.KormMultipleDatabases.Infrastructure;
using Sample.KormMultipleDatabases.KORM.Extensions;
using Kros.KORM.Extensions.Asp;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Http;

namespace Sample.KormMultipleDatabases
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddHttpContextAccessor();
            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Sample.KormMultipleDatabases", Version = "v1" });
            });

            services.AddKorm(Configuration)
                .UseDatabaseConfiguration<MasterDbConfiguration>()
                .AddKormMigrations(o =>
                {
                    o.AddAssemblyScriptsProvider(Assembly.GetEntryAssembly(), "Sample.KormMultipleDatabases.SqlScripts.MasterDb");
                })
                .Migrate();

            services.AddScoped<ITenantDatabaseFactory>(f => new TenantDatabaseFactory(
                Configuration.GetKormConnectionString("TenantDb"),
                services,
                f.GetRequiredService<IHttpContextAccessor>()));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Sample.KormMultipleDatabases v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
