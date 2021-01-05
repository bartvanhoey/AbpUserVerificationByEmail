using AbpUserVerificationByEmail.Localization;
using Volo.Abp.AspNetCore.Mvc;

namespace AbpUserVerificationByEmail.Controllers
{
    /* Inherit your controllers from this class.
     */
    public abstract class AbpUserVerificationByEmailController : AbpController
    {
        protected AbpUserVerificationByEmailController()
        {
            LocalizationResource = typeof(AbpUserVerificationByEmailResource);
        }
    }
}