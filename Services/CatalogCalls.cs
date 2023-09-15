using Newtonsoft.Json;
using System.Net;
using WebApp.Models.General;

namespace WebApp.Services
{
    public class CatalogCalls
    {

        public static async Task<List<Catalog>> getAllCatalogsAsync()
        {
            List<Catalog> CatalogList = new List<Catalog>();
            HttpClient client = new();
            using (var apiResponce = await client.GetAsync("https://localhost:7185/api/v1/Catlog/catlogtypes"))
            {
                if (apiResponce.StatusCode == HttpStatusCode.OK)
                {
                    string result = await apiResponce.Content.ReadAsStringAsync();
                    CatalogList = JsonConvert.DeserializeObject<List<Catalog>>(result);
                }
                else
                {
                    return null;
                }
            }

            return CatalogList;

        }

    }
}
