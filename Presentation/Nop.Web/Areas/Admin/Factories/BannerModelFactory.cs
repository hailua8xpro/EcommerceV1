using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Nop.Core.Domain.Banners;
using Nop.Core.Domain.Catalog;
using Nop.Services.Banners;
using Nop.Services.Media;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Areas.Admin.Models.Banners;
using Nop.Web.Framework.Extensions;
using Nop.Web.Framework.Factories;
using Nop.Web.Framework.Models.Extensions;
using Nop.Web.Models.Media;

namespace Nop.Web.Areas.Admin.Factories
{
    public class BannerModelFactory : IBannerModelFactory
    {
        #region Fields
        private readonly IBaseAdminModelFactory _baseAdminModelFactory;
        private readonly IBannerService _bannerService;
        private readonly IPictureService _pictureService;
        
        private readonly CatalogSettings _catalogSettings;
        private readonly IStoreMappingSupportedModelFactory _storeMappingSupportedModelFactory;
        #endregion
        #region Ctor
        public BannerModelFactory(IBaseAdminModelFactory baseAdminModelFactory,
            CatalogSettings catalogSettings,
            IBannerService bannerService,
            IStoreMappingSupportedModelFactory storeMappingSupportedModelFactory,
            IPictureService pictureService
            )
        {
            _baseAdminModelFactory = baseAdminModelFactory;
            _catalogSettings = catalogSettings;
            _bannerService = bannerService;
            _storeMappingSupportedModelFactory = storeMappingSupportedModelFactory;
            _pictureService = pictureService;
        }
        #endregion
        #region Method
        public BannerListModel PrepareBannerListModel(BannerSearchModel searchModel)
        {

            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //get countries
            var banners = _bannerService.GetAllBanners(showHidden: true).ToPagedList(searchModel);

            //prepare list model
            var model = new BannerListModel().PrepareToGrid(searchModel, banners, () =>
            {
                //fill in model values from the entity
                return banners.Select(banner =>
                {
                    var bannerModel = banner.ToModel<BannerModel>();
                    var picture = _pictureService.GetPictureById(bannerModel.PictureId);
                    var pictureModel = new PictureModel
                    {
                        FullSizeImageUrl = _pictureService.GetPictureUrl(picture),
                        ImageUrl = _pictureService.GetPictureUrl(picture, 150),
                    };
                    bannerModel.PictureModel = pictureModel;
                    return bannerModel;
                });
            });

            return model;
        }

        public BannerModel PrepareBannerModel(BannerModel model, Banner banner)
        {
            if (banner!=null)
            {
                if (model == null)
                {
                    model = banner.ToModel<BannerModel>();
                }
            }
            if (banner == null)
            {
                model.Published = true;
            }
            _storeMappingSupportedModelFactory.PrepareModelStores(model, banner, true);

            return model;
        }

        public BannerSearchModel PrepareBannerSearchModel(BannerSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //prepare available stores
            _baseAdminModelFactory.PrepareStores(searchModel.AvailableStores);

            searchModel.HideStoresList = _catalogSettings.IgnoreStoreLimitations || searchModel.AvailableStores.SelectionIsNotPossible();
            _baseAdminModelFactory.PrepareBannerTypes(searchModel.AvailableBannerTypes);

            //prepare page parameters
            searchModel.SetGridPageSize();

            return searchModel;
        }
        #endregion

    }
}
