using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ClonaTwitter.Models;
using ServiceStack.Data;
using System.Collections.Generic;
using ServiceStack.OrmLite;
using ClonaTwitter.Data;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ClonaTwitter.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IDbConnectionFactory _db;

        public HomeController(ILogger<HomeController> logger, IDbConnectionFactory db)
        {
            _logger = logger;
            _db = db;
            using (var dbo = _db.Open())
            {
                dbo.CreateTableIfNotExists<User>();
                dbo.CreateTableIfNotExists<Post>();
                dbo.CreateTableIfNotExists<Reply>();
            }
        }

        public async Task<IActionResult> Index(int pageNumber = 1)
        {
            List<Post> posts = new List<Post>();

            using (var db = await _db.OpenAsync())
            {
                db.CreateTableIfNotExists<User>();
                db.CreateTableIfNotExists<Post>();
                posts = await db.SelectAsync<Post>();

            }
            posts.Sort((x, y) =>
            {
                if (x.Views == 0 && y.Views == 0)
                    return DateTime.Compare(y.CreatedAt, x.CreatedAt);
                else
                    return y.Views - x.Views;
            });
            return View(PaginatedList<Post>.Create(posts.AsQueryable(), pageNumber, 10));
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
