using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace AbpUserVerificationByEmail.EntityFrameworkCore
{
    /* This class is needed for EF Core console commands
     * (like Add-Migration and Update-Database commands) */
    public class AbpUserVerificationByEmailDbContextFactory : IDesignTimeDbContextFactory<AbpUserVerificationByEmailDbContext>
    {
        public AbpUserVerificationByEmailDbContext CreateDbContext(string[] args)
        {
            AbpUserVerificationByEmailEfCoreEntityExtensionMappings.Configure();

            var configuration = BuildConfiguration();

            var builder = new DbContextOptionsBuilder<AbpUserVerificationByEmailDbContext>()
                .UseSqlServer(configuration.GetConnectionString("Default"));

            return new AbpUserVerificationByEmailDbContext(builder.Options);
        }

        private static IConfigurationRoot BuildConfiguration()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "../AbpUserVerificationByEmail.DbMigrator/"))
                .AddJsonFile("appsettings.json", optional: false);

            return builder.Build();
        }
    }
}
