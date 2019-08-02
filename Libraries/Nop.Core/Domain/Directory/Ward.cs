
namespace Nop.Core.Domain.Directory
{
   public class Ward : BaseEntity
    {
        /// <summary>
        /// Gets or sets the country identifier
        /// </summary>
        public int DistrictId { get; set; }

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
        public virtual District District { get; set; }
    }
}
