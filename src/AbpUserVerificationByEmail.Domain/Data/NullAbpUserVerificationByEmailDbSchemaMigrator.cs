using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace AbpUserVerificationByEmail.Data
{
    /* This is used if database provider does't define
     * IAbpUserVerificationByEmailDbSchemaMigrator implementation.
     */
    public class NullAbpUserVerificationByEmailDbSchemaMigrator : IAbpUserVerificationByEmailDbSchemaMigrator, ITransientDependency
    {
        public Task MigrateAsync()
        {
            return Task.CompletedTask;
        }
    }
}