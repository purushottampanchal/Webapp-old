using shoppingApi.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using WebApp.Models;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Identity;
using WebApp.Models.General;
using System.Net;
using WebApp.Models.AuthenticationModels;
using WebApp.CustomAttribute;

namespace WebApp.Controllers
{
    public class HomeController : Controller
    {
        //  private readonly ILogger<HomeController> _logger;
        private readonly IHttpContextAccessor _HttpContextAccessor;

        public HomeController(IHttpContextAccessor contextAccessor)
        {
            _HttpContextAccessor = contextAccessor;
        }



        public IActionResult Logout()
        {
            _HttpContextAccessor.HttpContext.Session.Clear();
            return RedirectToAction("Index");
        }
        public IActionResult Login()
        {
            if (HttpContext.Session.GetString("AuthData") != null)
            {
                return RedirectToAction("Index", "Catalog");
            }
            return View();
        }
        public IActionResult AdminLogin()
        {
            return RedirectToAction("Login");
            //if(!string.IsNullOrEmpty(HttpContext.Session.GetString("AuthData")))
            //{
            //    return RedirectToAction("Index", "Admin");
            //}
            //return View();
        }

        [AuthorizeCustomer]

        public async Task<IActionResult> AddCard(IFormCollection col)
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

            CardDto cardDto = new ()
            {
                UserId = userId,
                CardNo = col["CardNo"],
                NameOnCard = col["NameOnCard"],
                CVV = col["cvv"],
                Expiration = col["Expiration"],
            };

            JsonContent content = JsonContent.Create(cardDto);
            try
            {
                using (var apiResponce = await client.PostAsync("https://localhost:7185/api/v1/User/AddCard/User", content))
                {
                    if (apiResponce.StatusCode == HttpStatusCode.OK)
                    {
                        return RedirectToAction("Profile");
                    }
                    else
                    {
                        ViewData["ErrorMessage"] = $"<h4> API Error While adding new card {apiResponce.StatusCode}</h4><br><b>---</b><br> {apiResponce.Content}";
                        return View("Error");

                    }
                }

            }
            catch (Exception e)
            {
                ViewData["ErrorMessage"] = "Exception- " + e.Message + "<br>stackTrace<br>" + e.StackTrace;
                return View("Error");
            }
        }

        [AuthorizeCustomer]

        public async Task<IActionResult> DeleteAddress(int Id)
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
                using (var apiResponce = await client.DeleteAsync("https://localhost:7185/api/v1/User/RemoveAddress/User/"+Id))
                {
                    if (apiResponce.StatusCode == HttpStatusCode.OK)
                    {
                        return RedirectToAction("Profile");
                    }
                    else
                    {
                        ViewData["ErrorMessage"] = $"<h4> API Error While Deleting address {apiResponce.StatusCode}</h4><br><b>---</b><br> {apiResponce.Content}";
                        return View("Error");

                    }
                }

            }
            catch (Exception e)
            {
                ViewData["ErrorMessage"] = "Exception- " + e.Message + "<br>stackTrace<br>" + e.StackTrace;
                return View("Error");
            }
        }

        [AuthorizeCustomer]

        public async Task<IActionResult> DeleteCard(int Id)
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


            try
            {
                using (var apiResponce = await client.DeleteAsync("https://localhost:7185/api/v1/User/RemoveCard/User/"+Id))
                {
                    if (apiResponce.StatusCode == HttpStatusCode.OK)
                    {
                        return RedirectToAction("Profile");
                    }
                    else
                    {
                        ViewData["ErrorMessage"] = $"<h4> API Error While Deleting address {apiResponce.StatusCode}</h4><br><b>---</b><br> {apiResponce.Content}";
                        return View("Error");

                    }
                }

            }
            catch (Exception e)
            {
                ViewData["ErrorMessage"] = "Exception- " + e.Message + "<br>stackTrace<br>" + e.StackTrace;
                return View("Error");
            }
        }
        [AuthorizeCustomer]

        public async Task<IActionResult> AddAddress(IFormCollection col)
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


            AddressDto addressDto = new AddressDto()
            {
                UserId = userId,
                HouseNo = col["HouseNo"],
                StreetArea = col["Street"],
                City = col["city"],
                Pincode = col["Zip"],
                State = col["state"],
            };

            JsonContent content = JsonContent.Create(addressDto);
            try
            {
                using (var apiResponce = await client.PostAsync("https://localhost:7185/api/v1/User/AddAddress/User", content))
                {
                    if (apiResponce.StatusCode == HttpStatusCode.OK)
                    {
                        return RedirectToAction("Profile");
                    }
                    else
                    {
                        ViewData["ErrorMessage"] = $"<h4> API Error While adding new address {apiResponce.StatusCode}</h4><br><b>---</b><br> {apiResponce.Content}";
                        return View("Error");

                    }
                }

            }
            catch (Exception e)
            {
                ViewData["ErrorMessage"] = "Exception- " + e.Message + "<br>stackTrace<br>" + e.StackTrace;
                return View("Error");
            }
        }

        [AuthorizeCustomer]

        public async Task<IActionResult> Profile()
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

            List<Address> addresses = new();
            List<CardDetail> cards = new();
            User user = new();

            int userId = _HttpContextAccessor.HttpContext.Session.GetInt32("UserId") == null ? 1 :
                (int)_HttpContextAccessor.HttpContext.Session.GetInt32("UserId");

            try
            {
                //items
                using (var apiResponce = await client.GetAsync("https://localhost:7185/api/v1/User/GetUserDetails/" + userId))
                {
                    if (apiResponce.StatusCode == HttpStatusCode.OK)
                    {
                        user = JsonConvert.DeserializeObject<User>(await apiResponce.Content.ReadAsStringAsync());
                    }
                    else
                    {
                        ViewData["ErrorMessage"] = $"<h4> API Error While fetching user {apiResponce.StatusCode}</h4><br><b>---</b><br> {apiResponce.Content}";
                        return View("Error");

                    }
                }

                using (var apiResponce = await client.GetAsync("https://localhost:7185/api/v1/User/GetAddress/User/" + userId))
                {
                    if (apiResponce.StatusCode == HttpStatusCode.OK)
                    {
                        addresses = JsonConvert.DeserializeObject<List<Address>>(await apiResponce.Content.ReadAsStringAsync());
                    }else if (apiResponce.StatusCode == HttpStatusCode.NotFound)
                    {
                        addresses = new List<Address>();
                    }
                    else
                    {
                        ViewData["ErrorMessage"] = $"<h4> API Error While fetching user address <br> {apiResponce.StatusCode}</h4><br><b>---</b><br> {apiResponce.Content}";
                        return View("Error");
                    }
                }

                using (var apiResponce = await client.GetAsync("https://localhost:7185/api/v1/User/GetCardDetails/User/" + userId))
                {
                    if (apiResponce.StatusCode == HttpStatusCode.OK)
                    {
                        cards = JsonConvert.DeserializeObject<List<CardDetail>>(await apiResponce.Content.ReadAsStringAsync());
                    }
                    else if (apiResponce.StatusCode == HttpStatusCode.NotFound)
                    {
                        cards = new List<CardDetail>();
                    }
                    else
                    {
                        ViewData["ErrorMessage"] = $"<h4> API Error While fetching user Cards <br> {apiResponce.StatusCode}</h4><br><b>---</b><br> {apiResponce.Content}";
                        return View("Error");
                    }
                }


            }
            catch (Exception e)
            {
                ViewData["ErrorMessage"] = "Exception- " + e.Message + "<br>stackTrace<br>" + e.StackTrace;
                return View("Error");
            }

            ProfileModel model = new()
            {
                addresses = addresses,
                user = user,
                cards = cards
            };
            return View(model);
        }
        public async Task<IActionResult> Index()
        {

            HttpClient client = new HttpClient();

            if (_HttpContextAccessor.HttpContext.Session.GetString("AuthData") != null)
            {
                string TokensString = _HttpContextAccessor.HttpContext.Session.GetString("AuthData");
                AuthData authData = JsonConvert.DeserializeObject<AuthData>(TokensString);

                if (authData.Role == "Admin")
                {
                    return RedirectToAction("Index", "Admin");
                }

            }

            //client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", authData.AccessToken);

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
                    else
                    {
                        TempData["ErrMsg"] = "Api Error: " + apiResponce.StatusCode;
                        //          return View("Index");
                        //Console.WriteLine("--------------------api res" + apiResponce.StatusCode);
                        ViewData["ErrorMessage"] = $"<h4> item { apiResponce.StatusCode }</h4><br><b>---</b><br> {apiResponce.RequestMessage.Content}";
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

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult RegisterUser()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> RegisterUser(RegisterUserModel model)
        {

            //string LoginView = login.UserRole.Equals("Admin") ? "AdminLogin" : "Login";

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            PasswordHasher<string> pw = new PasswordHasher<string>();
            HttpClient client = new HttpClient();

            UserDto newUser = new()
            {
                Email = model.Email,
                UserName = model.UserName,
                HashedPassword = pw.HashPassword(model.UserName, model.Password),
                Name = model.Name,
                PhoneNumber = model.PhoneNo,
                Role = model.UserRole
            };

            JsonContent content = JsonContent.Create(newUser);
            var res = await client.PostAsync("https://localhost:7185/api/v1/register", content);
            if(res.StatusCode == HttpStatusCode.OK)
            {
                TempData["msg"] = "New User Created Successfully";
                TempData["code"] = "200";

                return RedirectToAction("Login", "Home");

            }
            else
            {
                TempData["ErrMsg"] = "Api Error: " + res.StatusCode;
                //          return View("Index");
                //Console.WriteLine("--------------------api res" + apiResponce.StatusCode);
                ViewData["ErrorMessage"] = $"<h4> item { res.StatusCode }</h4><br><b>---</b><br> {await res.Content.ReadAsStringAsync()}";
                return View("Error");
            }

          

            return View(model);

        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            //if(RouteData.Values["Authorized"] != null)
            ViewData["ErrorMessage"] = $"You are not authorized to visit this page!!";

            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }


        public IActionResult UnAuthorized()
        {
            //if(RouteData.Values["Authorized"] != null)
            ViewData["ErrorMessage"] = $"You are not authorized to visit this page!!";

            return View();
        }
    }
}