﻿using Nop.Web.Framework.Models;

namespace Nop.Web.Areas.Admin.Models.Directory
{
    /// <summary>
    /// Represents a state and province search model
    /// </summary>
    public partial class DistrictSearchModel : BaseSearchModel
    {
        #region Properties

        public int StateProvinceId { get; set; }

        #endregion
    }
}