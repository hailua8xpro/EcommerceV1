using System.Collections.Generic;
using Nop.Web.Framework.Models;

namespace Nop.Web.Areas.Admin.Models.News
{
    /// <summary>
    /// Represents a news model to add to the category
    /// </summary>
    public partial class AddNewsToCategoryModel : BaseNopModel
    {
        #region Ctor

        public AddNewsToCategoryModel()
        {
            SelectedNewsIds = new List<int>();
        }
        #endregion

        #region Properties

        public int NewsCategoryId { get; set; }

        public IList<int> SelectedNewsIds { get; set; }

        #endregion
    }
}