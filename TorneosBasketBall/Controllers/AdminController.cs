using Microsoft.AspNetCore.Mvc;

namespace TorneosBasketBall.Controllers
{
    public class AdminController : Controller
    {
        // Solo admin con sesión activa debería ver esto.
        private bool IsAdminLogged() =>
            HttpContext.Session.GetInt32("AdminID") != null;

        public IActionResult Index()
        {
            if (!IsAdminLogged())
                return RedirectToAction("Index", "Login");

            return View();
        }
    }
}
