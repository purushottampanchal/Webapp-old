namespace WebApp.Models.Order
{
    public class ReceiptModel
    {

        public string UserName { get; set; }
        public string PhoneNo { get; set; }
        public string Address { get; set; }
        public string Remark { get; set; }
        public int id { get; set; }
        public List<ReceiptItems> Orderitems { get; set; }
        public int shippingChareges { get; set; }
        public int Cost { get; set; }
        public int TotalCost { get; set; }

    }

    public class ReceiptItems
    {
        public string Name { get; set; }
        public int itemQty { get; set; }
        public int Cost { get; set; }

    }


}
