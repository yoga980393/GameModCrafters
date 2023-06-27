using GameModCrafters.Data;
using GameModCrafters.Data;
using GameModCrafters.Models;
using GameModCrafters.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using MoreLinq;
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

        [HttpGet]
        public async Task<IActionResult> GetChatHistory()
        {
            string userEmail = User.FindFirstValue(ClaimTypes.Email); // 從當前用戶的憑證中獲取電子郵件

            // 從 PrivateMessage 表中查找包含當前用戶電子郵件的所有消息
            var messages = await _context.PrivateMessages
                .Where(m => m.Sender.Email == userEmail || m.Receiver.Email == userEmail)
                .Include(m => m.Sender)
                .Include(m => m.Receiver)
                .OrderByDescending(m => m.MessageTime)
                .ToListAsync();

            // 從消息中提取所有獨特的對話者的電子郵件以及是否有未讀訊息
            var chatHistory = messages
                .GroupBy(m => m.Sender.Email == userEmail ? m.Receiver.Email : m.Sender.Email)
                .Select(g => new
                {
                    Email = g.Key,
                    HasUnread = g.Any(m => !m.IsRead && m.Receiver.Email == userEmail)
                })
                .ToList();

            return Ok(chatHistory);
        }


        //自動搜尋
        [HttpGet]
        public async Task<IActionResult>AutoSearch(string keyword)
        {
            
            var searchResults = await GetAutoSearchResults(keyword);

            return PartialView("_AutoSearchResults", searchResults);
        }
        
        private async Task<AutoSearchViewModel> GetAutoSearchResults(string keyword)
        {
            var searchResultsGame = await _context.Games
                .Where(g=>g.GameName.Contains(keyword))
                .ToListAsync();
            var searchResultMod = await _context.Mods
                .Where(m => m.ModName.Contains(keyword))
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
                .ToListAsync();
            var vm = new AutoSearchViewModel()
            {
                Games = searchResultsGame,
                Mods = searchResultMod,
                Count = searchResultMod.Count + searchResultsGame.Count
            };
            return vm;
        }



        //搜尋按鈕按下1

        [HttpGet]

        public async Task<IActionResult> SearchResult(string keyword, int page = 1)
        {

            int pageSize = 8;
            var searchResults = await GetAutoSearchResults(keyword);
            var pagedMods = searchResults.Mods
              .Skip((page - 1) * pageSize)
              .Take(pageSize)
              .ToList();
            var pagedGames = searchResults.Games
              .Skip((page - 1) * pageSize)
              .Take(pageSize)
              .ToList();
            var GametotalPages = (int)Math.Ceiling((double)searchResults.Games.Count / pageSize);
            var ModtotalPages = (int)Math.Ceiling((double)searchResults.Mods.Count / pageSize);
            var VM = new NavbarSearchResultViewmodel()
            {
                Mods = pagedMods,
                ModTotalPages = ModtotalPages,
                GameTotalPages = GametotalPages,
                ModCurrentPage = page,
                Games = pagedGames,
                SearchString = keyword
            };
            return View(VM);


        }
        [HttpPost]

        public async Task<IActionResult> ModSearchResultPage(string keyword, int page = 1)
        {

            int pageSize = 16;
            var searchResults = await GetAutoSearchResults(keyword);
            var pagedMods = searchResults.Mods
              .Skip((page - 1) * pageSize)
              .Take(pageSize)
              .ToList();
            //var pagedGames = searchResults.Games
            //  .Skip((page - 1) * pageSize)
            //  .Take(pageSize)
            //  .ToList();
            var totalPages = (int)Math.Ceiling((double)searchResults.Mods.Count / pageSize);
            var VM = new NavbarSearchResultViewmodel()
            {
                Mods = pagedMods,
                ModTotalPages = totalPages,
                ModCurrentPage = page,
               // Games = pagedGames,
                SearchString = keyword
            };
            return PartialView("_SearchModListPartial", VM);


        }
        [HttpPost]

        public async Task<IActionResult> GameSearchResultPage(string keyword, int page = 1)
        {

            int pageSize = 2;
            var searchResults = await GetAutoSearchResults(keyword);
            //var pagedMods = searchResults.Mods
            //  .Skip((page - 1) * pageSize)
            //  .Take(pageSize)
            //  .ToList();
            var pagedGames = searchResults.Games
              .Skip((page - 1) * pageSize)
              .Take(pageSize)
              .ToList();
            var totalPages = (int)Math.Ceiling((double)searchResults.Games.Count / pageSize);
            var VM = new NavbarSearchResultViewmodel()
            {
                //Mods = pagedMods,
                GameTotalPages = totalPages,
                GameCurrentPage = page,
                Games = pagedGames,
                SearchString = keyword
            };
            return PartialView("_SearchGameListPartial", VM);


        }
       

        [HttpGet]
        public IActionResult GetChatHistoryWithUser(string receiverId)
        {
            var senderId = User.FindFirst(ClaimTypes.Email)?.Value;
            var chatHistory = _context.PrivateMessages
                .Where(m => m.SenderId == senderId && m.ReceiverId == receiverId || m.SenderId == receiverId && m.ReceiverId == senderId)
                .OrderBy(m => m.MessageTime)
                .Select(m => new {
                    SenderId = m.SenderId,
                    ReceiverId = m.ReceiverId,
                    MessageContent = m.MessageContent,
                    MessageTime = m.MessageTime
                })
                .ToList();
            return Json(chatHistory);
        }

        // GET: /Home/GetUserByEmail
        [HttpGet]
        public async Task<IActionResult> GetUserByEmail(string email)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);

            if (user == null)
            {
                return NotFound(new { message = "No user found with this email." });
            }

            return Ok(user);
        }

        // GET: /Home/GetAllUserEmails
        [HttpGet]
        public async Task<IActionResult> GetAllUserEmails()
        {
            var emails = await _context.Users.Select(u => u.Email).ToListAsync();
            return Ok(emails);
        }
    }
}
