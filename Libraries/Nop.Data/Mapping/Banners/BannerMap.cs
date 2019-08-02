using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nop.Core.Domain.Banners;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Data.Mapping.Banners
{
  public  class BannerMap : NopEntityTypeConfiguration<Banner>
    {
        public override void Configure(EntityTypeBuilder<Banner> builder)
        {
            builder.ToTable(nameof(Banner));
            builder.HasKey(blogPost => blogPost.Id);
            base.Configure(builder);
        }
    }
}
