using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nop.Core.Domain.Directory;


namespace Nop.Data.Mapping.Directory
{
    public partial class WardMap:NopEntityTypeConfiguration<Ward>
    {
        #region Methods

        /// <summary>
        /// Configures the entity
        /// </summary>
        /// <param name="builder">The builder to be used to configure the entity</param>
        public override void Configure(EntityTypeBuilder<Ward> builder)
        {
            builder.ToTable(nameof(Ward));
            builder.HasKey(ward => ward.Id);

            builder.Property(ward => ward.Name).HasMaxLength(100).IsRequired();

            builder.HasOne(ward => ward.District)
                .WithMany(district => district.Wards)
                .HasForeignKey(ward => ward.DistrictId)
                .IsRequired();

            base.Configure(builder);
        }

        #endregion
    }
}
