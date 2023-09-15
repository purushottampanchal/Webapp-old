using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net;
using WebApp.CustomAttribute;
using WebApp.Models;
using WebApp.Models.AuthenticationModels;
using WebApp.Models.General;
using WebApp.Models.Order;
using System.Linq;

namespace WebApp.Controllers
{

    [AuthorizeAdmin]
    public class AdminController : Controller
    {
        private readonly IHttpContextAccessor _HttpContextAccessor;

        public AdminController(IHttpContextAccessor httpContextAccessor)
        {
            this._HttpContextAccessor = httpContextAccessor;
        }

        public async Task<IActionResult> Index()
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
                    if (apiResponce.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        string result = await apiResponce.Content.ReadAsStringAsync();
                        CatalogItems = JsonConvert.DeserializeObject<List<CatalogItem>>(result);
                    }
                    else
                    {
                        ViewData["ErrorMessage"] = $"<h4>  { apiResponce.StatusCode }</h4><br><b>---</b><br> {apiResponce.Content}";
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
                    if (apiResponce.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        string result = await apiResponce.Content.ReadAsStringAsync();
                        CatalogList = JsonConvert.DeserializeObject<List<Catalog>>(result);
                    }
                    else
                    {
                        ViewData["ErrorMessage"] = $"<h4>  { apiResponce.StatusCode }</h4><br><b>---</b><br> {apiResponce.Content}";
                        return View("Error");
                    }
                }

            }
            catch (Exception e)
            {
                ViewData["ErrorMessage"] = "Exception- " + e.Message + "<br>stackTrace<br>" + e.StackTrace;
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
                        ViewData["ErrorMessage"] = $"<h4>  { apiResponce.StatusCode }</h4><br><b>---</b><br> {apiResponce.Content}";
                        return View("Error");
                    }
                }

            }
            catch (Exception e)
            {

                ViewData["ErrorMessage"] = "Exception- " + e.Message + "<br>stackTrace<br>" + e.StackTrace;
                return View("Error");
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

        public async Task<IActionResult> EditItem(int id)
        {
            CatalogItem item;
            List<Catalog> catalogs;
            List<Brand> brands;

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



            try
            {
                using (var apiResponce = await client.GetAsync("https://localhost:7185/api/v1/Catlog/Items/" + id))
                {
                    if (apiResponce.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        string resultString = await apiResponce.Content.ReadAsStringAsync();
                        item = JsonConvert.DeserializeObject<CatalogItem>(resultString);
                    }
                    else
                    {
                        ViewData["ErrorMessage"] = $"<h4>  { apiResponce.StatusCode }</h4><br><b>---</b><br> {apiResponce.Content}";
                        return View("Error");
                    }
                }

                using (var apiResponce = await client.GetAsync("https://localhost:7185/api/v1/Catlog/catlogtypes"))
                {
                    if (apiResponce.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        string resultString = await apiResponce.Content.ReadAsStringAsync();
                        catalogs = JsonConvert.DeserializeObject<List<Catalog>>(resultString);
                    }
                    else
                    {
                        ViewData["ErrorMessage"] = $"<h4>  { apiResponce.StatusCode }</h4><br><b>---</b><br> {apiResponce.Content}";
                        return View("Error");
                    }
                }

                using (var apiResponce = await client.GetAsync("https://localhost:7185/api/v1/Catlog/catlogBrands"))
                {
                    if (apiResponce.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        string resultString = await apiResponce.Content.ReadAsStringAsync();
                        brands = JsonConvert.DeserializeObject<List<Brand>>(resultString);
                    }
                    else
                    {
                        ViewData["ErrorMessage"] = $"<h4>  { apiResponce.StatusCode }</h4><br><b>---</b><br> {apiResponce.Content}";
                        return View("Error");
                    }
                }

            }
            catch (Exception e)
            {
                ViewData["ErrorMessage"] = "Exception- " + e.Message + "<br>stackTrace<br>" + e.StackTrace;
                return View("Error");
            }

            CompositSingleProductModel model = new()
            {
                Brands = brands,
                Catalogs = catalogs,
                Item = item
            };

            return View(model);
        }
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
            int userId = authData.UserId;
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", authData.AccessToken);

            try
            {
                using (var apiResponce = await client.GetAsync("https://localhost:7185/api/v1/Catlog/Items/" + id))
                {
                    if (apiResponce.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        string resultString = await apiResponce.Content.ReadAsStringAsync();
                        item = JsonConvert.DeserializeObject<CatalogItem>(resultString);
                    }
                    else
                    {
                        ViewData["ErrorMessage"] = $"<h4>  { apiResponce.StatusCode }</h4><br><b>---</b><br> {apiResponce.Content}";
                        return View("Error");
                    }
                }
            }
            catch (Exception e)
            {
                ViewData["ErrorMessage"] = "Exception- " + e.Message + "<br>stackTrace<br>" + e.StackTrace;
                return View("Error");

            }

            return View(item);

        }

        [HttpPost]
        public async Task<IActionResult> Save(CatalogItem item)
        {

            // return Content(item.ToString());
            //Console.WriteLine("Posted Item ===========" + item.ToString());
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

            var content = JsonContent.Create(item);

            try
            {
                using (var apiResponce = await client.PutAsync("https://localhost:7185/api/v1/Catlog/Items", content))
                {
                    var res = apiResponce.StatusCode;
                    if (res != System.Net.HttpStatusCode.OK)
                    {
                        ViewData["ErrorMessage"] = $"<h4>  { apiResponce.StatusCode }</h4><br><b>---</b><br> {apiResponce.Content}";
                        return View("Error");
                    }

                }
            }
            catch (Exception e)
            {
                ViewData["ErrorMessage"] = "Exception- " + e.Message + "<br>stackTrace<br>" + e.StackTrace;
                return View("Error");

            }
            TempData["Message"] = $"Item with id {item.Id} has been updated ";
            return RedirectToAction("Index");

        }


        public async Task<IActionResult> DeleteItem(int id)
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

            try
            {
                using (var apiResponce = await client.DeleteAsync("https://localhost:7185/api/v1/Catlog/Delete/" + id))
                {
                    var res = apiResponce.StatusCode;
                    if (res != System.Net.HttpStatusCode.OK)
                    {
                        ViewData["ErrorMessage"] = "Error- " + apiResponce.StatusCode;

                        return View("Error");
                    }
                }
            }
            catch (Exception e)
            {
                ViewData["ErrorMessage"] = "Exception- " + e.Message + "<br>stackTrace<br>" + e.StackTrace;
                return View("Error");
            }
            TempData["Message"] = $"Item with id {id} has been deleted from database";
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> AddItem()
        {
            List<Catalog> catalogs;
            List<Brand> brands;
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

            try
            {

                using (var apiResponce = await client.GetAsync("https://localhost:7185/api/v1/Catlog/catlogtypes"))
                {
                    if (apiResponce.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        string resultString = await apiResponce.Content.ReadAsStringAsync();
                        catalogs = JsonConvert.DeserializeObject<List<Catalog>>(resultString);
                    }
                    else
                    {
                        ViewData["ErrorMessage"] = $"<h4>  { apiResponce.StatusCode }</h4><br><b>---</b><br> {apiResponce.Content}";
                        return View();
                    }
                }

                using (var apiResponce = await client.GetAsync("https://localhost:7185/api/v1/Catlog/catlogBrands"))
                {
                    if (apiResponce.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        string resultString = await apiResponce.Content.ReadAsStringAsync();
                        brands = JsonConvert.DeserializeObject<List<Brand>>(resultString);
                    }
                    else
                    {
                        ViewData["ErrorMessage"] = $"<h4>  { apiResponce.StatusCode }</h4><br><b>---</b><br> {apiResponce.Content}";
                        return View();
                    }
                }

            }
            catch (Exception e)
            {
                ViewData["ErrorMessage"] = "Exception- " + e.Message + "<br>stackTrace<br>" + e.StackTrace;
                return View("Error");
            }

            CompositAddProductModel model = new()
            {
                Brands = brands,
                Catalogs = catalogs,
                Item = null
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> AddItem(CatalogItemDto item)
        {

            if (!ModelState.IsValid)
            {
                ViewData["ErrorMessage"] = "Enter all fields";
                return RedirectToAction();
            }

            // return Content(item.ToString());

//            Console.WriteLine(item.ToString());
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

            var content = JsonContent.Create(item);

            try
            {
                using (var apiResponce = await client.PostAsync("https://localhost:7185/api/v1/Catlog/Items", content))
                {
                    var res = apiResponce.StatusCode;
                    if (res != System.Net.HttpStatusCode.OK)
                    {
                        ViewData["ErrorMessage"] = $"{ apiResponce.StatusCode } -- {await apiResponce.Content.ReadAsStringAsync()}";
                        Console.WriteLine(await apiResponce.Content.ReadAsStringAsync());
                        return RedirectToAction("AddItem");
                    }

                }
            }
            catch (Exception e)
            {
                ViewData["ErrorMessage"] = "Exception- " + e.Message + "<br>stackTrace<br>" + e.StackTrace;
                return View("Error");

            }

            TempData["Message"] = $"new item - {item.Name} added successfully ";
            return RedirectToAction("Index");
        }


        [HttpPost]
        public async Task<IActionResult> AddBrand(string brand)
        {
            if (brand == null)
            {
                TempData["ErrorMessage"] = "Brand Name cannot be empty";
                return RedirectToAction("Index");

            }
            var item = new BrandDto()
            {
                Name = brand
            };

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

            var content = JsonContent.Create(item);

            try
            {
                using (var apiResponce = await client.PostAsync("https://localhost:7185/api/v1/Catlog/AddBrand", content))
                {
                    var res = apiResponce.StatusCode;
                    if (res != System.Net.HttpStatusCode.OK)
                    {
                        ViewData["ErrorMessage"] = $"{ apiResponce.StatusCode }---{apiResponce.Content}";
                        return RedirectToAction("Index");

                    }

                }
            }
            catch (Exception e)
            {
                ViewData["ErrorMessage"] = "Exception- " + e.Message + "<br>stackTrace<br>" + e.StackTrace;
                return View("Error");

            }
            TempData["Message"] = $"New Brand {brand} has been Added ";
            return RedirectToAction("Index");

        }


        [HttpPost]
        public async Task<IActionResult> AddCatalog(string catalog)
        {

            if (catalog == null)
            {
                TempData["ErrorMessage"] = "Brand Name cannot be empty";
                return RedirectToAction("Index");

            }
            var item = new CatalogDto()
            {
                Name = catalog
            };

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

            var content = JsonContent.Create(item);

            try
            {
                using (var apiResponce = await client.PostAsync("https://localhost:7185/api/v1/Catlog/AddCatalog", content))
                {
                    var res = apiResponce.StatusCode;
                    if (res != System.Net.HttpStatusCode.OK)
                    {
                        ViewData["ErrorMessage"] = $"{ apiResponce.StatusCode }---{apiResponce.Content}";
                        return RedirectToAction("Index");

                    }

                }
            }
            catch (Exception e)
            {
                ViewData["ErrorMessage"] = "Exception- " + e.Message + "<br>stackTrace<br>" + e.StackTrace;
                return View("Error");

            }
            TempData["Message"] = $"New catalog type {catalog} has been Added ";
            return RedirectToAction("Index");

        }


        public async Task<IActionResult> ViewAllOrders(string? filter)
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


            using (var apiResponce = await client.GetAsync("https://localhost:7185/api/v1/Order/GetAllOrders"))
            {
                if (apiResponce.StatusCode == HttpStatusCode.OK)
                {
                    var resString = await apiResponce.Content.ReadAsStringAsync();
                    //   return Content(resString);

                    //    _HttpContextAccessor.HttpContext.Session.SetString("Cart", resString);

                    List<OrderViewModel> m = JsonConvert.DeserializeObject<List<OrderViewModel>>(resString);

                    if (!string.IsNullOrEmpty(filter))
                    {
                        List<OrderViewModel> oldList = new();
                        oldList.AddRange(m);
                        m.Clear();
                        m.AddRange(from o in oldList
                                   where o.OrderStatus == filter
                                   select o);
                    }
                    return View(m);
                }
                else
                {
                    ViewData["ErrorMessage"] = $"<h4> b {apiResponce.StatusCode}</h4><br><b>---</b><br> {await apiResponce.Content.ReadAsStringAsync()}";
                    return View("Error");
                }
            }
        }


        public async Task<IActionResult> Receipt(int Id)
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


            using (var apiResponce = await client.GetAsync("https://localhost:7185/api/v1/Order/GetOrderDetails/"+Id))
            {
                if (apiResponce.StatusCode == HttpStatusCode.OK)
                {
                    var resString = await apiResponce.Content.ReadAsStringAsync();
                    //   return Content(resString);

                    //    _HttpContextAccessor.HttpContext.Session.SetString("Cart", resString);

                    ReceiptModel m = JsonConvert.DeserializeObject<ReceiptModel>(resString);
                    return View(m);
                }
                else
                {
                    ViewData["ErrorMessage"] = $"<h4> b {apiResponce.StatusCode}</h4><br><b>---</b><br> {await apiResponce.Content.ReadAsStringAsync()}";
                    return View("Error");
                }
            }
        }

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
            int userId = authData.UserId;
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", authData.AccessToken);


            using (var apiResponce = await client.PutAsync("https://localhost:7185/api/v1/Order/SetOrderCancelled/" + id, null))
            {
                if (apiResponce.StatusCode == HttpStatusCode.OK)
                {
                    return RedirectToAction("ViewAllOrders");
                }
                else
                {
                    ViewData["ErrorMessage"] = $"<h4> b {apiResponce.StatusCode}</h4><br><b>---</b><br> {await apiResponce.Content.ReadAsStringAsync()}";
                    return View("Error");
                }
            }
        }
        public async Task<IActionResult> DeliverOrder(int id)
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


            using (var apiResponce = await client.PutAsync("https://localhost:7185/api/v1/Order/SetOrderDelivered/" + id, null))
            {
                if (apiResponce.StatusCode == HttpStatusCode.OK)
                {
                    return RedirectToAction("ViewAllOrders");
                }
                else
                {
                    ViewData["ErrorMessage"] = $"<h4> b {apiResponce.StatusCode}</h4><br><b>---</b><br> {await apiResponce.Content.ReadAsStringAsync()}";
                    return View("Error");
                }
            }
        }


        public async Task<IActionResult> ShipedOrder(int id)
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


            using (var apiResponce = await client.PutAsync("https://localhost:7185/api/v1/Order/SetOrderShipped/" + id, null))
            {
                if (apiResponce.StatusCode == HttpStatusCode.OK)
                {
                    return RedirectToAction("ViewAllOrders");
                }
                else
                {
                    ViewData["ErrorMessage"] = $"<h4> b {apiResponce.StatusCode}</h4><br><b>---</b><br> {await apiResponce.Content.ReadAsStringAsync()}";
                    return View("Error");
                }
            }
        }


    }
}
