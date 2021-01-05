using AbpUserVerificationByEmail.Localization;
using Volo.Abp.AspNetCore.Components;

namespace AbpUserVerificationByEmail.Blazor
{
    public abstract class AbpUserVerificationByEmailComponentBase : AbpComponentBase
    {
        protected AbpUserVerificationByEmailComponentBase()
        {
            LocalizationResource = typeof(AbpUserVerificationByEmailResource);
        }
    }
}
