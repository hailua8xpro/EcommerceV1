using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Nop.Services.Media;
using Nop.Services.Testimonials;
using Nop.Web.Models.Media;
using Nop.Web.Models.Testimonial;

namespace Nop.Web.Factories
{
    public class TestimonialModelFactory : ITestimonialModelFactory
    {
        private readonly ITestimonialService _testimonialService;
        private readonly IPictureService _pictureService;
        public TestimonialModelFactory(ITestimonialService testimonialService,
            IPictureService pictureService)
        {
            _testimonialService = testimonialService;
            _pictureService = pictureService;
        }
        public List<TestimonialModel> PrepareHomepageTestimonialsModel()
        {
            var testimonials=_testimonialService.GetAllTestimonials(string.Empty).Select(testimonial=> {
                var testimonialmodel = new TestimonialModel {
                    Description=testimonial.Description,
                    FullDescription=testimonial.FullDescription,
                    FullName=testimonial.FullName,
                    
                };
                var picture = _pictureService.GetPictureById(testimonial.PictureId);
                var pictureModel = new PictureModel
                {
                    ImageUrl=_pictureService.GetPictureUrl(picture, 90),
                };
                testimonialmodel.PictureModel = pictureModel;
                return testimonialmodel;
            }).ToList();
            return testimonials;
        }
    }
}
