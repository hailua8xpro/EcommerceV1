using System.Collections.Generic;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Web.Areas.Admin.Models.Directory
{
    public class WardModel : BaseNopEntityModel
    {
        public int DistrictId { get; set; }
        [NopResourceDisplayName("Admin.Configuration.Countries.States.Fields.Name")]
        public string Name { get; set; }
        [NopResourceDisplayName("Admin.Configuration.Countries.States.Fields.Published")]
        public bool Published { get; set; }
        [NopResourceDisplayName("Admin.Configuration.Countries.States.Fields.DisplayOrder")]
        public int DisplayOrder { get; set; }
    }
}
