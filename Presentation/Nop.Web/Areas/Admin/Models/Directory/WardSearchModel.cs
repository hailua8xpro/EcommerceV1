using Nop.Web.Framework.Models;

namespace Nop.Web.Areas.Admin.Models.Directory
{
    /// <summary>
    /// Represents a state and province search model
    /// </summary>
    public partial class WardSearchModel : BaseSearchModel
    {
        #region Properties

        public int DistrictId { get; set; }

        #endregion
    }
}