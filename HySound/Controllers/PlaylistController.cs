using CloudinaryDotNet;
using HySound.Core.Service;
using HySound.Core.Service.IService;
using HySound.Models.Models;
using HySound.ViewModels.Playlist;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Net.WebSockets;
using System.Runtime.InteropServices;

namespace HySound.Controllers
{
    public class PlaylistController : Controller
    {
        CloudinaryService cloudService;
        private readonly Cloudinary _cloudinary;
        private readonly IConfiguration _configuration;

        IPlaylistService playlistService;


        public PlaylistController(IConfiguration configuration, CloudinaryService cloud ,IPlaylistService _playlistService)
        {
            playlistService = _playlistService;

            this.cloudService = cloud;

            _configuration = configuration;
            var account = new Account(
           _configuration["Cloudinary:CloudName"],
           _configuration["Cloudinary:ApiKey"],
           _configuration["Cloudinary:ApiSecret"]
       );
            _cloudinary = new Cloudinary(account);
        }

        [HttpPost]
        [HttpPost]
        public async Task<IActionResult> AddPlaylist(PlaylistViewModel model)
        {
            if (model == null) return Content("Model is null");
            if (model.Picture == null) return Content("Picture is null");
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                return Content("Validation failed: " + string.Join(", ", errors));
            }

            var imageUploadResult = await cloudService.UploadImageAsync(model.Picture);
            Playlist playlist = new Playlist()
            {
                Title = model.Title,
                CoverImage = imageUploadResult
            };
            await playlistService.AddPlaylistAsync(playlist);
            return RedirectToAction("AllPlaylists");
        }
        public async Task<IActionResult> AllPlaylists()
        {
            var playlists = playlistService.AllWithInclude().Include(x => x.User).Select(x => new PlaylistViewModel()
            {
                CoverImage =x.CoverImage,
                Title = x.Title,
                UserName = x.User.Username
            });

            return View(playlists);
            
        }

        public async Task<IActionResult> Delete(int id)
        {
            await playlistService.DeletePlaylistByIdAsync(id);
            return RedirectToAction("AllPlaylists");
        }
        public async Task<IActionResult> Update(int id)
        {
            Playlist playlist = await playlistService.GetPlaylistByIdAsync(id);
            return View(playlist);
        }

        [HttpPost]
        public async Task<IActionResult> Update(Playlist model)
        {
            if(ModelState.IsValid)
            {
                await playlistService.UpdatePlaylistAsync(model);
                return RedirectToAction("AllPlaylists");
            }
            else
            {
                return View(model);
            }
        }
    }
}
