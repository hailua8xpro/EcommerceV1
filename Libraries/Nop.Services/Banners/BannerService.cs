﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nop.Core;
using Nop.Core.Data;
using Nop.Core.Domain.Banners;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Stores;
using Nop.Services.Events;

namespace Nop.Services.Banners
{
    public class BannerService : IBannerService
    {
        #region Fields
        private readonly IRepository<Banner> _bannerRepository;
        private readonly IEventPublisher _eventPublisher;
        private readonly CatalogSettings _catalogSettings;
        private readonly IRepository<StoreMapping> _storeMappingRepository;

        #endregion
        #region Ctor
        public BannerService(IRepository<Banner> bannerRepository,
            IEventPublisher eventPublisher,
            CatalogSettings catalogSettings,
            IRepository<StoreMapping> storeMappingRepository)
        {
            _bannerRepository = bannerRepository;
            _eventPublisher = eventPublisher;
            _catalogSettings = catalogSettings;
            _storeMappingRepository = storeMappingRepository;
        }
        #endregion
        public void DeleteBanner(Banner banner)
        {
            if (banner == null)
                throw new ArgumentNullException(nameof(banner));
            _bannerRepository.Delete(banner);
            //event notification
            _eventPublisher.EntityDeleted(banner);
        }


        public IPagedList<Banner> GetAllBanners(int storeId = 0, int type = 0, int categoryId = 0, int pageIndex = 0, int pageSize = int.MaxValue, bool showHidden = false)
        {
            var query = _bannerRepository.Table;
            if (type > 0)
            {
                query = query.Where(b => b.Type == type);
            }
            if (categoryId > 0)
            {
                query = query.Where(b => b.CategoryId == categoryId);
            }
            if (!showHidden)
            {
                query = query.Where(b => b.Published);
            }

            if (storeId > 0 && !_catalogSettings.IgnoreStoreLimitations)
            {
                //Store mapping
                query = from bp in query
                        join sm in _storeMappingRepository.Table
                        on new { c1 = bp.Id, c2 = nameof(Banner) } equals new { c1 = sm.EntityId, c2 = sm.EntityName } into bp_sm
                        from sm in bp_sm.DefaultIfEmpty()
                        where !bp.LimitedToStores || storeId == sm.StoreId
                        select bp;

                query = query.Distinct();
            }

            query = query.OrderByDescending(b => b.CreatedOnUtc);

            var banners = new PagedList<Banner>(query, pageIndex, pageSize);
            return banners;
        }

        public Banner GetBannerById(int bannerId)
        {
            if (bannerId == 0)
                return null;
            return _bannerRepository.GetById(bannerId);
        }

        public void InsertBanner(Banner banner)
        {
            if (banner == null)
                throw new ArgumentNullException(nameof(banner));

            _bannerRepository.Insert(banner);
            //event notification
            _eventPublisher.EntityInserted(banner);
        }

        public void UpdateBanner(Banner banner)
        {
            if (banner == null)
                throw new ArgumentNullException(nameof(banner));

            _bannerRepository.Update(banner);

            //event notification
            _eventPublisher.EntityUpdated(banner);
        }
    }
}
