using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Web.Areas.Admin.Models.News
{
    /// <summary>
    /// Represents a product search model to add to the category
    /// </summary>
    public partial class AddNewsToCategorySearchModel : BaseSearchModel
    {
        #region Ctor

        public AddNewsToCategorySearchModel()
        {
            AvailableCategories = new List<SelectListItem>();
            AvailableStores = new List<SelectListItem>();
        }

        #endregion

        #region Properties

        [NopResourceDisplayName("Admin.Catalog.Products.List.SearchProductName")]
        public string SearchNewsTitle { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Products.List.SearchCategory")]
        public int SearchNewsCategoryId { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Products.List.SearchStore")]
        public int SearchStoreId { get; set; }
        public IList<SelectListItem> AvailableCategories { get; set; }
        public IList<SelectListItem> AvailableStores { get; set; }
        #endregion
    }
}