using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Nop.Web.Factories;
using Nop.Web.Framework.Components;

namespace Nop.Web.Components
{
    public class HomepageProductsViewComponent : NopViewComponent
    {
        private readonly IProductModelFactory _productModelFactory;

        public HomepageProductsViewComponent(IProductModelFactory productModelFactory)
        {
            _productModelFactory = productModelFactory;
        }

        public IViewComponentResult Invoke()
        {
            var model = _productModelFactory.PrepareHomePageProductModel().ToList();
            return View(model);
        }
    }
}