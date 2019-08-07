using Nop.Web.Models.Testimonial;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nop.Web.Factories
{
   public interface ITestimonialModelFactory
    {
        List<TestimonialModel> PrepareHomepageTestimonialsModel();
    }
}
