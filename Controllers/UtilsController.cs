using Microsoft.AspNetCore.Mvc;

namespace NextCommerce.Controllers
{
    public class UtilsController : Controller
    {
        public IActionResult AgreeCookies()
        {
            var options = new CookieOptions
            {
                Expires = DateTime.Now.AddYears(1),
                IsEssential = true
            };
            Response.Cookies.Append("CookieConsent", "Agreed", options);

            return Json(new { success = true });
        }
    }
}
