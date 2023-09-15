using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace WebApp.CustomAttribute
{
    public class AuthorizeCustomerAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);

            if ((context.HttpContext.Session.GetString("AuthData")) == null)
            {
                context.Result = new RedirectToRouteResult(new RouteValueDictionary(new
                {
                    controller = "Home",
                    action = "Login",

                }));
            }

            else if (context.HttpContext.Session.GetString("UserRole") != "User")
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
