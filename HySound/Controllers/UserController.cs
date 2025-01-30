using HySound.Core.Service;
using HySound.Core.Service.IService;
using HySound.Models.Models;
using Microsoft.AspNetCore.Mvc;

namespace HySound.Controllers
{
    public class UserController : Controller
    {
        IUserService userService;
        public UserController(IUserService _userService)
        {
            userService = _userService;
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
