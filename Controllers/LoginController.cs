using shoppingApi.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using WebApp.Models;
using WebApp.Models.AuthenticationModels;
using Newtonsoft.Json;

namespace WebApp.Controllers
{
    public class LoginController : Controller
    {
        IHttpContextAccessor _HttpContextAccessor;

        public LoginController(IHttpContextAccessor httpContextAccessor)
        {
            _HttpContextAccessor = httpContextAccessor;
        }

        public async Task<IActionResult> AttemptLogin(IFormCollection collection)
        {
            var login = new LoginRequest()
            {
                UserName = collection["username"].ToString().Trim(),
                Password = collection["password"].ToString().Trim(),
               // UserRole = collection["UserRole"].ToString().Trim()
            };

            string LoginView = "Login";
            //string LoginView = login.UserRole.Equals("Admin") ? "AdminLogin" : "Login";

            HttpClient client = new HttpClient();
            JsonContent content = JsonContent.Create(login);
            var res = await client.PostAsync("https://localhost:7185/api/v1/login", content);

            switch (res.StatusCode)
            {
                case System.Net.HttpStatusCode.OK:
                    //TempData["msg"] = "logged In";
                    //TempData["code"] = "200";
                    string authDataString = await res.Content.ReadAsStringAsync();
                    AuthData authData = JsonConvert.DeserializeObject<AuthData>(authDataString);

                    _HttpContextAccessor.HttpContext.Session.SetString("AuthData", authDataString);
                    _HttpContextAccessor.HttpContext.Session.SetString("CurrentUser", login.UserName);
                    _HttpContextAccessor.HttpContext.Session.SetString("UserRole", authData.Role);
                    _HttpContextAccessor.HttpContext.Session.SetInt32("UserId", authData.UserId);

                    return authData.Role.Trim() == "Admin" ? (IActionResult)RedirectToAction("Index", "Admin") :
                        (IActionResult)RedirectToAction("Index", "Home");

                case System.Net.HttpStatusCode.BadRequest:
                    TempData["msg"] = "bad request";
                    TempData["code"] = "400";
                    break;
                case System.Net.HttpStatusCode.Unauthorized:
                    TempData["msg"] = "invalid credentials";
                    TempData["code"] = "401";

                    break;
                case System.Net.HttpStatusCode.InternalServerError:
                    TempData["msg"] = "invternal server error";
                    TempData["code"] = "500";
                    break;
                case System.Net.HttpStatusCode.NotFound:
                    TempData["msg"] = "User Not found";
                    TempData["code"] = "404";
                    break;
            }
            return RedirectToAction(LoginView, "Home");

        }


        

    }
}
