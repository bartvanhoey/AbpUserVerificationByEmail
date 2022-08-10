using Volo.Abp.Modularity;

namespace AbpUserVerificationByEmail;

[DependsOn(
    typeof(AbpUserVerificationByEmailApplicationModule),
    typeof(AbpUserVerificationByEmailDomainTestModule)
    )]
public class AbpUserVerificationByEmailApplicationTestModule : AbpModule
{

}
