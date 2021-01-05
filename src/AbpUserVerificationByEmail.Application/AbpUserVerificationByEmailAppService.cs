using System;
using System.Collections.Generic;
using System.Text;
using AbpUserVerificationByEmail.Localization;
using Volo.Abp.Application.Services;

namespace AbpUserVerificationByEmail
{
    /* Inherit your application services from this class.
     */
    public abstract class AbpUserVerificationByEmailAppService : ApplicationService
    {
        protected AbpUserVerificationByEmailAppService()
        {
            LocalizationResource = typeof(AbpUserVerificationByEmailResource);
        }
    }
}
