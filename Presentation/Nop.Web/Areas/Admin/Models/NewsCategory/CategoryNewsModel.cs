using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Web.Areas.Admin.Models.News
{
    /// <summary>
    /// Represents a category product model
    /// </summary>
    public partial class CategoryNewsModel : BaseNopEntityModel
    {
        #region Properties

        public int NewsCategoryId { get; set; }

        public int NewsId { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Categories.Products.Fields.Product")]
        public string Title { get; set; }
        #endregion
    }
}