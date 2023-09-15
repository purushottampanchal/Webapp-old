using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace WebApp.CustomAttribute
{
    public class AuthorizeAdminAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);
            Console.WriteLine("user role " + context.HttpContext.Session.GetString("UserRole"));

            if ((context.HttpContext.Session.GetString("AuthData")) == null)
            {
                context.Result = new RedirectToRouteResult(new RouteValueDictionary(new
                {
                    controller = "Home",
                    action = "Login",

                }));
            }
            else
            if (context.HttpContext.Session.GetString("UserRole") != "Admin")
            {
                context.RouteData.Values.Add("Authorized", "Not");

                context.Result = new RedirectToRouteResult(new RouteValueDictionary(new
                {
                    controller = "Home",
                    action = "UnAuthorized",

                }));
            }

        }
    }
}
