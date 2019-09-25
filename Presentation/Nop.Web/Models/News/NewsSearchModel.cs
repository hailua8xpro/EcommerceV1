using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Mvc.ModelBinding;
using Nop.Web.Framework.Models;

namespace Nop.Web.Models.News
{
    public partial class NewsSearchModel : BaseNopModel
    {
        public NewsSearchModel()
        {
            NewsItems = new List<NewsItemModel>();
            PagingContext = new NewsPagingFilteringModel();
        }

        public bool NoResults { get; set; }

        /// <summary>
        /// Query string
        /// </summary>
        [NopResourceDisplayName("Search.SearchTerm")]
        public string q { get; set; }

        public IList<NewsItemModel> NewsItems { get; set; }

        #region Nested classes

        public class CategoryModel : BaseNopEntityModel
        {
            public string Breadcrumb { get; set; }
        }
        public NewsPagingFilteringModel PagingContext { get; set; }
        #endregion
    }
}