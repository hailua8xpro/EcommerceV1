using System;
using System.Collections.Generic;
using System.Linq;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Services.Directory;
using Nop.Services.Localization;
using Nop.Web.Infrastructure.Cache;
using Nop.Web.Models.Directory;

namespace Nop.Web.Factories
{
    /// <summary>
    /// Represents the country model factory
    /// </summary>
    public partial class CountryModelFactory : ICountryModelFactory
    {
        #region Fields

        private readonly ICountryService _countryService;
        private readonly ILocalizationService _localizationService;
        private readonly IStateProvinceService _stateProvinceService;
        private readonly IStaticCacheManager _cacheManager;
        private readonly IWorkContext _workContext;

        #endregion

        #region Ctor

        public CountryModelFactory(ICountryService countryService,

            ILocalizationService localizationService,
            IStateProvinceService stateProvinceService,
            IStaticCacheManager cacheManager,
            IWorkContext workContext)
        {
            _countryService = countryService;
            _localizationService = localizationService;
            _stateProvinceService = stateProvinceService;
            _cacheManager = cacheManager;
            _workContext = workContext;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Get states and provinces by country identifier
        /// </summary>
        /// <param name="countryId">Country identifier</param>
        /// <param name="addSelectStateItem">Whether to add "Select state" item to list of states</param>
        /// <returns>List of identifiers and names of states and provinces</returns>
        public virtual IList<StateProvinceModel> GetStatesByCountryId(string countryId, bool addSelectStateItem)
        {
            if (string.IsNullOrEmpty(countryId))
                throw new ArgumentNullException(nameof(countryId));

            var cacheKey = string.Format(NopModelCacheDefaults.StateProvincesByCountryModelKey, countryId, addSelectStateItem, _workContext.WorkingLanguage.Id);
            var cachedModel = _cacheManager.Get(cacheKey, () =>
            {
                var country = _countryService.GetCountryById(Convert.ToInt32(countryId));
                var states = _stateProvinceService.GetStateProvincesByCountryId(country != null ? country.Id : 0, _workContext.WorkingLanguage.Id).ToList();
                var result = new List<StateProvinceModel>();
                foreach (var state in states)
                    result.Add(new StateProvinceModel
                    {
                        id = state.Id,
                        name = _localizationService.GetLocalized(state, x => x.Name)
                    });

                if (country == null)
                {
                    //country is not selected ("choose country" item)
                    if (addSelectStateItem)
                    {
                        result.Insert(0, new StateProvinceModel
                        {
                            id = 0,
                            name = _localizationService.GetResource("Address.SelectState")
                        });
                    }
                    else
                    {
                        result.Insert(0, new StateProvinceModel
                        {
                            id = 0,
                            name = _localizationService.GetResource("Address.OtherNonUS")
                        });
                    }
                }
                else
                {
                    //some country is selected
                    if (!result.Any())
                    {
                        //country does not have states
                        result.Insert(0, new StateProvinceModel
                        {
                            id = 0,
                            name = _localizationService.GetResource("Address.OtherNonUS")
                        });
                    }
                    else
                    {
                        //country has some states
                        if (addSelectStateItem)
                        {
                            result.Insert(0, new StateProvinceModel
                            {
                                id = 0,
                                name = _localizationService.GetResource("Address.SelectState")
                            });
                        }
                    }
                }

                return result;
            });
            return cachedModel;
        }


        public IList<DistrictModel> GetDistrictsByStateId(string stateId)
        {
            if (string.IsNullOrEmpty(stateId))
                throw new ArgumentNullException(nameof(stateId));

            var cacheKey = string.Format(NopModelCacheDefaults.DistrictsByStateModelKey, stateId, _workContext.WorkingLanguage.Id);
            var cachedModel = _cacheManager.Get(cacheKey, () =>
            {
                var districts = _stateProvinceService.GetDistrictsByStateProvinceId(Convert.ToInt32(stateId));
                var result = new List<DistrictModel>();
                foreach (var state in districts)
                    result.Add(new DistrictModel
                    {
                        id = state.Id,
                        name = state.Name
                    });

                if (!result.Any())
                {
                    result.Insert(0, new DistrictModel
                    {
                        id = 0,
                        name = _localizationService.GetResource("Address.DataUpdating")
                    });
                }
                else
                {
                    result.Insert(0, new DistrictModel
                    {
                        id = 0,
                        name = _localizationService.GetResource("Address.SelectDistrict")
                    });
                }

                return result;
            });
            return cachedModel;
        }
        public IList<WardModel> GetWardsByDistrictId(string districtId)
        {
            if (string.IsNullOrEmpty(districtId))
                throw new ArgumentNullException(nameof(districtId));

            var cacheKey = string.Format(NopModelCacheDefaults.WardsByDistrictModelKey, districtId, _workContext.WorkingLanguage.Id);
            var cachedModel = _cacheManager.Get(cacheKey, () =>
            {
                var wards = _stateProvinceService.GetWardsByDistrictId(Convert.ToInt32(districtId));
                var result = new List<WardModel>();
                foreach (var state in wards)
                    result.Add(new WardModel
                    {
                        id = state.Id,
                        name = state.Name
                    });

                if (!result.Any())
                {
                    result.Insert(0, new WardModel
                    {
                        id = 0,
                        name = _localizationService.GetResource("Address.DataUpdating")
                    });
                }
                else
                {
                    result.Insert(0, new WardModel
                    {
                        id = 0,
                        name = _localizationService.GetResource("Address.SelectWard")
                    });
                }

                return result;
            });
            return cachedModel;
        }
        #endregion
    }
}
