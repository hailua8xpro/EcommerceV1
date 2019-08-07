using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nop.Web.Models.Banner
{
    public class BannerModel
    {
        public int Type { get; set; }
        public string Url { get; set; }
        public string Title { get; set; }
        public string Caption { get; set; }
        public bool ShowCaption { get; set; }
        public string ImageUrl { get; set; }

    }
}
