using Nop.Web.Models.Banner;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nop.Web.Factories
{
    public interface IBannerModelFactory
    {
        IList<BannerModel> PrepareBannerModel(int storeId, int type, int categoryId);
    }
}
