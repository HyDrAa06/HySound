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
        ILikeService likeService;
        IAlbumService albumService;
        IPlaylistService playlistService;
        IArtistRequestService artistRequestService;
        private readonly Cloudinary _cloudinary;
        private readonly IConfiguration _configuration;
        CloudinaryService cloudService;

        public UserController(IPlaylistService _playlistService, IAlbumService _albumService,ILikeService _likeService,IArtistRequestService _artistRequestService,IConfiguration configuration, CloudinaryService cloud, IFollowerService _followerService, UserManager<IdentityUser> _userManager, SignInManager<IdentityUser> _signInManager, IUserService _userService)
        {
            albumService = _albumService;
            playlistService = _playlistService;
            likeService = _likeService;
            userManager = _userManager;
            userService = _userService;
            signInManager = _signInManager;
            followerService = _followerService;
            artistRequestService = _artistRequestService;
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
                await artistRequestService.DeleteAsync(id);
                await followerService.DeleteAllFollowersAndFollowing(id);
                await likeService.DeleteAllLikesByUsers(id);
                await playlistService.DeleteAllPlaylistsOfUser(id);
                await albumService.DeleteAllAlbumsOfUser(id);
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
            if (tempUser == null)
            {
                return NotFound("No Identity user found.");
            }
            User user = await userService.GetUserAsync(x => x.Email == tempUser.Email);
            if (user == null)
            {
                return NotFound("No application user found for email: " + tempUser.Email);
            }

            int followers = 0;
            int following = 0;

            var followedByUsers = await followerService.GetAllFollowersAsync(x => x.FollowedId == user.Id);
            var followingUsers = await followerService.GetAllFollowersAsync(x => x.FollowedById == user.Id);

            if (followedByUsers != null)
            {
                followers = followedByUsers.ToList().Count();
            }
            if (followingUsers != null)
            {
                following = followingUsers.ToList().Count();
            }

            var followedByUsersAsUsers = new List<User>();
            var followingUsersAsUsers = new List<User>();

            foreach(var temp in followedByUsers)
            {
                var toAdd = await userService.GetUserByIdAsync(temp.FollowedById);
                followedByUsersAsUsers.Add(toAdd);
            }

            foreach(var temp in followingUsers)
            {
                var toAdd = await userService.GetUserByIdAsync(temp.FollowedId);
                followingUsersAsUsers.Add(toAdd);
            }

            UserViewModel model = new UserViewModel
            {
                Email = user.Email,
                Bio = user.Bio,
                FollowingAsUsers = followingUsersAsUsers,
                Name = user.Username,
                ProfilePicture = user.ProfilePicture,
                FollowersCount = followers,
                FollowingCount = following,
                FollowersAsUsers = followedByUsersAsUsers
            };
            return View(model);
        }

        public async Task<IActionResult> UserDetails(int id)
        {
            var tempUser = await userManager.FindByEmailAsync(User.Identity.Name);
            if (tempUser == null)
            {
                return NotFound("No Identity user found.");
            }
            
            User followingUser = await userService.GetUserAsync(x => x.Email == tempUser.Email);

            Followed follow = await followerService.GetFollowerAsync(x => x.FollowedId == id && x.FollowedById == followingUser.Id);
            bool isFollowed = false;
            if (follow != null)
            {
                isFollowed = true;
            }

            int followers = 0;
            int following = 0;

            var followedByUsers = await followerService.GetAllFollowersAsync(x => x.FollowedId == id);
            var followingUsers = await followerService.GetAllFollowersAsync(x=>x.FollowedById == id);
            
            if(followedByUsers != null)
            {
                followers = followedByUsers.ToList().Count();
            }
            if(followingUsers != null)
            {
                following = followingUsers.ToList().Count();
            }
            

            var model = await userService.GetAll().Where(x => x.Id == id).Include(x=>x.FollowedBy)
                .Include(x=>x.Following).Select(x=>new UserViewModel 
            {
                Bio = x.Bio,
                Email = x.Email,
                Followers = x.FollowedBy,
                Following = x.Following,
                ProfilePicture = x.ProfilePicture,
                Name = x.Username,
                Id = id,
                IsFollowed = isFollowed,
                FollowersCount = followers,
                FollowingCount = following,
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

     
        public async Task<IActionResult> Unfollow(int followedId)
        {
            var tempUser = await userManager.FindByEmailAsync(User.Identity.Name);
            User followingUser = await userService.GetUserAsync(x => x.Email == tempUser.Email);

            Followed follow = await followerService.GetFollowerAsync(x => x.FollowedId == followedId && x.FollowedById == followingUser.Id);

            if(follow != null)
            {
                await followerService.DeleteFollowerAsync(follow);
            }
            return RedirectToAction("UserDetails", "User", new { id = followedId });

        }

        public async Task<IActionResult> Follow(int followedId)
        {
            var tempUser = await userManager.FindByEmailAsync(User.Identity.Name);
            User followingUser = await userService.GetUserAsync(x => x.Email == tempUser.Email);
            User followedUser = await userService.GetUserAsync(x => x.Id == followedId);

            if (followingUser == null || followedUser == null)
            {
                return NotFound(); // One of the users doesn't exist
            }

            Followed follow = new Followed
            {
                FollowedById = followingUser.Id,
                FollowedId = followedId
            };

            await followerService.AddFollowerAsync(follow);
            return RedirectToAction("UserDetails", "User", new { id = followedId });
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
