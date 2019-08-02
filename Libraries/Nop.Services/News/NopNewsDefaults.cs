using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Services.News
{
  public static  class NopNewsDefaults
    {
        /// <summary>
        /// Gets a key pattern to clear cache
        /// </summary>
        public static string NewsCategoriesPrefixCacheKey => "Nop.newscategory.";
        /// <summary>
        /// 
        /// </summary>
        public static string NewsCategoryMappingsPrefixCacheKey => "Nop.newscategorymapping.";
        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : current store ID
        /// {1} : comma separated list of customer roles
        /// {2} : show hidden records?
        /// </remarks>
        public static string NewsCategoriesAllCacheKey => "Nop.newscategory.all-{0}-{1}-{2}";
        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : parent news category ID
        /// {1} : show hidden records?
        /// {2} : current customer ID
        /// {3} : store ID
        /// </remarks>
        public static string CategoriesByParentCategoryIdCacheKey => "Nop.newscategory.byparent-{0}-{1}-{2}-{3}";
        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : parent news category id
        /// {1} : comma separated list of customer roles
        /// {2} : current store ID
        /// {3} : show hidden records?
        /// </remarks>
        public static string NewsCategoriesChildIdentifiersCacheKey => "Nop.newscategory.childidentifiers-{0}-{1}-{2}-{3}";

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} :news category ID
        /// </remarks>
        public static string NewsCategoriesByIdCacheKey => "Nop.newscategory.id-{0}";

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : show hidden records?
        /// {1} :news category ID
        /// {2} : page index
        /// {3} : page size
        /// {4} : current customer ID
        /// {5} : store ID
        /// </remarks>
        public static string NewsCategoryMappingsAllByCategoryIdCacheKey => "Nop.newscategorymapping.allbycategoryid-{0}-{1}-{2}-{3}-{4}-{5}";
        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : show hidden records?
        /// {1} : news ID
        /// {2} : current customer ID
        /// {3} : store ID
        /// </remarks>
        public static string NewsCategoryMappingsAllByNewsIdCacheKey => "Nop.newscategorymapping.allbyproductid-{0}-{1}-{2}-{3}";

        /// <summary>
        /// Gets a key pattern to clear cache
        /// </summary>
    }
}
