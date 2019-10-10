using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Nop.Web.Areas.Admin.Models.Testimonials
{
    public class TestimonialModel: BaseNopEntityModel
    {
        [NopResourceDisplayName("Admin.ContentManagement.Testimonials.Fields.FullName")]
        public string FullName { get; set; }
        [NopResourceDisplayName("Admin.ContentManagement.Testimonials.Fields.Description")]
        public string Description { get; set; }
        [NopResourceDisplayName("Admin.ContentManagement.Testimonials.Fields.FullDescription")]
        public string FullDescription { get; set; }
        [NopResourceDisplayName("Admin.ContentManagement.Testimonials.Fields.PictureId")]
        [UIHint("Picture")]
        public int PictureId { get; set; }
        [NopResourceDisplayName("Admin.ContentManagement.Testimonials.Fields.Deleted")]
        public bool Deleted { get; set; }
        [NopResourceDisplayName("Admin.ContentManagement.Testimonials.Fields.Published")]
        public bool Published { get; set; }
        [NopResourceDisplayName("Admin.ContentManagement.Testimonials.Fields.DisplayOrder")]
        public int DisplayOrder { get; set; }
    }
}
