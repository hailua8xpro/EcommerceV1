using Nop.Core.Caching;
using Nop.Services.Banners;
using Nop.Services.Media;
using Nop.Web.Infrastructure.Cache;
using Nop.Web.Models.Banner;
using Nop.Web.Models.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nop.Web.Factories
{
    public class BannerModelFactory: IBannerModelFactory
    {
        #region Fields
        private readonly IStaticCacheManager _cacheManager;
        private readonly IBannerService _bannerService;
        private readonly IPictureService _pictureService;
        #endregion
        #region Ctor
        public BannerModelFactory(IStaticCacheManager cacheManager,
            IBannerService bannerService,
            IPictureService pictureService)
        {
            _cacheManager = cacheManager;
            _bannerService = bannerService;
            _pictureService = pictureService;
        }
        #endregion
        #region Method
        public IList<BannerModel> PrepareBannerModel(int storeId, int type, int categoryId)
        {
            var cacheKey = string.Format(NopModelCacheDefaults.BannerModelKey,type, categoryId);
            var cacheModel = _cacheManager.Get(cacheKey, () => {
                var banners = _bannerService.GetAllBanners(storeId, type, categoryId);
                return banners.Select(b =>
                {
                    var picture = _pictureService.GetPictureById(b.PictureId);
                    var bannerModel = new BannerModel
                    {
                        Caption = b.Caption,
                        Url = b.Url,
                        Title = b.Title,
                        ShowCaption = b.ShowCaption,
                        ImageUrl= _pictureService.GetPictureUrl(picture)
                    };
                    return bannerModel;
                }).ToList();
            });
            return cacheModel;
        }
        #endregion
    }
}
