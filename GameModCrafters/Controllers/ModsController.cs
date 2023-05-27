using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using GameModCrafters.Data;
using GameModCrafters.Models;

namespace GameModCrafters.Controllers
{
    public class ModsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ModsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Mods
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Mods.Include(m => m.Author).Include(m => m.Game);
            return View(await applicationDbContext.ToListAsync());
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
                .FirstOrDefaultAsync(m => m.ModId == id);
            if (mod == null)
            {
                return NotFound();
            }

            return View(mod);
        }

        // GET: Mods/Create
        public IActionResult Create()
        {
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
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ModId,GameId,AuthorId,ModName,Description,InstallationInstructions,DownloadLink,Price,Thumbnail,CreateTime,UpdateTime,IsDone")] Mod mod, string[] SelectedTags)
        {
            var counter = await _context.Counters.FindAsync(1);
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

            ModelState.Clear();
            if (TryValidateModel(mod))
            {
                mod.CreateTime = DateTime.Now;
                mod.UpdateTime = DateTime.Now;
                _context.Add(mod);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }


            ViewData["AuthorId"] = new SelectList(_context.Users, "Email", "Email", mod.AuthorId);
            ViewData["GameId"] = new SelectList(_context.Games, "GameId", "GameId", mod.GameId);
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
            ViewData["AuthorId"] = new SelectList(_context.Users, "Email", "Email", mod.AuthorId);
            ViewData["GameId"] = new SelectList(_context.Games, "GameId", "GameId", mod.GameId);
            return View(mod);
        }

        // POST: Mods/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("ModId,GameId,AuthorId,ModName,Description,InstallationInstructions,DownloadLink,Price,Thumbnail,CreateTime,UpdateTime,IsDone")] Mod mod)
        {
            if (id != mod.ModId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(mod);
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
    }
}
