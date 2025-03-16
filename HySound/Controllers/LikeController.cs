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
        IPlaylistService _playlistService;
        IAlbumService _albumService;


        public LikeController(IAlbumService albumService,IPlaylistService playlistService,IUserService userService, UserManager<IdentityUser> userManager,ITrackService trackService,ILikeService likeService)
        {
            _likeService = likeService;
            _trackService = trackService;
            _userManager = userManager;
            _userService = userService;
            _playlistService = playlistService;
            _albumService = albumService;
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
        public async Task<IActionResult> LikeAlbum(int id)
        {
            var tempUser = await _userManager.FindByEmailAsync(User.Identity.Name);
            User user = await _userService.GetUserAsync(x => x.Email == tempUser.Email);

            Album album = await _albumService.GetAlbumByIdAsync(id);

            Like like = new Like()
            {
                AlbumId = id,
                Album = album,
                User = user,
                UserId = user.Id
            };
            await _likeService.AddLikeAsync(like);
            return RedirectToAction("Library", "Home");
        }
        public async Task<IActionResult> DislikeAlbum(int id)
        {
            var tempUser = await _userManager.FindByEmailAsync(User.Identity.Name);
            User user = await _userService.GetUserAsync(x => x.Email == tempUser.Email);

            Like like = await _likeService.GetLikeAsync(x => x.AlbumId == id && x.UserId == user.Id);
            if (like != null)
            {
                await _likeService.DeleteLikeAsync(like);
            }
            return RedirectToAction("Library", "Home");
        }
        public async Task<IActionResult> LikePlaylist(int id)
        {
            var tempUser = await _userManager.FindByEmailAsync(User.Identity.Name);
            User user = await _userService.GetUserAsync(x => x.Email == tempUser.Email);

            Playlist playlist = await _playlistService.GetPlaylistByIdAsync(id);

            Like like = new Like()
            {
                PlaylistId = id,
                Playlist = playlist,
                User = user,
                UserId = user.Id
            };
            await _likeService.AddLikeAsync(like);
            return RedirectToAction("Library", "Home");
        }
        public async Task<IActionResult> DislikePlaylist(int id)
        {
            var tempUser = await _userManager.FindByEmailAsync(User.Identity.Name);
            User user = await _userService.GetUserAsync(x => x.Email == tempUser.Email);

            Like like = await _likeService.GetLikeAsync(x => x.PlaylistId == id && x.UserId == user.Id);
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
