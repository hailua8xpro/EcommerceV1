using System;
using System.Collections.Generic;
using System.Text;
using Nop.Core;
using Nop.Core.Domain.News;
using Nop.Core.Caching;
using Nop.Services.Events;
using Nop.Services.Catalog;
using Nop.Core.Data;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Common;
using Nop.Core.Data.Extensions;
using Nop.Core.Domain.Catalog;
using Nop.Data;
using System.Linq;
using Nop.Core.Domain.Security;
using Nop.Core.Domain.Stores;
using Nop.Services.Security;
using Nop.Services.Stores;
using Nop.Services.Localization;

namespace Nop.Services.News
{
    public partial class NewsCategoryService : INewsCategoryService
    {
        #region Fields
        private readonly IEventPublisher _eventPublisher;
        private readonly IRepository<NewsCategoryMapping> _newsCategoryMappingRepository;
        private readonly IRepository<NewsCategory> _newsCategoryRepository;
        private readonly IRepository<NewsItem> _newsRepository;
        private readonly ICacheManager _cacheManager;
        private readonly IWorkContext _workContext;
        private readonly IStaticCacheManager _staticCacheManager;
        private readonly CommonSettings _commonSettings;
        private readonly IDataProvider _dataProvider;
        private readonly CatalogSettings _catalogSettings;
        private readonly IDbContext _dbContext;
        private readonly IRepository<AclRecord> _aclRepository;
        private readonly IRepository<StoreMapping> _storeMappingRepository;
        private readonly IStoreContext _storeContext;
        private readonly IAclService _aclService;
        private readonly IStoreMappingService _storeMappingService;
        private readonly ILocalizationService _localizationService;

        #endregion
        #region ctor
        public NewsCategoryService(IEventPublisher eventPublisher,
            IRepository<NewsCategoryMapping> newsCategoryMappingRepository,
            ICacheManager cacheManager,
            IWorkContext workContext,
            IStaticCacheManager staticCacheManager,
            CommonSettings commonSettings,
            IDataProvider dataProvider,
            IDbContext dbContext,
            CatalogSettings catalogSettings,
            IRepository<NewsCategory> newsCategoryRepository,
            IRepository<AclRecord> aclRepository,
            IRepository<StoreMapping> storeMappingRepository,
            IStoreContext storeContext,
            IAclService aclService,
            IStoreMappingService storeMappingService,
            ILocalizationService localizationService,
            IRepository<NewsItem> newsRepository
            )
        {
            _eventPublisher = eventPublisher;
            _newsCategoryMappingRepository = newsCategoryMappingRepository;
            _cacheManager = cacheManager;
            _workContext = workContext;
            _staticCacheManager = staticCacheManager;
            _commonSettings = commonSettings;
            _dataProvider = dataProvider;
            _catalogSettings = catalogSettings;
            _dbContext = dbContext;
            _newsCategoryRepository = newsCategoryRepository;
            _aclRepository = aclRepository;
            _storeMappingRepository = storeMappingRepository;
            _storeContext = storeContext;
            _aclService = aclService;
            _storeMappingService = storeMappingService;
            _localizationService = localizationService;
            _newsRepository = newsRepository;
        }
        #endregion
        #region method
        public void DeleteNewsCategory(NewsCategory newsCategory)
        {
            if (newsCategory == null)
                throw new ArgumentNullException(nameof(newsCategory));

            if (newsCategory is IEntityForCaching)
                throw new ArgumentException("Cacheable entities are not supported by Entity Framework");

            newsCategory.Deleted = true;
            UpdateCategory(newsCategory);

            //event notification
            _eventPublisher.EntityDeleted(newsCategory);

            //reset a "Parent category" property of all child subcategories
            var subcategories = GetAllNewsCategoriesByParentCategoryId(newsCategory.Id, true);
            foreach (var subcategory in subcategories)
            {
                subcategory.ParentCategoryId = 0;
                UpdateCategory(subcategory);
            }
        }

        public void DeleteNewsCategoryMapping(NewsCategoryMapping newsCategoryMapping)
        {
            if (newsCategoryMapping == null)
                throw new ArgumentNullException(nameof(newsCategoryMapping));

            _newsCategoryMappingRepository.Delete(newsCategoryMapping);

            //cache
            _cacheManager.RemoveByPrefix(NopNewsDefaults.NewsCategoryMappingsPrefixCacheKey);

            //event notification
            _eventPublisher.EntityDeleted(newsCategoryMapping);
        }

        public NewsCategoryMapping FindNewsCategoryMapping(IList<NewsCategoryMapping> source, int newsId, int newsCategoryId)
        {
            foreach (var newsCategoryMapping in source)
                if (newsCategoryMapping.NewsId == newsId && newsCategoryMapping.NewsCategoryId == newsCategoryId)
                    return newsCategoryMapping;

            return null;
        }

        public IList<NewsCategory> GetAllNewsCategories(int storeId = 0, bool showHidden = false, bool loadCacheableCopy = true)
        {
            IList<NewsCategory> LoadNewsCategoriesFunc() => GetAllNewsCategories(string.Empty, storeId, showHidden: showHidden);

            IList<NewsCategory> newsCategories;
            if (loadCacheableCopy)
            {
                //cacheable copy
                var key = string.Format(NopNewsDefaults.NewsCategoriesAllCacheKey,
                    storeId,
                    string.Join(",", _workContext.CurrentCustomer.GetCustomerRoleIds()),
                    showHidden);
                newsCategories = _staticCacheManager.Get(key, () =>
                {
                    var result = new List<NewsCategory>();
                    foreach (var newsCategory in LoadNewsCategoriesFunc())
                        result.Add(new NewsCategoryForCaching(newsCategory));
                    return result;
                });
            }
            else
            {
                newsCategories = LoadNewsCategoriesFunc();
            }

            return newsCategories;
        }

        public IPagedList<NewsCategory> GetAllNewsCategories(string newsCategoryName, int storeId = 0, int pageIndex = 0, int pageSize = int.MaxValue, bool showHidden = false)
        {
            if (_commonSettings.UseStoredProcedureForLoadingCategories)
            {
                //stored procedures are enabled for loading categories and supported by the database. 
                //It's much faster with a large number of categories than the LINQ implementation below 

                //prepare parameters
                var showHiddenParameter = _dataProvider.GetBooleanParameter("ShowHidden", showHidden);
                var nameParameter = _dataProvider.GetStringParameter("Name", newsCategoryName ?? string.Empty);
                var storeIdParameter = _dataProvider.GetInt32Parameter("StoreId", !_catalogSettings.IgnoreStoreLimitations ? storeId : 0);
                var pageIndexParameter = _dataProvider.GetInt32Parameter("PageIndex", pageIndex);
                var pageSizeParameter = _dataProvider.GetInt32Parameter("PageSize", pageSize);
                //pass allowed customer role identifiers as comma-delimited string
                var customerRoleIdsParameter = _dataProvider.GetStringParameter("CustomerRoleIds", !_catalogSettings.IgnoreAcl ? string.Join(",", _workContext.CurrentCustomer.GetCustomerRoleIds()) : string.Empty);

                var totalRecordsParameter = _dataProvider.GetOutputInt32Parameter("TotalRecords");

                //invoke stored procedure
                var categories = _dbContext.EntityFromSql<NewsCategory>("NewsCategoryLoadAllPaged",
                    showHiddenParameter, nameParameter, storeIdParameter, customerRoleIdsParameter,
                    pageIndexParameter, pageSizeParameter, totalRecordsParameter).ToList();
                var totalRecords = totalRecordsParameter.Value != DBNull.Value ? Convert.ToInt32(totalRecordsParameter.Value) : 0;

                //paging
                return new PagedList<NewsCategory>(categories, pageIndex, pageSize, totalRecords);
            }

            //don't use a stored procedure. Use LINQ
            var query = _newsCategoryRepository.Table;
            if (!showHidden)
                query = query.Where(c => c.Published);
            if (!string.IsNullOrWhiteSpace(newsCategoryName))
                query = query.Where(c => c.Name.Contains(newsCategoryName));
            query = query.Where(c => !c.Deleted);
            query = query.OrderBy(c => c.ParentCategoryId).ThenBy(c => c.DisplayOrder).ThenBy(c => c.Id);

            if ((storeId > 0 && !_catalogSettings.IgnoreStoreLimitations) || (!showHidden && !_catalogSettings.IgnoreAcl))
            {
                if (!showHidden && !_catalogSettings.IgnoreAcl)
                {
                    //ACL (access control list)
                    var allowedCustomerRolesIds = _workContext.CurrentCustomer.GetCustomerRoleIds();
                    query = from c in query
                            join acl in _aclRepository.Table
                                on new { c1 = c.Id, c2 = nameof(Category) } equals new { c1 = acl.EntityId, c2 = acl.EntityName } into c_acl
                            from acl in c_acl.DefaultIfEmpty()
                            where !c.SubjectToAcl || allowedCustomerRolesIds.Contains(acl.CustomerRoleId)
                            select c;
                }

                if (storeId > 0 && !_catalogSettings.IgnoreStoreLimitations)
                {
                    //Store mapping
                    query = from c in query
                            join sm in _storeMappingRepository.Table
                                on new { c1 = c.Id, c2 = nameof(Category) } equals new { c1 = sm.EntityId, c2 = sm.EntityName } into c_sm
                            from sm in c_sm.DefaultIfEmpty()
                            where !c.LimitedToStores || storeId == sm.StoreId
                            select c;
                }

                query = query.Distinct().OrderBy(c => c.ParentCategoryId).ThenBy(c => c.DisplayOrder).ThenBy(c => c.Id);
            }

            var unsortedCategories = query.ToList();

            //sort categories
            var sortedCategories = SortNewsCategoriesForTree(unsortedCategories);

            //paging
            return new PagedList<NewsCategory>(sortedCategories, pageIndex, pageSize);
        }

        public IList<NewsCategory> GetAllNewsCategoriesByParentCategoryId(int parentNewsCategoryId, bool showHidden = false)
        {
            var key = string.Format(NopNewsDefaults.CategoriesByParentCategoryIdCacheKey, parentNewsCategoryId, showHidden, _workContext.CurrentCustomer.Id, _storeContext.CurrentStore.Id);
            return _cacheManager.Get(key, () =>
            {
                var query = _newsCategoryRepository.Table;
                if (!showHidden)
                    query = query.Where(c => c.Published);
                query = query.Where(c => c.ParentCategoryId == parentNewsCategoryId);
                query = query.Where(c => !c.Deleted);
                query = query.OrderBy(c => c.DisplayOrder).ThenBy(c => c.Id);

                if (!showHidden && (!_catalogSettings.IgnoreAcl || !_catalogSettings.IgnoreStoreLimitations))
                {
                    if (!_catalogSettings.IgnoreAcl)
                    {
                        //ACL (access control list)
                        var allowedCustomerRolesIds = _workContext.CurrentCustomer.GetCustomerRoleIds();
                        query = from c in query
                                join acl in _aclRepository.Table
                                on new { c1 = c.Id, c2 = nameof(Category) } equals new { c1 = acl.EntityId, c2 = acl.EntityName } into c_acl
                                from acl in c_acl.DefaultIfEmpty()
                                where !c.SubjectToAcl || allowedCustomerRolesIds.Contains(acl.CustomerRoleId)
                                select c;
                    }

                    if (!_catalogSettings.IgnoreStoreLimitations)
                    {
                        //Store mapping
                        var currentStoreId = _storeContext.CurrentStore.Id;
                        query = from c in query
                                join sm in _storeMappingRepository.Table
                                on new { c1 = c.Id, c2 = nameof(Category) } equals new { c1 = sm.EntityId, c2 = sm.EntityName } into c_sm
                                from sm in c_sm.DefaultIfEmpty()
                                where !c.LimitedToStores || currentStoreId == sm.StoreId
                                select c;
                    }

                    query = query.Distinct().OrderBy(c => c.DisplayOrder).ThenBy(c => c.Id);
                }

                var categories = query.ToList();
                return categories;
            });
        }

        public IList<NewsCategory> GetAllNewsCategoriesDisplayedOnHomepage(bool showHidden = false)
        {
            var query = from c in _newsCategoryRepository.Table
                        orderby c.DisplayOrder, c.Id
                        where c.Published &&
                        !c.Deleted &&
                        c.ShowOnHomepage
                        select c;

            var newsCategories = query.ToList();
            if (!showHidden)
            {
                newsCategories = newsCategories
                    .Where(c => _aclService.Authorize(c) && _storeMappingService.Authorize(c))
                    .ToList();
            }

            return newsCategories;
        }

        public IList<int> GetChildNewsCategoryIds(int parentNewsCategoryId, int storeId = 0, bool showHidden = false)
        {
            var cacheKey = string.Format(NopNewsDefaults.NewsCategoriesChildIdentifiersCacheKey,
                parentNewsCategoryId,
                string.Join(",", _workContext.CurrentCustomer.GetCustomerRoleIds()),
                _storeContext.CurrentStore.Id,
                showHidden);
            return _staticCacheManager.Get(cacheKey, () =>
            {
                //little hack for performance optimization
                //there's no need to invoke "GetAllCategoriesByParentCategoryId" multiple times (extra SQL commands) to load childs
                //so we load all categories at once (we know they are cached) and process them server-side
                var newsCategoriesIds = new List<int>();
                var newsCategories = GetAllNewsCategories(storeId: storeId, showHidden: showHidden)
                    .Where(c => c.ParentCategoryId == parentNewsCategoryId);
                foreach (var category in newsCategories)
                {
                    newsCategoriesIds.Add(category.Id);
                    newsCategoriesIds.AddRange(GetChildNewsCategoryIds(category.Id, storeId, showHidden));
                }

                return newsCategoriesIds;
            });
        }

        public string GetFormattedBreadCrumb(NewsCategory newsCategory, IList<NewsCategory> allNewsCategories = null, string separator = ">>", int languageId = 0)
        {
            var result = string.Empty;

            var breadcrumb = GetNewsCategoryBreadCrumb(newsCategory, allNewsCategories, true);
            for (var i = 0; i <= breadcrumb.Count - 1; i++)
            {
                var newsCategoryName = _localizationService.GetLocalized(breadcrumb[i], x => x.Name, languageId);
                result = string.IsNullOrEmpty(result) ? newsCategoryName : $"{result} {separator} {newsCategoryName}";
            }

            return result;
        }

        public List<NewsCategory> GetNewsCategoriesByIds(int[] newsCategoryIds)
        {
            if (newsCategoryIds == null || newsCategoryIds.Length == 0)
                return new List<NewsCategory>();

            var query = from p in _newsCategoryRepository.Table
                        where newsCategoryIds.Contains(p.Id) && !p.Deleted
                        select p;

            return query.ToList();
        }

        public IList<NewsCategory> GetNewsCategoryBreadCrumb(NewsCategory newsCategory, IList<NewsCategory> allNewsCategories = null, bool showHidden = false)
        {
            if (newsCategory == null)
                throw new ArgumentNullException(nameof(newsCategory));

            var result = new List<NewsCategory>();

            //used to prevent circular references
            var alreadyProcessedBewsCategoryIds = new List<int>();

            while (newsCategory != null && //not null
                !newsCategory.Deleted && //not deleted
                (showHidden || newsCategory.Published) && //published
                (showHidden || _aclService.Authorize(newsCategory)) && //ACL
                (showHidden || _storeMappingService.Authorize(newsCategory)) && //Store mapping
                !alreadyProcessedBewsCategoryIds.Contains(newsCategory.Id)) //prevent circular references
            {
                result.Add(newsCategory);

                alreadyProcessedBewsCategoryIds.Add(newsCategory.Id);

                newsCategory = allNewsCategories != null ? allNewsCategories.FirstOrDefault(c => c.Id == newsCategory.ParentCategoryId)
                    : GetNewsCategoryById(newsCategory.ParentCategoryId);
            }

            result.Reverse();
            return result;
        }

        public NewsCategory GetNewsCategoryById(int newsCategoryId)
        {
            if (newsCategoryId == 0)
                return null;

            var key = string.Format(NopNewsDefaults.NewsCategoriesByIdCacheKey, newsCategoryId);
            return _cacheManager.Get(key, () => _newsCategoryRepository.GetById(newsCategoryId));
        }

        public NewsCategoryMapping GetNewsCategoryMappingById(int newsCategoryMappingId)
        {
            if (newsCategoryMappingId == 0)
                return null;

            return _newsCategoryMappingRepository.GetById(newsCategoryMappingId);
        }

        public IPagedList<NewsCategoryMapping> GetNewsCategoryMappingsByCategoryId(int newsCategoryId, int pageIndex = 0, int pageSize = int.MaxValue, bool showHidden = false)
        {
            if (newsCategoryId == 0)
                return new PagedList<NewsCategoryMapping>(new List<NewsCategoryMapping>(), pageIndex, pageSize);

            var key = string.Format(NopNewsDefaults.NewsCategoryMappingsAllByCategoryIdCacheKey, showHidden, newsCategoryId, pageIndex, pageSize, _workContext.CurrentCustomer.Id, _storeContext.CurrentStore.Id);
            return _cacheManager.Get(key, () =>
            {
                var query = from ncm in _newsCategoryMappingRepository.Table
                            join n in _newsRepository.Table on ncm.NewsId equals n.Id
                            where ncm.NewsCategoryId == newsCategoryId &&
                                  (showHidden || n.Published)
                            orderby ncm.Id
                            select ncm;

                if (!showHidden && (!_catalogSettings.IgnoreAcl || !_catalogSettings.IgnoreStoreLimitations))
                {
                    if (!_catalogSettings.IgnoreAcl)
                    {
                        //ACL (access control list)
                        var allowedCustomerRolesIds = _workContext.CurrentCustomer.GetCustomerRoleIds();
                        query = from ncm in query
                                join nc in _newsCategoryRepository.Table on ncm.NewsCategoryId equals nc.Id
                                join acl in _aclRepository.Table
                                on new { c1 = nc.Id, c2 = nameof(Category) } equals new { c1 = acl.EntityId, c2 = acl.EntityName } into c_acl
                                from acl in c_acl.DefaultIfEmpty()
                                where !nc.SubjectToAcl || allowedCustomerRolesIds.Contains(acl.CustomerRoleId)
                                select ncm;
                    }

                    if (!_catalogSettings.IgnoreStoreLimitations)
                    {
                        //Store mapping
                        var currentStoreId = _storeContext.CurrentStore.Id;
                        query = from ncm in query
                                join c in _newsCategoryRepository.Table on ncm.NewsCategoryId equals c.Id
                                join sm in _storeMappingRepository.Table
                                on new { c1 = c.Id, c2 = nameof(Category) } equals new { c1 = sm.EntityId, c2 = sm.EntityName } into c_sm
                                from sm in c_sm.DefaultIfEmpty()
                                where !c.LimitedToStores || currentStoreId == sm.StoreId
                                select ncm;
                    }

                    query = query.Distinct().OrderBy(pc => pc.Id);
                }

                var newsCategoryMappings = new PagedList<NewsCategoryMapping>(query, pageIndex, pageSize);
                return newsCategoryMappings;
            });
        }

        public IList<NewsCategoryMapping> GetNewsCategoryMappingsByNewsId(int newsId, bool showHidden = false)
        {
            return GetNewsCategoryMappingsByNewsId(newsId, _storeContext.CurrentStore.Id, showHidden);

        }

        public IList<NewsCategoryMapping> GetNewsCategoryMappingsByNewsId(int newsId, int storeId, bool showHidden = false)
        {
            if (newsId == 0)
                return new List<NewsCategoryMapping>();

            var key = string.Format(NopNewsDefaults.NewsCategoryMappingsAllByNewsIdCacheKey, showHidden, newsId, _workContext.CurrentCustomer.Id, storeId);
            return _cacheManager.Get(key, () =>
            {
                var query = from ncm in _newsCategoryMappingRepository.Table
                            join nc in _newsCategoryRepository.Table on ncm.NewsCategoryId equals nc.Id
                            where ncm.NewsId == newsId &&
                                  !nc.Deleted &&
                                  (showHidden || nc.Published)
                            orderby ncm.Id
                            select ncm;

                var allNewsCategoryMapping = query.ToList();
                var result = new List<NewsCategoryMapping>();
                if (!showHidden)
                {
                    foreach (var ncm in allNewsCategoryMapping)
                    {
                        //ACL (access control list) and store mapping
                        var newsCategory = ncm.NewsCategory;
                        if (_aclService.Authorize(newsCategory) && _storeMappingService.Authorize(newsCategory, storeId))
                            result.Add(ncm);
                    }
                }
                else
                {
                    //no filtering
                    result.AddRange(allNewsCategoryMapping);
                }

                return result;
            });
        }

        public string[] GetNotExistingNewsCategories(string[] newsCategoryIdsNames)
        {
            if (newsCategoryIdsNames == null)
                throw new ArgumentNullException(nameof(newsCategoryIdsNames));

            var query = _newsCategoryRepository.Table;
            var queryFilter = newsCategoryIdsNames.Distinct().ToArray();
            //filtering by name
            var filter = query.Select(c => c.Name).Where(c => queryFilter.Contains(c)).ToList();
            queryFilter = queryFilter.Except(filter).ToArray();

            //if some names not found
            if (!queryFilter.Any())
                return queryFilter.ToArray();

            //filtering by IDs
            filter = query.Select(c => c.Id.ToString()).Where(c => queryFilter.Contains(c)).ToList();
            queryFilter = queryFilter.Except(filter).ToArray();

            return queryFilter.ToArray();
        }

        public void InsertNewsCategory(NewsCategory newsCategory)
        {
            if (newsCategory == null)
                throw new ArgumentNullException(nameof(newsCategory));

            if (newsCategory is IEntityForCaching)
                throw new ArgumentException("Cacheable entities are not supported by Entity Framework");

            _newsCategoryRepository.Insert(newsCategory);

            //cache
            _cacheManager.RemoveByPrefix(NopNewsDefaults.NewsCategoriesPrefixCacheKey);
            _staticCacheManager.RemoveByPrefix(NopNewsDefaults.NewsCategoriesPrefixCacheKey);
            _cacheManager.RemoveByPrefix(NopNewsDefaults.NewsCategoryMappingsPrefixCacheKey);

            //event notification
            _eventPublisher.EntityInserted(newsCategory);
        }

        public void InsertNewsCategoryMapping(NewsCategoryMapping newsCategoryMapping)
        {
            if (newsCategoryMapping == null)
                throw new ArgumentNullException(nameof(newsCategoryMapping));

            _newsCategoryMappingRepository.Insert(newsCategoryMapping);

            //cache
            _cacheManager.RemoveByPrefix(NopNewsDefaults.NewsCategoryMappingsPrefixCacheKey);

            //event notification
            _eventPublisher.EntityInserted(newsCategoryMapping);
        }

        public IList<NewsCategory> SortNewsCategoriesForTree(IList<NewsCategory> source, int parentId = 0, bool ignoreNewsCategoriesWithoutExistingParent = false)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            var result = new List<NewsCategory>();

            foreach (var cat in source.Where(c => c.ParentCategoryId == parentId).ToList())
            {
                result.Add(cat);
                result.AddRange(SortNewsCategoriesForTree(source, cat.Id, true));
            }

            if (ignoreNewsCategoriesWithoutExistingParent || result.Count == source.Count)
                return result;

            //find categories without parent in provided category source and insert them into result
            foreach (var cat in source)
                if (result.FirstOrDefault(x => x.Id == cat.Id) == null)
                    result.Add(cat);

            return result;
        }

        public void UpdateCategory(NewsCategory newsCategory)
        {
            if (newsCategory == null)
                throw new ArgumentNullException(nameof(newsCategory));

            if (newsCategory is IEntityForCaching)
                throw new ArgumentException("Cacheable entities are not supported by Entity Framework");

            //validate category hierarchy
            var parentCategory = GetNewsCategoryById(newsCategory.ParentCategoryId);
            while (parentCategory != null)
            {
                if (newsCategory.Id == parentCategory.Id)
                {
                    newsCategory.ParentCategoryId = 0;
                    break;
                }

                parentCategory = GetNewsCategoryById(parentCategory.ParentCategoryId);
            }

            _newsCategoryRepository.Update(newsCategory);

            //cache
            _cacheManager.RemoveByPrefix(NopNewsDefaults.NewsCategoriesPrefixCacheKey);
            _staticCacheManager.RemoveByPrefix(NopNewsDefaults.NewsCategoriesPrefixCacheKey);
            _cacheManager.RemoveByPrefix(NopNewsDefaults.NewsCategoryMappingsPrefixCacheKey);

            //event notification
            _eventPublisher.EntityUpdated(newsCategory);
        }

        public void UpdateNewsCategoryMapping(NewsCategoryMapping newsCategoryMapping)
        {
            throw new NotImplementedException();
        }
        #endregion

    }
}
