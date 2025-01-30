using HySound.Core.Service;
using HySound.Core.Service.IService;
using HySound.Models;
using HySound.Models.Album;
using HySound.Models.Models;
using HySound.Models.Track;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace HySound.Controllers
{
    public class AlbumController : Controller
    {
        IAlbumService albumService;
        IUserService userService;
        public AlbumController(IAlbumService _albumService, IUserService userService)
        {
            albumService = _albumService;
            this.userService = userService;
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]

        public async Task<IActionResult> Update(int id)
        {
            var model = albumService.GetAll().Where(x => x.Id == id).Include(x => x.User)
                .Select(x=>new EditAlbumViewModel()
                {
                    AlbumCover = x.CoverImage,
                    ReleaseDate = x.ReleaseDate,
                    Title = x.Title,
                    UserId = x.UserId,
                    UserList = new SelectList(userService.GetAll(), "Id", "Username")
                }).FirstOrDefault();

            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> Update(int id, EditAlbumViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.UserList = new SelectList(await userService.GetAllUsersAsync(), "Id", "Username");

                return View(model);
            }

            Album album = albumService.GetAll().Where(x => x.Id == id).FirstOrDefault();
            album.Title = model.Title;
            album.CoverImage = model.AlbumCover;
            album.ReleaseDate = model.ReleaseDate;
            album.UserId = model.UserId;

            await albumService.UpdateAlbumAsync(album);
            return RedirectToAction("AllAlbums");
        }
        public async Task<IActionResult> Delete(int id)
        {
            if (id != null)
            {
                await albumService.DeleteAlbumByIdAsync(id);
                return RedirectToAction("AllAlbums");
            }
            return RedirectToAction("AllAlbums");

        }
        public async Task<IActionResult> AddAlbum()
        {
            var model = new AddAlbumViewModel();
            model.UserList = new SelectList(await userService.GetAllUsersAsync(), "Id", "Username");
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> AddAlbum(AddAlbumViewModel model)
        {

            if (ModelState.IsValid)
            {
                Album album = new Album
                {
                    Title = model.Title,
                    ReleaseDate = model.ReleaseDate,
                    UserId = model.UserId,

                };
                await albumService.AddAlbumAsync(album);
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

            if (string.IsNullOrEmpty(filter.ArtistName))
            {
                var model = albumService.AllWithInclude().Include(x => x.User).Select(x => new AlbumViewModel()
                {
                    Id = x.Id,
                    Title = x.Title,
                    CoverImage = x.CoverImage,
                    ReleaseDate = x.ReleaseDate,
                    UserName = x.User.Username
                }).ToList();

                var filterModel = new AlbumFilterViewModel
                {
                    Albums = model,
                    ArtistId = filter.ArtistId,
                    ArtistName = filter.ArtistName

                };
                return View(filterModel);
            }
            else
            {
                if (filter.ArtistName != null)
                {
                    query = query.Where(x => x.User.Username == filter.ArtistName);
                }
                var filterModel = new AlbumFilterViewModel
                {
                    Albums = query.Include(x => x.User)
                .Select(x => new AlbumViewModel()
                {
                    Title = x.Title,
                    CoverImage = x.CoverImage,
                    ReleaseDate = x.ReleaseDate,
                    Id = x.Id,
                    UserName = x.User.Username
                }).ToList(),
                    ArtistId = filter.ArtistId,
                    ArtistName = filter.ArtistName
                };

                return View(filterModel);
            }
        }
    }
}
