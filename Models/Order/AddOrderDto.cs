namespace WebApp.Models.Order
{
    public class AddOrderDto
    {

        public int UserId { get; set; }
        public List<OrderItemDto> OrderItems { get; set; }
        public int TotalQty { get; set; }
        // public int ShippingCharges { get; set; }
        // public int OrderCost { get; set; } // calculating on server side
        public string OrderStatus { get; set; }
        public string Remark { get; set; }
        public string Address { get; set; }

    }

   
}
