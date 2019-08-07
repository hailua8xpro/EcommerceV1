using Microsoft.AspNetCore.Mvc;
using Nop.Web.Factories;
using Nop.Web.Framework.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nop.Web.Components
{
    public class HomepageTestimonialsViewComponent : NopViewComponent
    {
        ITestimonialModelFactory _testimonialModelFactory;
        public HomepageTestimonialsViewComponent(ITestimonialModelFactory testimonialModelFactory)
        {
            _testimonialModelFactory = testimonialModelFactory;
        }
        public IViewComponentResult Invoke()
        {
            var model = _testimonialModelFactory.PrepareHomepageTestimonialsModel();
            return View(model);
        }
    }
}
