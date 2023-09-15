namespace WebApp.Models
{
    public class AddToCartDto
    {
        public int UserId { get; set; }
        public string CartStatus { get; set; }
        //   public int CartCost { get; set; }
        public List<CartItemDto> CartItems { get; set; }

    }

    public class CartItemDto
    {
        public int ItemId { get; set; }
        public int Qty { get; set; }
    }

}
