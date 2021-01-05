using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Modularity;

namespace AbpUserVerificationByEmail.EntityFrameworkCore
{
    [DependsOn(
        typeof(AbpUserVerificationByEmailEntityFrameworkCoreModule)
        )]
    public class AbpUserVerificationByEmailEntityFrameworkCoreDbMigrationsModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            context.Services.AddAbpDbContext<AbpUserVerificationByEmailMigrationsDbContext>();
        }
    }
}
