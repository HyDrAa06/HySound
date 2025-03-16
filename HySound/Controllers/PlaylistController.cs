using CloudinaryDotNet;
using HySound.Core.Service;
using HySound.Core.Service.IService;
using HySound.Models.Models;
using HySound.ViewModels.Album;
using HySound.ViewModels.Playlist;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Net.WebSockets;
using System.Runtime.InteropServices;

namespace HySound.Controllers
{
    public class PlaylistController : Controller
    {
        private readonly UserManager<IdentityUser> userManager;
        CloudinaryService cloudService;
        private readonly Cloudinary _cloudinary;
        private readonly IConfiguration _configuration;

        ITrackService trackService;
        ILikeService likeService;
        IPlaylistService playlistService;
        IUserService userService;


        public PlaylistController(ILikeService like,ITrackService track,IUserService _userService,UserManager<IdentityUser>_userManager,IConfiguration configuration, CloudinaryService cloud ,IPlaylistService _playlistService)
        {
            trackService = track;
            playlistService = _playlistService;
            userManager = _userManager;
            userService = _userService;
            likeService = like;

            this.cloudService = cloud;

            _configuration = configuration;
            var account = new Account(
           _configuration["Cloudinary:CloudName"],
           _configuration["Cloudinary:ApiKey"],
           _configuration["Cloudinary:ApiSecret"]
       );
            _cloudinary = new Cloudinary(account);
        }

        public async Task<IActionResult> AddPlaylist()
        {
            AddPlaylistViewModel model = new AddPlaylistViewModel();
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> AddPlaylist(AddPlaylistViewModel model)
        {
            if (model == null) return Content("Model is null");
            if (model.Picture == null) return Content("Picture is null");
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                return Content("Validation failed: " + string.Join(", ", errors));
            }
            var tempUser = await userManager.FindByEmailAsync(User.Identity.Name);
            User user = await userService.GetUserAsync(x => x.Email == tempUser.Email);

            var imageUploadResult = await cloudService.UploadImageAsync(model.Picture);
            Playlist playlist = new Playlist()
            {
                Title = model.Title,
                CoverImage = imageUploadResult,
                UserId = user.Id,

            };
            await playlistService.AddPlaylistAsync(playlist);
            return RedirectToAction("AllPlaylists");
        }


     //   [HttpPost]
     //   public async Task<IActionResult> AddPlaylist(PlaylistViewModel model)
     //   {
     //       if (model == null) return Content("Model is null");
     //       if (model.Picture == null) return Content("Picture is null");
     //       if (!ModelState.IsValid)
     //       {
     //           var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
     //           return Content("Validation failed: " + string.Join(", ", errors));
     //       }
     //       var tempUser = await userManager.FindByEmailAsync(User.Identity.Name);
     //       User user = await userService.GetUserAsync(x => x.Email == tempUser.Email);
     //
     //       var imageUploadResult = await cloudService.UploadImageAsync(model.Picture);
     //       Playlist playlist = new Playlist()
     //       {
     //           Title = model.Title,
     //           CoverImage = imageUploadResult,
     //           UserId = user.Id,
     //           
     //       };
     //       await playlistService.AddPlaylistAsync(playlist);
     //       return RedirectToAction("AllPlaylists");
     //   }


        public async Task<IActionResult> PlaylistDetails(int id)
        {
            var tracks = await playlistService.GetTracksOfPlaylist(id);
            var tempUser = await userManager.FindByEmailAsync(User.Identity.Name);
            User user = await userService.GetUserAsync(x => x.Email == tempUser.Email);



            bool isLiked = false;

            var like = likeService.GetAll().Where(x => x.UserId == user.Id && x.PlaylistId == id);
            if (like.Any())
            {
                isLiked = true;
            }

            var playlist = await playlistService.GetAll().Where(x => x.Id == id).Include(x => x.PlaylistTracks).ThenInclude(x => x.Track).Include(x => x.User)
                .Select(x => new PlaylistViewModel
                {
                    CoverImage = x.CoverImage,
                    Id = id,
                    Title = x.Title,
                    Tracks = tracks,
                    Description = x.Description,
                    IsLiked = isLiked
                }).FirstOrDefaultAsync();

            return View(playlist);
        }

        [HttpGet]
        public async Task<IActionResult> Tracks(int playlistId)
        {
            try
            {
                var tracks = await playlistService.GetTracksOfPlaylist(playlistId);

                return Json(tracks);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Error fetching playlist tracks" });
            }
        }

        public async Task<IActionResult> AllPlaylists()
        {
            var playlists = playlistService.AllWithInclude().Include(x => x.User).Select(x => new PlaylistViewModel()
            {
                CoverImage =x.CoverImage,
                Title = x.Title,
                UserName = x.User.Username,
                Id = x.Id,
                Description = x.Description,
            });

            return View(playlists);
            
        }

       
        public async Task<IActionResult> Delete(int id)
        {
            await playlistService.DeletePlaylistByIdAsync(id);
            return RedirectToAction("AllPlaylists");
        }
        [HttpPost]
        public async Task<IActionResult> Update(int id, string title, IFormFile picture)
        {
            try
            {
                Console.WriteLine($"Update called with ID: {id}, Title: {title}, Picture: {(picture != null ? picture.FileName : "null")}");
                var imageUploadResult = await cloudService.UploadImageAsync(picture);

                if (id <= 0)
                {
                    return BadRequest("Invalid playlist ID");
                }

                if (string.IsNullOrWhiteSpace(title))
                {
                    return BadRequest("Title cannot be empty");
                }

                var playlist = await playlistService.GetPlaylistByIdAsync(id);
                if (playlist == null)
                {
                    return NotFound("Playlist not found");
                }

                playlist.Title = title;
                if (picture != null && picture.Length > 0)
                {
                    using var memoryStream = new MemoryStream();
                    await picture.CopyToAsync(memoryStream);
                    playlist.CoverImage = imageUploadResult;
                }

                await playlistService.UpdatePlaylistAsync(playlist);
                return Ok("Playlist updated successfully");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Update error: {ex.Message}");
                return StatusCode(500, $"Error updating playlist: {ex.Message}");
            }
        }
       
    }
}
