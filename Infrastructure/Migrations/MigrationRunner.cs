using FluentMigrator.Runner;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Migrations
{
    public static class MigrationRunner
    {
        public static IHost RunMigrations(this IHost host)
        {
            var configuration = host.Services.GetRequiredService<IConfiguration>();
            var connectionString = configuration.GetConnectionString("Postgres")
                ?? throw new ArgumentNullException("Postgres connection string not found");

            RunMigrations(connectionString);

            return host;
        }

        public static void RunMigrations(string connectionString)
        {
            var serviceContext = CreateService(connectionString);
            using var scope = serviceContext.CreateScope();
            var runner = scope.ServiceProvider.GetRequiredService<IMigrationRunner>();
            runner.MigrateUp();
        }

        private static IServiceProvider CreateService(string connectionString)
        {
            return new ServiceCollection()
                .AddFluentMigratorCore()
                .ConfigureRunner(builder => builder
                    .AddPostgres()
                    .WithGlobalConnectionString(connectionString)
                    .ScanIn(typeof(MigrationRunner).Assembly).For.Migrations()
                    .ConfigureGlobalProcessorOptions(op => op.ProviderSwitches = "Force Quote=false"))
                .AddLogging(log => log.AddFluentMigratorConsole())
                .BuildServiceProvider(false);
        }
    }
}
