using HySound.Core.Service;
using HySound.Core.Service.IService;
using HySound.Models;
using HySound.ViewModels.Album;
using HySound.Models.Models;
using HySound.ViewModels.Track;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using HySound.ViewModels;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Identity;

namespace HySound.Controllers
{
    public class AlbumController : Controller
    {
        private readonly UserManager<IdentityUser> userManager;
        IAlbumService albumService;
        IUserService userService;
        ITrackService trackService;
        ILikeService likeService;

        private readonly Cloudinary _cloudinary;
        private readonly IConfiguration _configuration;
        CloudinaryService cloudService;
        public AlbumController(ILikeService _likeService,UserManager<IdentityUser> _userManager, IConfiguration configuration, CloudinaryService cloud, ITrackService _trackService, IAlbumService _albumService, IUserService userService)
        {
            albumService = _albumService;
            this.userService = userService;
            trackService = _trackService;
            likeService = _likeService;
            userManager = _userManager;

            this.cloudService = cloud;

            _configuration = configuration;
            var account = new Account(
           _configuration["Cloudinary:CloudName"],
           _configuration["Cloudinary:ApiKey"],
           _configuration["Cloudinary:ApiSecret"]
       );
            _cloudinary = new Cloudinary(account);
        }
        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> AlbumDetails(int id)
        {
            var tempUser = await userManager.FindByEmailAsync(User.Identity.Name);
            User user = await userService.GetUserAsync(x => x.Email == tempUser.Email);

            bool isLiked = false;

            var like = likeService.GetAll().Where(x => x.UserId == user.Id && x.AlbumId == id);
            if (like.Any())
            {
                isLiked = true;
            }

            var album = await albumService.GetAll().Where(x=>x.Id==id).Include(x=>x.User).Include(x=>x.Tracks)
                .Select(x=> new AlbumViewModel
                {
                    CoverImage = x.CoverImage,
                    Id = id,
                    Title=x.Title,
                    UserName=x.User.Username,
                    Tracks = x.Tracks.ToList(),
                    IsLiked=isLiked
                }).FirstOrDefaultAsync();


            return View(album);
        }

        [HttpGet]
        public async Task<IActionResult> Update(int id)
        {
            var model = albumService.GetAll().Where(x => x.Id == id).Include(x => x.User)
                .Select(x => new EditAlbumViewModel()
                {
                    AlbumCover = x.CoverImage,
                    Title = x.Title,
                    UserId = x.UserId,
                    Id = x.Id
                }).FirstOrDefault();

            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> Update(EditAlbumViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            if (model.ImageFile != null)
            {
                var imageUploadResult = await cloudService.UploadImageAsync(model.ImageFile);

                Album album = albumService.GetAll().Where(x => x.Id == model.Id).FirstOrDefault();
                album.Title = model.Title;
                album.CoverImage = imageUploadResult;
                album.UserId = model.UserId;

                await albumService.UpdateAlbumAsync(album);
                return RedirectToAction("AllAlbums");
            }
            else
            {
                Album album = albumService.GetAll().Where(x => x.Id == model.Id).FirstOrDefault();
                album.Title = model.Title;
                album.CoverImage = model.AlbumCover;
                album.UserId = model.UserId;

                await albumService.UpdateAlbumAsync(album);
                return RedirectToAction("AllAlbums");
            }


        }
        public async Task<IActionResult> Delete(int id)
        {
            if (id != null)
            {
                var tracks = await trackService.GetAllTracksAsync(x => x.AlbumId == id);
                tracks.ToList().ForEach(x => x.AlbumId = null);

                await likeService.DeleteAllLikesByAlbum(id);
                await albumService.DeleteAlbumByIdAsync(id);

                return RedirectToAction("AllAlbums");
            }
            return RedirectToAction("AllAlbums");

        }
        public async Task<IActionResult> AddAlbum()
        {
            var tempUser = await userManager.FindByEmailAsync(User.Identity.Name);
            User user = await userService.GetUserAsync(x => x.Email == tempUser.Email);

            var singles = await trackService.GetAllTracksAsync(x=>x.IsYoutube == false);
            singles = singles.Where(x => x.AlbumId is null && x.UserId == user.Id).ToList();

            if(singles.Count() <= 0)
            {
                TempData["Message"] = "No records with audio files found.";
                return RedirectToAction("AllAlbums");
            }

            var model = new AddAlbumViewModel();
            Dictionary<int, string> pics = new Dictionary<int, string>();

           
            foreach (var item in singles)
            {
                pics.Add(item.Id, item.CoverImage);
            }
            var tracks = singles.Select(t => new SelectListItem
            {
                Value = t.Id.ToString(),
                Text = t.Title,
                Selected = false
            }).ToList();
            model.Tracks = tracks;
            model.TrackPictures = pics;

            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> AddAlbum(AddAlbumViewModel model)
        {
            if (ModelState.IsValid)
            {
                var tempUser = await userManager.FindByEmailAsync(User.Identity.Name);
                User user = await userService.GetUserAsync(x => x.Email == tempUser.Email);

                var imageUploadResult = await cloudService.UploadImageAsync(model.Picture);

                Album album = new Album
                {
                    Title = model.Title,
                    UserId = user.Id,
                    CoverImage = imageUploadResult
                };
                album.UserId = user.Id;
                await albumService.AddAlbumAsync(album);

                var tracks = await trackService.GetAllTracksAsync(x=>x.IsYoutube == false);

                if (model.SelectedTracksIds != null && model.SelectedTracksIds.Any())
                {
                    var selectedTracks = tracks.Where(x => model.SelectedTracksIds.Contains(x.Id)).ToList();

                    foreach (var track in selectedTracks)
                    {
                        track.AlbumId = album.Id;
                        await trackService.UpdateTrackAsync(track);
                    }

                }
                else 
                {
                    ModelState.AddModelError("SelectedTracksIds", "Please select at least one track.");
                }
                

                return RedirectToAction("AllAlbums");
            }
            else
            {
                return View(model);
            }
        }

        public async Task<IActionResult> AllAlbums(AlbumFilterViewModel? filter)
        {
            var query = albumService.GetAll().AsQueryable();
            var filterModel = new AlbumFilterViewModel();

            if (string.IsNullOrEmpty(filter.Search))
            {

                var model = albumService.AllWithInclude().Include(x => x.User).Select(x => new AlbumViewModel()
                {
                    Id = x.Id,
                    Title = x.Title,
                    CoverImage = x.CoverImage,
                    UserName = x.User.Username
                }).ToList();

                filterModel = new AlbumFilterViewModel
                {
                    Albums = model,
                    ArtistId = filter.ArtistId,
                    Search = filter.Search,

                };
            }
            else
            {
                var tempUsers = await userService.GetAllUserNamesAsync();
                var tempAlbums = await albumService.GetAllAlbumNamesAsync();

                if (tempUsers.Contains(filter.Search))
                {
                    query = query.Where(x => x.User.Username == filter.Search);
                }
                if (tempAlbums.Contains(filter.Search))
                {
                    query = query.Where(x => x.Title == filter.Search);
                }

                filterModel = new AlbumFilterViewModel
                {
                    Albums = query.Include(x => x.User)
                .Select(x => new AlbumViewModel()
                {
                    Title = x.Title,
                    CoverImage = x.CoverImage,
                    Id = x.Id,
                    UserName = x.User.Username
                }).ToList(),
                    ArtistId = filter.ArtistId,
                    Search = filter.Search
                };

            }
            return View(filterModel);

        }


        [HttpGet]
        public IActionResult Tracks(int albumId)
        {
            var tracks = trackService.AllWithInclude().Include(x=>x.User)
                .Where(t => t.AlbumId == albumId)
                .Select(t => new
                {
                    TrackId = t.Id,
                    Name = t.Title,
                    UserName = t.User.Username,
                    ImageLink = t.CoverImage,
                    AudioUrl = t.AudioUrl // This must be a valid URL
                })
                .ToList();

            if(tracks == null)
            {
                return NotFound();
            }

            return Ok(tracks);
        }
    }
}
