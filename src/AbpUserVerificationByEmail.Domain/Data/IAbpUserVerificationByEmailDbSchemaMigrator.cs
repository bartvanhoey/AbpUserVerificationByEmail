using System.Threading.Tasks;

namespace AbpUserVerificationByEmail.Data;

public interface IAbpUserVerificationByEmailDbSchemaMigrator
{
    Task MigrateAsync();
}
