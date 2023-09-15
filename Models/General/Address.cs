namespace WebApp.Models.General
{
    public class Address
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string HouseNo { get; set; }
        public string StreetArea { get; set; }
        public string City { get; set; }
        public string Pincode { get; set; }
        public string State { get; set; }
    }


    public class AddressDto
    {
        public int UserId { get; set; }
        public string HouseNo { get; set; }
        public string StreetArea { get; set; }
        public string City { get; set; }
        public string Pincode { get; set; }
        public string State { get; set; }
    }

}
