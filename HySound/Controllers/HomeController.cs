using HySound.Core.Service;
using HySound.Core.Service.IService;
using HySound.Models.Models;
using HySound.ViewModels;
using HySound.ViewModels.Album;
using HySound.ViewModels.Main;
using HySound.ViewModels.Playlist;
using HySound.ViewModels.User;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.IdentityModel.Tokens;
using System.Diagnostics;

namespace HySound.Controllers
{
    public class HomeController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ILogger<HomeController> _logger;
        IUserService _userService;
        ITrackService _trackService;
        ILikeService _likeService;
        IAlbumService _albumService;
        IPlaylistService _playlistService;

        public HomeController(IPlaylistService playlistService,IAlbumService albumService, ILikeService likeService,UserManager<IdentityUser>userManager,IUserService userService,ITrackService trackService,ILogger<HomeController> logger, ITrackService track)
        {
            _playlistService = playlistService;
            _albumService=albumService;
            _likeService = likeService;
            _userManager = userManager;
            _userService = userService;
            _trackService = trackService;
            _logger = logger;
        }

        public async Task<IActionResult> Search(SearchViewModel? model)
        {
            if(model.filter == "songs")
            {
                var query = _trackService.GetAll().AsQueryable().Where(x=>x.Title == model.SearchFilter);

                if (!query.IsNullOrEmpty())
                {
                    model.Tracks = query.Include(x => x.Genre)
                .Include(x => x.User)
                .Select(x => new TrackViewModel()
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
            }
            if(model.filter == "albums")
            {
                var query = _albumService.GetAll().AsQueryable().Where(x => x.Title == model.SearchFilter);

                if (!query.IsNullOrEmpty())
                {
                    
                    var tracks = await _trackService.GetAllTracksAsync();
                    model.Albums = query.Include(x=>x.Tracks)
                .Include(x => x.User)
                .Select(x => new AlbumViewModel()
                {
                    Title = x.Title,
                    CoverImage = x.CoverImage,
                    ReleaseDate = x.ReleaseDate,
                    Id = x.Id,
                    UserName= x.User.Username
                }).ToList();


                    foreach(var album in model.Albums)
                    {
                        tracks = tracks.Where(x => x.AlbumId == album.Id);

                        album.Tracks = tracks.ToList();
                    }
                    return View(model);
                }
            }
            if (model.filter == "playlists")
            {
                var query = _playlistService.GetAll().Where(x => x.Title == model.SearchFilter);

                if (!query.IsNullOrEmpty())
                {
                    model.Playlists = query.Include(x => x.User)
                        .Select(x => new PlaylistViewModel()
                        {
                            CoverImage=x.CoverImage,
                            Id = x.Id,
                            Title = x.Title,
                            UserName= x.User.Username
                        }).ToList();
                }

                return View(model);
            }
            if (model.filter == "users")
            {
                var query = _userService.GetAll().Where(x => x.Username == model.SearchFilter);

                if (!query.IsNullOrEmpty())
                {
                    model.User = query
                        .Select(x => new UserViewModel()
                        {
                            Bio=x.Bio,
                            Email=x.Email,
                            Followers=x.FollowedBy,
                            Following=x.Following,
                            ProfilePicture=x.ProfilePicture,
                            Name=x.Username,
                            Id=x.Id
                        }).ToList();
                }
            }

            return View(model);
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

            var albumModel = _albumService.AllWithInclude().Include(x=>x.Tracks).Include(x => x.User).Select(x => new AlbumViewModel()
            {
                Id = x.Id,
                Title = x.Title,
                CoverImage = x.CoverImage,
                ReleaseDate = x.ReleaseDate,
                UserName = x.User.Username,
                Tracks = x.Tracks.Select(t=> new Track
                {
                    Id=t.Id,
                    Title=t.Title,
                    AlbumId=t.AlbumId,
                    AudioUrl=t.AudioUrl,
                    CoverImage=t.CoverImage,
                    UserId=t.UserId,
                    IsYoutube=t.IsYoutube,
                    Plays = t.Plays
                }).ToList()
            }).ToList();

            var playlistsModel = _playlistService.AllWithInclude().Include(x => x.User).Select(x => new PlaylistViewModel()
            {
                Id=x.Id,    
                CoverImage = x.CoverImage,
                Title = x.Title,
                UserName = x.User.Username
            }).ToList();

            libraryModel.Tracks = model;
            libraryModel.Albums = albumModel;
            libraryModel.Playlists = playlistsModel;
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
