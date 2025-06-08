using Microsoft.AspNetCore.Mvc;
using TorneosBasketBall.Models;
using TorneosBasketBall.Servicios;

public class LoginController : Controller
{
    private readonly ContextoBD _context;

    public LoginController(ContextoBD context)
    {
        _context = context;
    }

    [HttpGet]
    public IActionResult Index()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Index(string usuario, string contrasenia)
    {
        var admin = _context.Administradores
            .FirstOrDefault(a => a.Usuario == usuario && a.Contrasenia == contrasenia);

        if (admin != null)
        {
            // Guardar sesión
            HttpContext.Session.SetInt32("AdminID", admin.AdministradorID);
            HttpContext.Session.SetString("AdminUsuario", admin.Usuario);
            return RedirectToAction("Index", "Equipos"); // O a donde quieras redirigir
        }
        else
        {
            ViewBag.Error = "Usuario o contraseña incorrectos.";
            return View();
        }
    }

    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return RedirectToAction("Index");
    }
}
