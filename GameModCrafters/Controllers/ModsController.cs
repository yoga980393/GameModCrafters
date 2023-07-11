using GameModCrafters.Data;
using GameModCrafters.Models;
using GameModCrafters.Services;
using GameModCrafters.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Threading.Tasks;

namespace GameModCrafters.Controllers
{
    public class ModsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger _logger;
        private readonly NotificationService _notification;
        private IWebHostEnvironment _hostingEnvironment;

        public ModsController(ApplicationDbContext context, ILogger<ModsController> logger, NotificationService notification, IWebHostEnvironment hostingEnvironment)
        {
            _context = context;
            _logger = logger;
            _notification = notification;
            _hostingEnvironment = hostingEnvironment;
        }

        // GET: Mods
        public async Task<IActionResult> Index(FilterViewModel filter, int page = 1)
        {
            var now = DateTime.Now;
            DateTime startDate;

            switch (filter.TimeFilter)
            {
                case 1:  // 一天内
                    startDate = now.AddDays(-1);
                    break;
                case 2:  // 一周内
                    startDate = now.AddDays(-7);
                    break;
                case 3:  // 一个月内
                    startDate = now.AddMonths(-1);
                    break;
                default:  // 全部（不设置开始日期）
                    startDate = DateTime.MinValue;
                    break;
            }

            var mods = _context.Mods
                .Where(m => m.ModId != null && (string.IsNullOrEmpty(filter.SearchString) || m.ModName.Contains(filter.SearchString)))
                .Where(m => m.CreateTime >= startDate)
                .Where(m => m.IsDone)
                .Include(m => m.Author)
                .Include(m => m.Game)
                .Include(m => m.ModLikes)
                .Include(m => m.Favorite)
                .Include(m => m.Downloaded)
                .Include(m => m.ModTags)
                    .ThenInclude(mt => mt.Tag)
                .Select(m => new ModViewModel
                {
                    ModId = m.ModId,
                    Thumbnail = m.Thumbnail,
                    ModName = m.ModName,
                    Price = (int)m.Price,
                    GameName = m.Game.GameName,
                    AuthorName = m.Author.Username,
                    CreateTime = m.CreateTime,
                    UpdateTime = m.UpdateTime,
                    Description = m.Description,
                    Capacity = 0,
                    LikeCount = m.ModLikes.Count,
                    FavoriteCount = m.Favorite.Count,
                    DownloadCount = m.Downloaded.Count,
                    TagNames = m.ModTags.Select(mt => mt.Tag.TagName).ToList()
                });
                

            switch (filter.SortFilter)
            {
                case "uploadTime":
                    mods = (filter.OrderFilter == "desc") ? mods.OrderByDescending(m => m.CreateTime) : mods.OrderBy(m => m.CreateTime);
                    break;
                case "updateTime":
                    mods = (filter.OrderFilter == "desc") ? mods.OrderByDescending(m => m.UpdateTime) : mods.OrderBy(m => m.UpdateTime);
                    break;
                case "downloadCount":
                    mods = (filter.OrderFilter == "desc") ? mods.OrderByDescending(m => m.DownloadCount) : mods.OrderBy(m => m.DownloadCount);
                    break;
                case "name":
                    mods = (filter.OrderFilter == "desc") ? mods.OrderByDescending(m => m.ModName) : mods.OrderBy(m => m.ModName);
                    break;
                default: 
                    mods = mods.OrderByDescending(m => m.CreateTime);
                    break;
            }

            if (filter.PageSize <= 0)
            {
                filter.PageSize = 8; // fallback to default value if invalid input
            }

            var pagedModel = new PagedModsModel
            {
                Mods = await mods.Skip((page - 1) * filter.PageSize).Take(filter.PageSize).ToListAsync(),
                CurrentPage = page,
                TotalPages = (int)Math.Ceiling(await mods.CountAsync() / (double)filter.PageSize)
            };

            return View(pagedModel);
        }

        // GET: Mods/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var mod = await _context.Mods
                .Include(m => m.Author)
                .Include(m => m.Game)
                .Include(m => m.ModLikes)
                .Include(m => m.Favorite)
                .Include(m => m.ModTags).ThenInclude(mt => mt.Tag)
                .FirstOrDefaultAsync(m => m.ModId == id);

            if (mod == null)
            {
                return NotFound();
            }

            // 取得用戶ID
            var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;

            // 檢查該用戶是否已經點過讚
            bool userHasLiked = _context.ModLikes.Any(m => m.ModId == id && m.UserId == userId);
            bool userHasFavorite = _context.Favorites.Any(m => m.ModId == id && m.UserId == userId);

            var comments = _context.ModComments
                .Where(c => c.ModId == id)
                .Include(c => c.User)
                .OrderBy(c => c.CommentDate) 
                .Select(c => new ModCommentViewModel
                {
                    CommentId = c.CommentId,
                    UserName = c.User.Username,
                    CommentContent = c.CommentContent
                })
                .ToList();

            var modDetailViewModel = new ModDetailViewModel
            {
                ModId = mod.ModId,
                ModName = mod.ModName,
                Tags = mod.ModTags?.Select(mt => mt.Tag?.TagName ?? "無資料").ToList() ?? new List<string> { "無資料" },
                CreateTime = mod.CreateTime,
                UpdateTime = mod.UpdateTime,
                Description = mod.Description,
                InstallationInstructions = mod.InstallationInstructions,
                LikeCount = mod.ModLikes?.Count() ?? 0,
                FavoriteCount = mod.Favorite?.Count() ?? 0,
                DownloadCount = mod.Downloaded?.Count() ?? 0,
                Price = (int)mod.Price,
                AuthorName = mod.Author.Username,
                AuthorId = mod.AuthorId,
                AuthorWorkCount = _context.Mods.Count(m => m.AuthorId == mod.AuthorId),
                AuthorLikesReceived = _context.ModLikes.Count(ml => ml.Mod.AuthorId == mod.AuthorId),
                GameId = mod.GameId,
                Comments = comments,
                UserHasLiked = userHasLiked,
                UserHasFavorite = userHasFavorite,
                userAtavar = mod.Author.Avatar,
                userCover = mod.Author.BackgroundImage,
                UserHasPurchased = _context.PurchasedMods.Any(pm => pm.UserId == userId && pm.ModId == mod.ModId),
                DownloadLink = mod.DownloadLink
            };

            return View(modDetailViewModel);
        }

        // GET: Mods/Create
        [Authorize]
        public async Task<IActionResult> Create(string gameId)
        {
            ViewData["SelectedGameId"] = gameId;

            string loggedInUserEmail = User.FindFirstValue(ClaimTypes.Email);

            var unfinishedMod = await _context.Mods
                .FirstOrDefaultAsync(m => m.AuthorId == loggedInUserEmail && m.IsDone == false);


            if (unfinishedMod != null)
            {
                // 如果有未完成的 Mod，則在 ViewBag 中添加一個標記
                ViewBag.HasUnfinishedMod = true;
                ViewBag.UnfinishedModId = unfinishedMod.ModId;
            }

            ViewData["AuthorId"] = new SelectList(_context.Users, "Email", "Email");
            ViewData["GameId"] = new SelectList(_context.Games, "GameId", "GameName");

            var tags = _context.Tags.Select(t => t.TagName).ToList();
            ViewData["Tags"] = tags;

            return View();
        }

        // POST: Mods/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("GameId,AuthorId,ModName,Description,InstallationInstructions,DownloadLink,Price,Thumbnail,CreateTime,UpdateTime,IsDone")] Mod mod, string[] SelectedTags, IFormFile gameFile)
        {
            var counter = await _context.Counters.SingleOrDefaultAsync(c => c.CounterName == "Mod");
            if (counter == null)
            {
                _logger.LogInformation("Counter with name 'Mod' was not found.");
                // Handle the case when there is no counter named 'Mod'
                // For example, create a new counter with name 'Mod' and value 0
            }
            string newModId = $"m{counter.Value + 1:D4}";  // Format as 'm0001'
            counter.Value++;  // Increment counter
            _context.Counters.Update(counter);
            await _context.SaveChangesAsync();
            mod.ModId = newModId;

            foreach (var tagName in SelectedTags)
            {
                if (tagName == "選擇一個tag") continue;

                // 根據 tag 名稱找到對應的 tag
                var tag = await _context.Tags.SingleOrDefaultAsync(t => t.TagName == tagName);
                if (tag != null)
                {
                    // 將新的 ModTag 加入到 ModTags 表中
                    var modTag = new ModTag { ModId = mod.ModId, TagId = tag.TagId };
                    _context.Add(modTag);
                }
            }

            if (!ModelState.IsValid)
            {
                foreach (var state in ModelState)
                {
                    _logger.LogInformation($"Key: {state.Key}, ValidationState: {state.Value.ValidationState}");

                    if (state.Value.Errors.Any())
                    {
                        var errors = String.Join(",", state.Value.Errors.Select(e => e.ErrorMessage));
                        _logger.LogInformation($"Errors: {errors}");
                    }
                }
            }

            // Handle file upload
            if (gameFile != null && gameFile.Length > 0)
            {
                // Create a unique filename
                var uniqueFileName = Guid.NewGuid().ToString() + "_" + gameFile.FileName;

                // Combine the filename with the path to your wwwroot/GameArchive folder
                var relativeFilePath = Path.Combine("GameArchive", uniqueFileName);
                var absoluteFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", relativeFilePath);

                // Use a FileStream to copy the file data to the specified filepath
                using (var stream = new FileStream(absoluteFilePath, FileMode.Create))
                {
                    await gameFile.CopyToAsync(stream);
                }

                // Save the relative filepath to the mod
                mod.DownloadLink = "/" + relativeFilePath.Replace("\\", "/");  // adjust slashes to web format
            }

            if (ModelState.IsValid)
            {
                mod.CreateTime = DateTime.Now;
                mod.UpdateTime = DateTime.Now;
                _context.Add(mod);
                await _context.SaveChangesAsync();
                FilterViewModel filterModel = new FilterViewModel();
                return RedirectToAction("Details", "Games", new { id = mod.GameId, filter = filterModel, page = 1 });
            }

            var tags = _context.Tags.Select(t => t.TagName).ToList();
            ViewData["Tags"] = tags;
            ViewData["AuthorId"] = new SelectList(_context.Users, "Email", "Email");
            ViewData["GameId"] = new SelectList(_context.Games, "GameId", "GameName");
            ViewData["SelectedTags"] = SelectedTags;
            return View(mod);
        }


        // GET: Mods/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var mod = await _context.Mods.FindAsync(id);
            if (mod == null)
            {
                return NotFound();
            }

            // 獲取該 Mod 的所有 Tag 名稱
            var selectedTags = await _context.ModTags
                .Where(mt => mt.ModId == id)
                .Select(mt => mt.Tag.TagName)
                .ToListAsync();

            var tags = _context.Tags.Select(t => t.TagName).ToList();
            ViewData["Tags"] = tags;
            ViewData["AuthorId"] = new SelectList(_context.Users, "Email", "Email", mod.AuthorId);
            ViewData["GameId"] = new SelectList(_context.Games, "GameId", "GameName");
            ViewData["SelectedTags"] = selectedTags;
            return View(mod);
        }


        // POST: Mods/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("ModId, GameId,AuthorId,ModName,Description,InstallationInstructions,DownloadLink,Price,Thumbnail,CreateTime,UpdateTime,IsDone")] Mod mod, string[] SelectedTags)
        {
            if (id != mod.ModId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    mod.UpdateTime = DateTime.Now;
                    _context.Update(mod);

                    // 首先，找出所有需要删除的 ModTags 并删除
                    var modTagsToDelete = _context.ModTags.Where(mt => mt.ModId == mod.ModId);
                    _context.ModTags.RemoveRange(modTagsToDelete);

                    // 然后，为模块添加新的 ModTags
                    foreach (var tagName in SelectedTags)
                    {
                        if (tagName == "選擇一個tag") continue;

                        // 根據 tag 名稱找到對應的 tag
                        var tag = await _context.Tags.SingleOrDefaultAsync(t => t.TagName == tagName);
                        if (tag != null)
                        {
                            // 將新的 ModTag 加入到 ModTags 表中
                            var modTag = new ModTag { ModId = mod.ModId, TagId = tag.TagId };
                            _context.Add(modTag);
                        }
                    }

                    // Save the changes to the database
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ModExists(mod.ModId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Details", "Games", new { id = mod.GameId });
            }
            ViewData["AuthorId"] = new SelectList(_context.Users, "Email", "Email", mod.AuthorId);
            ViewData["GameId"] = new SelectList(_context.Games, "GameId", "GameId", mod.GameId);
            return View(mod);
        }


        // GET: Mods/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var mod = await _context.Mods
                .Include(m => m.Author)
                .Include(m => m.Game)
                .FirstOrDefaultAsync(m => m.ModId == id);
            if (mod == null)
            {
                return NotFound();
            }

            return View(mod);
        }

        // POST: Mods/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var mod = await _context.Mods.FindAsync(id);

            // Find all related entities to the Mod and remove them.
            var modTags = _context.ModTags.Where(mt => mt.ModId == id);
            _context.ModTags.RemoveRange(modTags);

            var modLikes = _context.ModLikes.Where(ml => ml.ModId == id);
            _context.ModLikes.RemoveRange(modLikes);

            var modComments = _context.ModComments.Where(mc => mc.ModId == id);
            _context.ModComments.RemoveRange(modComments);

            var favorites = _context.Favorites.Where(f => f.ModId == id);
            _context.Favorites.RemoveRange(favorites);

            var logs = _context.Logs.Where(l => l.ModId == id);
            _context.Logs.RemoveRange(logs);

            var downloadedMods = _context.Downloadeds.Where(dm => dm.ModId == id);
            _context.Downloadeds.RemoveRange(downloadedMods);

            var purchasedMods = _context.PurchasedMods.Where(pm => pm.ModId == id);
            _context.PurchasedMods.RemoveRange(purchasedMods);

            var gameId = mod.GameId;

            _context.Mods.Remove(mod);
            await _context.SaveChangesAsync();
            return RedirectToAction("Details", "Games", new { id = gameId });
        }


        private bool ModExists(string id)
        {
            return _context.Mods.Any(e => e.ModId == id);
        }

        [HttpPost]
        public async Task<IActionResult> UploadFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("Invalid file.");
            }

            var fileName = Path.GetFileNameWithoutExtension(file.FileName);
            var extension = Path.GetExtension(file.FileName);
            var date = DateTime.Now.ToString("yyyyMMddHHmmss");
            var random = Guid.NewGuid().ToString().Substring(0, 4); // 生成一個4位數的隨機字串
            var newFileName = $"{fileName}_{date}_{random}{extension}";

            var filePath = Path.Combine("wwwroot/ModImages", newFileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }

            var fileUrl = Url.Content("~/ModImages/" + newFileName);
            return Ok(new { fileUrl });
        }

        [HttpPost]
        public async Task<IActionResult> Upload(IFormFile upload)
        {
            var fileName = Path.GetFileNameWithoutExtension(upload.FileName);
            var extension = Path.GetExtension(upload.FileName);
            var date = DateTime.Now.ToString("yyyyMMddHHmmss");
            var random = Guid.NewGuid().ToString().Substring(0, 4); // 生成一個4位數的隨機字串
            var newFileName = $"{fileName}_{date}_{random}{extension}";

            var path = Path.Combine("wwwroot/ModDescriptionImages", newFileName);

            using (var stream = System.IO.File.Create(path))
            {
                await upload.CopyToAsync(stream);
            }

            return Json(new
            {
                uploaded = true,
                url = Url.Content("~/ModDescriptionImages/" + newFileName) // Update the path here
            });
        }

        [HttpPost]
        public async Task<IActionResult> CreateComment(string modId, string userId, string content)
        {
            var newComment = new ModComment
            {
                CommentId = Guid.NewGuid().ToString(),
                ModId = modId,
                UserId = userId,
                CommentContent = content,
                CommentDate = DateTime.Now
            };

            _context.Add(newComment);
            await _context.SaveChangesAsync();

            if (modId == null)
            {
                return NotFound();
            }

            var mod = await _context.Mods
                .Include(m => m.Author)
                .Include(m => m.Game)
                .Include(m => m.ModTags).ThenInclude(mt => mt.Tag)
                .FirstOrDefaultAsync(m => m.ModId == modId);

            if (mod == null)
            {
                return NotFound();
            }

            var comments = _context.ModComments
                .Where(c => c.ModId == modId)
                .Include(c => c.User)
                .OrderBy(c => c.CommentDate)
                .Select(c => new ModCommentViewModel
                {
                    CommentId = c.CommentId,
                    UserName = c.User.Username,
                    CommentContent = c.CommentContent
                })
                .ToList();

            var modDetailViewModel = new ModDetailViewModel
            {
                ModId = mod.ModId,
                ModName = mod.ModName,
                Tags = mod.ModTags?.Select(mt => mt.Tag?.TagName ?? "無資料").ToList() ?? new List<string> { "無資料" },
                CreateTime = mod.CreateTime,
                UpdateTime = mod.UpdateTime,
                Description = mod.Description,
                InstallationInstructions = mod.InstallationInstructions,
                LikeCount = mod.ModLikes?.Count() ?? 0,
                FavoriteCount = mod.Favorite?.Count() ?? 0,
                DownloadCount = mod.Downloaded?.Count() ?? 0,
                Price = (int)mod.Price, 
                AuthorName = mod.Author.Username,
                AuthorWorkCount = _context.Mods.Count(m => m.AuthorId == mod.AuthorId),
                AuthorLikesReceived = _context.ModLikes.Count(ml => ml.Mod.AuthorId == mod.AuthorId),
                GameId = mod.GameId,
                Comments = comments
            };

            // 返回帶有新留言的 Partial View
            return PartialView("_CommentsPartial", modDetailViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteComment(string modId, string commentId)
        {
            var comment = await _context.ModComments.FindAsync(commentId);
            if (comment != null)
            {
                _context.ModComments.Remove(comment);
                await _context.SaveChangesAsync();
            }

            // 重新載入模型並返回 Partial View
            var mod = await _context.Mods
                .Include(m => m.Author)
                .Include(m => m.Game)
                .Include(m => m.ModTags).ThenInclude(mt => mt.Tag)
                .FirstOrDefaultAsync(m => m.ModId == modId);

            if (mod == null)
            {
                return NotFound();
            }

            var comments = _context.ModComments
                .Where(c => c.ModId == modId)
                .Include(c => c.User)
                .OrderBy(c => c.CommentDate)
                .Select(c => new ModCommentViewModel
                {
                    CommentId = c.CommentId,
                    UserName = c.User.Username,
                    CommentContent = c.CommentContent
                })
                .ToList();

            var modDetailViewModel = new ModDetailViewModel
            {
                ModId = mod.ModId,
                ModName = mod.ModName,
                Tags = mod.ModTags?.Select(mt => mt.Tag?.TagName ?? "無資料").ToList() ?? new List<string> { "無資料" },
                CreateTime = mod.CreateTime,
                UpdateTime = mod.UpdateTime,
                Description = mod.Description,
                InstallationInstructions = mod.InstallationInstructions,
                LikeCount = mod.ModLikes?.Count() ?? 0,
                FavoriteCount = mod.Favorite?.Count() ?? 0,
                DownloadCount = mod.Downloaded?.Count() ?? 0,
                Price = (int)mod.Price,
                AuthorName = mod.Author.Username,
                AuthorWorkCount = _context.Mods.Count(m => m.AuthorId == mod.AuthorId),
                AuthorLikesReceived = _context.ModLikes.Count(ml => ml.Mod.AuthorId == mod.AuthorId),
                GameId = mod.GameId,
                Comments = comments
            };

            return PartialView("_CommentsPartial", modDetailViewModel);
        }

        [HttpPost]
        public IActionResult Like(string modId)
        {
            try
            {
                var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;

                if (string.IsNullOrEmpty(userId))
                {
                    return Json(new { success = false, errorMessage = "User is not authenticated." });
                }

                // 確認該用戶是否已經點過讚
                var existingLike = _context.ModLikes.FirstOrDefault(m => m.ModId == modId && m.UserId == userId);
                if (existingLike == null)
                {
                    // 如果還沒點過讚，就建立新的 ModLike
                    var newLike = new ModLike
                    {
                        ModId = modId,
                        UserId = userId,
                        Liked = true,
                        RatingDate = DateTime.Now
                    };
                    _context.ModLikes.Add(newLike);
                }
                else
                {
                    // 如果已經點過讚，就刪除該筆記錄
                    _context.ModLikes.Remove(existingLike);
                }
                _context.SaveChanges();

                // 回傳新的點讚數量
                var newLikeCount = _context.ModLikes.Count(m => m.ModId == modId);
                return Json(new { success = true, newLikeCount = newLikeCount });
            }
            catch (Exception ex)
            {
                // 發生錯誤時，回傳錯誤訊息
                return Json(new { success = false, errorMessage = ex.Message });
            }
        }

        [HttpPost]
        public IActionResult Favorite(string modId)
        {
            try
            {
                var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;

                if (string.IsNullOrEmpty(userId))
                {
                    return Json(new { success = false, errorMessage = "User is not authenticated." });
                }

                // 確認該用戶是否已經點過讚
                var existingFavorite = _context.Favorites.FirstOrDefault(m => m.ModId == modId && m.UserId == userId);
                if (existingFavorite == null)
                {
                    // 如果還沒點過讚，就建立新的 ModLike
                    var newFavorite = new Favorite
                    {
                        ModId = modId,
                        UserId = userId,
                        AddTime = DateTime.Now
                    };
                    _context.Favorites.Add(newFavorite);
                }
                else
                {
                    // 如果已經點過讚，就刪除該筆記錄
                    _context.Favorites.Remove(existingFavorite);
                }
                _context.SaveChanges();

                // 回傳新的點讚數量
                var newFavoriteCount = _context.Favorites.Count(m => m.ModId == modId);
                return Json(new { success = true, newFavoriteCount = newFavoriteCount });
            }
            catch (Exception ex)
            {
                // 發生錯誤時，回傳錯誤訊息
                return Json(new { success = false, errorMessage = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> PurchaseMod([FromBody] PurchaseRequest request)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    var user = await _context.Users.SingleOrDefaultAsync(u => u.Email == User.FindFirstValue(ClaimTypes.Email));
                    var mod = await _context.Mods.SingleOrDefaultAsync(m => m.ModId == request.ModId);
                    if (user == null || mod == null)
                    {
                        return NotFound();
                    }

                    // 驗證用戶的ModCoin餘額是否足夠
                    if (user.ModCoin < mod.Price)
                    {
                        return BadRequest("ModCoin不足");
                    }

                    // 扣除用戶的ModCoin
                    user.ModCoin -= (int)mod.Price;

                    // 獲取Mod的作者
                    var author = await _context.Users.SingleOrDefaultAsync(u => u.Email == mod.AuthorId);

                    // 將Mod的價格添加到作者的ModCoin
                    author.ModCoin += (int)mod.Price;

                    // 在PurchasedMod表中添加一條新的紀錄
                    var purchasedMod = new PurchasedMod
                    {
                        UserId = user.Email,
                        ModId = mod.ModId,
                        AddTime = DateTime.UtcNow
                    };
                    _context.PurchasedMods.Add(purchasedMod);
                    // 儲存變更
                    await _context.SaveChangesAsync();

                    var notifierId = user.Email;
                    var recipientId = mod.AuthorId;
                    var content = $"{User.FindFirstValue(ClaimTypes.Name)} 購買了你的Mod({mod.ModName})";

                    await _notification.CreateNotificationAsync(notifierId, recipientId, content);

                    // 提交事務
                    transaction.Commit();

                    return Ok(new { newModCoin = user.ModCoin });
                }
                catch (Exception ex)
                {
                    // 事務回滾
                    transaction.Rollback();

                    // 返回錯誤訊息
                    return BadRequest(ex.Message);
                }
            }
        }

        public class PurchaseRequest
        {
            public string ModId { get; set; }
        }

        [Authorize]
        [HttpPost]
        public async Task DownloadMod(string modId)
        {
            var mod = await _context.Mods.FindAsync(modId);

            var downloaded = new Downloaded
            {
                DownloadId = Guid.NewGuid().ToString(),
                UserId = User.FindFirstValue(ClaimTypes.Email),
                ModId = modId,
                DownloadTime = DateTime.Now
            };

            await _context.AddAsync(downloaded);
            await _context.SaveChangesAsync();
        }
    }
}
