using System.Collections.Generic;

namespace Nop.Core.Domain.Directory
{
   public class District : BaseEntity
    {
        private ICollection<Ward> _wards;

        /// <summary>
        /// Gets or sets the country identifier
        /// </summary>
        public int StateProvinceId { get; set; }

        /// <summary>
        /// Gets or sets the name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the abbreviation
        /// </summary>

        /// <summary>
        /// Gets or sets a value indicating whether the entity is published
        /// </summary>
        public bool Published { get; set; }

        /// <summary>
        /// Gets or sets the display order
        /// </summary>
        public int DisplayOrder { get; set; }

        /// <summary>
        /// Gets or sets the country
        /// </summary>
        public virtual StateProvince StateProvince { get; set; }
        public virtual ICollection<Ward> Wards
        {
            get => _wards ?? (_wards = new List<Ward>());
            protected set => _wards = value;
        }
    }
}
