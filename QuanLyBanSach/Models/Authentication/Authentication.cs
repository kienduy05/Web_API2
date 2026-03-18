using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace QuanLyBanSach.Models.Authentication
{
    public class Authentication : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var session = context.HttpContext.Session;

            string? username = session.GetString("Username");
            string? role = session.GetString("AccountType");

            if (string.IsNullOrEmpty(username))
            {
                context.Result = new RedirectToActionResult(
                    "Login",
                    "Access",
                    new { area = "" }
                );
                return;
            }

            var area = context.RouteData.Values["area"]?.ToString();

            // Nếu không phải admin mà cố vào Admin
            if (area == "Admin" && role == "2")
            {
                context.Result = new RedirectToActionResult(
                    "Index",
                    "Home",
                    new { area = "" }
                );
                return;
            }

            base.OnActionExecuting(context);
        }
    }
}