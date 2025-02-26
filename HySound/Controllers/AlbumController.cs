﻿using HySound.Core.Service;
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

        private readonly Cloudinary _cloudinary;
        private readonly IConfiguration _configuration;
        CloudinaryService cloudService;
        public AlbumController(UserManager<IdentityUser> _userManager,IConfiguration configuration, CloudinaryService cloud, ITrackService _trackService,IAlbumService _albumService, IUserService userService)
        {
            albumService = _albumService;
            this.userService = userService;
            trackService = _trackService;

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

            if(model.ImageFile != null)
            {
                var imageUploadResult = await cloudService.UploadImageAsync(model.ImageFile);

                Album album = albumService.GetAll().Where(x => x.Id == id).FirstOrDefault();
                album.Title = model.Title;
                album.CoverImage = imageUploadResult;
                album.ReleaseDate = model.ReleaseDate;
                album.UserId = model.UserId;

                await albumService.UpdateAlbumAsync(album);
                return RedirectToAction("AllAlbums");
            }
            else
            {
                Album album = albumService.GetAll().Where(x => x.Id == id).FirstOrDefault();
                album.Title = model.Title;
                album.CoverImage = model.AlbumCover;
                album.ReleaseDate = model.ReleaseDate;
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
                tracks.ToList().ForEach(x=> x.AlbumId = null);

                foreach(var track in tracks)
                {
                    await trackService.UpdateTrackAsync(track);
                }
                await albumService.DeleteAlbumByIdAsync(id);
                
                return RedirectToAction("AllAlbums");
            }
            return RedirectToAction("AllAlbums");

        }
        public async Task<IActionResult> AddAlbum()  
        {
            var model = new AddAlbumViewModel();
            Dictionary<int, string> pics = new Dictionary<int, string>();

            var singles = await trackService.GetAllTracksAsync();
            singles = singles.Where(x=>x.AlbumId is null).ToList();

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
                    ReleaseDate = model.ReleaseDate,
                    UserId = user.Id,
                    CoverImage = imageUploadResult
                };
                album.UserId = user.Id;
                await albumService.AddAlbumAsync(album);

                var tracks = await trackService.GetAllTracksAsync();

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
                    ModelState.AddModelError("", "Please select at least one track.");
                    return View(model);
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
                    ReleaseDate = x.ReleaseDate,
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
                    query = query.Where(x=>x.Title ==filter.Search);
                }
                
                filterModel = new AlbumFilterViewModel
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
                    Search = filter.Search
                };

            }
            return View(filterModel);

        }
    }
}
