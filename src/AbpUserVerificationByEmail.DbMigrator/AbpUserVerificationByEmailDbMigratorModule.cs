using AbpUserVerificationByEmail.EntityFrameworkCore;
using Volo.Abp.Autofac;
using Volo.Abp.BackgroundJobs;
using Volo.Abp.Modularity;

namespace AbpUserVerificationByEmail.DbMigrator
{
    [DependsOn(
        typeof(AbpAutofacModule),
        typeof(AbpUserVerificationByEmailEntityFrameworkCoreModule),
        typeof(AbpUserVerificationByEmailApplicationContractsModule)
        )]
    public class AbpUserVerificationByEmailDbMigratorModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            Configure<AbpBackgroundJobOptions>(options => options.IsJobExecutionEnabled = false);
        }
    }
}
