using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nop.Core.Domain.Directory;

namespace Nop.Data.Mapping.Directory
{
    /// <summary>
    /// Represents a state and province mapping configuration
    /// </summary>
    public partial class DistrictMap : NopEntityTypeConfiguration<District>
    {
        #region Methods

        /// <summary>
        /// Configures the entity
        /// </summary>
        /// <param name="builder">The builder to be used to configure the entity</param>
        public override void Configure(EntityTypeBuilder<District> builder)
        {
            builder.ToTable(nameof(District));
            builder.HasKey(district => district.Id);

            builder.Property(district => district.Name).HasMaxLength(100).IsRequired();

            builder.HasOne(district => district.StateProvince)
                .WithMany(stateprovince => stateprovince.Districts)
                .HasForeignKey(district => district.StateProvinceId)
                .IsRequired();

            base.Configure(builder);
        }

        #endregion
    }
}