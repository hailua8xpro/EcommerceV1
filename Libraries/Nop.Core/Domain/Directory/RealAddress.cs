using Nop.Core.Domain.Localization;
namespace Nop.Core.Domain.Directory
{
   public class RealAddress: BaseEntity, ILocalizedEntity
    {
        public int CountryId { get; set; }
        public int StateProvinceId { get; set; }
        public int DistrictId { get; set; }
        public int WardId { get; set; }
        public string Address { get; set; }
        public string Longitude { get; set; }
        public string Latitude { get; set; }
        public virtual Country Country { get; set; }
        public virtual StateProvince StateProvince { get; set; }
        public virtual District District { get; set; }
        public virtual Ward Ward { get; set; }

    }
}
