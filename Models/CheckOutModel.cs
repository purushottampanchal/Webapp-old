using WebApp.Models.General;
using WebApp.Models.Order;

namespace WebApp.Models
{
    public class CheckOutModel
    {
        public List<Address> addresses { get; set; }
        public Address DelivaryAddresses { get; set; }
        public string Payment{ get; set; }
        public ConfirmOrderModel confirmOrderModel { get; set; }

    }


    public class RemarkDto
    {
        public string Remark { get; set; }
    }
}
