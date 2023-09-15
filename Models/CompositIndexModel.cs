using WebApp.Models.General;

namespace WebApp.Models
{
    public class CompositIndexModel
    {

        public List<Brand> BrandsList { get; set; }
        public List<CatalogItem> ItemsList { get; set; }
        public List<Catalog> CatalogsList { get; set; }

        public string FilterBrandName { get; set; }
        public int FilterBrandId { get; set; }
        public string FilterTypeName { get; set; }
        public int FilterTypeId { get; set; }

        //public ItemFilterObject FilterObject { get; set; }
    }
}
