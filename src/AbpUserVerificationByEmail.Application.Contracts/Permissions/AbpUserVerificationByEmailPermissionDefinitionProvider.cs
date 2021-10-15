using AbpUserVerificationByEmail.Localization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;

namespace AbpUserVerificationByEmail.Permissions
{
    public class AbpUserVerificationByEmailPermissionDefinitionProvider : PermissionDefinitionProvider
    {
        public override void Define(IPermissionDefinitionContext context)
        {
            var myGroup = context.AddGroup(AbpUserVerificationByEmailPermissions.GroupName);
            //Define your own permissions here. Example:
            //myGroup.AddPermission(AbpUserVerificationByEmailPermissions.MyPermission1, L("Permission:MyPermission1"));
        }

        private static LocalizableString L(string name)
        {
            return LocalizableString.Create<AbpUserVerificationByEmailResource>(name);
        }
    }
}
