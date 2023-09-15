using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace WebApp.Models.General
{
    public class CatalogItem
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int BrandId { get; set; }
        public int CatlogId { get; set; }
        public int Cost { get; set; }
        public string ImageUrl { get; set; }
        public string Description { get; set; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
    
    public class CatalogItemDto
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public int BrandId { get; set; }
        [Required]
        public int CatlogId { get; set; }
        [Required]
        public int Cost { get; set; }
        [Required]
        public string ImageUrl { get; set; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
