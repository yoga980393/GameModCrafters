using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using GameModCrafters.Data;
using GameModCrafters.Models;
using GameModCrafters.ViewModels;

namespace GameModCrafters.Controllers
{
    public class GamesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public GamesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Games
        public async Task<IActionResult> Index()
        {
            return View(await _context.Games.ToListAsync());
        }

        // GET: Games/Details/5
        public async Task<IActionResult> Details(string id, FilterViewModel filter, int page = 1)
        {
            if (id == null)
            {
                return NotFound();
            }

            var game = await _context.Games
                .FirstOrDefaultAsync(m => m.GameId == id);
            if (game == null)
            {
                return NotFound();
            }

            ViewBag.Game = game;

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
                .Where(m => m.GameId == id)
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
                TotalPages = (int)Math.Ceiling(await mods.CountAsync() / (double)filter.PageSize),
                GameId = id
            };

            return View(pagedModel);
        }

        // GET: Games/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Games/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("GameId,GameName,Description,Thumbnail,CreateTime")] Game game)
        {
            if (ModelState.IsValid)
            {
                _context.Add(game);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(game);
        }

        // GET: Games/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var game = await _context.Games.FindAsync(id);
            if (game == null)
            {
                return NotFound();
            }
            return View(game);
        }

        // POST: Games/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("GameId,GameName,Description,Thumbnail,CreateTime")] Game game)
        {
            if (id != game.GameId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(game);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!GameExists(game.GameId))
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
            return View(game);
        }

        // GET: Games/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var game = await _context.Games
                .FirstOrDefaultAsync(m => m.GameId == id);
            if (game == null)
            {
                return NotFound();
            }

            return View(game);
        }

        // POST: Games/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var game = await _context.Games.FindAsync(id);
            _context.Games.Remove(game);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool GameExists(string id)
        {
            return _context.Games.Any(e => e.GameId == id);
        }
    }
}
