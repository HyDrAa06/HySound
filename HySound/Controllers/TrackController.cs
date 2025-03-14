﻿using CloudinaryDotNet;
using HySound.Core.Service;
using HySound.Core.Service.IService;
using HySound.Models.Models;
using HySound.ViewModels;
using HySound.ViewModels.Track;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Numerics;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

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
        public TrackController(ICommentService commentService,UserManager<IdentityUser> _userManager, CloudinaryService cloud,IConfiguration configuration, IPlaylistService playlistService,ITrackService trackService, IUserService userService, IGenreService genreService)
        {
            userManager = _userManager;
            this.cloudService = cloud;
            this.trackService = trackService;
            this.userService = userService;
            this.genreService = genreService;
            this.playlistService = playlistService;
            this.commentService = commentService;

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
            if(id != null)
            {
                await trackService.DeleteTrackByIdAsync(id);
            }
            return RedirectToAction("AllTracks");
        }

        [HttpPost]
        public async Task<IActionResult> Update(int id, EditTrackViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.GenresList = new SelectList(await genreService.GetAllGenresAsync(), "Id", "Name");
                model.UserList = new SelectList(await userService.GetAllUsersAsync(), "Id", "Username");

                return View(model);
            }
            var imageUploadResult = await cloudService.UploadImageAsync(model.ImageFile);

            Track track = trackService.GetAll().Where(x => x.Id == id).FirstOrDefault();
            track.Title = model.Title;
            track.AudioUrl = model.AudioUrl;
            track.Plays=model.Plays;
            track.GenreId = model.GenreId;
            track.UserId = model.UserId;
            track.CoverImage = imageUploadResult;
            await trackService.UpdateTrackAsync(track);
            return RedirectToAction("AllTracks");
        }

        [HttpGet]
        public async Task<IActionResult> Update(int id)
        {
            var model = trackService.GetAll().Where(x => x.Id == id).Include(x => x.Genre).Include(x => x.User)
            .Select(x => new EditTrackViewModel()
            {
                Title=x.Title,
                Plays=x.Plays,
                AudioUrl=x.AudioUrl,
                ImageUrl = x.CoverImage,
                GenreId=x.GenreId,
                GenresList= new SelectList(genreService.GetAll(), "Id", "Name"),
                UserId=x.UserId,
                UserList = new SelectList(userService.GetAll(), "Id", "Username")

            }).FirstOrDefault();

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
                        IsYoutube=true,
                        AudioUrl = model.AudioUrl,
                        UserId = user.Id,
                        Plays = model.Plays,
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
                        Plays = model.Plays,
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
            Track track = await trackService.GetTrackByIdAsync(id);

            var comments = commentService.AllWithInclude().Include(x => x.User).Where(x=>x.TrackId==id);
            User trackUser = null;
            Genre trackGenre = null;
            if( track != null)
            {
                if (track.UserId.HasValue)
                {
                    trackUser = await userService.GetUserByIdAsync(track.UserId.Value); // Assuming you have a UserService
                }

                if (track.GenreId.HasValue)
                {
                    trackGenre = await genreService.GetGenreByIdAsync(track.GenreId.Value); // Assuming you have a GenreService
                }
                TrackDetailsViewModel model = new TrackDetailsViewModel

                {
                    Id = id,
                    Title = track.Title,
                    Plays = track.Plays,
                    TrackImage = track.CoverImage,
                    Comments = comments.ToList(),
                    Username = track.User.Username,
                    Genre = trackGenre.Name

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
                    Plays = x.Plays,
                    IsYoutube = x.IsYoutube,
                    ImageLink=x.CoverImage
                }).ToList();

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
                    Plays = x.Plays,
                    ImageLink=x.CoverImage
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
