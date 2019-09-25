using Nop.Web.Framework.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nop.Web.Models.News
{
    public class NewsCategoryModel : BaseNopEntityModel
    {
        public NewsCategoryModel()
        {
            SubCategories = new List<NewsCategoryModel>();
            NewsCategoryBreadcrumb = new List<NewsCategoryModel>();
            PagingFilteringContext = new NewsPagingFilteringModel();
            NewsItems = new List<NewsItemModel>();
        }
        public string Name { get; set; }
        public string Description { get; set; }
        public string MetaKeywords { get; set; }
        public string MetaDescription { get; set; }
        public string MetaTitle { get; set; }
        public string SeName { get; set; }
        public List<NewsCategoryModel> SubCategories { get; set; }
        public bool HaveSubCategories { get; set; }
        public IList<NewsCategoryModel> NewsCategoryBreadcrumb { get; set; }
        public NewsPagingFilteringModel PagingFilteringContext { get; set; }
        public IList<NewsItemModel> NewsItems { get; set; }


    }
}
