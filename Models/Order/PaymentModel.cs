using WebApp.Models.General;

namespace WebApp.Models.Order
{
    public class PaymentModel
    {
       public List<CardDetail> cards;
       
       public int oid { get; set; }
    }
}
