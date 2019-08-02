using System.Collections.Generic;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Web.Areas.Admin.Models.Directory
{
    public class DistrictModel: BaseNopEntityModel
    {
        public DistrictModel()
        {
            WardSearchModel = new WardSearchModel();
        }
        public int StateProvinceId { get; set; }
        [NopResourceDisplayName("Admin.Configuration.Countries.States.Fields.Name")]
        public string Name { get; set; }
        [NopResourceDisplayName("Admin.Configuration.Countries.States.Fields.Published")]
        public bool Published { get; set; }
        [NopResourceDisplayName("Admin.Configuration.Countries.States.Fields.DisplayOrder")]
        public int DisplayOrder { get; set; }
        public WardSearchModel WardSearchModel { get; set; }

    }
}
