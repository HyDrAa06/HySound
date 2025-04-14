using HySound.Core.Service.IService;
using HySound.Models.Models;
using HySound.ViewModels.Comment;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace HySound.Controllers
{
    public class CommentController : Controller
    {
        ICommentService commentService;
        ITrackService trackService;
        IUserService userService;
        private readonly UserManager<IdentityUser> userManager;


        public CommentController(UserManager<IdentityUser> _userManager,IUserService _userService,ITrackService _trackService,ICommentService _commentService)
        {
            this.commentService = _commentService;
            this.trackService = _trackService;
            this.userService = _userService;
            this.userManager = _userManager;
        }
        [HttpPost]
        public async Task<IActionResult> AddComment(AddCommentViewModel model)
        {
            if (ModelState.IsValid)
            {
                var tempUser = await userManager.FindByEmailAsync(User.Identity.Name);
                User user = await userService.GetUserAsync(x => x.Email == tempUser.Email);

                Comment comment = new Comment()
                {
                    Content = model.Content,
                    TrackId = model.TrackId,
                    UserId = user.Id
                };

                await commentService.AddCommentAsync(comment);
                return RedirectToAction("TrackDetails", "Track", new { id = model.TrackId });

            }
            else
            {
                return RedirectToAction("TrackDetails", "Track", new { id = model.TrackId });

            }

        }
        public IActionResult Index()
        {
            return View();
        }
    }
}
