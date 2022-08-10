using Localization.Resources.AbpUi;
using AbpUserVerificationByEmail.Localization;
using Volo.Abp.Account;
using Volo.Abp.FeatureManagement;
using Volo.Abp.Identity;
using Volo.Abp.Localization;
using Volo.Abp.Modularity;
using Volo.Abp.PermissionManagement.HttpApi;
using Volo.Abp.SettingManagement;
using Volo.Abp.TenantManagement;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace AbpUserVerificationByEmail;

[DependsOn(
    typeof(AbpUserVerificationByEmailApplicationContractsModule),
    typeof(AbpAccountHttpApiModule),
    typeof(AbpIdentityHttpApiModule),
    typeof(AbpPermissionManagementHttpApiModule),
    typeof(AbpTenantManagementHttpApiModule),
    typeof(AbpFeatureManagementHttpApiModule),
    typeof(AbpSettingManagementHttpApiModule)
    )]
public class AbpUserVerificationByEmailHttpApiModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        ConfigureLocalization();
        ConfigureIdentityOptions(context);
    }

    private void ConfigureLocalization()
    {
        Configure<AbpLocalizationOptions>(options =>
        {
            options.Resources
                .Get<AbpUserVerificationByEmailResource>()
                .AddBaseTypes(
                    typeof(AbpUiResource)
                );
        });
    }

    private void ConfigureIdentityOptions(ServiceConfigurationContext context)
{
    context.Services.Configure<IdentityOptions>(options =>
    {
      options.SignIn.RequireConfirmedAccount = true;
      options.SignIn.RequireConfirmedEmail = true;
      options.SignIn.RequireConfirmedPhoneNumber = false;
    });
}
}
