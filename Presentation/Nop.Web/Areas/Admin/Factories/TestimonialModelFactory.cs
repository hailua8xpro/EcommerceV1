using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Nop.Core.Domain.Testimonials;
using Nop.Services.Testimonials;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Areas.Admin.Models.Testimonials;
using Nop.Web.Framework.Models.Extensions;

namespace Nop.Web.Areas.Admin.Factories
{
    public class TestimonialModelFactory : ITestimonialModelFactory
    {
        #region Fields
        ITestimonialService _testimonialService;
        #endregion
        #region Ctor
        public TestimonialModelFactory(ITestimonialService testimonialService)
        {
            _testimonialService = testimonialService;
        }
        #endregion
        public TestimonialListModel PrepareTestimonialListModel(TestimonialSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            var testimonials = _testimonialService.GetAllTestimonials(searchModel.SearchTestimonialName, showHidden: true).ToPagedList(searchModel);

            //prepare list model
            var model = new TestimonialListModel().PrepareToGrid(searchModel, testimonials, () =>
            {
                //fill in model values from the entity
                return testimonials.Select(testimonial =>
                {
                    var testimonialModel = testimonial.ToModel<TestimonialModel>();
                    return testimonialModel;
                });
            });

            return model;
        }

        public TestimonialModel PrepareTestimonialModel(TestimonialModel model, Testimonial testimonial)
        {
            if (testimonial != null)
            {
                if (model == null)
                {
                    model = testimonial.ToModel<TestimonialModel>();
                }
            }
            if (testimonial == null)
            {
                model.Published = true;
            }
            return model;
        }

        public TestimonialSearchModel PrepareTestimonialSearchModel(TestimonialSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));
            searchModel.SetGridPageSize();

            return searchModel;
        }
    }
}
