using AbpUserVerificationByEmail.EntityFrameworkCore;
using Volo.Abp.Modularity;

namespace AbpUserVerificationByEmail
{
    [DependsOn(
        typeof(AbpUserVerificationByEmailEntityFrameworkCoreTestModule)
        )]
    public class AbpUserVerificationByEmailDomainTestModule : AbpModule
    {

    }
}