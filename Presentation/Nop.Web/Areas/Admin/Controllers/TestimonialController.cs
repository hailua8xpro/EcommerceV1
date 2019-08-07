using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Media;
using Nop.Services.Messages;
using Nop.Services.Security;
using Nop.Services.Stores;
using Nop.Web.Areas.Admin.Factories;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Framework.Mvc.Filters;
using Nop.Web.Areas.Admin.Models.Testimonials;
using Nop.Services.Testimonials;
using Nop.Core.Domain.Testimonials;

namespace Nop.Web.Areas.Admin.Controllers
{
    public class TestimonialController : BaseAdminController
    {
        #region Fields
        private readonly IPermissionService _permissionService;
        private readonly ITestimonialService _testimonialService;
        private readonly IPictureService _pictureService;
        private readonly ITestimonialModelFactory _testimonialModelFactory;
        private readonly ICustomerActivityService _customerActivityService;
        private readonly ILocalizationService _localizationService;
        private readonly INotificationService _notificationService;
        #endregion
        #region ctor
        public TestimonialController(IPermissionService permissionService,
            ITestimonialService testimonialService,
            IPictureService pictureService,
            ITestimonialModelFactory testimonialModelFactory,
            ICustomerActivityService customerActivityService,
            ILocalizationService localizationService,
            INotificationService notificationService)
        {
            _permissionService = permissionService;
            _testimonialService = testimonialService;
            _pictureService = pictureService;
            _testimonialModelFactory = testimonialModelFactory;
            _customerActivityService = customerActivityService;
            _localizationService = localizationService;
            _notificationService = notificationService;
        }
        #endregion
        #region Method
        protected virtual void UpdatePictureSeoNames(Testimonial testimonial)
        {
            var picture = _pictureService.GetPictureById(testimonial.PictureId);
            if (picture != null)
                _pictureService.SetSeoFilename(picture.Id, _pictureService.GetPictureSeName(testimonial.Description));
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
            var model = _testimonialModelFactory.PrepareTestimonialSearchModel(new TestimonialSearchModel());

            return View(model);
        }
        [HttpPost]
        public virtual IActionResult List(TestimonialSearchModel searchModel)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.AccessAdminPanel))
                return AccessDeniedDataTablesJson();

            //prepare model
            var model = _testimonialModelFactory.PrepareTestimonialListModel(searchModel);

            return Json(model);
        }
        public virtual IActionResult Create()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.AccessAdminPanel))
                return AccessDeniedView();

            //prepare model
            var model = _testimonialModelFactory.PrepareTestimonialModel(new TestimonialModel(), null);

            return View(model);
        }
        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual IActionResult Create(TestimonialModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.AccessAdminPanel))
                return AccessDeniedView();

            if (ModelState.IsValid)
            {
                var testimonial = model.ToEntity<Testimonial>();
                testimonial.CreatedOnUtc = DateTime.UtcNow;
                testimonial.UpdatedOnUtc = DateTime.UtcNow;
                _testimonialService.InsertTestimonial(testimonial);
                //update picture seo file name
                UpdatePictureSeoNames(testimonial);
                //activity log
                _customerActivityService.InsertActivity("AddNewTestimonial",
                    string.Format(_localizationService.GetResource("ActivityLog.AddNewTestimonial"), testimonial.Description), testimonial);

                _notificationService.SuccessNotification(_localizationService.GetResource("Admin.ContentManagement.Testimonials.Added"));

                if (!continueEditing)
                    return RedirectToAction("List");

                return RedirectToAction("Edit", new { id = testimonial.Id });
            }

            //prepare model
            model = _testimonialModelFactory.PrepareTestimonialModel(model, null);

            //if we got this far, something failed, redisplay form
            return View(model);
        }
        public virtual IActionResult Edit(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.AccessAdminPanel))
                return AccessDeniedView();

            //try to get a category with the specified id
            var testimonial = _testimonialService.GetTestimonialById(id);
            if (testimonial == null)
                return RedirectToAction("List");

            //prepare model
            var model = _testimonialModelFactory.PrepareTestimonialModel(null, testimonial);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual IActionResult Edit(TestimonialModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.AccessAdminPanel))
                return AccessDeniedView();

            //try to get a category with the specified id
            var Testimonial = _testimonialService.GetTestimonialById(model.Id);
            if (Testimonial == null)
                return RedirectToAction("List");

            if (ModelState.IsValid)
            {
                var prevPictureId = Testimonial.PictureId;

                Testimonial = model.ToEntity(Testimonial);
                Testimonial.UpdatedOnUtc = DateTime.UtcNow;
                _testimonialService.UpdateTestimonial(Testimonial);

                //delete an old picture (if deleted or updated)
                if (prevPictureId > 0 && prevPictureId != Testimonial.PictureId)
                {
                    var prevPicture = _pictureService.GetPictureById(prevPictureId);
                    if (prevPicture != null)
                        _pictureService.DeletePicture(prevPicture);
                }

                //update picture seo file name
                UpdatePictureSeoNames(Testimonial);
                //activity log
                _customerActivityService.InsertActivity("EditTestimonial",
                    string.Format(_localizationService.GetResource("ActivityLog.EditTestimonial"), Testimonial.Description), Testimonial);

                _notificationService.SuccessNotification(_localizationService.GetResource("Admin.ContentManagement.Testimonials.Updated"));

                if (!continueEditing)
                    return RedirectToAction("List");

                return RedirectToAction("Edit", new { id = Testimonial.Id });
            }

            //prepare model
            model = _testimonialModelFactory.PrepareTestimonialModel(model, Testimonial);

            //if we got this far, something failed, redisplay form
            return View(model);
        }
        [HttpPost]
        public virtual IActionResult Delete(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.AccessAdminPanel))
                return AccessDeniedView();

            //try to get a manufacturer with the specified id
            var testimonial = _testimonialService.GetTestimonialById(id);
            if (testimonial == null)
                return RedirectToAction("List");

            _testimonialService.DeleteTestimonial(testimonial);

            //activity log
            _customerActivityService.InsertActivity("DeleteTestimonial",
                string.Format(_localizationService.GetResource("ActivityLog.DeleteTestimonial"), testimonial.Description), testimonial);

            _notificationService.SuccessNotification(_localizationService.GetResource("Admin.ContentManagement.Testimonials.Deleted"));

            return RedirectToAction("List");
        }
        #endregion

    }
}