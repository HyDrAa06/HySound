using HySound.Core.Service.IService;
using HySound.ViewModels.Account;
using HySound.Models.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace HySound.Controllers
{
    public class AccountController : Controller
    {
       
        private readonly UserManager<IdentityUser> _userManager;

        private readonly SignInManager<IdentityUser> _signInManager;

        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IUserService _userService;



        public AccountController(IUserService user,UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _userService = user;
        }

        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Login() => View();

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            
            if (ModelState.IsValid)
            {
                if (model.RememberMe)
                {
                    var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, true, false);

                    if (result.Succeeded)
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }
                else
                {
                    var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, false, false);

                    if (result.Succeeded)
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }


                ModelState.AddModelError("", "Invalid login attempt.");

            }
            return View(model);

        }

        public IActionResult Register() => View();

        [HttpPost]

        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new IdentityUser { UserName = model.Email, Email = model.Email };
                var result = await _userManager.CreateAsync(user, model.Password);


                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, "User"); 
                                   await _signInManager.SignInAsync(user, isPersistent: false);
                    User user2 = new User()
                    {
                        Username = model.Username,
                        Email = model.Email,
                        Password = model.Password,
                        UserIdentity = user,
                        UserIdentityId = user.Id
                    };
                    await _userService.AddUserAsync(user2);
                    return RedirectToAction("Index", "Home");

                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }

            }

            return View(model);

        }

        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        public IActionResult AccessDenied()
        { 
            return View();
        }

    
    }
}
