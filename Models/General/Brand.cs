namespace WebApp.Models.General
{
    public class Brand
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string BrandCover { get; set; }
        public int TotalItems { get; set; }
    } 
    
    public class BrandDto
    {
        public string Name { get; set; }
    }
}
