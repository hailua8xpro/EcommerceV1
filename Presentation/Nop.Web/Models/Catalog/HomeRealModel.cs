using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nop.Web.Models.Catalog
{
    public class HomeRealModel
    {
        public CategoryModel CategoryModel { get; set; }
        public IEnumerable<ProductOverviewModel> Products { get; set; }
        public IEnumerable<CategoryModel> ChildCategory { get; set; }
    }
}
