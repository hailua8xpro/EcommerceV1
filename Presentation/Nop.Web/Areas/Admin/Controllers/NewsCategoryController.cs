using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nop.Core.Domain.News;
using Nop.Services.Customers;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Media;
using Nop.Services.Messages;
using Nop.Services.News;
using Nop.Services.Security;
using Nop.Services.Seo;
using Nop.Services.Stores;
using Nop.Web.Areas.Admin.Factories;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Areas.Admin.Models.News;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Web.Areas.Admin.Controllers
{
    public class NewsCategoryController : BaseAdminController
    {
        #region Fields
        private readonly IAclService _aclService;
        private readonly IPermissionService _permissionService;
        private readonly INewsCategoryModelFactory _newsCategoryModelFactory;
        private readonly INewsCategoryService _newsCategoryService;
        private readonly INewsService _newsService;
        private readonly IUrlRecordService _urlRecordService;
        private readonly ILocalizedEntityService _localizedEntityService;
        private readonly IPictureService _pictureService;
        private readonly ICustomerService _customerService;
        private readonly IStoreMappingService _storeMappingService;
        private readonly IStoreService _storeService;
        private readonly ICustomerActivityService _customerActivityService;
        private readonly ILocalizationService _localizationService;
        private readonly INotificationService _notificationService;
        #endregion
        #region Ctor
        public NewsCategoryController(IPermissionService permissionService,
            INewsCategoryModelFactory newsCategoryModelFactory,
            INewsCategoryService newsCategoryService,
            IUrlRecordService urlRecordService,
            ILocalizedEntityService localizedEntityService,
            IPictureService pictureService,
            IAclService aclService,
            ICustomerService customerService,
            IStoreMappingService storeMappingService,
            IStoreService storeService,
            ICustomerActivityService customerActivityService,
            ILocalizationService localizationService,
            INotificationService notificationService,
            INewsService newsService)
        {
            _permissionService = permissionService;
            _newsCategoryModelFactory = newsCategoryModelFactory;
            _newsCategoryService = newsCategoryService;
            _urlRecordService = urlRecordService;
            _localizedEntityService = localizedEntityService;
            _pictureService = pictureService;
            _aclService = aclService;
            _customerService = customerService;
            _storeMappingService = storeMappingService;
            _storeService = storeService;
            _customerActivityService = customerActivityService;
            _localizationService = localizationService;
            _notificationService = notificationService;
            _newsService = newsService;
        }
        #endregion
        #region Utilities
        protected virtual void UpdateLocales(NewsCategory category, NewsCategoryModel model)
        {
            foreach (var localized in model.Locales)
            {
                _localizedEntityService.SaveLocalizedValue(category,
                    x => x.Name,
                    localized.Name,
                    localized.LanguageId);

                _localizedEntityService.SaveLocalizedValue(category,
                    x => x.Description,
                    localized.Description,
                    localized.LanguageId);

                _localizedEntityService.SaveLocalizedValue(category,
                    x => x.MetaKeywords,
                    localized.MetaKeywords,
                    localized.LanguageId);

                _localizedEntityService.SaveLocalizedValue(category,
                    x => x.MetaDescription,
                    localized.MetaDescription,
                    localized.LanguageId);

                _localizedEntityService.SaveLocalizedValue(category,
                    x => x.MetaTitle,
                    localized.MetaTitle,
                    localized.LanguageId);

                //search engine name
                var seName = _urlRecordService.ValidateSeName(category, localized.SeName, localized.Name, false);
                _urlRecordService.SaveSlug(category, seName, localized.LanguageId);
            }
        }
        protected virtual void UpdatePictureSeoNames(NewsCategory category)
        {
            var picture = _pictureService.GetPictureById(category.PictureId);
            if (picture != null)
                _pictureService.SetSeoFilename(picture.Id, _pictureService.GetPictureSeName(category.Name));
        }
        protected virtual void SaveCategoryAcl(NewsCategory category, NewsCategoryModel model)
        {
            category.SubjectToAcl = model.SelectedCustomerRoleIds.Any();

            var existingAclRecords = _aclService.GetAclRecords(category);
            var allCustomerRoles = _customerService.GetAllCustomerRoles(true);
            foreach (var customerRole in allCustomerRoles)
            {
                if (model.SelectedCustomerRoleIds.Contains(customerRole.Id))
                {
                    //new role
                    if (existingAclRecords.Count(acl => acl.CustomerRoleId == customerRole.Id) == 0)
                        _aclService.InsertAclRecord(category, customerRole.Id);
                }
                else
                {
                    //remove role
                    var aclRecordToDelete = existingAclRecords.FirstOrDefault(acl => acl.CustomerRoleId == customerRole.Id);
                    if (aclRecordToDelete != null)
                        _aclService.DeleteAclRecord(aclRecordToDelete);
                }
            }
        }
        protected virtual void SaveStoreMappings(NewsCategory category, NewsCategoryModel model)
        {
            category.LimitedToStores = model.SelectedStoreIds.Any();

            var existingStoreMappings = _storeMappingService.GetStoreMappings(category);
            var allStores = _storeService.GetAllStores();
            foreach (var store in allStores)
            {
                if (model.SelectedStoreIds.Contains(store.Id))
                {
                    //new store
                    if (existingStoreMappings.Count(sm => sm.StoreId == store.Id) == 0)
                        _storeMappingService.InsertStoreMapping(category, store.Id);
                }
                else
                {
                    //remove store
                    var storeMappingToDelete = existingStoreMappings.FirstOrDefault(sm => sm.StoreId == store.Id);
                    if (storeMappingToDelete != null)
                        _storeMappingService.DeleteStoreMapping(storeMappingToDelete);
                }
            }
        }

        #endregion
        #region List
        public IActionResult Index()
        {
            return RedirectToAction("List");
        }
        public virtual IActionResult List()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageNews))
                return AccessDeniedView();

            //prepare model
            var model = _newsCategoryModelFactory.PrepareNewsCategorySearchModel(new NewsCategorySearchModel());

            return View(model);
        }
        [HttpPost]
        public virtual IActionResult List(NewsCategorySearchModel searchModel)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageNews))
                return AccessDeniedDataTablesJson();

            //prepare model
            var model = _newsCategoryModelFactory.PrepareNewsCategoryListModel(searchModel);

            return Json(model);
        }
        #endregion
        #region Create / Edit / Delete
        public virtual IActionResult Create()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageNews))
                return AccessDeniedView();

            //prepare model
            var model = _newsCategoryModelFactory.PrepareNewsCategoryModel(new NewsCategoryModel(), null);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual IActionResult Create(NewsCategoryModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageNews))
                return AccessDeniedView();

            if (ModelState.IsValid)
            {
                var category = model.ToEntity<NewsCategory>();
                category.CreatedOnUtc = DateTime.UtcNow;
                category.UpdatedOnUtc = DateTime.UtcNow;
                _newsCategoryService.InsertNewsCategory(category);

                //search engine name
                model.SeName = _urlRecordService.ValidateSeName(category, model.SeName, category.Name, true);
                _urlRecordService.SaveSlug(category, model.SeName, 0);

                //locales
                UpdateLocales(category, model);

                _newsCategoryService.UpdateCategory(category);

                //update picture seo file name
                UpdatePictureSeoNames(category);

                //ACL (customer roles)
                SaveCategoryAcl(category, model);

                //stores
                SaveStoreMappings(category, model);

                //activity log
                _customerActivityService.InsertActivity("AddNewNewsCategory",
                    string.Format(_localizationService.GetResource("ActivityLog.AddNewNewsCategory"), category.Name), category);

                _notificationService.SuccessNotification(_localizationService.GetResource("Admin.ContentManagement.News.Category.Added"));

                if (!continueEditing)
                    return RedirectToAction("List");

                return RedirectToAction("Edit", new { id = category.Id });
            }

            //prepare model
            model = _newsCategoryModelFactory.PrepareNewsCategoryModel(model, null, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        public virtual IActionResult Edit(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageNews))
                return AccessDeniedView();

            //try to get a category with the specified id
            var category = _newsCategoryService.GetNewsCategoryById(id);
            if (category == null || category.Deleted)
                return RedirectToAction("List");

            //prepare model
            var model = _newsCategoryModelFactory.PrepareNewsCategoryModel(null, category);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual IActionResult Edit(NewsCategoryModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageNews))
                return AccessDeniedView();

            //try to get a category with the specified id
            var category = _newsCategoryService.GetNewsCategoryById(model.Id);
            if (category == null || category.Deleted)
                return RedirectToAction("List");

            if (ModelState.IsValid)
            {
                var prevPictureId = category.PictureId;

                category = model.ToEntity(category);
                category.UpdatedOnUtc = DateTime.UtcNow;
                _newsCategoryService.UpdateCategory(category);

                //search engine name
                model.SeName = _urlRecordService.ValidateSeName(category, model.SeName, category.Name, true);
                _urlRecordService.SaveSlug(category, model.SeName, 0);

                //locales
                UpdateLocales(category, model);
                _newsCategoryService.UpdateCategory(category);

                //delete an old picture (if deleted or updated)
                if (prevPictureId > 0 && prevPictureId != category.PictureId)
                {
                    var prevPicture = _pictureService.GetPictureById(prevPictureId);
                    if (prevPicture != null)
                        _pictureService.DeletePicture(prevPicture);
                }

                //update picture seo file name
                UpdatePictureSeoNames(category);

                //ACL
                SaveCategoryAcl(category, model);

                //stores
                SaveStoreMappings(category, model);

                //activity log
                _customerActivityService.InsertActivity("EditNewsCategory",
                    string.Format(_localizationService.GetResource("ActivityLog.EditNewsCategory"), category.Name), category);

                _notificationService.SuccessNotification(_localizationService.GetResource("Admin.ContentManagement.News.Category.Updated"));

                if (!continueEditing)
                    return RedirectToAction("List");

                return RedirectToAction("Edit", new { id = category.Id });
            }

            //prepare model
            model = _newsCategoryModelFactory.PrepareNewsCategoryModel(model, category, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpPost]
        public virtual IActionResult Delete(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageNews))
                return AccessDeniedView();

            //try to get a category with the specified id
            var category = _newsCategoryService.GetNewsCategoryById(id);
            if (category == null)
                return RedirectToAction("List");

            _newsCategoryService.DeleteNewsCategory(category);

            //activity log
            _customerActivityService.InsertActivity("DeleteNewsCategory",
                string.Format(_localizationService.GetResource("ActivityLog.DeleteNewsCategory"), category.Name), category);

            _notificationService.SuccessNotification(_localizationService.GetResource("Admin.ContentManagement.News.Category.Deleted"));

            return RedirectToAction("List");
        }
        #endregion
        #region News
        [HttpPost]
        public virtual IActionResult NewsList(CategoryNewsSearchModel searchModel)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageNews))
                return AccessDeniedDataTablesJson();

            //try to get a category with the specified id
            var category = _newsCategoryService.GetNewsCategoryById(searchModel.NewsCategoryId)
                ?? throw new ArgumentException("No category found with the specified id");

            //prepare model
            var model = _newsCategoryModelFactory.PrepareCategoryNewsListModel(searchModel, category);

            return Json(model);
        }

        public virtual IActionResult NewsUpdate(CategoryNewsModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageNews))
                return AccessDeniedView();

            //try to get a product category with the specified id
            var newsCategoryMapping = _newsCategoryService.GetNewsCategoryMappingById(model.Id)
                ?? throw new ArgumentException("No news category mapping found with the specified id");

            //fill entity from product
            newsCategoryMapping = model.ToEntity(newsCategoryMapping);
            _newsCategoryService.UpdateNewsCategoryMapping(newsCategoryMapping);

            return new NullJsonResult();
        }

        public virtual IActionResult NewsDelete(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageNews))
                return AccessDeniedView();

            //try to get a product category with the specified id
            var productCategory = _newsCategoryService.GetNewsCategoryMappingById(id)
                ?? throw new ArgumentException("No product category mapping found with the specified id", nameof(id));

            _newsCategoryService.DeleteNewsCategoryMapping(productCategory);

            return new NullJsonResult();
        }

        public virtual IActionResult NewsAddPopup(int categoryId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageNews))
                return AccessDeniedView();

            //prepare model
            var model = _newsCategoryModelFactory.PrepareAddNewsToCategorySearchModel(new AddNewsToCategorySearchModel());

            return View(model);
        }

        [HttpPost]
        public virtual IActionResult NewsAddPopupList(AddNewsToCategorySearchModel searchModel)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageNews))
                return AccessDeniedDataTablesJson();

            //prepare model
            var model = _newsCategoryModelFactory.PrepareAddNewsToCategoryListModel(searchModel);

            return Json(model);
        }

        [HttpPost]
        [FormValueRequired("save")]
        public virtual IActionResult NewsAddPopup(AddNewsToCategoryModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageNews))
                return AccessDeniedView();

            //get selected products
            var selectedProducts = _newsService.GetNewsByIds(model.SelectedNewsIds.ToArray());
            if (selectedProducts.Any())
            {
                var existingProductCategories = _newsCategoryService.GetNewsCategoryMappingsByCategoryId(model.NewsCategoryId, showHidden: true);
                foreach (var product in selectedProducts)
                {
                    //whether product category with such parameters already exists
                    if (_newsCategoryService.FindNewsCategoryMapping(existingProductCategories, product.Id, model.NewsCategoryId) != null)
                        continue;

                    //insert the new product category mapping
                    _newsCategoryService.InsertNewsCategoryMapping(new NewsCategoryMapping
                    {
                        NewsCategoryId = model.NewsCategoryId,
                        NewsId = product.Id,
                    });
                }
            }

            ViewBag.RefreshPage = true;

            return View(new AddNewsToCategorySearchModel());
        }
        #endregion

    }
}