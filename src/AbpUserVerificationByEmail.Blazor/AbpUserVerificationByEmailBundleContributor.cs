using Volo.Abp.Bundling;

namespace AbpUserVerificationByEmail.Blazor
{
    public class AbpUserVerificationByEmailBundleContributor : IBundleContributor
    {
        public void AddScripts(BundleContext context)
        {

        }

        public void AddStyles(BundleContext context)
        {
            context.Add("main.css");
        }
    }
}
