using HySound.Core.Service;
using HySound.Core.Service.IService;
using HySound.Models.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace HySound.Controllers
{
    public class LikeController : Controller
    {
        ILikeService _likeService;
        ITrackService _trackService;
        private readonly UserManager<IdentityUser> _userManager;
        IUserService _userService;


        public LikeController(IUserService userService, UserManager<IdentityUser> userManager,ITrackService trackService,ILikeService likeService)
        {
            _likeService = likeService;
            _trackService = trackService;
            _userManager = userManager;
            _userService = userService;
        }
        public async Task<IActionResult> Dislike(int id)
        {
            var tempUser = await _userManager.FindByEmailAsync(User.Identity.Name);
            User user = await _userService.GetUserAsync(x => x.Email == tempUser.Email);

            Like like = await _likeService.GetLikeAsync(x => x.TrackId == id && x.UserId == user.Id);
            if (like != null)
            {
                await _likeService.DeleteLikeAsync(like);
            }
            return RedirectToAction("Library", "Home");
        }
        public async Task<IActionResult> Like(int id)
        {
            var tempUser = await _userManager.FindByEmailAsync(User.Identity.Name);
            User user = await _userService.GetUserAsync(x => x.Email == tempUser.Email);

            Track track = await _trackService.GetTrackByIdAsync(id);

            Like like = new Like()
            {
                TrackId = id,
                Track = track,
                User = user,
                UserId = user.Id
            };
            await _likeService.AddLikeAsync(like);
            return RedirectToAction("Library", "Home");
        }
        public IActionResult Index()
        {
            return View();
        }
    }
}
