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

       
        public AccountController(ApplicationDbContext context, IHashService hashService, ISendGridClient sendGridClient, ModService modService)
        {
            _hashService = hashService;
            _context = context;
            _sendGridClient = sendGridClient;
            _modService = modService;
           
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



                return RedirectToAction("WaitConfirmEmail"); // 等待email驗證


            }

            return View("RegisterEmailPage"); // 失敗
        }
        [AllowAnonymous]
        public IActionResult WaitConfirmEmail()
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
            var from = new EmailAddress("wggisddejp@gmail.com", "第七小組遊戲mod");
            var to = new EmailAddress(email);
            var subject = "確認您的電子郵件地址";
            var cssContent = @"
            body, p, div {
              font-family: tahoma,geneva,sans-serif;
              font-size: 14px;
            }
            body {
              color: #000000;
            }
            body a {
              color: #000000;
              text-decoration: none;
            }
            p { margin: 0; padding: 0; }
            table.wrapper {
              width:100% !important;
              table-layout: fixed;
              -webkit-font-smoothing: antialiased;
              -webkit-text-size-adjust: 100%;
              -moz-text-size-adjust: 100%;
              -ms-text-size-adjust: 100%;
            }
            img.max-width {
              max-width: 100% !important;
            }
            .column.of-2 {
              width: 50%;
            }
            .column.of-3 {
              width: 33.333%;
            }
            .column.of-4 {
              width: 25%;
            }
            ul ul ul ul  {
              list-style-type: disc !important;
            }
            ol ol {
              list-style-type: lower-roman !important;
            }
            ol ol ol {
              list-style-type: lower-latin !important;
            }
            ol ol ol ol {
              list-style-type: decimal !important;
            }
            .cover-image {
              max-width: 100%;
              height: auto !important;
              display: block;
              color: #000000;
              text-decoration: none;
              font-family: Helvetica, Arial, sans-serif;
              font-size: 16px;
              border-radius: 10px;
            }
            @media screen and (max-width:480px) {
              .preheader .rightColumnContent,
              .footer .rightColumnContent {
                text-align: left !important;
              }
              .preheader .rightColumnContent div,
              .preheader .rightColumnContent span,
              .footer .rightColumnContent div,
              .footer .rightColumnContent span {
                text-align: left !important;
              }
              .preheader .rightColumnContent,
              .preheader .leftColumnContent {
                font-size: 80% !important;
                padding: 5px 0;
              }
              table.wrapper-mobile {
                width: 100% !important;
                table-layout: fixed;
              }
              img.max-width {
                height: auto !important;
                max-width: 100% !important;
              }
              a.bulletproof-button {
                display: block !important;
                width: auto !important;
                font-size: 80%;
                padding-left: 0 !important;
                padding-right: 0 !important;
              }
              .columns {
                width: 100% !important;
              }
              .column {
                display: block !important;
                width: 100% !important;
                padding-left: 0 !important;
                padding-right: 0 !important;
                margin-left: 0 !important;
                margin-right: 0 !important;
              }
              .social-icon-column {
                display: inline-block !important;
              }
             ";//打開直接起飛
            var htmlContent = $@"
        <html>
        <head>
             <style>{cssContent}</style>
        </head>
        <body>
      <center class=""wrapper"" data-link-color=""#000000"" data-body-style=""font-size:14px; font-family:tahoma,geneva,sans-serif; color:#000000; background-color:#FFFFFF;"">
        <div class=""webkit"">
          <table cellpadding=""0"" cellspacing=""0"" border=""0"" width=""100%"" class=""wrapper"" bgcolor=""#FFFFFF"">
            <tr>
              <td valign=""top"" bgcolor=""#FFFFFF"" width=""100%"">
                <table width=""100%"" role=""content-container"" class=""outer"" align=""center"" cellpadding=""0"" cellspacing=""0"" border=""0"">
                  <tr>
                    <td width=""100%"">
                      <table width=""100%"" cellpadding=""0"" cellspacing=""0"" border=""0"">
                        <tr>
                          <td>
                            <!--[if mso]>
    <center>
    <table><tr><td width=""700"">
  <![endif]-->
                                    <table width=""100%"" cellpadding=""0"" cellspacing=""0"" border=""0"" style=""width:100%; max-width:700px;"" align=""center"">
                                      <tr>
                                        <td role=""modules-container"" style=""padding:0px 0px 0px 0px; color:#000000; text-align:left;"" bgcolor=""#FFFFFF"" width=""100%"" align=""left""><table class=""module preheader preheader-hide"" role=""module"" data-type=""preheader"" border=""0"" cellpadding=""0"" cellspacing=""0"" width=""100%"" style=""display: none !important; mso-hide: all; visibility: hidden; opacity: 0; color: transparent; height: 0; width: 0;"">
    <tr>
      <td role=""module-content"">
        <p>歡迎加入我們其中一員!!</p>
      </td>
    </tr>
  </table><table class=""module"" role=""module"" data-type=""text"" border=""0"" cellpadding=""0"" cellspacing=""0"" width=""100%"" style=""table-layout: fixed;"" data-muid=""a9749b00-f200-4352-a302-1d63bdf0a1fc"" data-mc-module-version=""2019-10-22"">
   
  </table><table class=""wrapper"" role=""module"" data-type=""image"" border=""0"" cellpadding=""0"" cellspacing=""0"" width=""100%"" style=""table-layout: fixed;"" data-muid=""9f868e33-21ac-40c8-86bc-8c97d7cd69f0"">
    <tbody>
      <tr>
        <td style=""font-size:6px; line-height:10px; padding:0px 0px 0px 0px;"" valign=""top"" align=""center"">
           <img class=""cover-image"" width=""600"" alt="""" data-proportionally-constrained=""true"" data-responsive=""true"" src=""https://fakeimg.pl/1400x600/ff8000,128/000?text=GameModCrafters&font=noto"">
        </td>
      </tr>
    </tbody>
  </table><table border=""0"" cellpadding=""0"" cellspacing=""0"" align=""center"" width=""100%"" role=""module"" data-type=""columns"" style=""padding:30px 10px 0px 10px;"" bgcolor=""#FFFFFF"" data-distribution=""1"">
    <tbody>
      <tr role=""module-content"">
        <td height=""100%"" valign=""top""><table width=""520"" style=""width:520px; border-spacing:0; border-collapse:collapse; margin:0px 80px 0px 80px;"" cellpadding=""0"" cellspacing=""0"" align=""left"" border=""0"" bgcolor="""" class=""column column-0"">
      <tbody>
        <tr>
          <td style=""padding:0px;margin:0px;border-spacing:0;""><table class=""wrapper"" role=""module"" data-type=""image"" border=""0"" cellpadding=""0"" cellspacing=""0"" width=""100%"" style=""table-layout: fixed;"" data-muid=""b8c5fbf7-011d-4743-b847-3e4473737d9e"">
    
  </table><table class=""module"" role=""module"" data-type=""text"" border=""0"" cellpadding=""0"" cellspacing=""0"" width=""100%"" style=""table-layout: fixed;"" data-muid=""9fb55e76-a41f-446c-9893-536720221578"" data-mc-module-version=""2019-10-22"">
    <tbody>
      <tr>
        <td style=""padding:20px 0px 20px 0px; line-height:24px; text-align:inherit;"" height=""100%"" valign=""top"" bgcolor="""" role=""module-content""><div><div style=""font-family: inherit; text-align: center""><span style=""font-size: 24px""><strong>確認您的郵件地址</strong></span></div><div></div></div></td>
      </tr>
    </tbody>
  </table><table class=""module"" role=""module"" data-type=""text"" border=""0"" cellpadding=""0"" cellspacing=""0"" width=""100%"" style=""table-layout: fixed;"" data-muid=""abfcf137-fac5-4666-93df-b6940cd872ca"" data-mc-module-version=""2019-10-22"">
    <tbody>
      <tr>
        <td style=""padding:0px 0px 25px 0px; line-height:22px; text-align:inherit;"" height=""100%"" valign=""top"" bgcolor="""" role=""module-content""><div><div style=""font-family: inherit; text-align: center"">請點擊下方按鈕以確認您的郵件地址，享受我們提供的高品質產品和愉悅的使用體驗。</div><div></div></div></td>
      </tr>
    </tbody>
  </table><table border=""0"" cellpadding=""0"" cellspacing=""0"" class=""module"" data-role=""module-button"" data-type=""button"" role=""module"" style=""table-layout:fixed;"" width=""100%"" data-muid=""d61458c8-876e-44ce-9554-b8854b1f44ba"">
      <tbody>
        <tr>
          <td align=""center"" bgcolor="""" class=""outer-td"" style=""padding:0px 0px 0px 0px;"">
            <table border=""0"" cellpadding=""0"" cellspacing=""0"" class=""wrapper-mobile"" style=""text-align:center;"">
              <tbody>
                <tr>
                <td align=""center"" bgcolor=""#FDBE84"" class=""inner-td"" style=""border-radius:6px; font-size:16px; text-align:center; background-color:inherit;"">
                  <a href=""{confirmationLink}"" style=""background-color:#FDBE84; border:0px solid #333333; border-color:#333333; border-radius:100px; border-width:0px; color:#000000; display:inline-block; font-size:14px; font-weight:bold; letter-spacing:1px; line-height:normal; padding:25px 85px 25px 85px; text-align:center; text-decoration:none; border-style:solid;"" target=""_blank"">確認郵件地址</a>
                </td>
                </tr>
              </tbody>
            </table>
          </td>
        </tr>
      </tbody>
    </table></td>
        </tr>
      </tbody>
    </table></td>
      </tr>
    </tbody>
  </table><table class=""module"" role=""module"" data-type=""spacer"" border=""0"" cellpadding=""0"" cellspacing=""0"" width=""100%"" style=""table-layout: fixed;"" data-muid=""7d922942-9306-4c3a-9888-e47841af10d1"">
   
    </table></td>
                                      </tr>
                                    </table>
                                    <!--[if mso]>
                                  </td>
                                </tr>
                              </table>
                            </center>
                            <![endif]-->
                          </td>
                        </tr>
                      </table>
                    </td>
                  </tr>
                </table>
              </td>
            </tr>
          </table>
        </div>
      </center>
    </body>
        </html>";//打開直接起飛

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
        public async Task<IActionResult> PersonPage(PersonViewModel personVM, int page = 1)
        {
            var usermail = User.FindFirstValue(ClaimTypes.Email);
            if (usermail == null)
            {
                return NotFound();
            }
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Email == usermail);
            var userAvatar = user.Avatar;
            var userCover = user.BackgroundImage;
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
            if (commissions.Count == 0)
            {
                return NotFound();
            }
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
                    var confirmationLink = Url.Action("RestPassword", "Account", new { email = useremail, confirmationCode }, Request.Scheme);
                    await SendRestEmail(useremail, confirmationLink,username);
                    return RedirectToAction("WaitConfirmEmail"); // 等待email驗證
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
            var cssContent = @"
            body, p, div {
              font-family: tahoma,geneva,sans-serif;
              font-size: 14px;
            }
            body {
              color: #000000;
            }
            body a {
              color: #000000;
              text-decoration: none;
            }
            p { margin: 0; padding: 0; }
            table.wrapper {
              width:100% !important;
              table-layout: fixed;
              -webkit-font-smoothing: antialiased;
              -webkit-text-size-adjust: 100%;
              -moz-text-size-adjust: 100%;
              -ms-text-size-adjust: 100%;
            }
            img.max-width {
              max-width: 100% !important;
            }
            .column.of-2 {
              width: 50%;
            }
            .column.of-3 {
              width: 33.333%;
            }
            .column.of-4 {
              width: 25%;
            }
            ul ul ul ul  {
              list-style-type: disc !important;
            }
            ol ol {
              list-style-type: lower-roman !important;
            }
            ol ol ol {
              list-style-type: lower-latin !important;
            }
            ol ol ol ol {
              list-style-type: decimal !important;
            }
            .cover-image {
              max-width: 100%;
              height: auto !important;
              display: block;
              color: #000000;
              text-decoration: none;
              font-family: Helvetica, Arial, sans-serif;
              font-size: 16px;
              border-radius: 10px;
            }
            @media screen and (max-width:480px) {
              .preheader .rightColumnContent,
              .footer .rightColumnContent {
                text-align: left !important;
              }
              .preheader .rightColumnContent div,
              .preheader .rightColumnContent span,
              .footer .rightColumnContent div,
              .footer .rightColumnContent span {
                text-align: left !important;
              }
              .preheader .rightColumnContent,
              .preheader .leftColumnContent {
                font-size: 80% !important;
                padding: 5px 0;
              }
              table.wrapper-mobile {
                width: 100% !important;
                table-layout: fixed;
              }
              img.max-width {
                height: auto !important;
                max-width: 100% !important;
              }
              a.bulletproof-button {
                display: block !important;
                width: auto !important;
                font-size: 80%;
                padding-left: 0 !important;
                padding-right: 0 !important;
              }
              .columns {
                width: 100% !important;
              }
              .column {
                display: block !important;
                width: 100% !important;
                padding-left: 0 !important;
                padding-right: 0 !important;
                margin-left: 0 !important;
                margin-right: 0 !important;
              }
              .social-icon-column {
                display: inline-block !important;
              }
             ";//打開直接起飛
            var htmlContent = $@"
        <html>
        <head>
             <style>{cssContent}</style>
        </head>
        <body>
      <center class=""wrapper"" data-link-color=""#000000"" data-body-style=""font-size:14px; font-family:tahoma,geneva,sans-serif; color:#000000; background-color:#FFFFFF;"">
        <div class=""webkit"">
          <table cellpadding=""0"" cellspacing=""0"" border=""0"" width=""100%"" class=""wrapper"" bgcolor=""#FFFFFF"">
            <tr>
              <td valign=""top"" bgcolor=""#FFFFFF"" width=""100%"">
                <table width=""100%"" role=""content-container"" class=""outer"" align=""center"" cellpadding=""0"" cellspacing=""0"" border=""0"">
                  <tr>
                    <td width=""100%"">
                      <table width=""100%"" cellpadding=""0"" cellspacing=""0"" border=""0"">
                        <tr>
                          <td>
                            <!--[if mso]>
    <center>
    <table><tr><td width=""700"">
  <![endif]-->
                                    <table width=""100%"" cellpadding=""0"" cellspacing=""0"" border=""0"" style=""width:100%; max-width:700px;"" align=""center"">
                                      <tr>
                                        <td role=""modules-container"" style=""padding:0px 0px 0px 0px; color:#000000; text-align:left;"" bgcolor=""#FFFFFF"" width=""100%"" align=""left""><table class=""module preheader preheader-hide"" role=""module"" data-type=""preheader"" border=""0"" cellpadding=""0"" cellspacing=""0"" width=""100%"" style=""display: none !important; mso-hide: all; visibility: hidden; opacity: 0; color: transparent; height: 0; width: 0;"">
    <tr>
      <td role=""module-content"">
        <p>Hello {UserName} !</p>
      </td>
    </tr>
  </table><table class=""module"" role=""module"" data-type=""text"" border=""0"" cellpadding=""0"" cellspacing=""0"" width=""100%"" style=""table-layout: fixed;"" data-muid=""a9749b00-f200-4352-a302-1d63bdf0a1fc"" data-mc-module-version=""2019-10-22"">
   
  </table><table class=""wrapper"" role=""module"" data-type=""image"" border=""0"" cellpadding=""0"" cellspacing=""0"" width=""100%"" style=""table-layout: fixed;"" data-muid=""9f868e33-21ac-40c8-86bc-8c97d7cd69f0"">
    <tbody>
      <tr>
        <td style=""font-size:6px; line-height:10px; padding:0px 0px 0px 0px;"" valign=""top"" align=""center"">
           <img class=""cover-image"" width=""600"" alt="""" data-proportionally-constrained=""true"" data-responsive=""true"" src=""https://fakeimg.pl/1400x600/ff8000,128/000?text=GameModCrafters&font=noto"">
        </td>
      </tr>
    </tbody>
  </table><table border=""0"" cellpadding=""0"" cellspacing=""0"" align=""center"" width=""100%"" role=""module"" data-type=""columns"" style=""padding:30px 10px 0px 10px;"" bgcolor=""#FFFFFF"" data-distribution=""1"">
    <tbody>
      <tr role=""module-content"">
        <td height=""100%"" valign=""top""><table width=""520"" style=""width:520px; border-spacing:0; border-collapse:collapse; margin:0px 80px 0px 80px;"" cellpadding=""0"" cellspacing=""0"" align=""left"" border=""0"" bgcolor="""" class=""column column-0"">
      <tbody>
        <tr>
          <td style=""padding:0px;margin:0px;border-spacing:0;""><table class=""wrapper"" role=""module"" data-type=""image"" border=""0"" cellpadding=""0"" cellspacing=""0"" width=""100%"" style=""table-layout: fixed;"" data-muid=""b8c5fbf7-011d-4743-b847-3e4473737d9e"">
    
  </table><table class=""module"" role=""module"" data-type=""text"" border=""0"" cellpadding=""0"" cellspacing=""0"" width=""100%"" style=""table-layout: fixed;"" data-muid=""9fb55e76-a41f-446c-9893-536720221578"" data-mc-module-version=""2019-10-22"">
    <tbody>
      <tr>
        <td style=""padding:20px 0px 20px 0px; line-height:24px; text-align:inherit;"" height=""100%"" valign=""top"" bgcolor="""" role=""module-content""><div><div style=""font-family: inherit; text-align: center""><span style=""font-size: 24px""><strong>{UserName}</strong></span></div><div></div></div></td>
      </tr>
    </tbody>
  </table><table class=""module"" role=""module"" data-type=""text"" border=""0"" cellpadding=""0"" cellspacing=""0"" width=""100%"" style=""table-layout: fixed;"" data-muid=""abfcf137-fac5-4666-93df-b6940cd872ca"" data-mc-module-version=""2019-10-22"">
    <tbody>
      <tr>
        <td style=""padding:0px 0px 25px 0px; line-height:22px; text-align:inherit;"" height=""100%"" valign=""top"" bgcolor="""" role=""module-content""><div><div style=""font-family: inherit; text-align: center"">請點擊下方按鈕以重置您的密碼。</div><div></div></div></td>
      </tr>
    </tbody>
  </table><table border=""0"" cellpadding=""0"" cellspacing=""0"" class=""module"" data-role=""module-button"" data-type=""button"" role=""module"" style=""table-layout:fixed;"" width=""100%"" data-muid=""d61458c8-876e-44ce-9554-b8854b1f44ba"">
      <tbody>
        <tr>
          <td align=""center"" bgcolor="""" class=""outer-td"" style=""padding:0px 0px 0px 0px;"">
            <table border=""0"" cellpadding=""0"" cellspacing=""0"" class=""wrapper-mobile"" style=""text-align:center;"">
              <tbody>
                <tr>
                <td align=""center"" bgcolor=""#FDBE84"" class=""inner-td"" style=""border-radius:6px; font-size:16px; text-align:center; background-color:inherit;"">
                  <a href=""{confirmationLink}"" style=""background-color:#FDBE84; border:0px solid #333333; border-color:#333333; border-radius:100px; border-width:0px; color:#000000; display:inline-block; font-size:14px; font-weight:bold; letter-spacing:1px; line-height:normal; padding:25px 85px 25px 85px; text-align:center; text-decoration:none; border-style:solid;"" target=""_blank"">重置密碼</a>
                </td>
                </tr>
              </tbody>
            </table>
          </td>
        </tr>
      </tbody>
    </table></td>
        </tr>
      </tbody>
    </table></td>
      </tr>
    </tbody>
  </table><table class=""module"" role=""module"" data-type=""spacer"" border=""0"" cellpadding=""0"" cellspacing=""0"" width=""100%"" style=""table-layout: fixed;"" data-muid=""7d922942-9306-4c3a-9888-e47841af10d1"">
   
    </table></td>
                                      </tr>
                                    </table>
                                    <!--[if mso]>
                                  </td>
                                </tr>
                              </table>
                            </center>
                            <![endif]-->
                          </td>
                        </tr>
                      </table>
                    </td>
                  </tr>
                </table>
              </td>
            </tr>
          </table>
        </div>
      </center>
    </body>
        </html>";//打開直接起飛
           

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
        public async Task<IActionResult> GetUserName()
        {
            var user = await _context.Users.Where(u => u.Email == User.FindFirstValue(ClaimTypes.Email)).Select(u => u.Username).FirstOrDefaultAsync();
            return Ok(user);
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
