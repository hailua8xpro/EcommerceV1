using Microsoft.AspNetCore.Mvc;
using Nop.Web.Factories;
using Nop.Web.Framework.Components;

namespace Nop.Web.Components
{
    public class NewsCategoryNavigationViewComponent : NopViewComponent
    {
        private readonly INewsModelFactory _newsModelFactory;

        public NewsCategoryNavigationViewComponent(INewsModelFactory newsModelFactory)
        {
            _newsModelFactory = newsModelFactory;
        }

        public IViewComponentResult Invoke(int currentCategoryId, int currentNewsId)
        {
            var model = _newsModelFactory.PrepareNewsCategoryNavigationModel(currentCategoryId, currentNewsId);
            return View(model);
        }
    }
}
