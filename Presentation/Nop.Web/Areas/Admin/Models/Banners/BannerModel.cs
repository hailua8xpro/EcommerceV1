using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;
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
        [NopResourceDisplayName("Admin.ContentManagement.Banners.Fields.Type")]
        public int Type { get; set; }
        public string Url { get; set; }
        [NopResourceDisplayName("Admin.ContentManagement.Banners.Fields.Title")]
        public string Title { get; set; }
        [NopResourceDisplayName("Admin.ContentManagement.Banners.Fields.Caption")]
        public string Caption { get; set; }
        public bool LimitedToStores { get; set; }
        [NopResourceDisplayName("Admin.ContentManagement.Banners.Fields.Published")]
        public bool Published { get; set; }
        [NopResourceDisplayName("Admin.ContentManagement.Banners.Fields.ShowCaption")]
        public bool ShowCaption { get; set; }
        [NopResourceDisplayName("Admin.ContentManagement.Banners.Fields.DisplayOrder")]
        public int DisplayOrder { get; set; }
        public DateTime CreatedOnUtc { get; set; }
        public DateTime UpdatedOnUtc { get; set; }
        [NopResourceDisplayName("Admin.ContentManagement.Banners.Fields.CategoryId")]
        public int CategoryId { get; set; }
        [UIHint("Picture")]
        [NopResourceDisplayName("Admin.ContentManagement.Banners.Fields.PictureId")]
        public int PictureId { get; set; }
        [NopResourceDisplayName("Admin.ContentManagement.Banners.Fields.SelectedStoreIds")]
        public IList<int> SelectedStoreIds { get; set; }
        public IList<SelectListItem> AvailableStores { get; set; }
        public PictureModel PictureModel { get; set; }
    }
}
