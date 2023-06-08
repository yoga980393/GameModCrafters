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
using System.Xml.Linq;
using System.IO;

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
        public IActionResult LoginPage(string returnUrl = null)
        {
            return View(new LoginViewModel { ReturnUrl = returnUrl });
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LoginPage(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var encryptedPassword = _hashService.SHA512Hash(model.Password);//加密
                var user = _context.Users.FirstOrDefault(x => (x.Username == model.Text || x.Email == model.Text) && x.Password == encryptedPassword);

                if (user != null)
                {
                   
                    //通過以上帳密比對成立後, 以下開始建立授權
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.NameIdentifier,user.Email ),//email
                        new Claim(ClaimTypes.Name,user.Username ),//增加使用者   model.Text.ToString()
                        //new Claim(ClaimTypes.Role, "Administrator") // 如果要有「群組、角色、權限」，可以加入這一段  

                    };
                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity)
                    );
                    HttpContext.Session.SetString("Email", user.Email);

                    if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
                    {
                        return Redirect(model.ReturnUrl); // 重定向到 returnUrl
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home"); // 否則重定向到主頁
                    }
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
                    Password = _hashService.SHA512Hash(register.Password1),
                    RegistrationDate = DateTime.UtcNow, // 取得當前的 UTC 時間
                    
                };
                _context.Users.Add(user);
                await _context.SaveChangesAsync();
                return RedirectToAction("LoginPage");//成功
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
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Email == userId);
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
        public async Task<IActionResult> PersonPage()
        {
            return View();
        }
        
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> PersonPage(string Name)
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CropperImage(IFormFile croppedPersonImage)
        {
            if (croppedPersonImage == null || croppedPersonImage.Length == 0)
            {
                return BadRequest("Invalid file.");
            }
            var fileName = Path.GetFileNameWithoutExtension(croppedPersonImage.FileName);
            var extension = Path.GetExtension(croppedPersonImage.FileName);
            var date = DateTime.Now.ToString("yyyyMMddHHmmss");
            var random = Guid.NewGuid().ToString().Substring(0, 4); // 生成一個4位數的隨機字串
            var newFileName = $"{fileName}_{date}_{random}{extension}";

            var filePath = Path.Combine("wwwroot/AvatarImages", newFileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await croppedPersonImage.CopyToAsync(fileStream);
            }

            var fileUrl = Url.Content("~/AvatarImages/" + newFileName);
            var useremail = ClaimTypes.NameIdentifier; // 假設使用者名稱存儲在 User.Identity.Name 中
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == useremail);
            if (user != null)
            {
                user.Avatar = fileUrl;
                await _context.SaveChangesAsync();
            }
            //var fileUrl = croppedPersonImage.ToString();
            return Ok(new { fileUrl });
           
        }
        [HttpGet]
        public IActionResult Forbidden()
        {
            // 拒絕訪問頁面
            return View();
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> IsUsernameInUse(string username)
        {
            var userWithNewUsername = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
            if (username.Length<20 && username.Length<3)
            {
                return Json("名字至少3-20字元");
            }
            if (userWithNewUsername == null)
            {
                // 從授權資訊取得當前用戶的 Email
                var currentUserEmail = User.FindFirstValue(ClaimTypes.NameIdentifier);

                // 使用這個 Email 找到當前用戶
                var currentUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == currentUserEmail);

                currentUser.Username = username;

                // 儲存變更到資料庫
                await _context.SaveChangesAsync();
                var claimsIdentity = new ClaimsIdentity();
                claimsIdentity.AddClaim(new Claim(ClaimTypes.Name, username));
                User.AddIdentity(claimsIdentity);
                return Json("更改成功");
            }
            else
            {
                return Json($"名字已有人使用");
            }
        }

    }
}
