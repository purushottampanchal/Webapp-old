using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using WebApp.CustomAttribute;
using WebApp.Models;
using WebApp.Models.AuthenticationModels;
using WebApp.Models.General;
using System.Linq;
using System.Net;
using WebApp.Models.Order;
using WebApp.Services;
using WebApp.Models.Cart;

namespace WebApp.Controllers
{
    [Route("/Items")]
    public class CatalogController : Controller
    {
        IHttpContextAccessor _HttpContextAccessor;

        public CatalogController(IHttpContextAccessor httpContextAccessor)
        {
            _HttpContextAccessor = httpContextAccessor;
        }

        [AuthorizeCustomer]
        [Route("all")]

        public async Task<IActionResult> Index(string searchString)
        {

            HttpClient client = new HttpClient();
            string TokensString = _HttpContextAccessor.HttpContext.Session.GetString("AuthData");
            //if (string.IsNullOrEmpty(TokensString))
            //{
            //    TempData["msg"] = "Session Expired, Login first";
            //    TempData["code"] = "403";
            //    return RedirectToAction("Login", "Home");
            //}
            AuthData authData = JsonConvert.DeserializeObject<AuthData>(TokensString);
            int userId = authData.UserId;
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", authData.AccessToken);

            List<CatalogItem> CatalogItems = new();
            List<Catalog> CatalogList = new();
            List<Brand> BrandsList = new();


            //items
            try
            {
                using (var apiResponce = await client.GetAsync("https://localhost:7185/api/v1/catlog/items"))
                {
                    if (apiResponce.IsSuccessStatusCode || apiResponce.StatusCode != System.Net.HttpStatusCode.OK)
                    {
                        string result = await apiResponce.Content.ReadAsStringAsync();
                        CatalogItems = JsonConvert.DeserializeObject<List<CatalogItem>>(result);
                    }
                    else if (apiResponce.IsSuccessStatusCode || apiResponce.StatusCode != System.Net.HttpStatusCode.NotFound)
                    {
                        CatalogItems.Clear();
                    }
                    else
                    {
                        TempData["ErrMsg"] = "Api Error: " + apiResponce.StatusCode;
                        //          return View("Index");
                        //Console.WriteLine("--------------------api res" + apiResponce.StatusCode);
                        ViewData["ErrorMessage"] = $"<h4> item { apiResponce.StatusCode }</h4><br><b>---</b><br> {await apiResponce.Content.ReadAsStringAsync()}";
                        return View("Error");
                    }
                }
            }
            catch (Exception e)
            {
                ViewData["ErrorMessage"] = "Exception- " + e.Message + "<br>stackTrace<br>" + e.StackTrace;
                return View("Error");
            }

            //types
            try
            {
                using (var apiResponce = await client.GetAsync("https://localhost:7185/api/v1/Catlog/catlogtypes"))
                {
                    if (apiResponce.StatusCode == HttpStatusCode.OK)
                    {
                        string result = await apiResponce.Content.ReadAsStringAsync();
                        CatalogList = JsonConvert.DeserializeObject<List<Catalog>>(result);
                    }
                    else
                    {
                        ViewData["ErrorMessage"] = $"<h4> type { apiResponce.StatusCode }</h4><br><b>---</b><br> {await apiResponce.Content.ReadAsStringAsync()}";
                        return View("Error");
                    }
                }

            }
            //catch (WebException e)
            //{
            //    HttpWebResponse httpWebResponse = (HttpWebResponse)e.Response;
            //    ViewData["ErrorMessage"] = "Exception- type" + e.Message + "<br>stackTrace<br>" + httpWebResponse.StatusDescription;
            //    return View("Error");

            //}
            catch (Exception e)
            {
                ViewData["ErrorMessage"] = "Exception- type" + e.Message + "<br>stackTrace<br>" + e.StackTrace;
                return View("Error");
            }


            //brands
            try
            {
                using (var apiResponce = await client.GetAsync("https://localhost:7185/api/v1/Catlog/catlogBrands"))
                {
                    if (apiResponce.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        string result = await apiResponce.Content.ReadAsStringAsync();
                        BrandsList = JsonConvert.DeserializeObject<List<Brand>>(result);
                    }
                    else
                    {
                        ViewData["ErrorMessage"] = $"<h4> b {apiResponce.StatusCode}</h4><br><b>---</b><br> {await apiResponce.Content.ReadAsStringAsync()}";
                        return View("Error");
                    }
                }

            }
            catch (Exception e)
            {

                ViewData["ErrorMessage"] = "Exception- " + e.Message + "<br>stackTrace<br>" + e.StackTrace;
                return View("Error");
            }

            if (!string.IsNullOrEmpty(searchString))
            {
                searchString = searchString.ToLower().Trim();
                var oldList = new List<CatalogItem>();
                oldList.AddRange(CatalogItems);
                CatalogItems.Clear();

                CatalogItems.AddRange(from item in oldList
                                      where
                                      item.Name.ToLower().StartsWith(searchString)
                                      || item.Name.ToLower().EndsWith(searchString)
                                      || item.Name.ToLower().Contains(searchString)

                                      select item);



            }

            CompositIndexModel indexModel = new CompositIndexModel()
            {
                BrandsList = BrandsList,
                CatalogsList = CatalogList,
                ItemsList = CatalogItems,
                FilterBrandName = null,
                FilterTypeName = null
            };

            return View(indexModel);

        }

        [AuthorizeCustomer]
        [Route("brand/all/type/{itemTypeId}")]
        public async Task<IActionResult> ItemsByItemType(int itemTypeId, string searchString)
        {

            HttpClient client = new HttpClient();
            string TokensString = _HttpContextAccessor.HttpContext.Session.GetString("AuthData");
            //if (string.IsNullOrEmpty(TokensString))
            //{
            //    TempData["msg"] = "Session Expired, Login first";
            //    TempData["code"] = "403";
            //    return RedirectToAction("Login", "Home");
            //}
            AuthData authData = JsonConvert.DeserializeObject<AuthData>(TokensString);
            int UserId = authData.UserId;
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", authData.AccessToken);

            //int UserId = _HttpContextAccessor.HttpContext.Session.GetInt32("UserId") == null ?
            //               1 :
            //               (int)_HttpContextAccessor.HttpContext.Session.GetInt32("UserId");


            List<CatalogItem> CatalogItems = new();
            List<Catalog> CatalogList = new();
            List<Brand> BrandsList = new();


            try
            {
                //items
                using (var apiResponce = await client.GetAsync("https://localhost:7185/api/v1/Catlog/Items/type/" + itemTypeId))
                {
                    if (apiResponce.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        string result = await apiResponce.Content.ReadAsStringAsync();
                        CatalogItems = JsonConvert.DeserializeObject<List<CatalogItem>>(result);
                    }
                    else if (apiResponce.IsSuccessStatusCode || apiResponce.StatusCode != System.Net.HttpStatusCode.NotFound)
                    {
                        CatalogItems.Clear();
                    }
                    else
                    {
                        ViewData["ErrorMessage"] = $"<h4>  {apiResponce.StatusCode}</h4><br><b>---</b><br> {await apiResponce.Content.ReadAsStringAsync()}";
                        return View("Error");
                    }
                }

                //catalogs
                using (var apiResponce = await client.GetAsync("https://localhost:7185/api/v1/Catlog/catlogtypes"))
                {
                    if (apiResponce.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        string result = await apiResponce.Content.ReadAsStringAsync();
                        CatalogList = JsonConvert.DeserializeObject<List<Catalog>>(result);
                    }
                    else
                    {
                        ViewData["ErrorMessage"] = $"<h4>  {apiResponce.StatusCode}</h4><br><b>---</b><br> {await apiResponce.Content.ReadAsStringAsync()}";
                        return View("Error");
                    }
                }


                //brands
                using (var apiResponce = await client.GetAsync("https://localhost:7185/api/v1/Catlog/catlogBrands"))
                {
                    if (apiResponce.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        string result = await apiResponce.Content.ReadAsStringAsync();
                        BrandsList = JsonConvert.DeserializeObject<List<Brand>>(result);
                    }
                    
                    else
                    {
                        ViewData["ErrorMessage"] = $"<h4>  {apiResponce.StatusCode}</h4><br><b>---</b><br> {await apiResponce.Content.ReadAsStringAsync()}";
                        return View("Error");
                    }
                }
            }
            catch (Exception e)
            {
                ViewData["ErrorMessage"] = "Exception- " + e.Message + "<br>stackTrace<br>" + e.StackTrace;
                return View("Error");
            }

            Catalog currCatalog = CatalogList.First(c => c.Id == itemTypeId);
            CatalogList.Remove(currCatalog);

            if (!string.IsNullOrEmpty(searchString))
            {
                searchString = searchString.ToLower().Trim();
                var oldList = new List<CatalogItem>();
                oldList.AddRange(CatalogItems);
                CatalogItems.Clear();

                CatalogItems.AddRange(from item in oldList
                                      where
                                      item.Name.ToLower().StartsWith(searchString)
                                      || item.Name.ToLower().EndsWith(searchString)
                                      || item.Name.ToLower().Contains(searchString)

                                      select item);



            }


            CompositIndexModel indexModel = new CompositIndexModel()
            {
                BrandsList = BrandsList,
                CatalogsList = CatalogList,
                ItemsList = CatalogItems,
                FilterBrandName = null,
                FilterBrandId = 0,
                FilterTypeName = currCatalog.Name,
                FilterTypeId = currCatalog.Id
            };

            return View("Index", indexModel);

        }

        [AuthorizeCustomer]
        [Route("brand/{itemBrandId}/type/all")]
        public async Task<IActionResult> ItemsByBrand(int itemBrandId, string searchString)
        {

            HttpClient client = new HttpClient();
            string TokensString = _HttpContextAccessor.HttpContext.Session.GetString("AuthData");
            //if (string.IsNullOrEmpty(TokensString))
            //{
            //    TempData["msg"] = "Session Expired, Login first";
            //    TempData["code"] = "403";
            //    return RedirectToAction("Login", "Home");
            //}
            AuthData authData = JsonConvert.DeserializeObject<AuthData>(TokensString);
            int UserId = authData.UserId;
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", authData.AccessToken);

            //int UserId = _HttpContextAccessor.HttpContext.Session.GetInt32("UserId") == null ?
            //               1 :
            //               (int)_HttpContextAccessor.HttpContext.Session.GetInt32("UserId");



            List<CatalogItem> CatalogItems = new();
            List<Catalog> CatalogList = new();
            List<Brand> BrandsList = new();


            try
            {
                //items
                using (var apiResponce = await client.GetAsync("https://localhost:7185/api/v1/Catlog/Items/type/all/brand/" + itemBrandId))
                {
                    if (apiResponce.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        string result = await apiResponce.Content.ReadAsStringAsync();
                //        Console.WriteLine("res---" + result);
                        CatalogItems = JsonConvert.DeserializeObject<List<CatalogItem>>(result);
                    }
                    else if (apiResponce.IsSuccessStatusCode || apiResponce.StatusCode != System.Net.HttpStatusCode.NotFound)
                    {
                        CatalogItems.Clear();
                    }
                    else
                    {
                        ViewData["ErrorMessage"] = $"<h4>  { apiResponce.StatusCode }</h4><br><b>---</b><br> {await apiResponce.Content.ReadAsStringAsync()}";
                        return View("Error");
                    }
                }

                //catalogs
                using (var apiResponce = await client.GetAsync("https://localhost:7185/api/v1/Catlog/catlogtypes"))
                {
                    if (apiResponce.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        string result = await apiResponce.Content.ReadAsStringAsync();
                        CatalogList = JsonConvert.DeserializeObject<List<Catalog>>(result);
                    }
                    else
                    {
                        ViewData["ErrorMessage"] = $"<h4>  {apiResponce.StatusCode}</h4><br><b>---</b><br> {await apiResponce.Content.ReadAsStringAsync()}";
                        return View("Error");
                    }
                }

                //brands
                using (var apiResponce = await client.GetAsync("https://localhost:7185/api/v1/Catlog/catlogBrands"))
                {
                    if (apiResponce.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        string result = await apiResponce.Content.ReadAsStringAsync();
                        BrandsList = JsonConvert.DeserializeObject<List<Brand>>(result);
                    }
                    else
                    {
                        ViewData["ErrorMessage"] = $"<h4>  {apiResponce.StatusCode}</h4><br><b>---</b><br> {await apiResponce.Content.ReadAsStringAsync()}";
                        return View("Error");
                    }
                }

            }
            catch (Exception e)
            {

                ViewData["ErrorMessage"] = "Exception- " + e.Message + "<br>stackTrace<br>" + e.StackTrace;
                return View("Error");

            }
            // Console.WriteLine("-----------"+CatalogItems.Count());
            Brand currBrand = BrandsList.First(b => b.Id == itemBrandId);
            BrandsList.Remove(currBrand);

            if (!string.IsNullOrEmpty(searchString))
            {
                searchString = searchString.ToLower().Trim();
                var oldList = new List<CatalogItem>();
                oldList.AddRange(CatalogItems);
                CatalogItems.Clear();

                CatalogItems.AddRange(from item in oldList
                                      where
                                      item.Name.ToLower().StartsWith(searchString)
                                      || item.Name.ToLower().EndsWith(searchString)
                                      || item.Name.ToLower().Contains(searchString)

                                      select item);



            }

            CompositIndexModel indexModel = new CompositIndexModel()
            {
                BrandsList = BrandsList,
                CatalogsList = CatalogList,
                ItemsList = CatalogItems,
                FilterBrandName = currBrand.Name,
                FilterBrandId = currBrand.Id,
                FilterTypeName = null
            };

            return View("Index", indexModel);

        }

        [AuthorizeCustomer]
        [Route("brand/{itemBrnadId}/type/{itemTypeId}")]
        public async Task<IActionResult> ItemsByBrandAndType(int itemBrnadId, int itemTypeId, string searchString)
        {

            HttpClient client = new HttpClient();
            string TokensString = _HttpContextAccessor.HttpContext.Session.GetString("AuthData");
            //if (string.IsNullOrEmpty(TokensString))
            //{
            //    TempData["msg"] = "Session Expired, Login first";
            //    TempData["code"] = "403";
            //    return RedirectToAction("Login", "Home");
            //}
            AuthData authData = JsonConvert.DeserializeObject<AuthData>(TokensString);
            int UserId = authData.UserId;
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", authData.AccessToken);

            //int UserId = _HttpContextAccessor.HttpContext.Session.GetInt32("UserId") == null ?
            //               1 :
            //               (int)_HttpContextAccessor.HttpContext.Session.GetInt32("UserId");


            List<CatalogItem> CatalogItems = new();
            List<Catalog> CatalogList = new();
            List<Brand> BrandsList = new();


            try
            {
                //items
                using (var apiResponce = await client.GetAsync("https://localhost:7185/api/v1/Catlog/Items/type/" + itemTypeId + "/brand/" + itemBrnadId))
                {
                    if (apiResponce.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        string result = await apiResponce.Content.ReadAsStringAsync();
                        CatalogItems = JsonConvert.DeserializeObject<List<CatalogItem>>(result);
                    }
                    else if (apiResponce.IsSuccessStatusCode || apiResponce.StatusCode != System.Net.HttpStatusCode.NotFound)
                    {
                        ViewData["ErrorMessage"] = $"No Catalog found";
                        return View("Error");
                    }
                    else
                    {
                        ViewData["ErrorMessage"] = $"<h4>  { apiResponce.StatusCode }</h4><br><b>---</b><br> {await apiResponce.Content.ReadAsStringAsync()}";
                        return View("Error");
                    }
                }

                //catalogs
                using (var apiResponce = await client.GetAsync("https://localhost:7185/api/v1/Catlog/catlogtypes"))
                {
                    if (apiResponce.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        string result = await apiResponce.Content.ReadAsStringAsync();
                        CatalogList = JsonConvert.DeserializeObject<List<Catalog>>(result);
                    }
                    else if (apiResponce.IsSuccessStatusCode || apiResponce.StatusCode != System.Net.HttpStatusCode.NotFound)
                    {
                        ViewData["ErrorMessage"] = $"No Catalog found";
                        return View("Error");
                    }
                    else
                    {
                        ViewData["ErrorMessage"] = $"<h4>  { apiResponce.StatusCode }</h4><br><b>---</b><br> {await apiResponce.Content.ReadAsStringAsync()}";
                        return View("Error");
                    }
                }

                //brands
                using (var apiResponce = await client.GetAsync("https://localhost:7185/api/v1/Catlog/catlogBrands"))
                {
                    if (apiResponce.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        string result = await apiResponce.Content.ReadAsStringAsync();
                        BrandsList = JsonConvert.DeserializeObject<List<Brand>>(result);
                    }
                    else if (apiResponce.IsSuccessStatusCode || apiResponce.StatusCode != System.Net.HttpStatusCode.NotFound)
                    {
                        ViewData["ErrorMessage"] = $"No Brand found";
                        return View("Error");
                    }
                    else
                    {
                        ViewData["ErrorMessage"] = $"<h4>  { apiResponce.StatusCode }</h4><br><b>---</b><br> {await apiResponce.Content.ReadAsStringAsync()}";
                        return View("Error");
                    }
                }
            }
            catch (Exception e)
            {
                //remove stacktrace 
                ViewData["ErrorMessage"] = $"<h4>  { e.Message }</h4><br><b>stackTrace</b><br> {e.StackTrace}";
                return View("Error");
            }

            Catalog currCatalog = CatalogList.First(c => c.Id == itemTypeId);
            Brand currBrand = BrandsList.First(c => c.Id == itemBrnadId);

            BrandsList.Remove(currBrand);
            CatalogList.Remove(currCatalog);

            if (!string.IsNullOrEmpty(searchString))
            {
                searchString = searchString.ToLower().Trim();
                var oldList = new List<CatalogItem>();
                oldList.AddRange(CatalogItems);
                CatalogItems.Clear();

                CatalogItems.AddRange(from item in oldList
                                      where
                                      item.Name.ToLower().StartsWith(searchString)
                                      || item.Name.ToLower().EndsWith(searchString)
                                      || item.Name.ToLower().Contains(searchString)
                                      select item);
            }


            CompositIndexModel indexModel = new CompositIndexModel()
            {
                BrandsList = BrandsList,
                CatalogsList = CatalogList,
                ItemsList = CatalogItems,
                FilterBrandName = currBrand.Name,
                FilterBrandId = currBrand.Id,
                FilterTypeName = currCatalog.Name,
                FilterTypeId = currCatalog.Id
            };

            return View("Index", indexModel);

        }

        [AuthorizeCustomer]
        [Route("viewItem/{id}")]
        public async Task<IActionResult> Viewitem(int id)
        {
            CatalogItem item;
            HttpClient client = new HttpClient();
            string TokensString = _HttpContextAccessor.HttpContext.Session.GetString("AuthData");
            //if (string.IsNullOrEmpty(TokensString))
            //{
            //    TempData["msg"] = "Session Expired, Login first";
            //    TempData["code"] = "403";
            //    return RedirectToAction("Login", "Home");
            //}
            AuthData authData = JsonConvert.DeserializeObject<AuthData>(TokensString);
            int UserId = authData.UserId;
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", authData.AccessToken);

            //int UserId = _HttpContextAccessor.HttpContext.Session.GetInt32("UserId") == null ?
            //               1 :
            //               (int)_HttpContextAccessor.HttpContext.Session.GetInt32("UserId");

            List<CatalogItem> itemsWithSameBrand = new();
            List<CatalogItem> itemsWithSameType = new();

            try
            {
                using (var apiResponce = await client.GetAsync("https://localhost:7185/api/v1/Catlog/Items/" + id))
                {
                    if (apiResponce.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        string resultString = await apiResponce.Content.ReadAsStringAsync();
                        item = JsonConvert.DeserializeObject<CatalogItem>(resultString);

                        using (var apiResponce2 = await client.GetAsync("https://localhost:7185/api/v1/Catlog/Items/type/all/brand/" + item.BrandId))
                        {
                            if (apiResponce2.StatusCode == System.Net.HttpStatusCode.OK)
                            {
                                string result = await apiResponce2.Content.ReadAsStringAsync();
                                //   Console.WriteLine("res---" + result);
                                itemsWithSameBrand = JsonConvert.DeserializeObject<List<CatalogItem>>(result);
                                itemsWithSameBrand = itemsWithSameBrand.Where(x => x.Id != item.Id).ToList();
                            }
                            else
                            {
                                ViewData["ErrorMessage"] = $"<h4> brand related  { apiResponce.StatusCode }</h4><br><b>---</b><br> {await apiResponce.Content.ReadAsStringAsync()}";
                                return View("Error");
                            }
                        }

                        using (var apiResponce2 = await client.GetAsync("https://localhost:7185/api/v1/Catlog/Items/type/" + item.CatlogId))
                        {
                            if (apiResponce2.StatusCode == System.Net.HttpStatusCode.OK)
                            {
                                string result = await apiResponce2.Content.ReadAsStringAsync();
                                itemsWithSameType = JsonConvert.DeserializeObject<List<CatalogItem>>(result);
                                itemsWithSameType = itemsWithSameType.Where(x => x.Id != item.Id).ToList();
                            }
                            else
                            {
                                ViewData["ErrorMessage"] = $"<h4> type related  {apiResponce.StatusCode}</h4><br><b>---</b><br> {await apiResponce.Content.ReadAsStringAsync()}";
                                return View("Error");
                            }
                        }


                        ViewData["itemsWithSameBrand"] = itemsWithSameBrand;
                        ViewData["itemsWithSameType"] = itemsWithSameType;
                        return View(item);

                    }
                    else
                    {
                        ViewData["msg"] = "Error: " + apiResponce.StatusCode;
                        return View();

                    }
                }



            }
            catch (Exception e)
            {
                ViewData["ErrorMessage"] = "Exception- " + e.Message + "<br>stackTrace<br>" + e.StackTrace;
                return View("Error");

            }
            //     Console.WriteLine(item.ToString());

        }

        [HttpPost]
        [AuthorizeCustomer]
        [Route("AddToCart")]
        public async Task<string> AddItemToCart(int id, int qty)
        {
            // return ($"hehe qty {qty} id "+id);
            HttpClient client = new HttpClient();
            string TokensString = _HttpContextAccessor.HttpContext.Session.GetString("AuthData");
            //if (string.IsNullOrEmpty(TokensString))
            //{
            //    TempData["msg"] = "Session Expired, Login first";
            //    TempData["code"] = "403";
            //    return RedirectToAction("Login", "Home");
            //}
            AuthData authData = JsonConvert.DeserializeObject<AuthData>(TokensString);
            int UserId = authData.UserId;
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", authData.AccessToken);

            //int UserId = _HttpContextAccessor.HttpContext.Session.GetInt32("UserId") == null ?
            //               1 :
            //               (int)_HttpContextAccessor.HttpContext.Session.GetInt32("UserId");

            //string TokensString = _HttpContextAccessor.HttpContext.Session.GetString("AuthData");

            //AuthData authData = JsonConvert.DeserializeObject<AuthData>(TokensString);
            //client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", authData.AccessToken);

            List<CartItemDto> cartItems = new();

            cartItems.Add(new CartItemDto()
            {
                ItemId = id,
                Qty = qty
            });

            AddToCartDto cartDto = new()
            {
                CartItems = cartItems,
                CartStatus = "Current",
                UserId = UserId,
            };

            JsonContent content = JsonContent.Create(cartDto);

            try
            {
                using (var apiResponce = await client.PostAsync("https://localhost:7185/api/v1/Cart/AddToCart", content))
                {
                    if (apiResponce.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        string resultString = await apiResponce.Content.ReadAsStringAsync();
                        return resultString;
                    }
                    else
                    {
                        return "API Error";
                    }
                }
            }
            catch (Exception e)
            {

                return "Error " + e.Message;

            }

            //  return "Success";

        }


        [AuthorizeCustomer]
        [Route("ClearCart")]
        public async Task<IActionResult> EmptyCart()
        {
            HttpClient client = new HttpClient();
            string TokensString = _HttpContextAccessor.HttpContext.Session.GetString("AuthData");
            //if (string.IsNullOrEmpty(TokensString))
            //{
            //    TempData["msg"] = "Session Expired, Login first";
            //    TempData["code"] = "403";
            //    return RedirectToAction("Login", "Home");
            //}
            AuthData authData = JsonConvert.DeserializeObject<AuthData>(TokensString);
            int UserId = authData.UserId;
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", authData.AccessToken);

            //int UserId = _HttpContextAccessor.HttpContext.Session.GetInt32("UserId") == null ?
            //               1 :
            //               (int)_HttpContextAccessor.HttpContext.Session.GetInt32("UserId");


            using (var apiResponce = await client.DeleteAsync("https://localhost:7185/api/v1/Cart/EmptyCartForUser/" + UserId))
            {
                if (apiResponce.StatusCode == HttpStatusCode.OK)
                {
                    TempData["msg"] = "Cart Cleared";
                    return RedirectToAction("Cart");
                }
                else
                {
                    TempData["ErrorMessage"] = $"status: {apiResponce.StatusCode} : " +
                        $"{ await apiResponce.Content.ReadAsStringAsync()}";

                    return RedirectToAction("Cart");

                }
            }

        }


        //individual item
        [AuthorizeCustomer]
        [Route("ConfirmOrder")]
        public async Task<IActionResult> ConfirmOrder(int id, int qty)
        {


            CatalogItem item;
            HttpClient client = new HttpClient();
            string TokensString = _HttpContextAccessor.HttpContext.Session.GetString("AuthData");
            //if (string.IsNullOrEmpty(TokensString))
            //{
            //    TempData["msg"] = "Session Expired, Login first";
            //    TempData["code"] = "403";
            //    return RedirectToAction("Login", "Home");
            //}
            AuthData authData = JsonConvert.DeserializeObject<AuthData>(TokensString);
            int UserId = authData.UserId;
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", authData.AccessToken);

            //int UserId = _HttpContextAccessor.HttpContext.Session.GetInt32("UserId") == null ?
            //               1 :
            //               (int)_HttpContextAccessor.HttpContext.Session.GetInt32("UserId");
            List<Catalog> CatalogList = new();
            List<Brand> BrandsList = new();
            List<Address> addressList = new();

          
            try
            {
                using (var apiResponce = await client.GetAsync("https://localhost:7185/api/v1/User/GetAddress/User/ " + UserId))
                {
                    if (apiResponce.StatusCode == HttpStatusCode.OK)
                    {
                        var resString = await apiResponce.Content.ReadAsStringAsync();
                        //   return Content(resString);

                        //    _HttpContextAccessor.HttpContext.Session.SetString("Cart", resString);

                        addressList = JsonConvert.DeserializeObject<List<Address>>(resString);
                    }
                    else if (apiResponce.StatusCode == HttpStatusCode.NotFound)
                    {
                        addressList = new List<Address>();
                    }
                    else
                    {
                        ViewData["ErrorMessage"] = $"<h4> {apiResponce.StatusCode}</h4><br><b>---</b><br> {await apiResponce.Content.ReadAsStringAsync()}";
                        return View("Error");
                    }
                }

                using (var apiResponce = await client.GetAsync("https://localhost:7185/api/v1/Catlog/catlogtypes"))
                {
                    if (apiResponce.StatusCode == HttpStatusCode.OK)
                    {
                        string result = await apiResponce.Content.ReadAsStringAsync();
                        CatalogList = JsonConvert.DeserializeObject<List<Catalog>>(result);
                    }
                    else
                    {
                        ViewData["ErrorMessage"] = $"<h4> b {apiResponce.StatusCode}</h4><br><b>---</b><br> {await apiResponce.Content.ReadAsStringAsync()}";
                        return View("Error");
                    }
                }
                using (var apiResponce = await client.GetAsync("https://localhost:7185/api/v1/Catlog/catlogBrands"))
                {
                    if (apiResponce.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        string result = await apiResponce.Content.ReadAsStringAsync();
                        BrandsList = JsonConvert.DeserializeObject<List<Brand>>(result);
                    }
                    else
                    {
                        ViewData["ErrorMessage"] = $"<h4> b {apiResponce.StatusCode}</h4><br><b>---</b><br> {await apiResponce.Content.ReadAsStringAsync()}";
                        return View("Error");
                    }
                }
                using (var apiResponce = await client.GetAsync("https://localhost:7185/api/v1/Catlog/Items/" + id))
                {
                    if (apiResponce.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        string resultString = await apiResponce.Content.ReadAsStringAsync();
                        item = JsonConvert.DeserializeObject<CatalogItem>(resultString);
                    }
                    else
                    {
                        ViewData["msg"] = "Error: " + apiResponce.StatusCode;
                        return View();

                    }
                }
            }
            catch (Exception e)
            {
                ViewData["ErrorMessage"] = "Exception- " + e.Message + "<br>stackTrace<br>" + e.StackTrace;
                return View("Error");

            }

            List<CarItemtResponceDto> items = new List<CarItemtResponceDto>() { new CarItemtResponceDto(){
            itemId = item.Id,
            BrandId = item.BrandId,
            TypeId = item.CatlogId,
            ItemCost = item.Cost,
            Description = item.Description,
            ImgUrl = item.ImageUrl,
            name = item.Name,
            qty = qty,
            TotalCost = item.Cost*qty
            } };

            List<OrderItemDto> orderItems = new()
            {
                new OrderItemDto()
                {
                    ItemId = id,
                    Qty = qty
                }
            };
            _HttpContextAccessor.HttpContext.Session.SetString("OrderItems", JsonConvert.SerializeObject(orderItems));

            ConfirmOrderModel confirmOrderModel = new()
            {
                Items = items,
                Qty = qty,
                brands = BrandsList,
                Types = CatalogList
            };

            CheckOutModel model = new()
            {
                confirmOrderModel = confirmOrderModel,
                addresses = addressList,
            };

            _HttpContextAccessor.HttpContext.Session.SetString("CheckOutModel", JsonConvert.SerializeObject(model));


            return View(model);
        }



        [AuthorizeCustomer]
        [Route("PutOrder")]
        public async Task<IActionResult> PutOrder(Address DelivaryAddresses, string Payment)
        {


            //CheckOutModel model = JsonConvert.DeserializeObject<CheckOutModel>(_HttpContextAccessor.HttpContext.Session.GetString("CheckOutModel"));
            string itemString = _HttpContextAccessor.HttpContext.Session.GetString("OrderItems");
            List<OrderItemDto> OrderItems = JsonConvert.DeserializeObject<List<OrderItemDto>>(itemString);
            //return Content(JsonConvert.SerializeObject(DelivaryAddresses) + "\n\n\n\n" + itemString + "\n\n\n" + Payment);

            HttpClient client = new HttpClient();
            string TokensString = _HttpContextAccessor.HttpContext.Session.GetString("AuthData");
            //if (string.IsNullOrEmpty(TokensString))
            //{
            //    TempData["msg"] = "Session Expired, Login first";
            //    TempData["code"] = "403";
            //    return RedirectToAction("Login", "Home");
            //}
            AuthData authData = JsonConvert.DeserializeObject<AuthData>(TokensString);
            int UserId = authData.UserId;
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", authData.AccessToken);

            //int UserId = _HttpContextAccessor.HttpContext.Session.GetInt32("UserId") == null ?
            //               1 :
            //               (int)_HttpContextAccessor.HttpContext.Session.GetInt32("UserId");

            string address = $"{DelivaryAddresses.HouseNo}, " +
                $"{DelivaryAddresses.StreetArea}, " +
                $"{DelivaryAddresses.City}, " +
                $"{DelivaryAddresses.State}, " +
                $"{DelivaryAddresses.Pincode}.";


            string orderRemark = (Payment == "Cod") ? "Cash on delivary" : "Payment Pending";

            AddOrderDto OrderDto = new()
            {
                Address = address,
                OrderItems = OrderItems,
                OrderStatus = "Processing",
                Remark = orderRemark,
                TotalQty = OrderItems.Count,
                UserId = UserId
            };

            JsonContent content = JsonContent.Create(OrderDto);
            // return Content(JsonConvert.SerializeObject(OrderDto));
            using (var apiResponce = await client.PostAsync("https://localhost:7185/api/v1/Order/AddOrder", content))
            {
                if (apiResponce.StatusCode == HttpStatusCode.OK)
                {
                    //TempData["msg"] = "Order Place Successfully";
                    if (Payment == "Cod")
                    {
                        return RedirectToAction("Orders");
                    }
                    else
                    {
                        string oid = await apiResponce.Content.ReadAsStringAsync();
                        _HttpContextAccessor.HttpContext.Session.SetString("OrderId", oid);
                        return RedirectToAction("PaymentPage");
                    }
                }
                else
                {
                    TempData["ErrMsg"] = "Api Error: " + apiResponce.StatusCode;
                    //          return View("Index");
                    //Console.WriteLine("--------------------api res" + apiResponce.StatusCode);
                    ViewData["ErrorMessage"] = $"<h4> { apiResponce.StatusCode }</h4><br><b>---</b><br> {await apiResponce.Content.ReadAsStringAsync()}";
                    return View("Error");
                }
            }

        }

        [AuthorizeCustomer]
        [Route("Cart")]
        [HttpPost]
        public async Task<IActionResult> Cart(CartModel model)
        {
            //       return Content(model.ToString());
            List<Address> addressList = new();
            List<OrderItemDto> orderItems = new();
            orderItems.AddRange(from item in model.CartItems
                                select new OrderItemDto()
                                {
                                    ItemId = item.itemId,
                                    Qty = item.qty
                                });

            List<CarItemtResponceDto> items = new();
            HttpClient client = new HttpClient();
            string TokensString = _HttpContextAccessor.HttpContext.Session.GetString("AuthData");
            //if (string.IsNullOrEmpty(TokensString))
            //{
            //    TempData["msg"] = "Session Expired, Login first";
            //    TempData["code"] = "403";
            //    return RedirectToAction("Login", "Home");
            //}
            AuthData authData = JsonConvert.DeserializeObject<AuthData>(TokensString);
            int UserId = authData.UserId;
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", authData.AccessToken);

            //int UserId = _HttpContextAccessor.HttpContext.Session.GetInt32("UserId") == null ?
            //               1 :
            //               (int)_HttpContextAccessor.HttpContext.Session.GetInt32("UserId");


            List<Catalog> CatalogList = new();
            List<Brand> BrandsList = new();

            try
            {

                using (var apiResponce = await client.GetAsync("https://localhost:7185/api/v1/Catlog/catlogtypes"))
                {
                    if (apiResponce.StatusCode == HttpStatusCode.OK)
                    {
                        string result = await apiResponce.Content.ReadAsStringAsync();
                        CatalogList = JsonConvert.DeserializeObject<List<Catalog>>(result);
                    }
                    else
                    {
                        ViewData["ErrorMessage"] = $"<h4> b {apiResponce.StatusCode}</h4><br><b>---</b><br> {await apiResponce.Content.ReadAsStringAsync()}";
                        return View("Error");
                    }
                }
                using (var apiResponce = await client.GetAsync("https://localhost:7185/api/v1/Catlog/catlogBrands"))
                {
                    if (apiResponce.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        string result = await apiResponce.Content.ReadAsStringAsync();
                        BrandsList = JsonConvert.DeserializeObject<List<Brand>>(result);
                    }
                    else
                    {
                        ViewData["ErrorMessage"] = $"<h4> b {apiResponce.StatusCode}</h4><br><b>---</b><br> {await apiResponce.Content.ReadAsStringAsync()}";
                        return View("Error");
                    }
                }
                using (var apiResponce = await client.GetAsync("https://localhost:7185/api/v1/Catlog/Items/"))
                {
                    if (apiResponce.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        string resultString = await apiResponce.Content.ReadAsStringAsync();
                        List<CatalogItem> list = JsonConvert.DeserializeObject<List<CatalogItem>>(resultString);
                        items.AddRange(from item in orderItems
                                       from i in list
                                       where item.ItemId == i.Id
                                       select new CarItemtResponceDto()
                                       {
                                           itemId = i.Id,
                                           BrandId = i.BrandId,
                                           TypeId = i.CatlogId,
                                           ItemCost = i.Cost,
                                           Description = i.Description,
                                           ImgUrl = i.ImageUrl,
                                           name = i.Name,
                                           qty = item.Qty,
                                           TotalCost = i.Cost
                                       });
                    }
                    else
                    {
                        ViewData["msg"] = "Error: " + apiResponce.StatusCode;
                        return View();

                    }
                }

                using (var apiResponce = await client.GetAsync("https://localhost:7185/api/v1/User/GetAddress/User/ " + UserId))
                {
                    if (apiResponce.StatusCode == HttpStatusCode.OK)
                    {
                        var resString = await apiResponce.Content.ReadAsStringAsync();
                        //   return Content(resString);

                        //    _HttpContextAccessor.HttpContext.Session.SetString("Cart", resString);

                        addressList = JsonConvert.DeserializeObject<List<Address>>(resString);
                    }
                    //else
                    //{
                    //    ViewData["ErrorMessage"] = $"<h4> {apiResponce.StatusCode}</h4><br><b>---</b><br> {await apiResponce.Content.ReadAsStringAsync()}";
                    //    return View("Error");
                    //}
                }


            }
            catch (Exception e)
            {
                ViewData["ErrorMessage"] = "Exception- " + e.Message + "<br>stackTrace<br>" + e.StackTrace;
                return View("Error");
            }



            _HttpContextAccessor.HttpContext.Session.SetString("OrderItems", JsonConvert.SerializeObject(orderItems));

            ConfirmOrderModel confirmOrderModel = new()
            {
                Items = items,
                Qty = items.Count,
                brands = BrandsList,
                Types = CatalogList
            };

            CheckOutModel viewModel = new()
            {
                addresses = addressList,
                confirmOrderModel = confirmOrderModel
            };

            return View("ConfirmOrder", viewModel);

        }

        [AuthorizeCustomer]
        [Route("Cart")]
        public async Task<IActionResult> Cart()
        {
            HttpClient client = new HttpClient();
            string TokensString = _HttpContextAccessor.HttpContext.Session.GetString("AuthData");
            //if (string.IsNullOrEmpty(TokensString))
            //{
            //    TempData["msg"] = "Session Expired, Login first";
            //    TempData["code"] = "403";
            //    return RedirectToAction("Login", "Home");
            //}
            AuthData authData = JsonConvert.DeserializeObject<AuthData>(TokensString);
            int UserId = authData.UserId;
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", authData.AccessToken);

            //int UserId = _HttpContextAccessor.HttpContext.Session.GetInt32("UserId") == null ?
            //               1 :
            //               (int)_HttpContextAccessor.HttpContext.Session.GetInt32("UserId");
            using (var apiResponce = await client.GetAsync("https://localhost:7185/api/v1/Cart/getcartforuser/" + UserId))
            {
                if (apiResponce.StatusCode == HttpStatusCode.OK)
                {
                    var resString = await apiResponce.Content.ReadAsStringAsync();
                    _HttpContextAccessor.HttpContext.Session.SetString("Cart", resString);

                    CartModel model = JsonConvert.DeserializeObject<CartModel>(resString);
                    return View(model);
                }
                else if (apiResponce.StatusCode == HttpStatusCode.NoContent)
                {
                    ViewData["EmptyCartMessage"] = "Cart is Empty";
                    return View();
                }

                else
                {
                    ViewData["ErrorMessage"] = $"<h4> b {apiResponce.StatusCode}</h4><br><b>---</b><br> {await apiResponce.Content.ReadAsStringAsync()}";
                    return View("Error");
                }
            }

        }

        [AuthorizeCustomer]
        [Route("Orders")]
        public async Task<IActionResult> Orders(string? filter)
        {
            HttpClient client = new HttpClient();
            string TokensString = _HttpContextAccessor.HttpContext.Session.GetString("AuthData");
            //if (string.IsNullOrEmpty(TokensString))
            //{
            //    TempData["msg"] = "Session Expired, Login first";
            //    TempData["code"] = "403";
            //    return RedirectToAction("Login", "Home");
            //}
            AuthData authData = JsonConvert.DeserializeObject<AuthData>(TokensString);
            int UserId = authData.UserId;
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", authData.AccessToken);

            //int UserId = _HttpContextAccessor.HttpContext.Session.GetInt32("UserId") == null ?
            //               1 :
            //               (int)_HttpContextAccessor.HttpContext.Session.GetInt32("UserId");
            using (var apiResponce = await client.GetAsync("https://localhost:7185/api/v1/Order/GetOrderDetailsForUser/ " + UserId))
            {
                if (apiResponce.StatusCode == HttpStatusCode.OK)
                {
                    var resString = await apiResponce.Content.ReadAsStringAsync();
                    //   return Content(resString);

                    //    _HttpContextAccessor.HttpContext.Session.SetString("Cart", resString);

                    List<OrderViewModel> model = JsonConvert.DeserializeObject<List<OrderViewModel>>(resString);
                    if (!string.IsNullOrEmpty(filter))
                    {
                        List<OrderViewModel> oldList = new();
                        oldList.AddRange(model);
                        model.Clear();
                        model.AddRange(from o in oldList
                                       where o.OrderStatus == filter
                                       select o);
                    }
                    return View(model);
                }
                else
                {
                    ViewData["ErrorMessage"] = $"<h4> b {apiResponce.StatusCode}</h4><br><b>---</b><br> {await apiResponce.Content.ReadAsStringAsync()}";
                    return View("Error");
                }
            }
        }

        [AuthorizeCustomer]
        [Route("CancelOrder")]
        public async Task<IActionResult> CancelOrder(int id)
        {

            HttpClient client = new HttpClient();
            string TokensString = _HttpContextAccessor.HttpContext.Session.GetString("AuthData");
            //if (string.IsNullOrEmpty(TokensString))
            //{
            //    TempData["msg"] = "Session Expired, Login first";
            //    TempData["code"] = "403";
            //    return RedirectToAction("Login", "Home");
            //}
            AuthData authData = JsonConvert.DeserializeObject<AuthData>(TokensString);
            int UserId = authData.UserId;
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", authData.AccessToken);
            
            //int UserId = _HttpContextAccessor.HttpContext.Session.GetInt32("UserId") == null ?
            //              1 :
            //              (int)_HttpContextAccessor.HttpContext.Session.GetInt32("UserId");

            using (var apiResponce = await client.PutAsync("https://localhost:7185/api/v1/Order/SetOrderCancelled/" + id, null))
            {
                if (apiResponce.StatusCode == HttpStatusCode.OK)
                {
                    //   TempData["msg"] = "Order Cancelled";
                    return RedirectToAction("Orders");
                }
                else
                {
                    ViewData["ErrorMessage"] = $"<h4> b {apiResponce.StatusCode}</h4><br><b>---</b><br> {await apiResponce.Content.ReadAsStringAsync()}";
                    return View("Error");
                }
            }
        }

        [AuthorizeCustomer]
        [Route("Pay")]
        public async Task<IActionResult> PaymentPage()
        {
            int UserId = _HttpContextAccessor.HttpContext.Session.GetInt32("UserId") == null ?
                1 :
                (int)_HttpContextAccessor.HttpContext.Session.GetInt32("UserId");

            List<CardDetail> cardsList = new();

            HttpClient client = new HttpClient();
            string TokensString = _HttpContextAccessor.HttpContext.Session.GetString("AuthData");
            //if (string.IsNullOrEmpty(TokensString))
            //{
            //    TempData["msg"] = "Session Expired, Login first";
            //    TempData["code"] = "403";
            //    return RedirectToAction("Login", "Home");
            //}
            AuthData authData = JsonConvert.DeserializeObject<AuthData>(TokensString);
            UserId = authData.UserId;
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", authData.AccessToken);

            try
            {
                using (var apiResponce = await client.GetAsync("https://localhost:7185/api/v1/User/GetCardDetails/User/" + UserId))
                {
                    if (apiResponce.StatusCode == HttpStatusCode.OK)
                    {
                        var resString = await apiResponce.Content.ReadAsStringAsync();
                        cardsList = JsonConvert.DeserializeObject<List<CardDetail>>(resString);
                    }
                    else if (apiResponce.StatusCode == HttpStatusCode.NotFound)
                    {
                        cardsList = new List<CardDetail>();
                    }
                    else
                    {
                        //          return View("Index");
                        //Console.WriteLine("--------------------api res" + apiResponce.StatusCode);
                        ViewData["ErrorMessage"] = $"<h4> item { apiResponce.StatusCode }</h4><br><b>---</b><br> {await apiResponce.Content.ReadAsStringAsync()}";
                        return View("Error");
                    }
                }
            }
            catch (Exception ex)
            {

                //          return View("Index");
                //Console.WriteLine("--------------------api res" + apiResponce.StatusCode);
                ViewData["ErrorMessage"] = $"<h4> item { ex.GetType }</h4><br><b>---</b><br> {ex.StackTrace}";
                return View("Error");
            }

            PaymentModel model = new()
            {
                cards = cardsList
            };

            ViewData["OrderId"] = TempData["oid"];
            return View(model);
        }

        //public async Task<IActionResult> Checkout()
        //{
        //    List<Address> addressList = new();
        //    List<CardDetail> cardsList = new();

        //    HttpClient client = new HttpClient();
        //    //string TokensString = _HttpContextAccessor.HttpContext.Session.GetString("AuthData");
        //    //if (string.IsNullOrEmpty(TokensString))
        //    //{
        //    //    TempData["msg"] = "Session Expired, Login first";
        //    //    TempData["code"] = "403";
        //    //    return RedirectToAction("Login", "Home");
        //    //}
        //    //AuthData authData = JsonConvert.DeserializeObject<AuthData>(TokensString);
        //    //int userId = authData.UserId;
        //    //client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", authData.AccessToken);

        //    int UserId = _HttpContextAccessor.HttpContext.Session.GetInt32("UserId") == null ?
        //                   1 :
        //                   (int)_HttpContextAccessor.HttpContext.Session.GetInt32("UserId");

        //    using (var apiResponce = await client.GetAsync("https://localhost:7185/api/v1/User/GetAddress/User/ " + UserId))
        //    {
        //        if (apiResponce.StatusCode == HttpStatusCode.OK)
        //        {
        //            var resString = await apiResponce.Content.ReadAsStringAsync();
        //            //   return Content(resString);

        //            //    _HttpContextAccessor.HttpContext.Session.SetString("Cart", resString);

        //            addressList = JsonConvert.DeserializeObject<List<Address>>(resString);
        //        }
        //        else
        //        {
        //            ViewData["ErrorMessage"] = $"<h4> {apiResponce.StatusCode}</h4><br><b>---</b><br> {await apiResponce.Content.ReadAsStringAsync()}";
        //            return View("Error");
        //        }
        //    }

        //    using (var apiResponce = await client.GetAsync("https://localhost:7185/api/v1/User/GetCardDetails/User/" + UserId))
        //    {
        //        if (apiResponce.StatusCode == HttpStatusCode.OK)
        //        {
        //            var resString = await apiResponce.Content.ReadAsStringAsync();
        //            //   return Content(resString);

        //            //    _HttpContextAccessor.HttpContext.Session.SetString("Cart", resString);

        //            cardsList = JsonConvert.DeserializeObject<List<CardDetail>>(resString);
        //        }
        //        else
        //        {
        //            ViewData["ErrorMessage"] = $"<h4> b {apiResponce.StatusCode}</h4><br><b>---</b><br> {await apiResponce.Content.ReadAsStringAsync()}";
        //            return View("Error");
        //        }
        //    }


        //    CheckOutModel model = new()
        //    {

        //        addresses = addressList,
        //        //   cards = cardsList
        //    };

        //    return View(model);
        //}

        [Route("MakePayment")]
        public async Task<IActionResult> MakePayment()
        {
         //   return Content("hehe");

            string oid = _HttpContextAccessor.HttpContext.Session.GetString("OrderId");
            //return RedirectToAction("PaymentPage");
            
            HttpClient client = new HttpClient();
            string TokensString = _HttpContextAccessor.HttpContext.Session.GetString("AuthData");
            //if (string.IsNullOrEmpty(TokensString))
            //{
            //    TempData["msg"] = "Session Expired, Login first";
            //    TempData["code"] = "403";
            //    return RedirectToAction("Login", "Home");
            //}
            AuthData authData = JsonConvert.DeserializeObject<AuthData>(TokensString);
            int userId = authData.UserId;
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", authData.AccessToken);

            JsonContent content = JsonContent.Create(new RemarkDto()
            {
                Remark = "Paid By Card"
            });

            try
            {
                using (var apiResponce = await client.PutAsync("https://localhost:7185/api/v1/Order/SetRemarkPaid/" + oid, content))
                {
                    if (apiResponce.StatusCode == HttpStatusCode.OK)
                    {
                        //   TempData["msg"] = "Order Cancelled";
                        _HttpContextAccessor.HttpContext.Session.Remove("OrderId");
                        return RedirectToAction("Orders");
                    }
                    else
                    {
                        ViewData["ErrorMessage"] = $"<h4> {apiResponce.StatusCode}</h4><br><b>---</b><br> {await apiResponce.Content.ReadAsStringAsync()}";
                        return View("Error");
                    }
                }
            }
            catch (Exception e)
            {

                ViewData["ErrorMessage"] = $"<h4> b {e.GetType}</h4><br><b>---</b><br> {e.StackTrace}";
                return View("Error");

            }

        }

        [Route("MakePaymentCod")]
        public async Task<IActionResult> MakePaymentCod()
        {
            string oid = _HttpContextAccessor.HttpContext.Session.GetString("OrderId");
            //return RedirectToAction("PaymentPage");
            HttpClient client = new HttpClient();
            string TokensString = _HttpContextAccessor.HttpContext.Session.GetString("AuthData");
            //if (string.IsNullOrEmpty(TokensString))
            //{
            //    TempData["msg"] = "Session Expired, Login first";
            //    TempData["code"] = "403";
            //    return RedirectToAction("Login", "Home");
            //}
            AuthData authData = JsonConvert.DeserializeObject<AuthData>(TokensString);
            int userId = authData.UserId;
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", authData.AccessToken);

            JsonContent content = JsonContent.Create(new RemarkDto()
            {
                Remark = "Cash on delivary"
            });

            try
            {
                using (var apiResponce = await client.PutAsync("https://localhost:7185/api/v1/Order/SetRemarkPaid/" + oid, content))
                {
                    if (apiResponce.StatusCode == HttpStatusCode.OK)
                    {
                        //   TempData["msg"] = "Order Cancelled";
                        _HttpContextAccessor.HttpContext.Session.Remove("OrderId");
                        return RedirectToAction("Orders");
                    }
                    else
                    {
                        ViewData["ErrorMessage"] = $"<h4> {apiResponce.StatusCode}</h4><br><b>---</b><br> {await apiResponce.Content.ReadAsStringAsync()}";
                        return View("Error");
                       }
                }
            }
            catch (Exception e)
            {

                ViewData["ErrorMessage"] = $"<h4> b {e.GetType}</h4><br><b>---</b><br> {e.StackTrace}";
                return View("Error");

            }

        }

        [Route("MakePaymentUPI")]
        public async Task<IActionResult> MakePaymentUPI()
        {
            string oid = _HttpContextAccessor.HttpContext.Session.GetString("OrderId");
            //return RedirectToAction("PaymentPage");

            HttpClient client = new HttpClient();
            string TokensString = _HttpContextAccessor.HttpContext.Session.GetString("AuthData");
            //if (string.IsNullOrEmpty(TokensString))
            //{
            //    TempData["msg"] = "Session Expired, Login first";
            //    TempData["code"] = "403";
            //    return RedirectToAction("Login", "Home");
            //}
            AuthData authData = JsonConvert.DeserializeObject<AuthData>(TokensString);
            int UserId = authData.UserId;
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", authData.AccessToken);

            //int UserId = _HttpContextAccessor.HttpContext.Session.GetInt32("UserId") == null ?
            //               1 :
            //               (int)_HttpContextAccessor.HttpContext.Session.GetInt32("UserId");

            JsonContent content = JsonContent.Create(new RemarkDto()
            {
                Remark = "Paid By UPI"
            });

            try
            {
                using (var apiResponce = await client.PutAsync("https://localhost:7185/api/v1/Order/SetRemarkPaid/" + oid, content))
                {
                    if (apiResponce.StatusCode == HttpStatusCode.OK)
                    {
                        //   TempData["msg"] = "Order Cancelled";
                        _HttpContextAccessor.HttpContext.Session.Remove("OrderId");
                        return RedirectToAction("Orders");
                    }
                    else
                    {
                        ViewData["ErrorMessage"] = $"<h4> {apiResponce.StatusCode}</h4><br><b>---</b><br> {await apiResponce.Content.ReadAsStringAsync()}";
                        return View("Error");
                    }
                }
            }
            catch (Exception e)
            {

                ViewData["ErrorMessage"] = $"<h4> b {e.GetType}</h4><br><b>---</b><br> {e.StackTrace}";
                return View("Error");

            }

        }


        [AuthorizeCustomer]
        [Route("PutOnlinePaymentOrder")]
        public async Task<IActionResult> PutOnlinePaymentOrder(Address DelivaryAddresses, string Payment)
        {

            Payment = "Online";
            //CheckOutModel model = JsonConvert.DeserializeObject<CheckOutModel>(_HttpContextAccessor.HttpContext.Session.GetString("CheckOutModel"));
            string itemString = _HttpContextAccessor.HttpContext.Session.GetString("OrderItems");
            List<OrderItemDto> OrderItems = JsonConvert.DeserializeObject<List<OrderItemDto>>(itemString);
            //return Content(JsonConvert.SerializeObject(DelivaryAddresses) + "\n\n\n\n" + itemString + "\n\n\n" + Payment);

            HttpClient client = new HttpClient();
            string TokensString = _HttpContextAccessor.HttpContext.Session.GetString("AuthData");
            //if (string.IsNullOrEmpty(TokensString))
            //{
            //    TempData["msg"] = "Session Expired, Login first";
            //    TempData["code"] = "403";
            //    return RedirectToAction("Login", "Home");
            //}
            AuthData authData = JsonConvert.DeserializeObject<AuthData>(TokensString);
            int UserId = authData.UserId;
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", authData.AccessToken);

            //int UserId = _HttpContextAccessor.HttpContext.Session.GetInt32("UserId") == null ?
            //               1 :
            //               (int)_HttpContextAccessor.HttpContext.Session.GetInt32("UserId");

            string address = $"{DelivaryAddresses.HouseNo}, " +
                $"{DelivaryAddresses.StreetArea}, " +
                $"{DelivaryAddresses.City}, " +
                $"{DelivaryAddresses.State}, " +
                $"{DelivaryAddresses.Pincode}.";


            string orderRemark = "Payment Pending";

            AddOrderDto OrderDto = new()
            {
                Address = address,
                OrderItems = OrderItems,
                OrderStatus = "Processing",
                Remark = orderRemark,
                TotalQty = OrderItems.Count,
                UserId = UserId
            };

            JsonContent content = JsonContent.Create(OrderDto);
            // return Content(JsonConvert.SerializeObject(OrderDto));
            using (var apiResponce = await client.PostAsync("https://localhost:7185/api/v1/Order/AddOrder", content))
            {
                if (apiResponce.StatusCode == HttpStatusCode.OK)
                {
                    //TempData["msg"] = "Order Place Successfully";
                    if (Payment == "Cod")
                    {
                        return RedirectToAction("Orders");
                    }
                    else
                    {
                        string oid = await apiResponce.Content.ReadAsStringAsync();
                        _HttpContextAccessor.HttpContext.Session.SetString("OrderId", oid);
                        return RedirectToAction("PaymentPage");
                    }
                }
                else
                {
                    TempData["ErrMsg"] = "Api Error: " + apiResponce.StatusCode;
                    //          return View("Index");
                    //Console.WriteLine("--------------------api res" + apiResponce.StatusCode);
                    ViewData["ErrorMessage"] = $"<h4> { apiResponce.StatusCode }</h4><br><b>---</b><br> {await apiResponce.Content.ReadAsStringAsync()}";
                    return View("Error");
                }
            }

        }

        [AuthorizeCustomer]
        [Route("PutCodOrder")]
        public async Task<IActionResult> PutCodOrder(Address DelivaryAddresses, string Payment)
        {

            Payment = "Cod";
            //CheckOutModel model = JsonConvert.DeserializeObject<CheckOutModel>(_HttpContextAccessor.HttpContext.Session.GetString("CheckOutModel"));
            string itemString = _HttpContextAccessor.HttpContext.Session.GetString("OrderItems");
            List<OrderItemDto> OrderItems = JsonConvert.DeserializeObject<List<OrderItemDto>>(itemString);
            //return Content(JsonConvert.SerializeObject(DelivaryAddresses) + "\n\n\n\n" + itemString + "\n\n\n" + Payment);

            HttpClient client = new HttpClient();
            string TokensString = _HttpContextAccessor.HttpContext.Session.GetString("AuthData");
            //if (string.IsNullOrEmpty(TokensString))
            //{
            //    TempData["msg"] = "Session Expired, Login first";
            //    TempData["code"] = "403";
            //    return RedirectToAction("Login", "Home");
            //}
            AuthData authData = JsonConvert.DeserializeObject<AuthData>(TokensString);
            int UserId = authData.UserId;
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", authData.AccessToken);

            //int UserId = _HttpContextAccessor.HttpContext.Session.GetInt32("UserId") == null ?
            //               1 :
            //               (int)_HttpContextAccessor.HttpContext.Session.GetInt32("UserId");

            string address = $"{DelivaryAddresses.HouseNo}, " +
                $"{DelivaryAddresses.StreetArea}, " +
                $"{DelivaryAddresses.City}, " +
                $"{DelivaryAddresses.State}, " +
                $"{DelivaryAddresses.Pincode}.";


            string orderRemark = "Cash On delivary";

            AddOrderDto OrderDto = new()
            {
                Address = address,
                OrderItems = OrderItems,
                OrderStatus = "Processing",
                Remark = orderRemark,
                TotalQty = OrderItems.Count,
                UserId = UserId
            };

            JsonContent content = JsonContent.Create(OrderDto);
            // return Content(JsonConvert.SerializeObject(OrderDto));
            using (var apiResponce = await client.PostAsync("https://localhost:7185/api/v1/Order/AddOrder", content))
            {
                if (apiResponce.StatusCode == HttpStatusCode.OK)
                {
                    //TempData["msg"] = "Order Place Successfully";
                    if (Payment == "Cod")
                    {
                        return RedirectToAction("Orders");
                    }
                    else
                    {
                        string oid = await apiResponce.Content.ReadAsStringAsync();
                        _HttpContextAccessor.HttpContext.Session.SetString("OrderId", oid);
                        return RedirectToAction("PaymentPage");
                    }
                }
                else
                {
                    TempData["ErrMsg"] = "Api Error: " + apiResponce.StatusCode;
                    //          return View("Index");
                    //Console.WriteLine("--------------------api res" + apiResponce.StatusCode);
                    ViewData["ErrorMessage"] = $"<h4> { apiResponce.StatusCode }</h4><br><b>---</b><br> {await apiResponce.Content.ReadAsStringAsync()}";
                    return View("Error");
                }
            }

        }


    }
}
