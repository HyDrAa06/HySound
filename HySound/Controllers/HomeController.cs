using HySound.Core.Service;
using HySound.Core.Service.IService;
using HySound.Models.Models;
using HySound.ViewModels;
using HySound.ViewModels.Album;
using HySound.ViewModels.Main;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace HySound.Controllers
{
    public class HomeController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        IUserService _userService;
        ITrackService _trackService;
        private readonly ILogger<HomeController> _logger;
        ILikeService _likeService;
        IAlbumService _albumService;
        public HomeController(IAlbumService albumService, ILikeService likeService,UserManager<IdentityUser>userManager,IUserService userService,ITrackService trackService,ILogger<HomeController> logger, ITrackService track)
        {
            _albumService=albumService;
            _likeService = likeService;
            _userManager = userManager;
            _userService = userService;
            _trackService = trackService;
            _logger = logger;
        }
        public async Task<IActionResult> Library()
        {
            LibraryViewModel libraryModel = new LibraryViewModel();
            var model = _trackService.AllWithInclude().Include(x => x.Genre).ThenInclude(x => x.Tracks).Include(x => x.User).ThenInclude(x => x.Tracks).Select(x => new TrackViewModel()
            {
                TrackId = x.Id,
                Title = x.Title,
                AudioUrl = x.AudioUrl,
                GenreName = x.Genre.Name,
                UserName = x.User.Username,
                Plays = x.Plays,
                ImageLink = x.CoverImage
            }).ToList();

            var tempUser = await _userManager.FindByEmailAsync(User.Identity.Name);
            User user = await _userService.GetUserAsync(x => x.Email == tempUser.Email);

            var likesList = await _likeService.GetAllLikesAsync();
            List<int?> trackIds = likesList.Where(x=>x.UserId == user.Id).Select(x=>x.TrackId).ToList();

            model = model.Where(x => trackIds.Contains(x.TrackId)).ToList();

            var albumModel = _albumService.AllWithInclude().Include(x => x.User).Select(x => new AlbumViewModel()
            {
                Id = x.Id,
                Title = x.Title,
                CoverImage = x.CoverImage,
                ReleaseDate = x.ReleaseDate,
                UserName = x.User.Username
            }).ToList();


            libraryModel.Tracks = model;
            libraryModel.Albums = albumModel;
            return View(libraryModel);
        }

        public IActionResult Index()
        {
            var model = _trackService.AllWithInclude().Include(x => x.Genre).ThenInclude(x => x.Tracks).Include(x => x.User).ThenInclude(x => x.Tracks).Select(x => new TrackViewModel()
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
