using System;
using System.Collections.Generic;
using System.Linq;
using Nop.Core.Caching;
using Nop.Core.Data;
using Nop.Core.Domain.Directory;
using Nop.Services.Events;
using Nop.Services.Localization;

namespace Nop.Services.Directory
{
    /// <summary>
    /// State province service
    /// </summary>
    public partial class StateProvinceService : IStateProvinceService
    {
        #region Fields

        private readonly ICacheManager _cacheManager;
        private readonly IEventPublisher _eventPublisher;
        private readonly ILocalizationService _localizationService;
        private readonly IRepository<StateProvince> _stateProvinceRepository;
        private readonly IRepository<District> _districtRepository;
        private readonly IRepository<Ward> _wardRepository;

        #endregion

        #region Ctor

        public StateProvinceService(ICacheManager cacheManager,
            IEventPublisher eventPublisher,
            ILocalizationService localizationService,
            IRepository<StateProvince> stateProvinceRepository,
            IRepository<District> districtRepository,
            IRepository<Ward> wardRepository)
        {
            _cacheManager = cacheManager;
            _eventPublisher = eventPublisher;
            _localizationService = localizationService;
            _stateProvinceRepository = stateProvinceRepository;
            _districtRepository = districtRepository;
            _wardRepository = wardRepository;
        }

        #endregion

        #region Methods
        /// <summary>
        /// Deletes a state/province
        /// </summary>
        /// <param name="stateProvince">The state/province</param>
        public virtual void DeleteStateProvince(StateProvince stateProvince)
        {
            if (stateProvince == null)
                throw new ArgumentNullException(nameof(stateProvince));

            _stateProvinceRepository.Delete(stateProvince);

            _cacheManager.RemoveByPrefix(NopDirectoryDefaults.StateProvincesPrefixCacheKey);

            //event notification
            _eventPublisher.EntityDeleted(stateProvince);
        }

        /// <summary>
        /// Gets a state/province
        /// </summary>
        /// <param name="stateProvinceId">The state/province identifier</param>
        /// <returns>State/province</returns>
        public virtual StateProvince GetStateProvinceById(int stateProvinceId)
        {
            if (stateProvinceId == 0)
                return null;

            return _stateProvinceRepository.GetById(stateProvinceId);
        }

        /// <summary>
        /// Gets a state/province by abbreviation
        /// </summary>
        /// <param name="abbreviation">The state/province abbreviation</param>
        /// <param name="countryId">Country identifier; pass null to load the state regardless of a country</param>
        /// <returns>State/province</returns>
        public virtual StateProvince GetStateProvinceByAbbreviation(string abbreviation, int? countryId = null)
        {
            if (string.IsNullOrEmpty(abbreviation))
                return null;

            var key = string.Format(NopDirectoryDefaults.StateProvincesByAbbreviationCacheKey, abbreviation, countryId.HasValue ? countryId.Value : 0);
            return _cacheManager.Get(key, () =>
            {
                var query = _stateProvinceRepository.Table.Where(state => state.Abbreviation == abbreviation);

                //filter by country
                if (countryId.HasValue)
                    query = query.Where(state => state.CountryId == countryId);

                return query.FirstOrDefault();
            });
        }

        /// <summary>
        /// Gets a state/province collection by country identifier
        /// </summary>
        /// <param name="countryId">Country identifier</param>
        /// <param name="languageId">Language identifier. It's used to sort states by localized names (if specified); pass 0 to skip it</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>States</returns>
        public virtual IList<StateProvince> GetStateProvincesByCountryId(int countryId, int languageId = 0, bool showHidden = false)
        {
            var key = string.Format(NopDirectoryDefaults.StateProvincesAllCacheKey, countryId, languageId, showHidden);
            return _cacheManager.Get(key, () =>
            {
                var query = from sp in _stateProvinceRepository.Table
                            orderby sp.DisplayOrder, sp.Name
                            where sp.CountryId == countryId &&
                            (showHidden || sp.Published)
                            select sp;
                var stateProvinces = query.ToList();

                if (languageId > 0)
                {
                    //we should sort states by localized names when they have the same display order
                    stateProvinces = stateProvinces
                        .OrderBy(c => c.DisplayOrder)
                        .ThenBy(c => _localizationService.GetLocalized(c, x => x.Name, languageId))
                        .ToList();
                }

                return stateProvinces;
            });
        }

        /// <summary>
        /// Gets all states/provinces
        /// </summary>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>States</returns>
        public virtual IList<StateProvince> GetStateProvinces(bool showHidden = false)
        {
            var query = from sp in _stateProvinceRepository.Table
                        orderby sp.CountryId, sp.DisplayOrder, sp.Name
                        where showHidden || sp.Published
                        select sp;
            var stateProvinces = query.ToList();
            return stateProvinces;
        }

        /// <summary>
        /// Inserts a state/province
        /// </summary>
        /// <param name="stateProvince">State/province</param>
        public virtual void InsertStateProvince(StateProvince stateProvince)
        {
            if (stateProvince == null)
                throw new ArgumentNullException(nameof(stateProvince));

            _stateProvinceRepository.Insert(stateProvince);

            _cacheManager.RemoveByPrefix(NopDirectoryDefaults.StateProvincesPrefixCacheKey);

            //event notification
            _eventPublisher.EntityInserted(stateProvince);
        }

        /// <summary>
        /// Updates a state/province
        /// </summary>
        /// <param name="stateProvince">State/province</param>
        public virtual void UpdateStateProvince(StateProvince stateProvince)
        {
            if (stateProvince == null)
                throw new ArgumentNullException(nameof(stateProvince));

            _stateProvinceRepository.Update(stateProvince);

            _cacheManager.RemoveByPrefix(NopDirectoryDefaults.StateProvincesPrefixCacheKey);

            //event notification
            _eventPublisher.EntityUpdated(stateProvince);
        }
        #region District
       public void DeleteDistrict(District district)
        {
            if (district == null)
                throw new ArgumentNullException(nameof(district));

            _districtRepository.Delete(district);

            _cacheManager.RemoveByPrefix(NopDirectoryDefaults.DistrictsPrefixCacheKey);

            //event notification
            _eventPublisher.EntityDeleted(district);
        }
        public District GetDistrictById(int districtId) {
            if (districtId == 0)
                return null;

            return _districtRepository.GetById(districtId);
        }
        public IList<District> GetDistricts(bool showHidden = false) {
            var query = from sp in _districtRepository.Table
                        orderby sp.StateProvinceId, sp.DisplayOrder, sp.Name
                        where showHidden || sp.Published
                        select sp;
            var districts = query.ToList();
            return districts;
        }
        public void InsertDistrict(District district) {
            if (district == null)
                throw new ArgumentNullException(nameof(district));

            _districtRepository.Insert(district);

            _cacheManager.RemoveByPrefix(NopDirectoryDefaults.DistrictsPrefixCacheKey);

            //event notification
            _eventPublisher.EntityInserted(district);
        }
        public void UpdateDistrict(District district) {
            if (district == null)
                throw new ArgumentNullException(nameof(district));

            _districtRepository.Update(district);

            _cacheManager.RemoveByPrefix(NopDirectoryDefaults.DistrictsPrefixCacheKey);

            //event notification
            _eventPublisher.EntityUpdated(district);
        }
        public IList<District> GetDistrictsByStateProvinceId(int stateProvinceId, bool showHidden = false) {
            var key = string.Format(NopDirectoryDefaults.DistrictAllCacheKey, stateProvinceId,showHidden);
            return _cacheManager.Get(key, () =>
            {
                var query = from sp in _districtRepository.Table
                            orderby sp.DisplayOrder, sp.Name
                            where sp.StateProvinceId == stateProvinceId &&
                            (showHidden || sp.Published)
                            select sp;
                var stateProvinces = query.ToList();
                return stateProvinces;
            });
        }
        #endregion
        #region Ward
        public void DeleteWard(Ward ward)
        {
            if (ward == null)
                throw new ArgumentNullException(nameof(Ward));

            _wardRepository.Delete(ward);

            _cacheManager.RemoveByPrefix(NopDirectoryDefaults.WardsPrefixCacheKey);

            //event notification
            _eventPublisher.EntityDeleted(ward);
        }
        public Ward GetWardById(int wardId)
        {
            if (wardId == 0)
                return null;

            return _wardRepository.GetById(wardId);
        }
        public IList<Ward> GetWards(bool showHidden = false)
        {
            var query = from sp in _wardRepository.Table
                        orderby sp.DistrictId, sp.DisplayOrder, sp.Name
                        where showHidden || sp.Published
                        select sp;
            var wards = query.ToList();
            return wards;
        }
        public void InsertWard(Ward Ward)
        {
            if (Ward == null)
                throw new ArgumentNullException(nameof(Ward));

            _wardRepository.Insert(Ward);

            _cacheManager.RemoveByPrefix(NopDirectoryDefaults.WardsPrefixCacheKey);

            //event notification
            _eventPublisher.EntityInserted(Ward);
        }
        public void UpdateWard(Ward Ward)
        {
            if (Ward == null)
                throw new ArgumentNullException(nameof(Ward));

            _wardRepository.Update(Ward);

            _cacheManager.RemoveByPrefix(NopDirectoryDefaults.WardsPrefixCacheKey);

            //event notification
            _eventPublisher.EntityUpdated(Ward);
        }
        public IList<Ward> GetWardsByDistrictId(int districtId, bool showHidden = false)
        {
            var key = string.Format(NopDirectoryDefaults.WardAllCacheKey, districtId, showHidden);
            return _cacheManager.Get(key, () =>
            {
                var query = from sp in _wardRepository.Table
                            orderby sp.DisplayOrder, sp.Name
                            where sp.DistrictId == districtId &&
                            (showHidden || sp.Published)
                            select sp;
                var districts = query.ToList();
                return districts;
            });
        }
        #endregion
        #region RealAddress

        #endregion
        #endregion
    }
}