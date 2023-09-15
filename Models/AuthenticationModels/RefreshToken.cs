using System.ComponentModel.DataAnnotations;

namespace shoppingApi.Models.AuthenticationModels
{

    public static class DemoRefreshDto
    {
        public static List<RefreshToken> refreshTokenDtos = new();

    }

    public class RefreshToken
    {
        [Required]
        public string UserName { get; set; }

        [Required]
        public string Token { get; set; }

    }
}
