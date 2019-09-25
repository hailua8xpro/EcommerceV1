using System;
using System.Collections.Generic;
using System.Linq;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Media;
using Nop.Core.Domain.News;
using Nop.Core.Domain.Security;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Media;
using Nop.Services.News;
using Nop.Services.Seo;
using Nop.Web.Infrastructure.Cache;
using Nop.Web.Models.Media;
using Nop.Web.Models.News;

namespace Nop.Web.Factories
{
    /// <summary>
    /// Represents the news model factory
    /// </summary>
    public partial class NewsModelFactory : INewsModelFactory
    {
        #region Fields

        private readonly CaptchaSettings _captchaSettings;
        private readonly CustomerSettings _customerSettings;
        private readonly ICustomerService _customerService;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly INewsService _newsService;
        private readonly INewsCategoryService _newsCategoryService;
        private readonly IPictureService _pictureService;
        private readonly IStaticCacheManager _cacheManager;
        private readonly IStoreContext _storeContext;
        private readonly IUrlRecordService _urlRecordService;
        private readonly IWorkContext _workContext;
        private readonly MediaSettings _mediaSettings;
        private readonly NewsSettings _newsSettings;
        private readonly ILocalizationService _localizationService;


        #endregion

        #region Ctor

        public NewsModelFactory(CaptchaSettings captchaSettings,
            CustomerSettings customerSettings,
            ICustomerService customerService,
            IDateTimeHelper dateTimeHelper,
            IGenericAttributeService genericAttributeService,
            INewsService newsService,
            INewsCategoryService newsCategoryService,
            IPictureService pictureService,
            IStaticCacheManager cacheManager,
            IStoreContext storeContext,
            IUrlRecordService urlRecordService,
            IWorkContext workContext,
            MediaSettings mediaSettings,
            NewsSettings newsSettings,
            ILocalizationService localizationService)
        {
            _captchaSettings = captchaSettings;
            _customerSettings = customerSettings;
            _customerService = customerService;
            _dateTimeHelper = dateTimeHelper;
            _genericAttributeService = genericAttributeService;
            _newsService = newsService;
            _pictureService = pictureService;
            _cacheManager = cacheManager;
            _storeContext = storeContext;
            _urlRecordService = urlRecordService;
            _workContext = workContext;
            _mediaSettings = mediaSettings;
            _newsSettings = newsSettings;
            _newsCategoryService = newsCategoryService;
            _localizationService = localizationService;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Prepare the news comment model
        /// </summary>
        /// <param name="newsComment">News comment</param>
        /// <returns>News comment model</returns>
        public virtual NewsCommentModel PrepareNewsCommentModel(NewsComment newsComment)
        {
            if (newsComment == null)
                throw new ArgumentNullException(nameof(newsComment));

            var model = new NewsCommentModel
            {
                Id = newsComment.Id,
                CustomerId = newsComment.CustomerId,
                CustomerName = _customerService.FormatUsername(newsComment.Customer),
                CommentTitle = newsComment.CommentTitle,
                CommentText = newsComment.CommentText,
                CreatedOn = _dateTimeHelper.ConvertToUserTime(newsComment.CreatedOnUtc, DateTimeKind.Utc),
                AllowViewingProfiles = _customerSettings.AllowViewingProfiles && newsComment.Customer != null && !newsComment.Customer.IsGuest(),
            };
            if (_customerSettings.AllowCustomersToUploadAvatars)
            {
                model.CustomerAvatarUrl = _pictureService.GetPictureUrl(
                    _genericAttributeService.GetAttribute<int>(newsComment.Customer, NopCustomerDefaults.AvatarPictureIdAttribute),
                    _mediaSettings.AvatarPictureSize, _customerSettings.DefaultAvatarEnabled, defaultPictureType: PictureType.Avatar);
            }

            return model;
        }

        /// <summary>
        /// Prepare the news item model
        /// </summary>
        /// <param name="model">News item model</param>
        /// <param name="newsItem">News item</param>
        /// <param name="prepareComments">Whether to prepare news comment models</param>
        /// <returns>News item model</returns>
        public virtual NewsItemModel PrepareNewsItemModel(NewsItemModel model, NewsItem newsItem, bool prepareComments, bool prepareBreadcrumb = false)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            if (newsItem == null)
                throw new ArgumentNullException(nameof(newsItem));

            model.Id = newsItem.Id;
            model.MetaTitle = newsItem.MetaTitle;
            model.MetaDescription = newsItem.MetaDescription;
            model.MetaKeywords = newsItem.MetaKeywords;
            model.SeName = _urlRecordService.GetSeName(newsItem, newsItem.LanguageId, ensureTwoPublishedLanguages: false);
            model.Title = newsItem.Title;
            model.Short = newsItem.Short;
            model.Full = newsItem.Full;
            model.AllowComments = newsItem.AllowComments;
            model.CreatedOn = _dateTimeHelper.ConvertToUserTime(newsItem.StartDateUtc ?? newsItem.CreatedOnUtc, DateTimeKind.Utc);
            model.AddNewComment.DisplayCaptcha = _captchaSettings.Enabled && _captchaSettings.ShowOnNewsCommentPage;
            if (prepareBreadcrumb)
            {
                model.Breadcrumb = PrepareNewsItemBreadcrumbModel(newsItem);
            }
            var picture = _pictureService.GetPictureById(newsItem.PictureId);
            var pictureModel = new PictureModel
            {
                FullSizeImageUrl = _pictureService.GetPictureUrl(picture),
                ImageUrl = _pictureService.GetPictureUrl(picture, 370),
                Title = newsItem.Title,
                AlternateText = newsItem.Title
            };
            model.PictureModel = pictureModel;
            //number of news comments
            var storeId = _newsSettings.ShowNewsCommentsPerStore ? _storeContext.CurrentStore.Id : 0;
            var cacheKey = string.Format(NopModelCacheDefaults.NewsCommentsNumberKey, newsItem.Id, storeId, true);
            model.NumberOfComments = _cacheManager.Get(cacheKey, () => _newsService.GetNewsCommentsCount(newsItem, storeId, true));

            if (prepareComments)
            {
                var newsComments = newsItem.NewsComments.Where(comment => comment.IsApproved);
                if (_newsSettings.ShowNewsCommentsPerStore)
                    newsComments = newsComments.Where(comment => comment.StoreId == _storeContext.CurrentStore.Id);

                foreach (var nc in newsComments.OrderBy(comment => comment.CreatedOnUtc))
                {
                    var commentModel = PrepareNewsCommentModel(nc);
                    model.Comments.Add(commentModel);
                }
            }

            return model;
        }

        /// <summary>
        /// Prepare the home page news items model
        /// </summary>
        /// <returns>Home page news items model</returns>
        public virtual HomepageNewsItemsModel PrepareHomepageNewsItemsModel()
        {
            var cacheKey = string.Format(NopModelCacheDefaults.HomepageNewsModelKey, _workContext.WorkingLanguage.Id, _storeContext.CurrentStore.Id);
            var cachedModel = _cacheManager.Get(cacheKey, () =>
            {
                var newsItems = _newsService.GetAllNews(_workContext.WorkingLanguage.Id, _storeContext.CurrentStore.Id, 0, _newsSettings.MainPageNewsCount);
                return new HomepageNewsItemsModel
                {
                    WorkingLanguageId = _workContext.WorkingLanguage.Id,
                    NewsItems = newsItems
                        .Select(x =>
                        {
                            var newsModel = new NewsItemModel();
                            PrepareNewsItemModel(newsModel, x, false);
                            return newsModel;
                        })
                        .ToList()
                };
            });

            //"Comments" property of "NewsItemModel" object depends on the current customer.
            //Furthermore, we just don't need it for home page news. So let's reset it.
            //But first we need to clone the cached model (the updated one should not be cached)
            var model = (HomepageNewsItemsModel)cachedModel.Clone();
            foreach (var newsItemModel in model.NewsItems)
                newsItemModel.Comments.Clear();
            return model;
        }

        /// <summary>
        /// Prepare the news item list model
        /// </summary>
        /// <param name="command">News paging filtering model</param>
        /// <returns>News item list model</returns>
        public virtual NewsItemListModel PrepareNewsItemListModel(NewsPagingFilteringModel command)
        {
            var model = new NewsItemListModel
            {
                WorkingLanguageId = _workContext.WorkingLanguage.Id
            };

            if (command.PageSize <= 0) command.PageSize = _newsSettings.NewsArchivePageSize;
            if (command.PageNumber <= 0) command.PageNumber = 1;

            var newsItems = _newsService.GetAllNews(_workContext.WorkingLanguage.Id, _storeContext.CurrentStore.Id,
                command.PageNumber - 1, command.PageSize);
            model.PagingFilteringContext.LoadPagedList(newsItems);

            model.NewsItems = newsItems
                .Select(x =>
                {
                    var newsModel = new NewsItemModel();
                    PrepareNewsItemModel(newsModel, x, false);
                    return newsModel;
                })
                .ToList();

            return model;
        }
        public virtual NewsCategoryNavigationModel PrepareNewsCategoryNavigationModel(int currentNewsCategoryId, int currentNewsId)
        {
            //get active category
            var activeNewsCategoryId = 0;
            if (currentNewsCategoryId > 0)
            {
                //category details page
                activeNewsCategoryId = currentNewsCategoryId;
            }
            else if (currentNewsCategoryId > 0)
            {
                //product details page
                var newsCategoryMappings = _newsCategoryService.GetNewsCategoryMappingsByNewsId(currentNewsId);
                if (newsCategoryMappings.Any())
                    activeNewsCategoryId = newsCategoryMappings[0].NewsCategoryId;
            }

            var cachedCategoriesModel = PrepareNewsCategorySimpleModels();
            var model = new NewsCategoryNavigationModel
            {
                CurrentNewsCategoryId = activeNewsCategoryId,
                NewsCategories = cachedCategoriesModel
            };

            return model;
        }

        public virtual List<NewsCategoryModel> PrepareNewsCategorySimpleModels()
        {
            //load and cache them
            var cacheKey = string.Format(NopModelCacheDefaults.NewsCategoryAllModelKey,
                _workContext.WorkingLanguage.Id,
                string.Join(",", _workContext.CurrentCustomer.GetCustomerRoleIds()),
                _storeContext.CurrentStore.Id);
            return _cacheManager.Get(cacheKey, () => PrepareNewsCategorySimpleModels(0));
        }

        /// <summary>
        /// Prepare category (simple) models
        /// </summary>
        /// <param name="rootCategoryId">Root category identifier</param>
        /// <param name="loadSubCategories">A value indicating whether subcategories should be loaded</param>
        /// <returns>List of category (simple) models</returns>
        public virtual List<NewsCategoryModel> PrepareNewsCategorySimpleModels(int rootNewsCategoryId, bool loadNewsSubCategories = true)
        {
            var result = new List<NewsCategoryModel>();

            //little hack for performance optimization
            //we know that this method is used to load top and left menu for categories.
            //it'll load all categories anyway.
            //so there's no need to invoke "GetAllCategoriesByParentCategoryId" multiple times (extra SQL commands) to load childs
            //so we load all categories at once (we know they are cached)
            var allCategories = _newsCategoryService.GetAllNewsCategories(storeId: _storeContext.CurrentStore.Id);
            var categories = allCategories.Where(c => c.ParentCategoryId == rootNewsCategoryId).ToList();
            foreach (var category in categories)
            {
                var categoryModel = new NewsCategoryModel
                {
                    Id = category.Id,
                    Name = _localizationService.GetLocalized(category, x => x.Name),
                    SeName = _urlRecordService.GetSeName(category),
                };

                //number of products in each category

                if (loadNewsSubCategories)
                {
                    var subCategories = PrepareNewsCategorySimpleModels(category.Id, loadNewsSubCategories);
                    categoryModel.SubCategories.AddRange(subCategories);
                }

                categoryModel.HaveSubCategories = categoryModel.SubCategories.Count > 0;

                result.Add(categoryModel);
            }

            return result;
        }
        public NewsCategoryModel PrepareNewsCategoryModel(NewsCategory category, NewsPagingFilteringModel command)
        {
            if (category == null)
                throw new ArgumentNullException(nameof(category));

            var model = new NewsCategoryModel
            {
                Id = category.Id,
                Name = _localizationService.GetLocalized(category, x => x.Name),
                Description = _localizationService.GetLocalized(category, x => x.Description),
                MetaKeywords = _localizationService.GetLocalized(category, x => x.MetaKeywords),
                MetaDescription = _localizationService.GetLocalized(category, x => x.MetaDescription),
                MetaTitle = _localizationService.GetLocalized(category, x => x.MetaTitle),
                SeName = _urlRecordService.GetSeName(category),
            };

            if (command == null)
                throw new ArgumentNullException(nameof(command));

            if (command.PageSize <= 0) command.PageSize = _newsSettings.NewsArchivePageSize;
            if (command.PageNumber <= 0) command.PageNumber = 1;

            var breadcrumbCacheKey = string.Format(NopModelCacheDefaults.CategoryBreadcrumbKey,
                category.Id,
                string.Join(",", _workContext.CurrentCustomer.GetCustomerRoleIds()),
                _storeContext.CurrentStore.Id,
                _workContext.WorkingLanguage.Id);
            model.NewsCategoryBreadcrumb = _cacheManager.Get(breadcrumbCacheKey, () =>
                _newsCategoryService.GetNewsCategoryBreadCrumb(category).Select(catBr => new NewsCategoryModel
                {
                    Id = catBr.Id,
                    Name = _localizationService.GetLocalized(catBr, x => x.Name),
                    SeName = _urlRecordService.GetSeName(catBr)
                })
                .ToList()
            );
            var pictureSize = _mediaSettings.CategoryThumbPictureSize;

            //subcategories
            var subCategoriesCacheKey = string.Format(NopModelCacheDefaults.NewsCategorySubcategoriesKey,
                category.Id,
                pictureSize,
                string.Join(",", _workContext.CurrentCustomer.GetCustomerRoleIds()),
                _storeContext.CurrentStore.Id,
                _workContext.WorkingLanguage.Id);
            model.SubCategories = _cacheManager.Get(subCategoriesCacheKey, () =>
                _newsCategoryService.GetAllNewsCategoriesByParentCategoryId(category.Id)
                .Select(x =>
                {
                    var subCatModel = new NewsCategoryModel
                    {
                        Id = x.Id,
                        Name = _localizationService.GetLocalized(x, y => y.Name),
                        SeName = _urlRecordService.GetSeName(x),
                        Description = _localizationService.GetLocalized(x, y => y.Description)
                    };
                    return subCatModel;
                })
                .ToList()
            );
            var categoryIds = new List<int>();
            categoryIds.Add(category.Id);
            categoryIds.AddRange(_newsCategoryService.GetChildNewsCategoryIds(category.Id, _storeContext.CurrentStore.Id));
            var newslist = _newsService.SearchNews(
                categoryIds: categoryIds,
                storeId: _storeContext.CurrentStore.Id,
                pageIndex: command.PageNumber - 1,
                pageSize: command.PageSize);
            model.PagingFilteringContext.LoadPagedList(newslist);
            model.NewsItems = newslist
            .Select(x =>
            {
                var newsModel = new NewsItemModel();
                PrepareNewsItemModel(newsModel, x, false);
                return newsModel;
            })
            .ToList();
            return model;
        }
        public NewsSearchModel PrepareSearchModel(NewsSearchModel model, NewsPagingFilteringModel command)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            var searchTerms = model.q;
            if (searchTerms == null)
                searchTerms = "";
            if (command.PageSize <= 0) command.PageSize = _newsSettings.NewsArchivePageSize;
            if (command.PageNumber <= 0) command.PageNumber = 1;
            searchTerms = searchTerms.Trim();
            var newslist = _newsService.SearchNews(keywords: searchTerms, pageIndex: command.PageNumber - 1, pageSize: command.PageSize);
            model.PagingContext.LoadPagedList(newslist);
            model.NewsItems = newslist.Select(x =>
             {
                 var newsModel = new NewsItemModel();
                 PrepareNewsItemModel(newsModel, x, false);
                 return newsModel;

             }).ToList();
            return model;
        }
        protected virtual NewsItemModel.NewsItemBreadcrumbModel PrepareNewsItemBreadcrumbModel(NewsItem newsItem)
        {
            if (newsItem == null)
                throw new ArgumentNullException(nameof(newsItem));

            var cacheKey = string.Format(NopModelCacheDefaults.NewsItemBreadcrumbModelKey,
                    newsItem.Id,
                    _workContext.WorkingLanguage.Id,
                    string.Join(",", _workContext.CurrentCustomer.GetCustomerRoleIds()),
                    _storeContext.CurrentStore.Id);
            var cachedModel = _cacheManager.Get(cacheKey, () =>
            {
                var breadcrumbModel = new NewsItemModel.NewsItemBreadcrumbModel
                {
                    NewsId = newsItem.Id,
                    Title = newsItem.Title,
                    SeName = _urlRecordService.GetSeName(newsItem)
                };
                var newsCategories = _newsCategoryService.GetNewsCategoryMappingsByNewsId(newsItem.Id);
                if (!newsCategories.Any())
                    return breadcrumbModel;

                var category = newsCategories[0].NewsCategory;
                if (category == null)
                    return breadcrumbModel;

                foreach (var catBr in _newsCategoryService.GetNewsCategoryBreadCrumb(category))
                {
                    breadcrumbModel.NewsCategoryBreadcrumb.Add(new NewsCategoryModel
                    {
                        Id = catBr.Id,
                        Name = _localizationService.GetLocalized(catBr, x => x.Name),
                        SeName = _urlRecordService.GetSeName(catBr)
                    });
                }

                return breadcrumbModel;
            });
            return cachedModel;
        }
        #endregion
    }
}