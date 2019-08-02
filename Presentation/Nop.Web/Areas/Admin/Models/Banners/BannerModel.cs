using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Models;
using Nop.Web.Models.Media;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Nop.Web.Areas.Admin.Models.Banners
{
    public class BannerModel : BaseNopEntityModel, IStoreMappingSupportedModel
    {
        public BannerModel()
        {
            SelectedStoreIds = new List<int>();
            AvailableStores = new List<SelectListItem>();
        }
        public int Type { get; set; }
        public string Url { get; set; }
        public string Title { get; set; }
        public string Caption { get; set; }
        public bool LimitedToStores { get; set; }
        public bool Published { get; set; }
        public bool ShowCaption { get; set; }
        public int DisplayOrder { get; set; }
        public DateTime CreatedOnUtc { get; set; }
        public DateTime UpdatedOnUtc { get; set; }
        public int CategoryId { get; set; }
        [UIHint("Picture")]
        public int PictureId { get; set; }
        public IList<int> SelectedStoreIds { get; set; }
        public IList<SelectListItem> AvailableStores { get; set; }
        public PictureModel PictureModel { get; set; }
    }
}
