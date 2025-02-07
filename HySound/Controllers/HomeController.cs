using HySound.Core.Service.IService;
using HySound.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace HySound.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        ITrackService trackService;
        public HomeController(ILogger<HomeController> logger, ITrackService track)
        {
            trackService = track;
            _logger = logger;
        }
        public IActionResult Library()
        {
            return View();
        }

        public IActionResult Index()
        {
            var model = trackService.AllWithInclude().Include(x => x.Genre).ThenInclude(x => x.Tracks).Include(x => x.User).ThenInclude(x => x.Tracks).Select(x => new TrackViewModel()
            {
                TrackId = x.Id,
                Title = x.Title,
                AudioUrl = x.AudioUrl,
                GenreName = x.Genre.Name,
                UserName = x.User.Username,
                Plays = x.Plays,
                ImageLink = x.CoverImage
            }).ToList();
            return View(model);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
