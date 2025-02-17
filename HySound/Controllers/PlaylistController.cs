using HySound.Core.Service;
using HySound.Core.Service.IService;
using HySound.Models.Models;
using HySound.ViewModels.Playlist;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net.WebSockets;

namespace HySound.Controllers
{
    public class PlaylistController : Controller
    {
        IPlaylistService playlistService;
        public PlaylistController(IPlaylistService _playlistService)
        {
            playlistService = _playlistService;
        }
        public async Task<IActionResult> AddPlaylist()
        {
            Playlist playlist = new Playlist();
            return View(playlist);
        }
        [HttpPost]
        public async Task<IActionResult> AddPlaylist(Playlist model)
        {
            if (ModelState.IsValid)
            {
                await playlistService.AddPlaylistAsync(model);
                return RedirectToAction("AllPlaylists");
            }
            else
            {
                return View(model);
            }
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
