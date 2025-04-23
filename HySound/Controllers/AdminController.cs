using Azure.Core;
using HySound.Core.Service.IService;
using HySound.Models.Models;
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
            var request = await _artistRequestService.GetByIdAsync(id);
            if (request != null)
            {
                var user = await _userService.GetUserAsync(x => x.Id == request.UserId);
                await _artistRequestService.ApproveRequestAsync(id,1);
                
            }

            TempData["Message"] = "Request approved!";
            return RedirectToAction("ArtistRequests");
        }
        [HttpPost("Deny/{id}")]
        public async Task<IActionResult> Deny(int id)
        {
            var request = await _artistRequestService.GetByIdAsync(id);
            if (request != null)
            {
                await _artistRequestService.DenyRequestAsync(id, 1);
            }
            

            TempData["Message"] = "Request denied!";
            return RedirectToAction("ArtistRequests");
        }
    }
}
