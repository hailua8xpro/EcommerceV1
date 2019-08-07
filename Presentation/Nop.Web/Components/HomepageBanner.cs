using Microsoft.AspNetCore.Mvc;
using Nop.Core.Domain.Banners;
using Nop.Web.Factories;
using Nop.Web.Framework.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nop.Web.Components
{
    public class HomepageBanner: NopViewComponent
    {
        private readonly IBannerModelFactory _bannerModelFactory;
        public HomepageBanner(IBannerModelFactory bannerModelFactory)
        {
            _bannerModelFactory = bannerModelFactory;
        }
        public IViewComponentResult Invoke()
        {
            var model = _bannerModelFactory.PrepareBannerModel(0, (int)BannerType.Home, 0);
            return View(model);
        }
    }
}
