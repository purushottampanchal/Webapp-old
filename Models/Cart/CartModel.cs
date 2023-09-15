using Newtonsoft.Json;

namespace WebApp.Models.Cart
{
    public class CartModel
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; }
        public List<CarItemtResponceDto> CartItems { get; set; }
        public string CartStatus { get; set; }
        public int CartCost { get; set; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }


    public class CarItemtResponceDto
    {
        public int itemId { get; set; }
        public string name { get; set; }
        public int qty { get; set; }
        public int ItemCost { get; set; }
        public int TotalCost { get; set; } //=itemCost* qty
        public string ImgUrl { get; set; }
        public int BrandId { get; set; }
        public int TypeId { get; set; }
        public string Description { get; set; }

    }


}
