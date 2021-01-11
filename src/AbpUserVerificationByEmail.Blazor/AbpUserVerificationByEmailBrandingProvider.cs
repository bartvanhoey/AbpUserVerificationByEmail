using Volo.Abp.DependencyInjection;
using Volo.Abp.Ui.Branding;

namespace AbpUserVerificationByEmail.Blazor
{
    [Dependency(ReplaceServices = true)]
    public class AbpUserVerificationByEmailBrandingProvider : DefaultBrandingProvider
    {
        public override string AppName => "User Email Verification";
    }
}
