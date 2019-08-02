using System.Collections.Generic;
using Nop.Core;
using Nop.Core.Domain.News;

namespace Nop.Services.News
{
    public partial interface INewsCategoryService
    {
        /// <summary>
        /// Delete news category
        /// </summary>
        /// <param name="newsCategory">news Category</param>
        void DeleteNewsCategory(NewsCategory newsCategory);

        /// <summary>
        /// Gets all news categories
        /// </summary>
        /// <param name="storeId">Store identifier; 0 if you want to get all records</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <param name="loadCacheableCopy">A value indicating whether to load a copy that could be cached (workaround until Entity Framework supports 2-level caching)</param>
        /// <returns>Categories</returns>
        IList<NewsCategory> GetAllNewsCategories(int storeId = 0, bool showHidden = false, bool loadCacheableCopy = true);

        /// <summary>
        /// Gets all news categories
        /// </summary>
        /// <param name="newsCategoryName">News Category name</param>
        /// <param name="storeId">Store identifier; 0 if you want to get all records</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>Categories</returns>
        IPagedList<NewsCategory> GetAllNewsCategories(string newsCategoryName, int storeId = 0,
            int pageIndex = 0, int pageSize = int.MaxValue, bool showHidden = false);

        /// <summary>
        /// Gets all News Category filtered by parent  News Category identifier
        /// </summary>
        /// <param name="parentNewsCategoryId">Parent news category identifier</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>NewsCategories</returns>
        IList<NewsCategory> GetAllNewsCategoriesByParentCategoryId(int parentNewsCategoryId, bool showHidden = false);

        /// <summary>
        /// Gets all news categories displayed on the home page
        /// </summary>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>Categories</returns>
        IList<NewsCategory> GetAllNewsCategoriesDisplayedOnHomepage(bool showHidden = false);

        /// <summary>
        /// Gets child news category identifiers
        /// </summary>
        /// <param name="parentNewsCategoryId">Parent news category identifier</param>
        /// <param name="storeId">Store identifier; 0 if you want to get all records</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>Category identifiers</returns>
        IList<int> GetChildNewsCategoryIds(int parentNewsCategoryId, int storeId = 0, bool showHidden = false);

        /// <summary>
        /// Gets a news category
        /// </summary>
        /// <param name="newsCategoryId">News category identifier</param>
        /// <returns>News category</returns>
        NewsCategory GetNewsCategoryById(int newsCategoryId);

        /// <summary>
        /// Inserts news category
        /// </summary>
        /// <param name="newsCategory">News category</param>
        void InsertNewsCategory(NewsCategory newsCategory);

        /// <summary>
        /// Updates the news category
        /// </summary>
        /// <param name="newsCategory">News category</param>
        void UpdateCategory(NewsCategory newsCategory);

        /// <summary>
        /// Deletes a news category mapping
        /// </summary>
        /// <param name="newsCategoryMapping">News category mapping</param>
        void DeleteNewsCategoryMapping(NewsCategoryMapping newsCategoryMapping);

        /// <summary>
        /// Gets news category mapping collection
        /// </summary>
        /// <param name="newsCategoryId">News category identifier</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>News category mapping collection</returns>
        IPagedList<NewsCategoryMapping> GetNewsCategoryMappingsByCategoryId(int newsCategoryId,
            int pageIndex = 0, int pageSize = int.MaxValue, bool showHidden = false);

        /// <summary>
        /// Gets a news category mapping collection
        /// </summary>
        /// <param name="newsId">News identifier</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>News category mapping collection</returns>
        IList<NewsCategoryMapping> GetNewsCategoryMappingsByNewsId(int newsId, bool showHidden = false);

        /// <summary>
        /// Gets a news category mapping collection
        /// </summary>
        /// <param name="newsId">News identifier</param>
        /// <param name="storeId">Store identifier (used in multi-store environment). "showHidden" parameter should also be "true"</param>
        /// <param name="showHidden"> A value indicating whether to show hidden records</param>
        /// <returns> News category mapping collection</returns>
        IList<NewsCategoryMapping> GetNewsCategoryMappingsByNewsId(int newsId, int storeId, bool showHidden = false);

        /// <summary>
        /// Gets a news category mapping 
        /// </summary>
        /// <param name="newsCategoryMappingId">News category mapping identifier</param>
        /// <returns>News category mapping</returns>
        NewsCategoryMapping GetNewsCategoryMappingById(int newsCategoryMappingId);

        /// <summary>
        /// Inserts a news category mapping
        /// </summary>
        /// <param name="newsCategoryMapping">>News category mapping</param>
        void InsertNewsCategoryMapping(NewsCategoryMapping newsCategoryMapping);

        /// <summary>
        /// Updates the news category mapping 
        /// </summary>
        /// <param name="newsCategoryMapping">>News category mapping</param>
        void UpdateNewsCategoryMapping(NewsCategoryMapping newsCategoryMapping);

        /// <summary>
        /// Returns a list of names of not existing news categories
        /// </summary>
        /// <param name="newsCategoryIdsNames">The names and/or IDs of the news categories to check</param>
        /// <returns>List of names and/or IDs not existing news categories</returns>
        string[] GetNotExistingNewsCategories(string[] newsCategoryIdsNames);


        /// <summary>
        /// Gets news categories by identifier
        /// </summary>
        /// <param name="categoryIds">NewsCategory identifiers</param>
        /// <returns>Categories</returns>
        List<NewsCategory> GetNewsCategoriesByIds(int[] newsCategoryIds);

        /// <summary>
        /// Sort categories for tree representation
        /// </summary>
        /// <param name="source">Source</param>
        /// <param name="parentId">Parent category identifier</param>
        /// <param name="ignoreNewsCategoriesWithoutExistingParent">A value indicating whether news categories without parent category in provided category list (source) should be ignored</param>
        /// <returns>Sorted news categories</returns>
        IList<NewsCategory> SortNewsCategoriesForTree(IList<NewsCategory> source, int parentId = 0,
            bool ignoreNewsCategoriesWithoutExistingParent = false);

        /// <summary>
        /// Returns a NewsCategoryMapping that has the specified values
        /// </summary>
        /// <param name="source">Source</param>
        /// <param name="newsId">News identifier</param>
        /// <param name="newsCategoryId">NewsCategory identifier</param>
        /// <returns>A ProductCategory that has the specified values; otherwise null</returns>
        NewsCategoryMapping FindNewsCategoryMapping(IList<NewsCategoryMapping> source, int newsId, int newsCategoryId);

        /// <summary>
        /// Get formatted news category breadcrumb 
        /// Note: ACL and store mapping is ignored
        /// </summary>
        /// <param name="newsCategory">NewsCategory</param>
        /// <param name="allNewsCategories">All news categories</param>
        /// <param name="separator">Separator</param>
        /// <param name="languageId">Language identifier for localization</param>
        /// <returns>Formatted breadcrumb</returns>
        string GetFormattedBreadCrumb(NewsCategory newsCategory, IList<NewsCategory> allNewsCategories = null,
            string separator = ">>", int languageId = 0);

        /// <summary>
        /// Get news category breadcrumb 
        /// </summary>
        /// <param name="newsCategory">NewsCategory</param>
        /// <param name="allNewsCategories">All categories</param>
        /// <param name="showHidden">A value indicating whether to load hidden records</param>
        /// <returns>Category breadcrumb </returns>
        IList<NewsCategory> GetNewsCategoryBreadCrumb(NewsCategory newsCategory, IList<NewsCategory> allNewsCategories = null, bool showHidden = false);
    }
}
