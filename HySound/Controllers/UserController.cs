using HySound.Core.Service;
using HySound.Core.Service.IService;
using HySound.Models.Models;
using HySound.Models.User;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Abstractions;

namespace HySound.Controllers
{
    public class UserController : Controller
    {
        IUserService userService;
        private readonly SignInManager<IdentityUser> signInManager;
        private readonly UserManager<IdentityUser> userManager;
        IFollowerService followerService;

        public UserController(IFollowerService _followerService, UserManager<IdentityUser> _userManager, SignInManager<IdentityUser> _signInManager, IUserService _userService)
        {
            userManager = _userManager;
            userService = _userService;
            signInManager = _signInManager;
            followerService = _followerService;
        }
        public async Task<IActionResult> AllUsers()
        {
            var users = await userService.GetAllUsersAsync();
            return View(users);
        }
        public async Task<IActionResult> Delete(int id)
        {
            if(id != null)
            {
                await userService.DeleteUserByIdAsync(id);
                return RedirectToAction("AllUsers");
            }
            else
            {
                return RedirectToAction("AllUsers");
            }
        }

        public async Task<IActionResult> Profile()
        {
            var tempUser = await userManager.FindByEmailAsync(User.Identity.Name);
            User user = await userService.GetUserAsync(x=>x.Email==tempUser.Email);
            UserViewModel model = new UserViewModel
            {
                Email = user.Email,
                Bio = user.Bio,
                Followers = followerService.GetAll().Where(x => x.FollowedId == user.Id).ToList(),
                Name = user.Username,
                ProfilePicture = user.ProfilePicture
            };
            return View(model);
        }

        [HttpPost]

        public async Task<IActionResult> Update(User user)
        {
            if (!ModelState.IsValid)
            {
                return View(user);
            }

            await userService.UpdateUserAsync(user);
            return RedirectToAction("AllUsers");
        }
        public async Task<IActionResult> Update(int id)
        {
            User user = await userService.GetUserByIdAsync(id);
            return View(user);
        }
        public IActionResult AddUser()
        {
            User user = new User();
            return View(user);
        }
        [HttpPost]
        public async Task<IActionResult> AddUser(User user)
        {
            if(ModelState.IsValid)
            {
                await userService.AddUserAsync(user);
                return RedirectToAction("AllUsers");
            }
            else
            {
                return View(user);
            }
        }
    }
}
