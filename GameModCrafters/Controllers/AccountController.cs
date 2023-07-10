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
using GameModCrafters.Services;
using System.Net;
using System.Text.RegularExpressions;


namespace GameModCrafters.Controllers
{
    public class AccountController : Controller
    {
        private readonly IHashService _hashService;

        private readonly ApplicationDbContext _context;

        private readonly ISendGridClient _sendGridClient;

        private readonly ModService _modService;

        private readonly SendEmail _sendEmail;
        public AccountController(ApplicationDbContext context, IHashService hashService, ISendGridClient sendGridClient, ModService modService, SendEmail sendEmail)
        {
            _hashService = hashService;
            _context = context;
            _sendGridClient = sendGridClient;
            _modService = modService;
            _sendEmail = sendEmail;
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult LoginPage(string returnUrl = null)
        {
            if (returnUrl == "/Account/ConfirmEmail")
            {
                returnUrl = "/Home/Index"; // 將 returnUrl 設定為首頁的路徑
            }
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
                    if (user.EmailConfirmed == false)
                    {
                        ModelState.AddModelError("Text", "email驗證錯誤");
                        return View(model); // 失敗
                    }
                    //通過以上帳密比對成立後, 以下開始建立授權
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Email,user.Email ),//email
                        new Claim(ClaimTypes.Name,user.Username ),//增加使用者   model.Text.ToString()
                        //new Claim ("AvatarUrl",user.Avatar ),
                        //new Claim(ClaimTypes.Role, "Administrator") // 如果要有「群組、角色、權限」，可以加入這一段  
                    };
                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity)
                    );
                    HttpContext.Session.SetString("Email", user.Email);

                    if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl) && model.ReturnUrl != "/Account/ConfirmEmail" && model.ReturnUrl != "/Account/WaitConfirmEmail")
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
        public  IActionResult RegisterPage()
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
               
                bool isUsernameExists = await _context.Users.AnyAsync(m => m.Username == register.Username);
                if (isUsernameExists)
                {
                    ModelState.AddModelError("Username", "該 Username 已被使用。");

                }
               
                bool isPasswordValid = IsPasswordValid(register.Password1);
                if (!isPasswordValid)
                {
                    ModelState.AddModelError("Password1", "密碼至少包含 8-20 個字符，並且包含至少一個大寫字母和一個數字");
                }

                if (isUsernameExists || !isPasswordValid)
                {
                    return View(register);
                }
               

                // 將使用者名稱和密碼存儲到暫時的變數中
                TempData["Username"] = register.Username;
                TempData["Password"] = register.Password1;
                return RedirectToAction("RegisterEmailPage"); // 重定向到填寫電子郵件的頁面
                
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
                    ConfirmationCode = confirmationCode, // 將確認碼儲存到使用者物件中111
                    Avatar = "/PreviewImage/Avatar_preview.jpg",
                    BackgroundImage = "https://fakeimg.pl/1400x600/?text=PreviewImage&font=lobster"
                };

                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                // 傳送確認郵件


                var confirmationLink = Url.Action("ConfirmEmail", "Account", new { email = user.Email, confirmationCode }, Request.Scheme);
                //bool sendemailTF= await SendConfirmationEmail(user.Email, confirmationLink);
                await SendConfirmationEmail(user.Email, confirmationLink);



                return RedirectToAction("RegisterValidationTime"); // 等待email驗證
          

            }

            return View("RegisterEmailPage"); // 失敗
        }
        [HttpPost]
        public async Task<IActionResult> RegisterValidationTime(bool deletedata)
        {
            if (deletedata)
            {
                var useremail = TempData["Email"]?.ToString();
                var user = _context.Users.FirstOrDefault(x => x.Email == useremail);
                if (user != null)
                {
                    _context.Users.Remove(user);
                    await _context.SaveChangesAsync();
                    // 刪除成功
                }
            }

            // 回傳回應給前端
            return Json(new { success = true });
        }
        [HttpGet]
        public IActionResult RegisterValidationTime()
        {
            return  View();
        }
        [HttpPost]
        public async Task<IActionResult> ResendConfirmationEmail()
        {
            // 獲取用戶的email，這裡假設您已經存儲在TempData中
            var email = TempData["Email"]?.ToString();
            var user = await _context.Users.FirstOrDefaultAsync(x=>x.Email == email);
            if (user != null)
            {
                var confirmationCode = Guid.NewGuid().ToString();

                // 更新使用者的確認碼
                user.ConfirmationCode = confirmationCode;
                _context.Update(user);
                await _context.SaveChangesAsync();

                var confirmationLink = Url.Action("ConfirmEmail", "Account", new { email, confirmationCode }, Request.Scheme);
                await SendConfirmationEmail(email, confirmationLink);

                // 回傳回應給前端
                return Json(new { success = true });
            }

            return Json(new { success = false });
        }
        //重置密碼
        [HttpPost]
        public async Task<IActionResult> RestPasswordValidationTime(bool deletedata)//時間到的話確認碼更換
        {
            if (deletedata)
            {
                var useremail = TempData["Email"]?.ToString();
                var user = await _context.Users.FirstOrDefaultAsync(x => x.Email == useremail);
                if (user != null)
                {
                    // 生成新的確認碼
                    var confirmationCode = Guid.NewGuid().ToString();

                    // 將確認碼儲存到使用者物件中
                    user.ConfirmationCode = confirmationCode;

                    // 將確認碼儲存到資料庫中
                    _context.SaveChanges();
                }
                return Json(new { success = false });
            }

            // 回傳回應給前端
            return Json(new { success = true });
        }
        [HttpGet]
        public IActionResult RestPasswordValidationTime()
        {

            return View();
        }
        [HttpPost]
        public async Task<IActionResult> ResendRestPasswordEmail()
        {
            
            // 從Session中獲取用戶的email和username
            var email = HttpContext.Session.GetString("Email");
            var username = HttpContext.Session.GetString("Username");
            
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Email == email);
            if (user != null)
            {
                var confirmationCode = Guid.NewGuid().ToString();

                // 更新使用者的確認碼
                user.ConfirmationCode = confirmationCode;
                _context.Update(user);
                await _context.SaveChangesAsync();

                var confirmationLink = Url.Action("RestPassword", "Account", new { email , confirmationCode }, Request.Scheme);
                await SendRestEmail(email, confirmationLink, username);

                // 回傳回應給前端
                return Json(new { success = true });
            }
            HttpContext.Session.Clear();
            return Json(new { success = false });
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
            var from = new EmailAddress("wggisddejp@gmail.com", "第七小組遊戲mod");
            var to = new EmailAddress(email);
            var subject = "確認您的電子郵件地址";
            var cssContent = _sendEmail.GetVerifyEmailCssContent();//打開直接起飛
            var htmlContent =  _sendEmail.GetVerifyEmailHtmlContent(cssContent,confirmationLink);//打開直接起飛

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
        public async Task<IActionResult> PersonPage(int page = 1)
        {
            var usermail = User.FindFirstValue(ClaimTypes.Email);
            if (usermail == null)
            {
                return NotFound();
            }
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Email == usermail);
            var userAvatar = user.Avatar;
            var userCover = user.BackgroundImage;
            PersonViewModel personVM = new PersonViewModel();

            personVM.Avatar = userAvatar;
            personVM.BackgroundImage = userCover;

            var commissions = await _context.Commissions
                .Where(c => c.DelegatorId == user.Email)
                .Include(c => c.Delegator)
                .Include(c => c.CommissionStatus)
                .Include(c => c.Game)
                .Select(c => new CommissionViewModel
                {
                    CommissionId = c.CommissionId,
                    GameID = c.Game.GameId,
                    GameName = c.Game.GameName,
                    DelegatorName = c.Delegator.Username,
                    CommissionTitle = c.CommissionTitle,
                    Budget = c.Budget,
                    CreateTime = c.CreateTime,
                    UpdateTime = c.UpdateTime,
                    Status = c.CommissionStatus.Status
                })
               .ToListAsync();
         
            personVM.Commissions = commissions;

            var publishedMods = await _modService.GetPublishedMods(User.FindFirstValue(ClaimTypes.Email), page, 8);
            personVM.PublishedMods = publishedMods.Mods;
            personVM.PublishedCurrentPage = page;
            personVM.PublishedTotalPages = publishedMods.TotalPages;

            var favoritedMods = await _modService.GetFavoritedMods(User.FindFirstValue(ClaimTypes.Email), page, 8);
            personVM.FavoritedMods = favoritedMods.Mods;
            personVM.FavoritedCurrentPage = page;
            personVM.FavoritedTotalPages = favoritedMods.TotalPages;

            var downloadedMods = await _modService.GetDownloadedMods(User.FindFirstValue(ClaimTypes.Email), page, 8);
            personVM.DownloadedMods = downloadedMods.Mods;
            personVM.DownloadedCurrentPage = page;
            personVM.DownloadedTotalPages = downloadedMods.TotalPages;

            //ViewData["UserAvatar"] = userAvatar;
            //ViewData["UserCover"] = userCover;
            return View(personVM);
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> OtherPage(string id,int page = 1)
        {
            var usermail = id;
            if (usermail == null)
            {
                return NotFound();
            }
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Email == usermail);
            var userAvatar = user.Avatar;
            var userCover = user.BackgroundImage;
            var userName = user.Username;
            PersonViewModel personVM = new PersonViewModel()
            {
                    Avatar = userAvatar,
                    BackgroundImage = userCover,
                    Username = userName,
                    Email = user.Email
            };
  
           

            var commissions = await _context.Commissions
                .Where(c => c.DelegatorId == user.Email)
                .Include(c => c.Delegator)
                .Include(c => c.CommissionStatus)
                .Include(c => c.Game)
                .Select(c => new CommissionViewModel
                {
                    CommissionId = c.CommissionId,
                    GameID = c.Game.GameId,
                    GameName = c.Game.GameName,
                    DelegatorName = c.Delegator.Username,
                    CommissionTitle = c.CommissionTitle,
                    Budget = c.Budget,
                    CreateTime = c.CreateTime,
                    UpdateTime = c.UpdateTime,
                    Status = c.CommissionStatus.Status
                })
               .ToListAsync();

            personVM.Commissions = commissions;

            var publishedMods = await _modService.GetPublishedMods(id, page, 8);
            personVM.PublishedMods = publishedMods.Mods;
            personVM.PublishedCurrentPage = page;
            personVM.PublishedTotalPages = publishedMods.TotalPages;

            var favoritedMods = await _modService.GetFavoritedMods(id, page, 8);
            personVM.FavoritedMods = favoritedMods.Mods;
            personVM.FavoritedCurrentPage = page;
            personVM.FavoritedTotalPages = favoritedMods.TotalPages;

            var downloadedMods = await _modService.GetDownloadedMods(id, page, 8);
            personVM.DownloadedMods = downloadedMods.Mods;
            personVM.DownloadedCurrentPage = page;
            personVM.DownloadedTotalPages = downloadedMods.TotalPages;

            return View(personVM);
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
            var useremail = User.FindFirstValue(ClaimTypes.Email);
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Email == useremail);

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
            return Ok(new { fileUrl ,message ="更新頭像成功"});

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
            if (username.Length < 20 && username.Length < 3)
            {
                return Json("名字至少3-20字元");
            }
            if (userWithNewUsername == null)
            {
                // 從授權資訊取得當前用戶的 Email
                var currentUserEmail = User.FindFirstValue(ClaimTypes.Email);

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

        [HttpPost]
        [Authorize]
        public IActionResult PayPalAccountSetting()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> RestPassword(string email, string confirmationCode)
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
            HttpContext.Session.SetString("Email", email);
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RestPassword(ForgetPasswordViewModel newuserpwVM)
        {
            if (ModelState.IsValid)
            {
                string email = HttpContext.Session.GetString("Email");

                // 根據電子郵件地址找到對應的使用者
                var user = await _context.Users.FirstOrDefaultAsync(x => x.Email == email);
                if (user == null)
                {
                    // 使用者不存在
                    return NotFound();
                }
                bool isPasswordValid = IsPasswordValid(newuserpwVM.Password1);
                if (!isPasswordValid)
                {
                    ModelState.AddModelError("Password1", "密碼至少包含 8-20 個字符，並且包含至少一個大寫字母和一個數字");
                    return View(newuserpwVM);
                }
                if (user.EmailConfirmed == false)
                {
                    return NotFound();
                }
                if (user.Password == _hashService.SHA512Hash(newuserpwVM.Password1))
                {
                    ModelState.AddModelError("Password1", "密碼不可以和之前重複");
                    return View(newuserpwVM);
                }
                user.Password = _hashService.SHA512Hash(newuserpwVM.Password1);
                _context.Update(user);
                await _context.SaveChangesAsync();

                //更改成功跳回登入頁面
                return View("LoginPage");
            }
            else
            {
                return View(newuserpwVM);
            }

        }
        [HttpGet]
        public IActionResult ForgotPasswordEmailPage()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ForgotPasswordEmailPage(string email)
        {
            if (ModelState.IsValid)
            {
                if (string.IsNullOrEmpty(email))
                {
                    // 未提供有效的電子郵件地址
                    return View("ForgotPasswordEmailPage");
                }

                var user = await _context.Users.FirstOrDefaultAsync(x => x.Email == email);
                if (user != null)
                {

                    // 確認碼
                    var username = user.Username;
                    var confirmationCode = user.ConfirmationCode;
                    var useremail = user.Email;
                    // 將需要傳遞的資料存儲在Session中
                    HttpContext.Session.SetString("Email", useremail);
                    HttpContext.Session.SetString("Username", username);
                    var confirmationLink = Url.Action("RestPassword", "Account", new { email = useremail, confirmationCode }, Request.Scheme);
                    await SendRestEmail(useremail, confirmationLink,username);
                    return RedirectToAction("RestPasswordValidationTime"); // 等待email驗證
                }
                else
                {
                    ModelState.AddModelError("Email", "不存在這個email");
                    return View("ForgotPasswordEmailPage");
                }
            }
            return View("ForgotPasswordEmailPage");
        }
        private async Task SendRestEmail(string email, string confirmationLink,string UserName)
        {
            var from = new EmailAddress("wggisddejp@gmail.com", "第七小組遊戲mod");
            var to = new EmailAddress(email);
            var subject = "確認您的電子郵件地址";
            var cssContent = _sendEmail.GetRestPasswordCssContent();//打開直接起飛
            var htmlContent =_sendEmail.GetRestPasswordHtmlContent(cssContent,UserName,confirmationLink);//打開直接起飛
           

            var msg = MailHelper.CreateSingleEmail(from, to, subject, null, htmlContent);
            await _sendGridClient.SendEmailAsync(msg);
        }

        [HttpGet]
        public async Task<IActionResult> PublishedModsPartial(int page = 1)
        {
            var usermail = User.FindFirstValue(ClaimTypes.Email);
            if (usermail == null)
            {
                return NotFound();
            }

            var mods = await _modService.GetPublishedMods(usermail, page, 8);
            var personVM = new PersonViewModel
            {
                PublishedCurrentPage = page,
                PublishedMods = mods.Mods,
                PublishedTotalPages = mods.TotalPages
            };

            return PartialView("_PublishedModsPartial", personVM);
        }

        [HttpGet]
        public async Task<IActionResult> FavoritedModsPartial(int page = 1)
        {
            var usermail = User.FindFirstValue(ClaimTypes.Email);
            if (usermail == null)
            {
                return NotFound();
            }

            var mods = await _modService.GetFavoritedMods(usermail, page, 8);
            var personVM = new PersonViewModel
            {
                FavoritedCurrentPage = page,
                FavoritedMods = mods.Mods,
                FavoritedTotalPages = mods.TotalPages
            };

            return PartialView("_FavoritedModsPartial", personVM);
        }

        [HttpGet]
        public async Task<IActionResult> DownloadedModsPartial(int page = 1)
        {
            var usermail = User.FindFirstValue(ClaimTypes.Email);
            if (usermail == null)
            {
                return NotFound();
            }

            var mods = await _modService.GetDownloadedMods(usermail, page, 8);
            var personVM = new PersonViewModel
            {
                DownloadedCurrentPage = page,
                DownloadedMods = mods.Mods,
                DownloadedTotalPages = mods.TotalPages
            };

            return PartialView("_DownloadedModsPartial", personVM);
        }



        [HttpGet]
        public async Task<IActionResult> PublishedModsPartialOther(string id,int page = 1)
        {
            var usermail = id;
            if (usermail == null)
            {
                return NotFound();
            }

            var mods = await _modService.GetPublishedMods(usermail, page, 8);
            var personVM = new PersonViewModel
            {
                PublishedCurrentPage = page,
                PublishedMods = mods.Mods,
                PublishedTotalPages = mods.TotalPages
            };

            return PartialView("_PublishedModsPartial", personVM);
        }

        [HttpGet]
        public async Task<IActionResult> FavoritedModsPartialOther(string id, int page = 1)
        {
            var usermail = id;
            if (usermail == null)
            {
                return NotFound();
            }

            var mods = await _modService.GetFavoritedMods(usermail, page, 8);
            var personVM = new PersonViewModel
            {
                FavoritedCurrentPage = page,
                FavoritedMods = mods.Mods,
                FavoritedTotalPages = mods.TotalPages
            };

            return PartialView("_FavoritedModsPartial", personVM);
        }

        [HttpGet]
        public async Task<IActionResult> DownloadedModsPartialOther(string id, int page = 1)
        {
            var usermail = id;
            if (usermail == null)
            {
                return NotFound();
            }

            var mods = await _modService.GetDownloadedMods(usermail, page, 8);
            var personVM = new PersonViewModel
            {
                DownloadedCurrentPage = page,
                DownloadedMods = mods.Mods,
                DownloadedTotalPages = mods.TotalPages
            };

            return PartialView("_DownloadedModsPartial", personVM);
        }
        [HttpGet]
        public async Task<IActionResult> GetUserName()
        {
            var user = await _context.Users.Where(u => u.Email == User.FindFirstValue(ClaimTypes.Email)).Select(u => u.Username).FirstOrDefaultAsync();
            return Ok(user);
        }
        [HttpGet]
        public async Task<IActionResult> GetUserAvatar()
        {
            var avatar = await _context.Users.Where(u => u.Email == User.FindFirstValue(ClaimTypes.Email)).Select(u => u.Avatar).FirstOrDefaultAsync();
            return Ok(avatar);
        }
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> SetPayPalAccount(string account)
        {
            var currentUserEmail = User.FindFirstValue(ClaimTypes.Email);
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == currentUserEmail);

            user.PayPalAccounts = account;
            await _context.SaveChangesAsync();

            return Json("更改成功");
        }

        [HttpGet]
        public async Task<IActionResult> GetUserModCoin()
        {
            var modCoin = await _context.Users.Where(u => u.Email == User.FindFirstValue(ClaimTypes.Email)).Select(u => u.ModCoin).FirstOrDefaultAsync();
            return Ok(modCoin);
        }
    }
}
