using Microsoft.AspNetCore.Mvc;
using Nop.Web.Factories;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Web.Controllers
{
    public partial class CountryController : BasePublicController
	{
        #region Fields

        private readonly ICountryModelFactory _countryModelFactory;
        
        #endregion
        
        #region Ctor

        public CountryController(ICountryModelFactory countryModelFactory)
		{
            _countryModelFactory = countryModelFactory;
		}
        
        #endregion
        
        #region States / provinces

        //available even when navigation is not allowed
        [CheckAccessPublicStore(true)]
        public virtual IActionResult GetStatesByCountryId(string countryId, bool addSelectStateItem)
        {
            var model = _countryModelFactory.GetStatesByCountryId(countryId, addSelectStateItem);
            return Json(model);
        }
        public virtual IActionResult GetDistrictsByStateId(string stateId)
        {
            var model = _countryModelFactory.GetDistrictsByStateId(stateId);
            return Json(model);
        }
        public virtual IActionResult GetWardsByDistrictId(string districtId)
        {
            var model = _countryModelFactory.GetWardsByDistrictId(districtId);
            return Json(model);
        }
        
      
        #endregion
    }
}