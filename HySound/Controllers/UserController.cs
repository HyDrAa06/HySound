using CloudinaryDotNet;
using HySound.Core.Service;
using HySound.Core.Service.IService;
using HySound.Models.Models;
using HySound.ViewModels.User;
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

        private readonly Cloudinary _cloudinary;
        private readonly IConfiguration _configuration;
        CloudinaryService cloudService;

        public UserController(IConfiguration configuration, CloudinaryService cloud, IFollowerService _followerService, UserManager<IdentityUser> _userManager, SignInManager<IdentityUser> _signInManager, IUserService _userService)
        {
            userManager = _userManager;
            userService = _userService;
            signInManager = _signInManager;
            followerService = _followerService;

            this.cloudService = cloud;

            _configuration = configuration;
            var account = new Account(
           _configuration["Cloudinary:CloudName"],
           _configuration["Cloudinary:ApiKey"],
           _configuration["Cloudinary:ApiSecret"]
       );
            _cloudinary = new Cloudinary(account);
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
        public async Task<IActionResult> Profile(UserViewModel model)
        {
            User user = await userService.GetUserAsync(x => x.Email == model.Email);
            user.Username = model.Name;
            user.ProfilePicture = model.ProfilePicture;
            user.Bio = model.Bio;
            user.Email = model.Email;
            await userService.UpdateUserAsync(user);
            
            return View(model);
        }
        public async Task<IActionResult> Profile()
        {
            var tempUser = await userManager.FindByEmailAsync(User.Identity.Name);
            if (tempUser == null)
            {
                return NotFound("No Identity user found.");
            }
            User user = await userService.GetUserAsync(x => x.Email == tempUser.Email);
            if (user == null)
            {
                return NotFound("No application user found for email: " + tempUser.Email);
            }
            UserViewModel model = new UserViewModel
            {
                Id = user.Id,
                Email = user.Email,
                Bio = user.Bio,
                Followers = followerService.GetAll().Where(x => x.FollowedId == user.Id).ToList(),
                Name = user.Username,
                ProfilePicture = user.ProfilePicture
            };
            return View(model);
        }

        public async Task<IActionResult> UserDetails(int id)
        {
            var model = await userService.GetAll().Include(x=>x.FollowedBy)
                .Include(x=>x.Following).Select(x=>new UserViewModel 
            {
                Bio = x.Bio,
                Email = x.Email,
                Followers = x.FollowedBy,
                Following = x.Following,
                ProfilePicture = x.ProfilePicture,
                Name = x.Username,
                Id = x.Id
            }).FirstOrDefaultAsync();

            return View(model);

        }

        [HttpPost]
        public async Task<IActionResult> Update(int id, UserViewModel user)
        {
            var tempUser = await userManager.FindByEmailAsync(User.Identity.Name);
            User userModel = await userService.GetUserAsync(x => x.Email == tempUser.Email);

            if (!ModelState.IsValid)
            {
                return View(user);
            }
            if (userModel == null)
            {
                return NotFound($"User with ID {id} not found.");
            }

            userModel.Username = user.Name;
            userModel.Email = user.Email;
            userModel.Bio = user.Bio;
            userModel.FollowedBy = user.Followers;
            userModel.Following = user.Following;
            userModel.Id = user.Id;
            if (user.ImageFile != null)
            {
                var imageUploadResult = await cloudService.UploadImageAsync(user.ImageFile);
                userModel.ProfilePicture = imageUploadResult;
            }

            await userService.UpdateUserAsync(userModel);
            return RedirectToAction("Profile");
        }
        public async Task<IActionResult> Update(int id)
        {
            User user = await userService.GetUserByIdAsync(id);
            UserViewModel model = new UserViewModel
            {
                Email = user.Email,
                Bio = user.Bio,
                Followers = user.FollowedBy,
                Following = user.Following,
                ProfilePicture = user.ProfilePicture,
                Name = user.Username,
                Id= id
            };
            return View(model);
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
