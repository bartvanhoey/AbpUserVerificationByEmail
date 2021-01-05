using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using AbpUserVerificationByEmail.Data;
using Volo.Abp.DependencyInjection;

namespace AbpUserVerificationByEmail.EntityFrameworkCore
{
    public class EntityFrameworkCoreAbpUserVerificationByEmailDbSchemaMigrator
        : IAbpUserVerificationByEmailDbSchemaMigrator, ITransientDependency
    {
        private readonly IServiceProvider _serviceProvider;

        public EntityFrameworkCoreAbpUserVerificationByEmailDbSchemaMigrator(
            IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task MigrateAsync()
        {
            /* We intentionally resolving the AbpUserVerificationByEmailMigrationsDbContext
             * from IServiceProvider (instead of directly injecting it)
             * to properly get the connection string of the current tenant in the
             * current scope.
             */

            await _serviceProvider
                .GetRequiredService<AbpUserVerificationByEmailMigrationsDbContext>()
                .Database
                .MigrateAsync();
        }
    }
}