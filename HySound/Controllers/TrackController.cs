using CloudinaryDotNet;
using HySound.Core.Service;
using HySound.Core.Service.IService;
using HySound.Models.Models;
using HySound.ViewModels;
using HySound.ViewModels.Track;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace HySound.Controllers
{
    public class TrackController : Controller
    {
        private readonly UserManager<IdentityUser> userManager;
        private readonly Cloudinary _cloudinary;
        private readonly IConfiguration _configuration;
        CloudinaryService cloudService;
        ITrackService trackService;
        IGenreService genreService;
        IUserService userService;
        IPlaylistService playlistService;
        ICommentService commentService;
        ILikeService likesService;
        public TrackController(ILikeService likes, ICommentService commentService, UserManager<IdentityUser> _userManager, CloudinaryService cloud, IConfiguration configuration, IPlaylistService playlistService, ITrackService trackService, IUserService userService, IGenreService genreService)
        {
            userManager = _userManager;
            this.cloudService = cloud;
            this.trackService = trackService;
            this.userService = userService;
            this.genreService = genreService;
            this.playlistService = playlistService;
            this.commentService = commentService;
            likesService = likes;

            _configuration = configuration;
            var account = new Account(
           _configuration["Cloudinary:CloudName"],
           _configuration["Cloudinary:ApiKey"],
           _configuration["Cloudinary:ApiSecret"]
       );
            _cloudinary = new Cloudinary(account);
        }
        public async Task<IActionResult> Delete(int id)
        {
            if (id != null)
            {
                await likesService.DeleteAllLikesByTracks(id);
                await playlistService.DeleteTrackFromAllPlaylistsAsync(id);

                await trackService.DeleteTrackByIdAsync(id);
            }
            return RedirectToAction("AllTracks");
        }

        [HttpPost]
        public async Task<IActionResult> Update(EditTrackViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.GenresList = new SelectList(await genreService.GetAllGenresAsync(), "Id", "Name");
                model.UserList = new SelectList(await userService.GetAllUsersAsync(), "Id", "Username");
                return View(model);
            }

            var tempUser = await userManager.FindByEmailAsync(User.Identity.Name);
            User user = await userService.GetUserAsync(x => x.Email == tempUser.Email);

            Track track = trackService.GetAll().FirstOrDefault(x => x.Id == model.Id);
            if (track == null)
                return NotFound();

            // Update image
            if (model.ImageFile != null)
            {
                var imageUploadResult = await cloudService.UploadImageAsync(model.ImageFile);
                track.CoverImage = imageUploadResult;
            }
            else
            {
                track.CoverImage = model.ImageUrl; // Preserve existing image
            }

            // Update basic fields
            track.Title = model.Title;
            track.GenreId = model.GenreId;
            track.UserId = user.Id;
            track.IsYoutube = model.IsYoutube;

            // Update audio
            if (model.IsYoutube)
            {
                track.AudioUrl = model.AudioUrl; // Use provided YouTube URL
            }
            else if (model.audioFile != null) // Only update AudioUrl if a new file is uploaded
            {
                var audioUploadResult = await cloudService.UploadTrackAsync(model.audioFile);
                if (string.IsNullOrEmpty(audioUploadResult))
                {
                    ModelState.AddModelError("", "Failed to upload audio file to Cloudinary.");
                    model.GenresList = new SelectList(await genreService.GetAllGenresAsync(), "Id", "Name");
                    model.UserList = new SelectList(await userService.GetAllUsersAsync(), "Id", "Username");
                    return View(model);
                }
                track.AudioUrl = audioUploadResult;
            }
            // If no new audioFile and not YouTube, track.AudioUrl remains unchanged

            await trackService.UpdateTrackAsync(track);
            return RedirectToAction("AllTracks");
        }
        [HttpGet]
        public async Task<IActionResult> Update(int id)
        {

            var model = trackService.GetAll()
                .Where(x => x.Id == id)
                .Include(x => x.Genre)
                .Include(x => x.User)
                .Select(x => new EditTrackViewModel()
                {
                    Id = x.Id,
                    Title = x.Title,
                    AudioUrl = x.AudioUrl,
                    ImageUrl = x.CoverImage,
                    GenreId = x.GenreId,
                    GenresList = new SelectList(genreService.GetAll(), "Id", "Name"),
                    UserId = x.UserId,
                    UserList = new SelectList(userService.GetAll(), "Id", "Username"),
                    IsYoutube = x.IsYoutube
                })
                .FirstOrDefault();

            if (model == null)
            {
                return NotFound(); // Handle missing track
            }

            if (!model.IsYoutube)
            {
            }

            return View(model);
        }

        public async Task<IActionResult> AddTrack()
        {
            var model = new AddTrackViewModel();
            model.GenresList = new SelectList(await genreService.GetAllGenresAsync(), "Id", "Name");

            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> AddTrack(AddTrackViewModel model)
        {
            if (ModelState.IsValid)
            {
                var tempUser = await userManager.FindByEmailAsync(User.Identity.Name);
                User user = await userService.GetUserAsync(x => x.Email == tempUser.Email);

                var imageUploadResult = await cloudService.UploadImageAsync(model.imageFile);

                if (model.IsYoutube)
                {
                    Track track = new Track()
                    {
                        Title = model.Title,
                        IsYoutube = true,
                        AudioUrl = model.AudioUrl,
                        UserId = user.Id,
                        GenreId = model.GenreId,
                        CoverImage = imageUploadResult
                    };
                    await trackService.AddTrackAsync(track);
                    return RedirectToAction("AllTracks");
                }
                else
                {
                    var audioUploadResult = await cloudService.UploadTrackAsync(model.audioFile);
                    Track track = new Track()
                    {
                        Title = model.Title,
                        IsYoutube = false,
                        AudioUrl = audioUploadResult,
                        UserId = user.Id,
                        GenreId = model.GenreId,
                        CoverImage = imageUploadResult
                    };
                    await trackService.AddTrackAsync(track);
                    return RedirectToAction("AllTracks");
                }

            }
            else
            {
                model.GenresList = new SelectList(await genreService.GetAllGenresAsync(), "Id", "Name");
                return View(model);
            }

        }


        [HttpPost]
        public async Task<IActionResult> AddToPlaylists([FromBody] AddToPlaylistsRequest request)
        {
            if (request == null || request.TrackId <= 0)
            {
                return Json(new { success = false, message = "No track selected." });
            }

            if (request.PlaylistIds == null || request.PlaylistIds.Count == 0)
            {
                return Json(new { success = false, message = "No playlists selected." });
            }

            try
            {
                foreach (var playlistId in request.PlaylistIds)
                {
                    if (playlistId <= 0)
                    {
                        return Json(new { success = false, message = $"Invalid playlist ID: {playlistId}" });
                    }

                    var track = await trackService.GetTrackByIdAsync(request.TrackId);
                    var playlist = await playlistService.GetPlaylistByIdAsync(playlistId);
                    if (track != null && playlist != null)
                    {
                        await playlistService.AddTrackToPlaylistAsync(playlist, track);
                    }
                    else
                    {
                        return Json(new { success = false, message = $"Invalid track or playlist: TrackId={request.TrackId}, PlaylistId={playlistId}" });
                    }
                }
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        public class AddToPlaylistsRequest
        {
            public int TrackId { get; set; }
            public List<int> PlaylistIds { get; set; } // Changed to List<int>
        }
        public async Task<IActionResult> TrackDetails(int id)
        {
            var tempUser = await userManager.FindByEmailAsync(User.Identity.Name);
            User user = await userService.GetUserAsync(x => x.Email == tempUser.Email);


            Track track = await trackService.GetTrackByIdAsync(id);

            var comments = commentService.AllWithInclude().Include(x => x.User).Where(x => x.TrackId == id);
            User trackUser = null;
            Genre trackGenre = null;
            if (track != null)
            {
                if (track.UserId.HasValue)
                {
                    trackUser = await userService.GetUserByIdAsync(track.UserId.Value); // Assuming you have a UserService
                }

                if (track.GenreId.HasValue)
                {
                    trackGenre = await genreService.GetGenreByIdAsync(track.GenreId.Value); // Assuming you have a GenreService
                }

                int likesCount = likesService.GetAll().Where(x => x.TrackId == id).Count();

                bool isLiked = false;

                var like = likesService.GetAll().Where(x => x.UserId == user.Id && x.TrackId == id);
                if (like.Any())
                {
                    isLiked = true;
                }

                TrackDetailsViewModel model = new TrackDetailsViewModel

                {
                    Id = id,
                    Title = track.Title,
                    TrackImage = track.CoverImage,
                    Comments = comments.ToList(),
                    Username = track.User.Username,
                    Genre = trackGenre.Name,
                    LikesCount = likesCount,
                    IsLiked = isLiked,
                    AudioUrl = track.AudioUrl
                };
                return View(model);
            }
            else
            {
                return RedirectToAction("Home", "Index");
            }

        }
        public async Task<IActionResult> AllTracks(TrackFilterViewModel? filter)
        {
            var query = trackService.GetAll().AsQueryable();
            var playlists = await playlistService.GetAllPlaylistsAsync();


            if (filter.GenreId == null && string.IsNullOrEmpty(filter.Title))
            {
                var model = trackService.AllWithInclude().Include(x => x.Genre).ThenInclude(x => x.Tracks).Include(x => x.User).ThenInclude(x => x.Tracks).Select(x => new TrackViewModel()
                {
                    TrackId = x.Id,
                    Title = x.Title,
                    AudioUrl = x.AudioUrl,
                    GenreName = x.Genre.Name,
                    UserName = x.User.Username,
                    IsYoutube = x.IsYoutube,
                    ImageLink = x.CoverImage,
                    LikesCount = likesService.GetAll().Where(y => y.TrackId == x.Id).Count()
                }).ToList();

                if (!User.IsInRole("Admin"))
                {
                    var tempUser = await userManager.FindByEmailAsync(User.Identity.Name);
                    User user = await userService.GetUserAsync(x => x.Email == tempUser.Email);
                    model = model.Where(x => x.UserName == user.Username).ToList();
                }

                var filterModel = new TrackFilterViewModel
                {
                    Tracks = model,
                    Genres = new SelectList(genreService.GetAll(), "Id", "Name"),
                    Playlists = playlists.ToList()

                };
                return View(filterModel);
            }
            else
            {
                if (filter.GenreId != null)
                {
                    query = query.Where(x => x.GenreId == filter.GenreId);
                }
                if (!string.IsNullOrEmpty(filter.Title))
                {
                    query = query.Where(x => x.Title == filter.Title);
                }

                if (!User.IsInRole("Admin"))
                {
                    var tempUser = await userManager.FindByEmailAsync(User.Identity.Name);
                    User user = await userService.GetUserAsync(x => x.Email == tempUser.Email);

                    var filterModel = new TrackFilterViewModel
                    {
                        Tracks = query.Include(x => x.Genre).ThenInclude(x => x.Tracks)
               .Include(x => x.User).ThenInclude(x => x.Tracks)
               .Select(x => new TrackViewModel()
               {
                   TrackId = x.Id,
                   Title = x.Title,
                   AudioUrl = x.AudioUrl,
                   GenreName = x.Genre.Name,
                   UserName = x.User.Username,
                   ImageLink = x.CoverImage,
                   LikesCount = likesService.GetAll().Where(y => y.TrackId == x.Id).Count()
               }).Where(x=>x.UserName==user.Username).ToList(),
                        Genres = new SelectList(genreService.GetAll(), "Id", "Name"),
                        Title = filter.Title,
                        GenreId = filter.GenreId,
                        Playlists = playlists.ToList()
                    };


                    return View(filterModel);

                }
                else
                {
                    var filterModel = new TrackFilterViewModel
                    {
                        Tracks = query.Include(x => x.Genre).ThenInclude(x => x.Tracks)
               .Include(x => x.User).ThenInclude(x => x.Tracks)
               .Select(x => new TrackViewModel()
               {
                   TrackId = x.Id,
                   Title = x.Title,
                   AudioUrl = x.AudioUrl,
                   GenreName = x.Genre.Name,
                   UserName = x.User.Username,
                   ImageLink = x.CoverImage,
                   LikesCount = likesService.GetAll().Where(y => y.TrackId == x.Id).Count()
               }).ToList(),
                        Genres = new SelectList(genreService.GetAll(), "Id", "Name"),
                        Title = filter.Title,
                        GenreId = filter.GenreId,
                        Playlists = playlists.ToList()
                    };

                    return View(filterModel);
                }

            }

        }
    }
}
