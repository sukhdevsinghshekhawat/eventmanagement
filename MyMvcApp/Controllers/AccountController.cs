
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MyMvcApp.Models;
using System.ComponentModel.DataAnnotations;

namespace MyMvcApp.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public AccountController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                await _signInManager.SignInAsync(user, isPersistent: false);
                return RedirectToAction("Index", "Student");
            }

            foreach (var error in result.Errors)
                ModelState.AddModelError(string.Empty, error.Description);

            return View(model);
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var result = await _signInManager.PasswordSignInAsync(
                model.Email,
                model.Password,
                model.RememberMe,
                lockoutOnFailure: true);

            if (result.Succeeded)
                return RedirectToAction("Index", "Student");

            ModelState.AddModelError(string.Empty, "Invalid email or password.");
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login");
        }
    }

    public class RegisterViewModel
    {
        [Required, EmailAddress, StringLength(256)]
        public string Email { get; set; } = "";

        [Required, MinLength(8), StringLength(128), RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^A-Za-z0-9]).{8,128}$", ErrorMessage = "Password must be 8–128 characters and include uppercase and lowercase letters, a number, and a special character.")]
        public string Password { get; set; } = "";

        [Required, StringLength(128), Compare(nameof(Password))]
        public string ConfirmPassword { get; set; } = "";
    }

    public class LoginViewModel
    {
        [Required, EmailAddress, StringLength(256)]
        public string Email { get; set; } = "";

        [Required, StringLength(128)]
        public string Password { get; set; } = "";

        public bool RememberMe { get; set; }
    }
}
