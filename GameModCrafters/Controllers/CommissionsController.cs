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
using Microsoft.Extensions.Logging;
using System.Security.Claims;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Authorization;
using GameModCrafters.Encryption;
using GameModCrafters.Services;
using SendGrid;
using Microsoft.AspNetCore.Http;
using System.IO;
using Newtonsoft.Json;
using System.Security.Cryptography;

namespace GameModCrafters.Controllers
{
    public class CommissionsController : Controller
    {
        private readonly IHashService _hashService;

        private readonly ApplicationDbContext _context;

        private readonly ISendGridClient _sendGridClient;

        private readonly ModService _modService;

        private readonly ILogger<CommissionsController> _logger;


        public CommissionsController(ApplicationDbContext context, IHashService hashService, ISendGridClient sendGridClient, ModService modService, ILogger<CommissionsController> logger)
        {
            _hashService = hashService;
            _context = context;
            _sendGridClient = sendGridClient;
            _modService = modService;
            _logger = logger;
        }

        private bool CommissionExists(string id)
        {
            return _context.Commissions.Any(e => e.CommissionId == id);
        }


        // GET: Commissions
        [Authorize]
        public async Task<IActionResult> Index()
        {
            var usermail = User.FindFirstValue(ClaimTypes.Email);
            if (usermail == null)
            {
                return NotFound();
            }
            var applicationDbContext = _context.Commissions.Include(c => c.CommissionStatus).Include(c => c.Delegator).Include(c => c.Game);
            var commissions = await _context.Commissions
               .Where(c => c.DelegatorId == usermail)
               .Include(c => c.Delegator)
               .Include(c => c.CommissionStatus)
               .Select(c => new CommissionViewModel
               {
                   CommissionId = c.CommissionId,
                   DelegatorName = c.Delegator.Username,
                   CommissionTitle = c.CommissionTitle,
                   Budget = c.Budget,
                   CreateTime = c.CreateTime,
                   UpdateTime = c.UpdateTime,
                   Status = c.CommissionStatus.Status
               })
               .ToListAsync();
            return View(commissions);
        }


        //GET: Commissions/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }
            ViewBag.TrackingEX = false;

            var commission = await _context.Commissions
                .Include(c => c.CommissionStatus)
                .Include(c => c.Delegator)
                .Include(c => c.Game)
                .FirstOrDefaultAsync(m => m.CommissionId == id);

            ViewData["CommissionStatusId"] = new SelectList(_context.CommissionStatuses, "CommissionStatusId", "Status", commission.CommissionStatusId);
            ViewData["DelegatorId"] = new SelectList(_context.Users, "Email", "Email", commission.DelegatorId);
            ViewData["GameName"] = new SelectList(_context.Games, "GameName", "GameName", commission.GameId);

            if (commission == null)
            {
                return NotFound();
            }
            var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;

            var existingTracking = _context.CommissionTrackings.FirstOrDefault(m => m.CommissionId == id && m.UserId == userId);
            if (existingTracking != null)
            {
                ViewBag.TraEX = true;
            }
            else
            {
                ViewBag.TraEX = false;
            }
            return View(commission);
        }
        //[Authorize]
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Details(string id, [Bind("CommissionId,DelegatorId,GameId,CommissionTitle,CommissionDescription,Budget,Deadline,CommissionStatusId,CreateTime,UpdateTime,IsDone,Trash")] Commission commission)
        //{
        //    if (id != commission.CommissionId)
        //    {
        //        return RedirectToAction(nameof(Index));
        //    }



        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            // 在適當的位置取得追蹤委託的相關資訊
        //            var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value; // 取得當前使用者的ID或識別符號
        //            string commissionId = id; // 請根據實際情況取得委託的ID

        //            // 建立新的CommissionTracking物件
        //            var commissionTracking = new CommissionTracking
        //            {
        //                UserId = userId,
        //                CommissionId = commissionId,
        //                AddTime = DateTime.Now // 可以根據需要指定添加時間,

        //            };

        //            // 將CommissionTracking物件新增到資料庫中
        //            _context.CommissionTrackings.Add(commissionTracking);
        //            await _context.SaveChangesAsync();
        //        }
        //        catch (DbUpdateConcurrencyException)
        //        {
        //            if (!CommissionExists(commission.CommissionId))
        //            {
        //                return RedirectToAction(nameof(Index));
        //            }
        //            else
        //            {
        //                throw;
        //            }
        //        }
        //        return RedirectToAction(nameof(Index));
        //    }
        //    return View();
        //}


        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddCommissionTracking(string comId)
        {
            try
            {
                var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;

                if (string.IsNullOrEmpty(userId))
                {
                    return Json(new { success = false, errorMessage = "User is not authenticated." });
                }

                // 確認該用戶是否已經追蹤
                var existingTracking = _context.CommissionTrackings.FirstOrDefault(m => m.CommissionId == comId && m.UserId == userId);
             
                if (existingTracking == null)
                {
                    // 如果還沒點過讚，就建立新的 ModLike
                    var newCommissionTracking = new CommissionTracking
                    {
                        CommissionId = comId,
                        UserId = userId,
                        AddTime = DateTime.Now
                    };
                    _context.CommissionTrackings.Add(newCommissionTracking);



                    await _context.SaveChangesAsync();
                    return Json("新增成功");
                }
                else
                {
                    // 如果已經追蹤過，就刪除該筆記錄
                    _context.CommissionTrackings.Remove(existingTracking);

                    await _context.SaveChangesAsync();
                    return Json("刪除成功");
                }
              
            }
            catch (Exception ex)
            {
                // 發生錯誤時，回傳錯誤訊息
                return Json(new { success = false, errorMessage = ex.Message });
            }
        }


        // GET: Commissions/Create
        public IActionResult Create(string gameid)
        {
            ViewData["CommissionStatusId"] = new SelectList(_context.CommissionStatuses, "CommissionStatusId", "Status");
            ViewData["DelegatorId"] = new SelectList(_context.Users, "Email", "Email");
            ViewData["GameName"] = new SelectList(_context.Games, "GameId", "GameName");
            ViewData["AuthorId"] = new SelectList(_context.Users, "Email", "Email");
            ViewData["SelectedGameId"] = gameid;

            return View();
        }

        // POST: Commissions/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(string gameid,[Bind("DelegatorId,CommissionId,GameId,CommissionTitle,CommissionDescription,Budget,Deadline,CommissionStatusId,CreateTime,UpdateTime,IsDone,Trash")] Commission commission)
        {
            //ViewData["CommissionStatusId"] = new SelectList(_context.CommissionStatuses, "CommissionStatusId", "CommissionStatusId", commission.CommissionStatusId);
            //ViewData["DelegatorId"] = new SelectList(_context.Users, "Email", "Email", commission.DelegatorId);
            //ViewData["GameName"] = new SelectList(_context.Games, "GameName", "GameName", commission.GameId);

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

            var counter = await _context.Counters.SingleOrDefaultAsync(c => c.CounterName == "Commission");
            if (counter == null)
            {
                _logger.LogInformation("Counter with name 'Mod' was not found.");
                // Handle the case when there is no counter named 'Commission'
                // For example, create a new counter with name 'Commission' and value 0
            }
            string newCommissionId = $"c{counter.Value + 1:D4}";  // Format as 'c0001'
            counter.Value++;  // Increment counter
            _context.Counters.Update(counter);
            await _context.SaveChangesAsync();
            commission.CommissionId = newCommissionId;

            if (ModelState.IsValid)
            {
                string loggedInUserEmail = User.FindFirstValue(ClaimTypes.Email);
                commission.DelegatorId = loggedInUserEmail;
                commission.CreateTime = DateTime.Now;
                commission.UpdateTime = DateTime.Now;
                //var SelectGameId = from gi in _context.Games
                //             where commission.GameId == gi.GameName
                //             select gi.GameId;
                //string gameId = SelectGameId.FirstOrDefault();

                //commission.GameId = gameId;
                commission.GameId = gameid;
                commission.IsDone = true;
                commission.Trash = false;
                _context.Add(commission);
                await _context.SaveChangesAsync();
                return RedirectToAction("Details", "Games", new {id = gameid});
            }
            else
            {
                return RedirectToAction(nameof(Index));
            }
            
        }

        public async Task<IActionResult> GetAllCommission()
        {
            string loggedInUserEmail = User.FindFirstValue(ClaimTypes.Email);
            var commissions = await _context.Commissions
                .Where(c => c.IsDone == true)
                .Where(c => c.DelegatorId != loggedInUserEmail)
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

            return View(commissions);
        }

        // GET: Commissions/Edit/5
        [Authorize]
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var commission = await _context.Commissions.FindAsync(id);
            if (commission == null)
            {
                return NotFound();
            }
            ViewData["CommissionStatusId"] = new SelectList(_context.CommissionStatuses, "CommissionStatusId", "Status", commission.CommissionStatusId);
            ViewData["DelegatorId"] = new SelectList(_context.Users, "Email", "Email", commission.DelegatorId);
            ViewData["GameName"] = new SelectList(_context.Games, "GameId", "GameName", commission.GameId);
            return View(commission);
        }

        // POST: Commissions/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("CommissionId,DelegatorId,GameId,CommissionTitle,CommissionDescription,Budget,Deadline,CommissionStatusId,CreateTime,UpdateTime,IsDone,Trash")] Commission commission)
        {
            if (id != commission.CommissionId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    
                    commission.UpdateTime = DateTime.Now;
                    _context.Update(commission);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CommissionExists(commission.CommissionId))
                    {
                        return RedirectToAction("PersonPage", "Account");
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("PersonPage", "Account");
            }
            ViewData["CommissionStatusId"] = new SelectList(_context.CommissionStatuses, "CommissionStatusId", "CommissionStatusId", commission.CommissionStatusId);
            ViewData["DelegatorId"] = new SelectList(_context.Users, "Email", "Email", commission.DelegatorId);
            ViewData["GameName"] = new SelectList(_context.Games, "GameName", "GameName", commission.GameId);
            return View(commission);
        }

        // GET: Commissions/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return RedirectToAction(nameof(Index));
            }

            var commission = await _context.Commissions
                .Include(c => c.CommissionStatus)
                .Include(c => c.Delegator)
                .Include(c => c.Game)
                .FirstOrDefaultAsync(m => m.CommissionId == id);
            if (commission == null)
            {
                return RedirectToAction(nameof(Index));
            }

            ViewData["CommissionStatusId"] = new SelectList(_context.CommissionStatuses, "CommissionStatusId", "CommissionStatusId", commission.CommissionStatusId);
            ViewData["DelegatorId"] = new SelectList(_context.Users, "Email", "Email", commission.DelegatorId);
            ViewData["GameName"] = new SelectList(_context.Games, "GameName", "GameName", commission.GameId);

            return View(commission);
        }

        // POST: Commissions/Delete/5
        [HttpPost, ActionName("Delete")]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var commission =  _context.Commissions.FirstOrDefault(c => c.CommissionId == id);
            _context.Commissions.Remove(commission);
            await _context.SaveChangesAsync();
            return RedirectToAction("PersonPage", "Account");
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
    }
}
