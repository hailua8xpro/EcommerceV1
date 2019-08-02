using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Core.Domain.News
{
    public partial class NewsCategoryMapping : BaseEntity
    {
        public int NewsId { get; set; }
        public int NewsCategoryId { get; set; }
        public virtual  NewsCategory NewsCategory { get; set; }
        public virtual  NewsItem NewsItem { get; set; }
    }
}
