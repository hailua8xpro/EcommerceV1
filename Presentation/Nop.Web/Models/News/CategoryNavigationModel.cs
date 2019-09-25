using System.Collections.Generic;
using Nop.Web.Framework.Models;

namespace Nop.Web.Models.News
{
    public partial class NewsCategoryNavigationModel : BaseNopModel
    {
        public NewsCategoryNavigationModel()
        {
            NewsCategories = new List<NewsCategoryModel>();
        }

        public int CurrentNewsCategoryId { get; set; }
        public List<NewsCategoryModel> NewsCategories { get; set; }

        #region Nested classes

        public class NewsCategoryLineModel : BaseNopModel
        {
            public int CurrentNewsCategoryId { get; set; }
            public NewsCategoryModel NewsCategory { get; set; }
        }

        #endregion
    }
}