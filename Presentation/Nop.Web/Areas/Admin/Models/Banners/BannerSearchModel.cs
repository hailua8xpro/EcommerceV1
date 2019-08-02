using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nop.Web.Areas.Admin.Models.Banners
{
    public class BannerSearchModel: BaseSearchModel
    {
        public BannerSearchModel()
        {
            AvailableStores = new List<SelectListItem>();
            AvailableBannerTypes=new List<SelectListItem>();
        }
        [NopResourceDisplayName("Admin.ContentManagement.Banner.List.SearchStore")]
        public int SearchStoreId { get; set; }
        public IList<SelectListItem> AvailableStores { get; set; }
        public IList<SelectListItem> AvailableBannerTypes { get; set; }

        public bool HideStoresList { get; set; }
    }
}
