using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.News;
using Nop.Web.Areas.Admin.Models.News;

namespace Nop.Web.Areas.Admin.Factories
{
    /// <summary>
    /// Represents the category model factory
    /// </summary>
    public partial interface INewsCategoryModelFactory
    {
        /// <summary>
        /// Prepare news category search model
        /// </summary>
        /// <param name="searchModel">News category search model</param>
        /// <returns>News category search model</returns>
        NewsCategorySearchModel PrepareNewsCategorySearchModel(NewsCategorySearchModel searchModel);

        /// <summary>
        /// Prepare paged news category list model
        /// </summary>
        /// <param name="searchModel">News category search model</param>
        /// <returns>Category list model</returns>
        NewsCategoryListModel PrepareNewsCategoryListModel(NewsCategorySearchModel searchModel);

        /// <summary>
        /// Prepare news category model
        /// </summary>
        /// <param name="model">News category model</param>
        /// <param name="category">News category</param>
        /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
        /// <returns>News category model</returns>
        NewsCategoryModel PrepareNewsCategoryModel(NewsCategoryModel model, NewsCategory newsCategory, bool excludeProperties = false);

        /// <summary>
        /// Prepare paged news category product list model
        /// </summary>
        /// <param name="searchModel">Category news search model</param>
        /// <param name="category">Category</param>
        /// <returns>Category news list model</returns>
        CategoryNewsListModel PrepareCategoryNewsListModel(CategoryNewsSearchModel searchModel, NewsCategory newsCategory);

        /// <summary>
        /// Prepare news search model to add to the category
        /// </summary>
        /// <param name="searchModel">News search model to add to the category</param>
        /// <returns>News search model to add to the category</returns>
        AddNewsToCategorySearchModel PrepareAddNewsToCategorySearchModel(AddNewsToCategorySearchModel searchModel);

        /// <summary>
        /// Prepare paged news list model to add to the category
        /// </summary>
        /// <param name="searchModel">News search model to add to the category</param>
        /// <returns>News list model to add to the category</returns>
        AddNewsToCategoryListModel PrepareAddNewsToCategoryListModel(AddNewsToCategorySearchModel searchModel);
    }
}