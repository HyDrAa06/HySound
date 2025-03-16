using HySound.Core.Service.IService;
using Microsoft.AspNetCore.Mvc;
using HySound.ViewModels.Artist;
using Microsoft.AspNetCore.Identity;
namespace HySound.Controllers
{
    public class ArtistRequestController : Controller
    {
        private readonly IArtistRequestService _artistRequestService;
        private readonly IUserService _userService;
        private readonly UserManager<IdentityUser> _userManager;

        public ArtistRequestController(UserManager<IdentityUser> userManager ,IUserService userService,IArtistRequestService artistRequestService)
        {
            _artistRequestService = artistRequestService;
            _userService = userService; 
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            if (!User.IsInRole("User") && !User.IsInRole("Artist") && !User.IsInRole("Admin"))
            {
                return RedirectToAction("Login", "Account");
            }
            return View();
        }

        public async Task<IActionResult> Request()
        {
            var tempUser = await _userManager.FindByEmailAsync(User.Identity.Name);

           

            var user = await _userService.GetUserAsync(x => x.Email == tempUser.Email);
  

            await _artistRequestService.SubmitArtistRequestAsync(user.Id, 
                user.UserIdentityId, user.ProfilePicture, user.Username,user.Email,user.Bio,user.Password);
            TempData["Message"] = "Artist request submitted successfully!";
            return RedirectToAction("Index");
        }
    }
}
