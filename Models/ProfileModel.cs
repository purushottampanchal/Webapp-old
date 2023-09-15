using WebApp.Models.General;

namespace WebApp.Models
{
    public class ProfileModel
    {
        public List<CardDetail> cards { get; set; }
        public List<Address> addresses{ get; set; }
        public User user{ get; set; }
    }
}
