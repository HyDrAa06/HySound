using HySound.Core.Service.IService;
using HySound.Models;
using HySound.Models.Models;
using HySound.Models.Track;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Numerics;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace HySound.Controllers
{
    public class TrackController : Controller
    {
        ITrackService trackService;
        IGenreService genreService;
        IUserService userService;
        public TrackController(ITrackService trackService, IUserService userService, IGenreService genreService)
        {
            this.trackService = trackService;
            this.userService = userService;
            this.genreService = genreService;
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

            Track track = trackService.GetAll().Where(x => x.Id == id).FirstOrDefault();
            track.Title = model.Title;
            track.AudioUrl = model.AudioUrl;
            track.Plays=model.Plays;
            track.Duration = model.Duration;
            track.GenreId = model.GenreId;
            track.UserId = model.UserId;
            track.CoverImage = model.ImageLink;
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
                Duration=x.Duration,
                AudioUrl=x.AudioUrl,
                ImageLink = x.CoverImage,
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
            model.UserList = new SelectList(await userService.GetAllUsersAsync(), "Id", "Username");

            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> AddTrack(AddTrackViewModel model)
        {
            if (ModelState.IsValid)
            {
                Track track = new Track()
                {
                    Title = model.Title,
                    AudioUrl = model.AudioUrl,
                    UserId = model.UserId,
                    Duration = model.Duration,
                    Plays = model.Plays,
                    GenreId = model.GenreId,
                    CoverImage= model.ImageLink
                };
                await trackService.AddTrackAsync(track);
                return RedirectToAction("AllTracks");
            }
            else
            {
                model.GenresList = new SelectList(await genreService.GetAllGenresAsync(), "Id", "Name");
                model.UserList = new SelectList(await userService.GetAllUsersAsync(), "Id", "Username");
                return View(model);
            }

        }
        public async Task<IActionResult> AllTracks(TrackFilterViewModel? filter)
        {
            var query = trackService.GetAll().AsQueryable();

            if(filter.GenreId == null && string.IsNullOrEmpty(filter.Title))
            {
                var model = trackService.AllWithInclude().Include(x => x.Genre).ThenInclude(x => x.Tracks).Include(x => x.User).ThenInclude(x => x.Tracks).Select(x => new TrackViewModel()
                {
                    TrackId = x.Id,
                    Title = x.Title,
                    AudioUrl = x.AudioUrl,
                    GenreName = x.Genre.Name,
                    UserName = x.User.Username,
                    Duration = x.Duration,
                    Plays = x.Plays,
                    ImageLink=x.CoverImage
                }).ToList();

                var filterModel = new TrackFilterViewModel
                {
                    Tracks = model,
                    Genres = new SelectList(genreService.GetAll(), "Id", "Name")

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
                    Duration = x.Duration,
                    Plays = x.Plays,
                    ImageLink=x.CoverImage
                }).ToList(),
                    Genres = new SelectList(genreService.GetAll(), "Id", "Name"),
                    Title = filter.Title,
                    GenreId = filter.GenreId
                };

                return View(filterModel);
            }
       
        }
    }
}
