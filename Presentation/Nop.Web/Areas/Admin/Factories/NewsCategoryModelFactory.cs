using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.News;
using Nop.Services.Localization;
using Nop.Services.News;
using Nop.Services.Seo;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Areas.Admin.Models.News;
using Nop.Web.Framework.Extensions;
using Nop.Web.Framework.Factories;
using Nop.Web.Framework.Models.Extensions;

namespace Nop.Web.Areas.Admin.Factories
{
    public class NewsCategoryModelFactory : INewsCategoryModelFactory
    {
        #region fields
        private readonly CatalogSettings _catalogSettings;
        private readonly INewsService _newsService;
        private readonly IAclSupportedModelFactory _aclSupportedModelFactory;
        private readonly IUrlRecordService _urlRecordService;
        private readonly IBaseAdminModelFactory _baseAdminModelFactory;
        private readonly INewsCategoryService _newsCategoryService;
        private readonly ILocalizationService _localizationService;
        private readonly ILocalizedModelFactory _localizedModelFactory;
        private readonly IStoreMappingSupportedModelFactory _storeMappingSupportedModelFactory;

        #endregion
        #region ctor
        public NewsCategoryModelFactory(INewsService newsService,
            IUrlRecordService urlRecordService,
            IBaseAdminModelFactory baseAdminModelFactory,
            INewsCategoryService newsCategoryService,
            ILocalizationService localizationService,
            CatalogSettings catalogSettings,
            ILocalizedModelFactory localizedModelFactory,
            IAclSupportedModelFactory aclSupportedModelFactory,
            IStoreMappingSupportedModelFactory storeMappingSupportedModelFactory)
        {
            _newsService = newsService;
            _urlRecordService = urlRecordService;
            _baseAdminModelFactory = baseAdminModelFactory;
            _newsCategoryService = newsCategoryService;
            _localizationService = localizationService;
            _catalogSettings = catalogSettings;
            _localizedModelFactory = localizedModelFactory;
            _aclSupportedModelFactory = aclSupportedModelFactory;
            _storeMappingSupportedModelFactory = storeMappingSupportedModelFactory;
        }
        #endregion
        #region Utilities
        protected virtual CategoryNewsSearchModel PrepareCategoryNewsSearchModel(CategoryNewsSearchModel searchModel, NewsCategory newscategory)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            if (newscategory == null)
                throw new ArgumentNullException(nameof(newscategory));

            searchModel.NewsCategoryId = newscategory.Id;

            //prepare page parameters
            searchModel.SetGridPageSize();

            return searchModel;
        }
        #endregion
        #region Methods
        public AddNewsToCategoryListModel PrepareAddNewsToCategoryListModel(AddNewsToCategorySearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //get products
            var result = _newsService.SearchNews(showHidden: true,
                categoryIds: new List<int> { searchModel.SearchNewsCategoryId },
                storeId: searchModel.SearchStoreId,
                keywords: searchModel.SearchNewsTitle,
                pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize);

            //prepare grid model
            var model = new AddNewsToCategoryListModel().PrepareToGrid(searchModel, result, () =>
            {
                return result.Select(news =>
                {
                    var productModel = news.ToModel<NewsItemModel>();
                    productModel.SeName = _urlRecordService.GetSeName(news, 0, true, false);

                    return productModel;
                });
            });

            return model;
        }

        public AddNewsToCategorySearchModel PrepareAddNewsToCategorySearchModel(AddNewsToCategorySearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //prepare available categories
            _baseAdminModelFactory.PrepareCategories(searchModel.AvailableCategories);
            //prepare available stores
            _baseAdminModelFactory.PrepareStores(searchModel.AvailableStores);

            //prepare page parameters
            searchModel.SetPopupGridPageSize();

            return searchModel;
        }

        public CategoryNewsListModel PrepareCategoryNewsListModel(CategoryNewsSearchModel searchModel, NewsCategory newsCategory)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            if (newsCategory == null)
                throw new ArgumentNullException(nameof(newsCategory));

            //get product categories
            var newsCategoryMappings = _newsCategoryService.GetNewsCategoryMappingsByCategoryId(newsCategory.Id,
                showHidden: true,
                pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize);

            //prepare grid model
            var model = new CategoryNewsListModel().PrepareToGrid(searchModel, newsCategoryMappings, () =>
            {
                return newsCategoryMappings.Select(newsCategoryMapping =>
                {
                    //fill in model values from the entity
                    var categoryNewsModel = newsCategoryMapping.ToModel<CategoryNewsModel>();

                    //fill in additional values (not existing in the entity)
                    categoryNewsModel.Title = _newsService.GetNewsById(newsCategoryMapping.NewsId)?.Title;

                    return categoryNewsModel;
                });
            });

            return model;
        }

        public NewsCategoryListModel PrepareNewsCategoryListModel(NewsCategorySearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //get categories
            var newscategories = _newsCategoryService.GetAllNewsCategories(newsCategoryName: searchModel.SearchNewsCategoryName,
                showHidden: true,
                storeId: searchModel.SearchStoreId,
                pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize);

            //prepare grid model
            var model = new NewsCategoryListModel().PrepareToGrid(searchModel, newscategories, () =>
            {
                return newscategories.Select(newscategory =>
                {
                    //fill in model values from the entity
                    var newsCategoryModel = newscategory.ToModel<NewsCategoryModel>();

                    //fill in additional values (not existing in the entity)
                    newsCategoryModel.Breadcrumb = _newsCategoryService.GetFormattedBreadCrumb(newscategory);
                    newsCategoryModel.SeName = _urlRecordService.GetSeName(newscategory, 0, true, false);

                    return newsCategoryModel;
                });
            });

            return model;
        }

        public NewsCategoryModel PrepareNewsCategoryModel(NewsCategoryModel model, NewsCategory newsCategory, bool excludeProperties = false)
        {
            Action<NewsCategoryLocalizedModel, int> localizedModelConfiguration = null;

            if (newsCategory != null)
            {
                //fill in model values from the entity
                if (model == null)
                {
                    model = newsCategory.ToModel<NewsCategoryModel>();
                    model.SeName = _urlRecordService.GetSeName(newsCategory, 0, true, false);
                }

                //prepare nested search model
                PrepareCategoryNewsSearchModel(model.CategoryNewsSearchModel, newsCategory);

                //define localized model configuration action
                localizedModelConfiguration = (locale, languageId) =>
                {
                    locale.Name = _localizationService.GetLocalized(newsCategory, entity => entity.Name, languageId, false, false);
                    locale.Description = _localizationService.GetLocalized(newsCategory, entity => entity.Description, languageId, false, false);
                    locale.MetaKeywords = _localizationService.GetLocalized(newsCategory, entity => entity.MetaKeywords, languageId, false, false);
                    locale.MetaDescription = _localizationService.GetLocalized(newsCategory, entity => entity.MetaDescription, languageId, false, false);
                    locale.MetaTitle = _localizationService.GetLocalized(newsCategory, entity => entity.MetaTitle, languageId, false, false);
                    locale.SeName = _urlRecordService.GetSeName(newsCategory, languageId, false, false);
                };
            }

            //set default values for the new model
            if (newsCategory == null)
            {
                model.PageSize = _catalogSettings.DefaultCategoryPageSize;
                model.PageSizeOptions = _catalogSettings.DefaultCategoryPageSizeOptions;
                model.Published = true;
                model.IncludeInTopMenu = true;
                model.AllowCustomersToSelectPageSize = true;
            }

            //prepare localized models
            if (!excludeProperties)
                model.Locales = _localizedModelFactory.PrepareLocalizedModels(localizedModelConfiguration);


            //prepare available parent categories
            _baseAdminModelFactory.PrepareNewsCategories(model.AvailableNewsCategories,
                defaultItemText: _localizationService.GetResource("Admin.Catalog.Categories.Fields.Parent.None"));

            //prepare model customer roles
            _aclSupportedModelFactory.PrepareModelCustomerRoles(model, newsCategory, excludeProperties);

            //prepare model stores
            _storeMappingSupportedModelFactory.PrepareModelStores(model, newsCategory, excludeProperties);

            return model;
        }

        public NewsCategorySearchModel PrepareNewsCategorySearchModel(NewsCategorySearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //prepare available stores
            _baseAdminModelFactory.PrepareStores(searchModel.AvailableStores);

            searchModel.HideStoresList = _catalogSettings.IgnoreStoreLimitations || searchModel.AvailableStores.SelectionIsNotPossible();
            //prepare page parameters
            searchModel.SetGridPageSize();
            return searchModel;
        }
        #endregion

    }
}
