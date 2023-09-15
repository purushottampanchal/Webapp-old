using Newtonsoft.Json;

namespace WebApp.Models.Order
{

    public class OrderItemsList
    {
        public List<OrderItemDto> orderItems;
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }

    }

    public class OrderItemDto
    {

        public int ItemId { get; set; }
        public int Qty { get; set; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
