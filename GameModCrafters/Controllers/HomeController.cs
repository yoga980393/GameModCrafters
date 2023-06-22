using GameModCrafters.Data;
using GameModCrafters.Models;
using GameModCrafters.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace GameModCrafters.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            var gameList = _context.Games
                .Select(g => new
                {
                    Game = g,
                    ModCount = _context.Mods.Count(m => m.GameId == g.GameId)
                })
                .OrderByDescending(x => x.ModCount)
                .Select(x => x.Game)
                .Take(5)
                .ToList();

            var modList = _context.Mods
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
                })
                .OrderByDescending(m => m.DownloadCount)
                .Take(4)
                .ToList();

            var viewModel = new HomeIndexViewModel
            {
                Games = gameList,
                Mods = modList
            };

            return View(viewModel);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }


        //自動搜尋
        [HttpGet]
        public async Task<IActionResult>AutoSearch(string keyword)
        {
            
            List<AutoSearchViewModelcs> searchResults = await GetAutoSearchResults(keyword);

            return PartialView("_AutoSearchResults", searchResults);
        }
        
        private async Task<List<AutoSearchViewModelcs>> GetAutoSearchResults(string keyword)
        {
            var searchResults = await _context.Games
                .Include(g => g.Mods)
                    .ThenInclude(m=>m.Author)
                .Where(g=>g.GameName.Contains(keyword) || g.Mods.Any(m => m.ModName.Contains(keyword)))
                .SelectMany(g=>g.Mods.Select(m => new AutoSearchViewModelcs
                {
                    GameId = g.GameId,
                    GameName = g.GameName,
                    ModId = m.ModId,
                    ModName = m.ModName,
                    Price = m.Price,
                    AuthorName = m.Author.Username,
                    ModThumbnail = m.Thumbnail,
                    GameThumbnail = g.Thumbnail,
                }))
                .ToListAsync();
            
            return searchResults;
        }



        //搜尋按鈕按下1
        [HttpPost]
        public IActionResult SearchResult(string keyword)
        {
            return View();
        }
       


    }
}
