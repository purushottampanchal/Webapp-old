using Newtonsoft.Json;
using WebApp.Models.Cart;
using WebApp.Models.General;

namespace WebApp.Models.Order
{
    public class ConfirmOrderModel
    {
        public List<CarItemtResponceDto> Items { get; set; }
        public List<Brand> brands{ get; set; }
        public List<Catalog> Types{ get; set; }
        public int Qty { get; set; }
        public string Address { get; set; }
        public string Remark { get; set; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }


}