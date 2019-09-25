using System;
using System.Collections.Generic;
using Nop.Web.Framework.Models;
using Nop.Web.Models.Media;

namespace Nop.Web.Models.News
{
    public partial class NewsItemModel : BaseNopEntityModel
    {
        public NewsItemModel()
        {
            Comments = new List<NewsCommentModel>();
            AddNewComment = new AddNewsCommentModel();
        }

        public string MetaKeywords { get; set; }
        public string MetaDescription { get; set; }
        public string MetaTitle { get; set; }
        public string SeName { get; set; }

        public string Title { get; set; }
        public string Short { get; set; }
        public string Full { get; set; }
        public bool AllowComments { get; set; }
        public int NumberOfComments { get; set; }
        public DateTime CreatedOn { get; set; }
        public PictureModel PictureModel { get; set; }

        public IList<NewsCommentModel> Comments { get; set; }
        public AddNewsCommentModel AddNewComment { get; set; }
        public NewsItemBreadcrumbModel Breadcrumb { get; set; }
        public partial class NewsItemBreadcrumbModel : BaseNopModel
        {
            public NewsItemBreadcrumbModel()
            {
                NewsCategoryBreadcrumb = new List<NewsCategoryModel>();
            }

            public bool Enabled { get; set; }
            public int NewsId { get; set; }
            public string Title { get; set; }
            public string SeName { get; set; }
            public IList<NewsCategoryModel> NewsCategoryBreadcrumb { get; set; }
        }
    }
}