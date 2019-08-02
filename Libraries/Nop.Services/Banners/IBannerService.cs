using Nop.Core;
using Nop.Core.Domain.Banners;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Services.Banners
{
   public interface IBannerService
    {
        void DeleteBanner(Banner Banner);
        IPagedList<Banner> GetAllBanners(int storeId = 0, int type = 0, int categoryId = 0,
          int pageIndex = 0, int pageSize = int.MaxValue, bool showHidden = false);
        Banner GetBannerById(int bannerId);
        void InsertBanner(Banner banner);
        void UpdateBanner(Banner banner);

    }
}
