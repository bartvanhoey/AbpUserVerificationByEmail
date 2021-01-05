using Volo.Abp.Settings;

namespace AbpUserVerificationByEmail.Settings
{
    public class AbpUserVerificationByEmailSettingDefinitionProvider : SettingDefinitionProvider
    {
        public override void Define(ISettingDefinitionContext context)
        {
            //Define your own settings here. Example:
            //context.Add(new SettingDefinition(AbpUserVerificationByEmailSettings.MySetting1));
        }
    }
}
