using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Core.Domain.Testimonials
{
    public class Testimonial: BaseEntity
    {
        public string FullName { get; set; }
        public string Description { get; set; }
        public string FullDescription { get; set; }
        public int PictureId { get; set; }
        public bool Deleted { get; set; }
        public bool Published { get; set; }
        public int DisplayOrder { get; set; }
        public DateTime CreatedOnUtc { get; set; }
        public DateTime UpdatedOnUtc { get; set; }
    }
}
