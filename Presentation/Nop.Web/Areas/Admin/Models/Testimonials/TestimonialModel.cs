using Nop.Web.Framework.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Nop.Web.Areas.Admin.Models.Testimonials
{
    public class TestimonialModel: BaseNopEntityModel
    {
        public string FullName { get; set; }
        public string Description { get; set; }
        public string FullDescription { get; set; }
        [UIHint("Picture")]
        public int PictureId { get; set; }
        public bool Deleted { get; set; }
        public bool Published { get; set; }
        public int DisplayOrder { get; set; }
    }
}
