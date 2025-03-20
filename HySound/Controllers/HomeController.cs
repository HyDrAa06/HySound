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
            
            if(!User.IsInRole("User") && !User.IsInRole("Artist") && !User.IsInRole("Admin"))
            {
                return RedirectToAction("Login","Account");
            }

            if(model.filter == "songs" || string.IsNullOrEmpty(model.filter))
            {
                var query = _trackService.GetAll().AsQueryable();

                if (!string.IsNullOrEmpty(model.SearchFilter))
                {
                    query = query.Where(x => x.Title == model.SearchFilter);
                }

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
                    ImageLink = x.CoverImage
                }).ToList();
                    
                    return View(model);
                }
            }
            if(model.filter == "albums")
            {
                var query = _albumService.GetAll().AsQueryable();

                if (!string.IsNullOrEmpty(model.SearchFilter))
                {
                    query = query.Where(x => x.Title == model.SearchFilter);
                }

                if (!query.IsNullOrEmpty())
                {
                    
                    var tracks = await _trackService.GetAllTracksAsync();
                    model.Albums = query.Include(x=>x.Tracks)
                .Include(x => x.User)
                .Select(x => new AlbumViewModel()
                {
                    Title = x.Title,
                    CoverImage = x.CoverImage,
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
                var query = _playlistService.GetAll();

                if (!string.IsNullOrEmpty(model.SearchFilter))
                {
                    query = query.Where(x => x.Title == model.SearchFilter);
                }

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
                var tempUser = await _userManager.FindByEmailAsync(User.Identity.Name);
                User user = await _userService.GetUserAsync(x => x.Email == tempUser.Email);

                var query = _userService.GetAll();

                if (!string.IsNullOrEmpty(model.SearchFilter))
                {
                    query = query.Where(x => x.Username == model.SearchFilter);
                }

                if (!query.IsNullOrEmpty())
                {
                    
                    model.User = query.Where(x=>x.Id != user.Id)
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
                ImageLink = x.CoverImage,
                IsYoutube = x.IsYoutube
            }).ToList();

            var tempUser = await _userManager.FindByEmailAsync(User.Identity.Name);
            User user = await _userService.GetUserAsync(x => x.Email == tempUser.Email);

            var likesList = await _likeService.GetAllLikesAsync();
            List<int?> trackIds = likesList.Where(x=>x.UserId == user.Id).Select(x=>x.TrackId).ToList();

            List<int?> albumIds = likesList.Where(x=>x.UserId==user.Id).Select(x=>x.AlbumId).ToList();

            List<int?> playlistIds = likesList.Where(x => x.UserId == user.Id).Select(x => x.PlaylistId).ToList();

            model = model.Where(x => trackIds.Contains(x.TrackId)).ToList();

            var albumModel = _albumService.AllWithInclude().Include(x=>x.Tracks).Include(x => x.User).Select(x => new AlbumViewModel()
            {
                Id = x.Id,
                Title = x.Title,
                CoverImage = x.CoverImage,
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
                }).ToList()
            }).ToList();

            albumModel = albumModel.Where(x => albumIds.Contains(x.Id)).ToList();

            var playlistsModel = _playlistService.AllWithInclude().Include(x => x.User).Select(x => new PlaylistViewModel()
            {
                Id=x.Id,    
                CoverImage = x.CoverImage,
                Title = x.Title,
                UserName = x.User.Username
            }).ToList();


            playlistsModel = playlistsModel.Where(x => playlistIds.Contains(x.Id)).ToList();

            var createdPlaylistIds = _playlistService.GetAll().Where(x => x.UserId == user.Id).Select(x=>x.Id);
            var playlistsToAdd = _playlistService.GetAll().Include(x=>x.User).Where(x => createdPlaylistIds.Contains(x.Id));

            foreach(var playlist in playlistsToAdd)
            {
                PlaylistViewModel tempViewModel = new PlaylistViewModel()
                {
                    Id = playlist.Id,
                    CoverImage = playlist.CoverImage,
                    Title = playlist.Title,
                    UserName = playlist.User.Username
                };
                if (!playlistsModel.Contains(tempViewModel))
                {
                    playlistsModel.Add(tempViewModel);
                }
            }


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
                ImageLink = x.CoverImage
            }).ToList();

            if(model.Count >= 8)
            {
                List<TrackViewModel> shownTracks = new List<TrackViewModel>();
                Random k = new Random();

                while(shownTracks.Count < 8)
                {
                    TrackViewModel track = model[k.Next(0, model.Count - 1)];
                    if (!shownTracks.Contains(track))
                    {
                        shownTracks.Add(track);
                    }
                }
                return View(shownTracks);

            }
            else
            {
                return View(model);
            }


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
