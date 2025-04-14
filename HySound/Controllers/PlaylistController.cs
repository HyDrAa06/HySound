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
            if (ModelState.IsValid)
            {

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

                if (User.IsInRole("User"))
                {
                    return RedirectToAction("Library", "Home");

                }

                return RedirectToAction("AllPlaylists");
            }
            else
            {
                return View(model);
            }
        }


   


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

            if (!User.IsInRole("Admin"))
            {
                var tempUser = await userManager.FindByEmailAsync(User.Identity.Name);
                User user = await userService.GetUserAsync(x => x.Email == tempUser.Email);

                playlists = playlists.Where(x => x.UserName ==user.Username);
            }

            return View(playlists);
            
        }

       
        public async Task<IActionResult> Delete(int id)
        {
            await likeService.DeleteAllLikesByPlaylist(id);
            await playlistService.DeletePlaylistByIdAsync(id);

            return RedirectToAction("AllPlaylists");
        }

        [HttpGet]
        public async Task<IActionResult> Update(int id)
        {
            Playlist playlist = await playlistService.GetPlaylistByIdAsync(id);

            EditPlaylistViewModel viewModel = new EditPlaylistViewModel()
            {
                Id = id,
                PictureUrl = playlist.CoverImage,
                Title= playlist.Title
            };
            return View(viewModel);
        }
        [HttpPost]
        public async Task<IActionResult> Update(EditPlaylistViewModel model)
        {
            try
            {
                Console.WriteLine($"model.Id: {model.Id}");
                Console.WriteLine($"model.Title: {model.Title}");
                Console.WriteLine($"model.Picture is null: {model.Picture == null}");
                if (model.Picture != null)
                {
                    Console.WriteLine($"File Name: {model.Picture.FileName}");
                    Console.WriteLine($"File Size: {model.Picture.Length}");
                }
                Console.WriteLine($"model.PictureUrl: {model.PictureUrl}");

                var playlist = await playlistService.GetPlaylistByIdAsync(model.Id);
                if (playlist == null)
                {
                    return NotFound("Playlist not found");
                }

                if (model.Picture != null && model.Picture.Length > 0)
                {
                    var imageUploadResult = await cloudService.UploadImageAsync(model.Picture);
                    Console.WriteLine($"Upload Result: {imageUploadResult}");
                    if (string.IsNullOrEmpty(imageUploadResult))
                    {
                        throw new Exception("Image upload failed");
                    }
                    playlist.CoverImage = imageUploadResult;
                }
                else
                {
                    Console.WriteLine("No new image, using existing PictureUrl");
                    playlist.CoverImage = model.PictureUrl ?? playlist.CoverImage;                 }

                playlist.Title = model.Title;

                await playlistService.UpdatePlaylistAsync(playlist);
                Console.WriteLine($"Saved - Title: {playlist.Title}, CoverImage: {playlist.CoverImage}");
                return RedirectToAction("AllPlaylists");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return StatusCode(500, $"Error updating playlist: {ex.Message}");
            }
        }
    }
}
