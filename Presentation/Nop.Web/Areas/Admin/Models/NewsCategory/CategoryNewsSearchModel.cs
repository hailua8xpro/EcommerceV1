using Nop.Web.Framework.Models;

namespace Nop.Web.Areas.Admin.Models.News
{
    /// <summary>
    /// Represents a category product search model
    /// </summary>
    public partial class CategoryNewsSearchModel : BaseSearchModel
    {
        #region Properties

        public int NewsCategoryId { get; set; }

        #endregion
    }
}