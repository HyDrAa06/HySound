using HySound.Core.Service.IService;
using HySound.ViewModels.Artist;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace HySound.Controllers
{
    public class AdminController : Controller
    {
        private readonly IArtistRequestService _artistRequestService;
        private readonly IUserService _userService;

        public AdminController(IUserService userService ,IArtistRequestService artistRequestService)
        {
            _artistRequestService = artistRequestService;
            _userService = userService;
        }

        [HttpGet]
        public async Task<IActionResult> ArtistRequests()
        {
            var requests = await _artistRequestService.GetPendingRequestsAsync();
            var viewModels = requests.Select(r => new PendingArtistRequestViewModel
            {
                Id = r.Id,
                Username = r.ArtistUsername,
                Bio = r.Bio,
                Email = r.Email,
                ProfilePicture = r.ProfilePicture
            }).ToList();

            return View(viewModels);
        }

        [HttpPost("Approve/{id}")]
        public async Task<IActionResult> Approve(int id)
        {
            var admin = await _userService.GetUserAsync(x => x.Email == "admin@admin.com"); 
            await _artistRequestService.ApproveRequestAsync(id, admin.Id);
            TempData["Message"] = "Request approved successfully!";
            return RedirectToAction("ArtistRequests");
        }

        [HttpPost("Deny/{id}")]
        public async Task<IActionResult> Deny(int id)
        {
            var admin = await _userService.GetUserAsync(x => x.Email == "admin@admin.com");
            await _artistRequestService.DenyRequestAsync(id, admin.Id);
            TempData["Message"] = "Request denied successfully!";
            return RedirectToAction("ArtistRequests");
        }
    }
}
