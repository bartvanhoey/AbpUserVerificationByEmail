using Microsoft.EntityFrameworkCore;
using Volo.Abp;

namespace AbpUserVerificationByEmail.EntityFrameworkCore
{
    public static class AbpUserVerificationByEmailDbContextModelCreatingExtensions
    {
        public static void ConfigureAbpUserVerificationByEmail(this ModelBuilder builder)
        {
            Check.NotNull(builder, nameof(builder));

            /* Configure your own tables/entities inside here */

            //builder.Entity<YourEntity>(b =>
            //{
            //    b.ToTable(AbpUserVerificationByEmailConsts.DbTablePrefix + "YourEntities", AbpUserVerificationByEmailConsts.DbSchema);
            //    b.ConfigureByConvention(); //auto configure for the base class props
            //    //...
            //});
        }
    }
}