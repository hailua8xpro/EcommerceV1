using Nop.Web.Models.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nop.Web.Models.Testimonial
{
    public class TestimonialModel
    {
        public string FullName { get; set; }
        public string Description { get; set; }
        public string FullDescription { get; set; }
        public PictureModel PictureModel { get; set; }

    }
}
