namespace WebApp.Models.General
{
    public class Catalog
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public string CatalogCover { get; set; }
        public int TotalItems { get; set; }
    }
    
    public class CatalogDto
    {
        public string Name { get; set; }
    }
}
