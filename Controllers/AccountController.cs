using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using ClonaTwitter.Data;
using ClonaTwitter.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServiceStack;
using ServiceStack.Data;
using ServiceStack.OrmLite;
using ServiceStack.OrmLite.Dapper;

namespace ClonaTwitter.Controllers
{
    public class AccountController : Controller
    {
        private readonly IDbConnectionFactory _db;
        public AccountController(IDbConnectionFactory db)
        {
            _db = db;
        }

        [AllowAnonymous]
        public void Index()
        {
            HttpContext.Response.Redirect("/Home/Index");
        }

        [AllowAnonymous]
        public IActionResult SignUp()
        {
            if (User.Identity.IsAuthenticated) return RedirectToAction("Index");
            return View();
        }

        [AllowAnonymous]
        public IActionResult AccessDenied()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> SignUpAsync(UserLogin user)
        {
            if (ModelState.IsValid)
            {
                using (var db = await _db.OpenAsync())
                {

                    if (!(await db.SelectAsync<User>(x => x.Username == user.Username.Trim())).IsEmpty())
                    {
                        ModelState.AddModelError("", "The username is taken!");
                        return View();
                    }
                    else
                    {
                        await db.InsertAsync<User>(new User
                        {
                            Username = user.Username.Trim(),
                            PasswordHash = BCrypt.Net.BCrypt.HashPassword(user.Password),
                        });
                        return RedirectToAction("SignIn");
                    }
                }
            }
            return View();
        }

        [AllowAnonymous]
        public IActionResult SignIn()
        {
            if (User.Identity.IsAuthenticated) return RedirectToAction("Index");
            return View();
        }
        [AllowAnonymous]
        public IActionResult Logout()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Logout(string returnUrl = null)
        {
            await HttpContext.SignOutAsync();
            return RedirectToAction("LogOut");
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> SignInAsync(UserLogin user, string returnUrl = "")
        {
            if (ModelState.IsValid)
            {
                using (var db = await _db.OpenAsync())
                {

                    var q = await db.SingleAsync<User>(x => x.Username == user.Username.Trim());
                    if (q == null || !BCrypt.Net.BCrypt.Verify(user.Password, q.PasswordHash))
                    {
                        ModelState.AddModelError("", "User not found!");
                        return View();
                    }
                    else
                    {
                        var Claims = new Claim[2];
                        Claims[0] = new Claim(ClaimTypes.Name, user.Username);
                        Claims[1] = new Claim(ClaimTypes.NameIdentifier, q.Id.ToString());

                        var Identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);
                        Identity.AddClaims(Claims.AsList());

                        var principal = new ClaimsPrincipal(Identity);
                        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                        if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                            return Redirect(returnUrl);
                        else
                            return RedirectToAction("Index", "Home");
                    }
                }
            }
            return View();
        }
    }
}
