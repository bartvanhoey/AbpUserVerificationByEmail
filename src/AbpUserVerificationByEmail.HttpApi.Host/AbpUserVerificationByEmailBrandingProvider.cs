using Volo.Abp.DependencyInjection;
using Volo.Abp.Ui.Branding;

namespace AbpUserVerificationByEmail;

[Dependency(ReplaceServices = true)]
public class AbpUserVerificationByEmailBrandingProvider : DefaultBrandingProvider
{
    public override string AppName => "AbpUserVerificationByEmail";
}
