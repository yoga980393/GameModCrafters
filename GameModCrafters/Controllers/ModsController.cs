using GameModCrafters.Data;
using GameModCrafters.Models;
using GameModCrafters.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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
        private const int PageSize = 8;

        public ModsController(ApplicationDbContext context, ILogger<ModsController> logger)
        {
            _context = context;
            _logger = logger;
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
                    Price = m.Price,
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
                .Include(m => m.ModTags).ThenInclude(mt => mt.Tag)
                .FirstOrDefaultAsync(m => m.ModId == id);

            if (mod == null)
            {
                return NotFound();
            }

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
                Price = mod.Price,
                AuthorName = mod.Author.Username,
                AuthorWorkCount = _context.Mods.Count(m => m.AuthorId == mod.AuthorId),
                AuthorLikesReceived = _context.ModLikes.Count(ml => ml.Mod.AuthorId == mod.AuthorId)
            };

            return View(modDetailViewModel);
        }

        // GET: Mods/Create
        [Authorize]
        public async Task<IActionResult> Create()
        {
            string loggedInUserEmail = User.FindFirstValue(ClaimTypes.NameIdentifier);

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
        public async Task<IActionResult> Create([Bind("GameId,AuthorId,ModName,Description,InstallationInstructions,DownloadLink,Price,Thumbnail,CreateTime,UpdateTime,IsDone")] Mod mod, string[] SelectedTags)
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

            if (ModelState.IsValid)
            {
                mod.CreateTime = DateTime.Now;
                mod.UpdateTime = DateTime.Now;
                _context.Add(mod);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
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
            ViewData["GameId"] = new SelectList(_context.Games, "GameId", "GameId", mod.GameId);
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
                return RedirectToAction(nameof(Index));
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

            // Find all ModTag entities related to the Mod.
            var modTags = _context.ModTags.Where(mt => mt.ModId == id);
            _context.ModTags.RemoveRange(modTags);

            _context.Mods.Remove(mod);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
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
    }
}
