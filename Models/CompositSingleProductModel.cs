using WebApp.Models.General;

namespace WebApp.Models
{
    public class CompositSingleProductModel
    {

        public List<Brand> Brands { get; set; }
        public List<Catalog> Catalogs{ get; set; }
        public CatalogItem Item{ get; set; }
    }
    
    public class CompositAddProductModel
    {

        public List<Brand> Brands { get; set; }
        public List<Catalog> Catalogs{ get; set; }
        public CatalogItemDto Item{ get; set; }
    }
}
