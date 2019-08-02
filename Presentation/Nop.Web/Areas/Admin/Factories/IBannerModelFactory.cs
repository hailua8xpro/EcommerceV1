﻿using Nop.Core.Domain.Banners;
using Nop.Web.Areas.Admin.Models.Banners;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nop.Web.Areas.Admin.Factories
{
    public interface IBannerModelFactory
    {
        BannerSearchModel PrepareBannerSearchModel(BannerSearchModel searchModel);
        BannerListModel PrepareBannerListModel(BannerSearchModel searchModel);
        BannerModel PrepareBannerModel(BannerModel model, Banner banner);
    }
}