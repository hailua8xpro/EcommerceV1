using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nop.Core.Domain.Banners;
using Nop.Services.Banners;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Media;
using Nop.Services.Messages;
using Nop.Services.Security;
using Nop.Services.Stores;
using Nop.Web.Areas.Admin.Factories;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Areas.Admin.Models.Banners;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Web.Areas.Admin.Controllers
{
    public class BannerController : BaseAdminController
    {
        #region Fields
        private readonly IPermissionService _permissionService;
        private readonly IBannerService _bannerService;
        private readonly IPictureService _pictureService;
        private readonly IStoreMappingService _storeMappingService;
        private readonly IStoreService _storeService;
        private readonly IBannerModelFactory _bannerModelFactory;
        private readonly ICustomerActivityService _customerActivityService;
        private readonly ILocalizationService _localizationService;
        private readonly INotificationService _notificationService;
        #endregion
        #region Ctor
        public BannerController(IPermissionService permissionService,
            IBannerModelFactory bannerModelFactory,
            IBannerService bannerService,
            IPictureService pictureService,
            IStoreMappingService storeMappingService,
            IStoreService storeService,
            ICustomerActivityService customerActivityService,
            ILocalizationService localizationService,
            INotificationService notificationService
            )
        {
            _permissionService = permissionService;
            _bannerModelFactory = bannerModelFactory;
            _bannerService = bannerService;
            _pictureService = pictureService;
            _storeMappingService = storeMappingService;
            _storeService = storeService;
            _customerActivityService = customerActivityService;
            _localizationService = localizationService;
            _notificationService = notificationService;
        }
        #endregion
        #region Method
        protected virtual void UpdatePictureSeoNames(Banner banner)
        {
            var picture = _pictureService.GetPictureById(banner.PictureId);
            if (picture != null)
                _pictureService.SetSeoFilename(picture.Id, _pictureService.GetPictureSeName(banner.Title));
        }
        protected virtual void SaveStoreMappings(Banner banner, BannerModel model)
        {
            banner.LimitedToStores = model.SelectedStoreIds.Any();

            var existingStoreMappings = _storeMappingService.GetStoreMappings(banner);
            var allStores = _storeService.GetAllStores();
            foreach (var store in allStores)
            {
                if (model.SelectedStoreIds.Contains(store.Id))
                {
                    //new store
                    if (existingStoreMappings.Count(sm => sm.StoreId == store.Id) == 0)
                        _storeMappingService.InsertStoreMapping(banner, store.Id);
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
        public virtual IActionResult Index()
        {
            return RedirectToAction("List");
        }

        public virtual IActionResult List()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.AccessAdminPanel))
                return AccessDeniedView();

            //prepare model
            var model = _bannerModelFactory.PrepareBannerSearchModel(new BannerSearchModel());

            return View(model);
        }
        [HttpPost]
        public virtual IActionResult List(BannerSearchModel searchModel)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.AccessAdminPanel))
                return AccessDeniedDataTablesJson();

            //prepare model
            var model = _bannerModelFactory.PrepareBannerListModel(searchModel);

            return Json(model);
        }
        public virtual IActionResult Create()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.AccessAdminPanel))
                return AccessDeniedView();

            //prepare model
            var model = _bannerModelFactory.PrepareBannerModel(new BannerModel(), null);

            return View(model);
        }
        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual IActionResult Create(BannerModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.AccessAdminPanel))
                return AccessDeniedView();

            if (ModelState.IsValid)
            {
                var banner = model.ToEntity<Banner>();
                banner.CreatedOnUtc = DateTime.UtcNow;
                banner.UpdatedOnUtc = DateTime.UtcNow;
                _bannerService.InsertBanner(banner);
                //update picture seo file name
                UpdatePictureSeoNames(banner);
                //stores
                SaveStoreMappings(banner, model);

                //activity log
                _customerActivityService.InsertActivity("AddNewBanner",
                    string.Format(_localizationService.GetResource("ActivityLog.AddNewBanner"), banner.Title), banner);

                _notificationService.SuccessNotification(_localizationService.GetResource("Admin.ContentManagement.Banners.Added"));

                if (!continueEditing)
                    return RedirectToAction("List");

                return RedirectToAction("Edit", new { id = banner.Id });
            }

            //prepare model
            model = _bannerModelFactory.PrepareBannerModel(model, null);

            //if we got this far, something failed, redisplay form
            return View(model);
        }
        public virtual IActionResult Edit(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.AccessAdminPanel))
                return AccessDeniedView();

            //try to get a category with the specified id
            var banner = _bannerService.GetBannerById(id);
            if (banner == null)
                return RedirectToAction("List");

            //prepare model
            var model = _bannerModelFactory.PrepareBannerModel(null, banner);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual IActionResult Edit(BannerModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.AccessAdminPanel))
                return AccessDeniedView();

            //try to get a category with the specified id
            var banner = _bannerService.GetBannerById(model.Id);
            if (banner == null)
                return RedirectToAction("List");

            if (ModelState.IsValid)
            {
                var prevPictureId = banner.PictureId;

                banner = model.ToEntity(banner);
                banner.UpdatedOnUtc = DateTime.UtcNow;
                _bannerService.UpdateBanner(banner);

                //delete an old picture (if deleted or updated)
                if (prevPictureId > 0 && prevPictureId != banner.PictureId)
                {
                    var prevPicture = _pictureService.GetPictureById(prevPictureId);
                    if (prevPicture != null)
                        _pictureService.DeletePicture(prevPicture);
                }

                //update picture seo file name
                UpdatePictureSeoNames(banner);
                //stores
                SaveStoreMappings(banner, model);

                //activity log
                _customerActivityService.InsertActivity("EditBanner",
                    string.Format(_localizationService.GetResource("ActivityLog.EditBanner"), banner.Title), banner);

                _notificationService.SuccessNotification(_localizationService.GetResource("Admin.ContentManagement.Banners.Updated"));

                if (!continueEditing)
                    return RedirectToAction("List");

                return RedirectToAction("Edit", new { id = banner.Id });
            }

            //prepare model
            model = _bannerModelFactory.PrepareBannerModel(model, banner);

            //if we got this far, something failed, redisplay form
            return View(model);
        }
        #endregion

    }
}