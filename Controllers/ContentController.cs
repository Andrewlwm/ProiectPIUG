using ClonaTwitter.Data;
using ClonaTwitter.Models;
using ClonaTwitter.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ServiceStack.Data;
using ServiceStack.OrmLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;


namespace ClonaTwitter.Views
{
    public class ContentController : Controller
    {
        private readonly ICensor _censor;
        private readonly IDbConnectionFactory _db;
        private readonly ILogger<ContentController> _logger;

        public ContentController(IDbConnectionFactory db, ILogger<ContentController> logger, ICensor censor)
        {
            _db = db;
            _logger = logger;
            _censor = censor;
        }

        public void Index()
        {
            HttpContext.Response.Redirect("/Home/Index");
        }

        [Authorize]
        [HttpGet]
        public IActionResult AddPost()
        {
            return View();
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> AddPost(PostModel post)
        {
            if (ModelState.IsValid)
            {
                using (var db = await _db.OpenAsync())
                {

                    if (post.MediaUrl.Contains("youtube.com"))
                        post.MediaUrl = post.MediaUrl.Replace("watch?v=", "embed/");

                    if (post.MediaUrl.Contains("youtu.be"))
                        post.MediaUrl = post.MediaUrl.Replace(".be/", "be.com/embed/");


                    var newPost = new Post
                    {
                        PostTitle = _censor.CensorText(post.PostTitle.Trim()),
                        PostBody = _censor.CensorText(post.PostBody.Trim()),
                        MediaUrl = post.MediaUrl,
                        AuthorId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)),
                        AuthorName = User.Identity.Name.ToString(),
                        CreatedAt = DateTime.Now
                    };

                    await db.SaveAsync(newPost, references: true);
                    return RedirectToAction("Index");
                }
            }
            else return View();
        }

        [HttpGet]
        public async Task<IActionResult> ViewPost(int Id)
        {
            Post post;
            using (var db = await _db.OpenAsync())
            {
                post = await db.LoadSingleByIdAsync<Post>(Id);
                post.Views += 1;
                await db.UpdateAsync(post);
                post.Replies.Sort((x, y) => DateTime.Compare(y.CreatedAt, x.CreatedAt));
            }

            return View("~/Views/Content/ViewPost.cshtml", post);
        }

        [Authorize]
        [HttpPost]
        public async Task AddComment(ReplyModel reply)
        {
            if (ModelState.IsValid)
            {
                using (var db = await _db.OpenAsync())
                {
                    var post = db.SingleById<Post>(reply.PostId);

                    post.Replies = new List<Reply>()
                    {
                        new Reply
                        {
                            ReplyBody = _censor.CensorText(reply.ReplyBody),
                            CreatedAt = DateTime.Now,
                            AuthorId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)),
                            AuthorName = User.Identity.Name.ToString()
                        }
                    };
                    await db.SaveAsync(post, references: true);
                }
            }
            HttpContext.Response.Redirect($"ViewPost/{reply.PostId}");
        }

        public async Task<IActionResult> ViewProfile(string profileName, int pageNumber = 1)
        {
            List<Post> posts = new List<Post>();
            ViewBag.UserName = profileName;
            using (var db = await _db.OpenAsync())
            {
                db.CreateTableIfNotExists<User>();
                db.CreateTableIfNotExists<Post>();
                posts = await db.SelectAsync<Post>(x=>x.AuthorName==profileName);

            }
            posts.Sort((x, y) =>
            {
                if (x.Views == 0 && y.Views == 0)
                    return DateTime.Compare(y.CreatedAt, x.CreatedAt);
                else
                    return y.Views - x.Views;
            });
            return View("../Home/Index", PaginatedList<Post>.Create(posts.AsQueryable(), pageNumber, 10));
        }

        [HttpGet]
        public IActionResult Report()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Report(ReportModel model)
        {
            
            return RedirectToAction("Index", "Home");
        }
    }
}
