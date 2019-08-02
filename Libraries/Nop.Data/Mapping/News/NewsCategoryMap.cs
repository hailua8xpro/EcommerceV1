using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nop.Core.Domain.News;

namespace Nop.Data.Mapping.News
{
    public partial class ServiceCategoryMap : NopEntityTypeConfiguration<NewsCategoryMapping>
    {
        #region Methods

        /// <summary>
        /// Configures the entity
        /// </summary>
        /// <param name="builder">The builder to be used to configure the entity</param>
        public override void Configure(EntityTypeBuilder<NewsCategoryMapping> builder)
        {
            builder.ToTable(NopMappingDefaults.NewsCategoryMappingTable);
            builder.HasKey(newsCategory => newsCategory.Id);

            builder.HasOne(newsCategory => newsCategory.NewsCategory)
                .WithMany()
                .HasForeignKey(newsCategory => newsCategory.NewsCategoryId)
                .IsRequired();

            builder.HasOne(newsCategory => newsCategory.NewsItem)
                .WithMany(newsitem => newsitem.NewsCategoryMappings)
                .HasForeignKey(newsCategory => newsCategory.NewsId)
                .IsRequired();

            base.Configure(builder);
        }

        #endregion
    }
}
