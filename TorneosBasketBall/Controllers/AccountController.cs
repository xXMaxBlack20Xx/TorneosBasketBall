using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using TorneosBasketBall.Models.ViewModels;

namespace TorneosBasketBall.Controllers
{
    [Route("[controller]/[action]")]
    public class AccountController : Controller
    {
        private readonly SignInManager<IdentityUser> _signInManager;

        public AccountController(SignInManager<IdentityUser> signInManager)
        {
            _signInManager = signInManager;
        }

        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            // Fix: Initialize required properties 'Username' and 'Password' with default values.  
            var vm = new LoginVM
            {
                Username = string.Empty,
                Password = string.Empty,
                ReturnUrl = returnUrl
            };
            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginVM vm)
        {
            if (!ModelState.IsValid)
                return View(vm);

            var result = await _signInManager
                .PasswordSignInAsync(vm.Username, vm.Password, vm.RememberMe, lockoutOnFailure: false);

            if (result.Succeeded)
                return LocalRedirect(vm.ReturnUrl ?? "/");

            ModelState.AddModelError("", "Usuario o contraseña inválidos");
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction(nameof(Login));
        }
    }
}
