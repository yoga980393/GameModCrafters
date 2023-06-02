using GameModCrafters.Data;
using GameModCrafters.Models;
using GameModCrafters.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;    // Claims會用到
using System.Collections.Generic;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System;
using Microsoft.Win32;
using Microsoft.EntityFrameworkCore;
using GameModCrafters.Encryption;

namespace GameModCrafters.Controllers
{
    public class AccountController : Controller
    {
        private readonly IHashService _hashService;
     
        private readonly ApplicationDbContext _context;
        public AccountController(ApplicationDbContext context,IHashService hashService)
        {
            _hashService = hashService;
            _context = context;
        }
        [HttpGet]
        [AllowAnonymous]
        public IActionResult LoginPage()
        {
            // 登入頁面
            return View();
        }
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LoginPage(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var encryptedPassword = _hashService.SHA512Hash(model.Password);
                var user = _context.Users.FirstOrDefault(x => (x.Username == model.Text || x.Email == model.Text) && x.Password == encryptedPassword);

                if (user != null)
                {
                    //通過以上帳密比對成立後, 以下開始建立授權
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.NameIdentifier, model.Text.ToString()),
                        //new Claim(ClaimTypes.Role, "Administrator") // 如果要有「群組、角色、權限」，可以加入這一段  

                    };
                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity)
                    );
                    HttpContext.Session.SetString("Email", user.Email);
                    return RedirectToAction("Index", "Home"); // 成功
                }

                ModelState.AddModelError("Text", "帳號或密碼錯誤");
            }

            return View(model); // 失敗
        }
        
        [HttpGet]
        [AllowAnonymous]
        public IActionResult RegisterPage()
        {
            return View();
        }
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RegisterPage([Bind("Email, Username, Password1,Password2")] RegisterViewModel register)
        {
            
            if (ModelState.IsValid)
            {
                bool isEmailExists = _context.Users.Any(u => u.Email == register.Email);
                bool isUsernameExists = _context.Users.Any(m => m.Username == register.Username);
                if (isUsernameExists)
                {
                    ModelState.AddModelError("Username", "該 Username 已被使用。");
                }

                if (isEmailExists)
                {
                    ModelState.AddModelError("Email", "該 Email 已被使用。");
                }

                if (isUsernameExists || isEmailExists)
                {
                    return View(register);
                }

                bool isPasswordValid = IsPasswordValid(register.Password1);

                if (!isPasswordValid)
                {
                    ModelState.AddModelError("Password1", "密碼至少包含 8-20 個字符，並且包含至少一個大寫字母和一個數字");
                    return View(register);
                }

                var user = new User
                {
                    Email = register.Email,
                    Username = register.Username,
                    Password = _hashService.SHA512Hash(register.Password1) ,
                    RegistrationDate = DateTime.UtcNow // 取得當前的 UTC 時間
                };

                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                return RedirectToAction("LoginPage"); // 成功!
            }

            return View(register); // 失敗
        }
        private bool IsPasswordValid(string password)
        {
            
            // 以下是一個示例：要求密碼至少包含 8 個字符，並且包含至少一個大寫字母和一個數字
            return password.Length <= 20 && password.Length >= 8 && password.Any(char.IsUpper) && password.Any(char.IsDigit);
        }
        async Task<IActionResult> RecordLast()
        {
            var userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Email == userId || x.Username == userId);
            if (user != null)
            {
                user.LastLogin = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }
            return View();
        }
        public async Task<IActionResult> Signout()
        {
            await RecordLast();
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return LocalRedirect("/");
        }
        [HttpGet]
        [Authorize]
        public IActionResult PersonPage()
        {
            // 個人專區，需要驗證才能訪問
            return View();
        }

        [HttpGet]
        public IActionResult Forbidden()
        {
            // 拒絕訪問頁面
            return View();
        }
    }
}
