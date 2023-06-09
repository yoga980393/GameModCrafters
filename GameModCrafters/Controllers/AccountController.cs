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
using SendGrid;
using SendGrid.Helpers.Mail;
using System.ComponentModel.DataAnnotations;

namespace GameModCrafters.Controllers
{
    public class AccountController : Controller
    {
        private readonly IHashService _hashService;
     
        private readonly ApplicationDbContext _context;

        private readonly ISendGridClient _sendGridClient;

     
        public AccountController(ApplicationDbContext context,IHashService hashService, ISendGridClient sendGridClient)
        {
            _hashService = hashService;
            _context = context;
            _sendGridClient = sendGridClient;
           
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
                   if(user.EmailConfirmed==false)
                    {
                        ModelState.AddModelError("Text", "帳號或密碼錯誤");
                        return View(model); // 失敗
                    }
                    //通過以上帳密比對成立後, 以下開始建立授權
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Email,user.Email ),//email
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
        public async Task<IActionResult> RegisterPage(RegisterViewModel register)
        {
            if (ModelState.IsValid)
            {
                


                //bool isEmailExists = _context.Users.Any(u => u.Email == register.Email);
                bool isUsernameExists = _context.Users.Any(m => m.Username == register.Username);
                if (isUsernameExists)
                {
                    ModelState.AddModelError("Username", "該 Username 已被使用。");
                  
                }

                //if (isEmailExists)
                //{
                //    ModelState.AddModelError("Email", "該 Email 已被使用。");
                //}

                //if (isUsernameExists || isEmailExists)
                //{
                //    return View(register);
                //}
                bool isPasswordValid = IsPasswordValid(register.Password1);
                if (!isPasswordValid)
                {
                    ModelState.AddModelError("Password1", "密碼至少包含 8-20 個字符，並且包含至少一個大寫字母和一個數字");
                }

                if (isUsernameExists || !isPasswordValid)
                {
                    return View(register);
                }



                //bool isPasswordValid = IsPasswordValid(register.Password1);
                //if (!isPasswordValid)
                //{
                //    ModelState.AddModelError("Password1", "密碼至少包含 8-20 個字符，並且包含至少一個大寫字母和一個數字");
                //    return View(register);
                //}

                // 將使用者名稱和密碼存儲到暫時的變數中
                TempData["Username"] = register.Username;
                TempData["Password"] = register.Password1;
                return RedirectToAction("RegisterEmailPage"); // 重定向到填寫電子郵件的頁面
                //var user = new User
                //{
                //    Email = register.Email,
                //    Username = register.Username,
                //    Password = _hashService.SHA512Hash(register.Password1),
                //    RegistrationDate = DateTime.UtcNow, // 取得當前的 UTC 時間

                //};
                //_context.Users.Add(user);
                //await _context.SaveChangesAsync();
                // return RedirectToAction("LoginPage");//成功
            }

            return View(register); // 失敗
        }
        [HttpGet]
        [AllowAnonymous]
        public IActionResult RegisterEmailPage()
        {
            return View();
        }
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RegisterEmailPage(string email)
        {
            if (ModelState.IsValid)
            {
                bool isEmailExists = _context.Users.Any(u => u.Email == email);
                if (isEmailExists)
                {
                    ModelState.AddModelError("Email", "該 Email 已被使用。");
                    return View("RegisterEmailPage");
                }
                TempData["Email"] = email;
                // 從暫時的變數中獲取使用者名稱和密碼
                var username = TempData["Username"].ToString();
                var password = TempData["Password"].ToString();
                // 生成確認碼
                var confirmationCode = Guid.NewGuid().ToString();
                // 創建使用者
                var user = new User
                {
                    Email = email,
                    Username = username,
                    Password = _hashService.SHA512Hash(password),
                    RegistrationDate = DateTime.UtcNow, // 取得當前的 UTC 時間
                    EmailConfirmed = false, // 初始狀態設為未確認
                    ConfirmationCode = confirmationCode // 將確認碼儲存到使用者物件中
                };

                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                // 傳送確認郵件
              
               
                var confirmationLink = Url.Action("ConfirmEmail", "Account", new { email = user.Email, confirmationCode }, Request.Scheme);
                await SendConfirmationEmail(user.Email, confirmationLink);
               
                return RedirectToAction("WaitConfirmEmail"); // 等待email驗證
            }

            return View("RegisterEmailPage"); // 失敗
        }
        [AllowAnonymous]
        public async Task<IActionResult> WaitConfirmEmail()
        {
            return View();
        }
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail(string email, string confirmationCode)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(confirmationCode))
            {
                // 未提供有效的電子郵件地址或確認碼
                return BadRequest();
            }

            // 根據電子郵件地址找到對應的使用者
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Email == email && x.ConfirmationCode == confirmationCode);
            if (user == null)
            {
                // 使用者不存在
                return NotFound();
            }

            // 郵件地址確認成功
            user.EmailConfirmed = true;
            _context.Update(user);
            await _context.SaveChangesAsync();

            // 郵件地址確認成功，返回成功頁面
            return View("ConfirmEmail");
        }
        
        private async Task SendConfirmationEmail(string email, string confirmationLink)
        {
            var from = new EmailAddress("buildschool99@gmail.com", "第七小組遊戲mod");
            var to = new EmailAddress(email);
            var subject = "確認您的電子郵件地址";

            var htmlContent = $"<html><body><p>請點擊以下連結以確認您的電子郵件地址：</p><p><a href='{confirmationLink}'>{confirmationLink}</a></p></body></html>";

            var msg = MailHelper.CreateSingleEmail(from, to, subject, null, htmlContent);
            await _sendGridClient.SendEmailAsync(msg);
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
        public async Task<IActionResult> PersonPage(PersonViewModel personVM)
        {
            var usermail = User.FindFirstValue(ClaimTypes.Email);
            if (usermail == null)
            {
                return NotFound();
            }
            var user = await _context.Users.FirstOrDefaultAsync(x=>x.Email==usermail);
            var userAvatar = user.Avatar;
            var userCover = user.BackgroundImage;
            personVM.Avatar = userAvatar;
            personVM.BackgroundImage = userCover;
            //ViewData["UserAvatar"] = userAvatar;
            //ViewData["UserCover"] = userCover;
            return View(personVM);
        }
        
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> PersonPage(string Name)
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CropperAvatarImage(IFormFile croppedPersonImage)
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
            var useremail = User.FindFirstValue(ClaimTypes.Email) ; 
            var user = await _context.Users.FirstOrDefaultAsync(x=>x.Email==useremail);

            var oldFilePath = user.Avatar;
            if (!string.IsNullOrEmpty(oldFilePath))
            {
                var oldFileName = Path.GetFileName(oldFilePath);
                oldFilePath = Path.Combine("wwwroot/AvatarImages", oldFileName);
                if (System.IO.File.Exists(oldFilePath))
                {
                    System.IO.File.Delete(oldFilePath);
                }
            }
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await croppedPersonImage.CopyToAsync(fileStream);
            }

            var fileUrl = Url.Content("~/AvatarImages/" + newFileName);
           
            if (user != null)
            {
                user.Avatar = fileUrl;
                await _context.SaveChangesAsync();
            }
            //var fileUrl = croppedPersonImage.ToString();
            return Ok(new { fileUrl });

        }
        [HttpPost]
        public async Task<IActionResult> CropperBackgroundImage(IFormFile croppedCoverImage)
        {
            if (croppedCoverImage == null || croppedCoverImage.Length == 0)
            {
                return BadRequest("Invalid file.");
            }
            var fileName = Path.GetFileNameWithoutExtension(croppedCoverImage.FileName);
            var extension = Path.GetExtension(croppedCoverImage.FileName);
            var date = DateTime.Now.ToString("yyyyMMddHHmmss");
            var random = Guid.NewGuid().ToString().Substring(0, 4); // 生成一個4位數的隨機字串
            var newFileName = $"{fileName}_{date}_{random}{extension}";

            var filePath = Path.Combine("wwwroot/BackgroundImages", newFileName);
            var useremail = User.FindFirstValue(ClaimTypes.Email);
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == useremail);
            var oldFilePath = user.BackgroundImage;
            if (!string.IsNullOrEmpty(oldFilePath))
            {
                var oldFileName = Path.GetFileName(oldFilePath);
                oldFilePath = Path.Combine("wwwroot/BackgroundImages", oldFileName);
                if (System.IO.File.Exists(oldFilePath))
                {
                    System.IO.File.Delete(oldFilePath);
                }
            }
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await croppedCoverImage.CopyToAsync(fileStream);
            }

            var fileUrl = Url.Content("~/BackgroundImages/" + newFileName);
            
            if (user != null)
            {
                user.BackgroundImage = fileUrl;
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
