using GameModCrafters.Data;
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
using System.Security.Claims;
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
            
            List<AutoSearchViewModel> searchResults = await GetAutoSearchResults(keyword);

            return PartialView("_AutoSearchResults", searchResults);
        }
        
        private async Task<List<AutoSearchViewModel>> GetAutoSearchResults(string keyword)
        {
            var searchResults = await _context.Games
                .Include(g => g.Mods)
                    .ThenInclude(m=>m.Author)
                .Where(g=>g.GameName.Contains(keyword) || g.Mods.Any(m => m.ModName.Contains(keyword)))
                .SelectMany(g=>g.Mods.Select(m => new AutoSearchViewModel
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
        //[HttpPost]
        //public IActionResult SearchResult(string keyword)
        //{

        //    return View();
        //}
        [HttpGet]
        public async Task<IActionResult> SearchResult(string keyword)
        {
            var searchResults = await GetSearchResults(keyword);
            var viewModel = new NavbarSearchResultViewmodel
            {
                Games = searchResults.Select(g => new Game
                {
                    GameId = g.GameId,
                    GameName = g.GameName,
                    Thumbnail = g.GameThumbnail
                }).ToList(),
                Mods = searchResults.Select(m => new ModViewModel
                {
                    ModId = m.ModId,
                    Thumbnail = m.ModThumbnail,
                    ModName = m.ModName,
                    Price = m.Price,
                    GameName = m.GameName,
                    AuthorName = m.AuthorName,
                    CreateTime = m.CreateTime,
                    UpdateTime = m.UpdateTime,
                    Description = m.Description,
                   // Capacity = m.Capacity,
                    LikeCount = m.LikeCount,
                    FavoriteCount = m.FavoriteCount,
                    DownloadCount = m.DownloadCount,
                    TagNames = m.TagNames
                }).ToList(),
                TotalPages = 0 // Set the total pages value according to your implementation
            };

            return View(viewModel);
        }

        private async Task<List<AutoSearchViewModel>> GetSearchResults(string keyword)
        {
            var searchResults = await _context.Games
                .Include(g => g.Mods)
                .ThenInclude(m => m.Author)
                .Where(g => g.GameName.Contains(keyword) || g.Mods.Any(m => m.ModName.Contains(keyword)))
                .SelectMany(g => g.Mods.Select(m => new AutoSearchViewModel
                {
                    GameId = g.GameId,
                    GameName = g.GameName,
                    ModId = m.ModId,
                    ModName = m.ModName,
                    Price = m.Price,
                    AuthorName = m.Author.Username,
                    ModThumbnail = m.Thumbnail,
                    GameThumbnail = g.Thumbnail,
                    CreateTime = m.CreateTime,
                    UpdateTime = m.UpdateTime,
                    Description = m.Description,
                    //Capacity = m.Capacity,
                    LikeCount = m.ModLikes.Count,
                    FavoriteCount = m.Favorite.Count,
                    DownloadCount = m.Downloaded.Count,
                    TagNames = m.ModTags.Select(t => t.Tag.TagName).ToList()
                }))
                .ToListAsync();

            return searchResults;
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
