using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using GameModCrafters.Data;
using GameModCrafters.Models;
using System.Security.Claims;
using Microsoft.VisualStudio.Web.CodeGeneration.Contracts.Messaging;
using GameModCrafters.Migrations;
using Microsoft.AspNetCore.Http;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using GameModCrafters.Services;

namespace GameModCrafters.Controllers
{
    public class TransactionsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;
        private readonly NotificationService _notification;

        public TransactionsController(ApplicationDbContext context, IWebHostEnvironment env, NotificationService notification)
        {
            _context = context;
            _env = env;
            _notification = notification;
        }

        // GET: Transactions
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Transactions
                .Include(t => t.Commission).Include(t => t.Payee).Include(t => t.Payer)
                .Where(c => c.PayeeId == User.FindFirstValue(ClaimTypes.Email) || c.PayerId == User.FindFirstValue(ClaimTypes.Email))
                .OrderByDescending(c => c.CreateTime);

            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Transactions/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var transaction = await _context.Transactions
                .Include(t => t.Commission)
                .Include(t => t.Payee)
                .Include(t => t.Payer)
                .FirstOrDefaultAsync(m => m.TransactionId == id);
            if (transaction == null)
            {
                return NotFound();
            }

            return View(transaction);
        }

        // GET: Transactions/Create
        public async Task<IActionResult> Create()
        {
            var commissions = await _context.Commissions
                .Where(c => c.IsDone)
                .Where(c => c.CommissionStatusId == "s01")
                .Where(c => c.DelegatorId == User.FindFirstValue(ClaimTypes.Email)).ToListAsync();

            ViewData["CommissionId"] = commissions.Select(c => new SelectListItem
            {
                Value = c.CommissionId,
                Text = c.CommissionTitle
            }).ToList();
            ViewData["PayeeId"] = new SelectList(_context.Users.Where(u => u.Email != User.FindFirstValue(ClaimTypes.Email)), "Email", "Email");

            ViewData["PayerId"] = new SelectList(_context.Users, "Email", "Email");
            return View();
        }

        // POST: Transactions/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("TransactionId,CommissionId,PayerId,PayeeId,Describe,TransactionStatus,CreateTime,Budget")] Transaction transaction)
        {
            if (ModelState.IsValid)
            {
                transaction.TransactionId = Guid.NewGuid().ToString();
                transaction.PayerId = User.FindFirstValue(ClaimTypes.Email);
                transaction.TransactionStatus = false;
                transaction.CreateTime = DateTime.Now;
                transaction.IsReceive = false;
                transaction.IsConfirm = false;
                transaction.IsSubmit = false;
                transaction.Isdone = false;

                var privateMessage = new PrivateMessage()
                {
                    MessageId = Guid.NewGuid().ToString(),
                    SenderId = User.FindFirstValue(ClaimTypes.Email),
                    ReceiverId = transaction.PayeeId,
                    MessageContent = $"<div class=\"RequestMessage\">" +
                    $"<p class=\"RequestMessageUserName\"> {User.FindFirstValue(ClaimTypes.Name)} 對你發起了委託申請</p>" +
                    $"<p class=\"RequestMessageCommissionName\"> 委託名稱： {_context.Commissions.FirstOrDefault(c => c.CommissionId == transaction.CommissionId).CommissionTitle}</p>" +
                    $"<p class=\"RequestMessageBudget\">預算： <span>{transaction.Budget}</span> Mod Coin</p>" +
                    $"<a href=\"/Transactions/Details/{transaction.TransactionId}\">點擊前往查看 </a></div>",
                    MessageTime = DateTime.Now,
                    IsRequestMessage = true
                };

                var notifierId = transaction.PayerId;
                var recipientId = transaction.PayeeId;
                var content = $"{User.FindFirstValue(ClaimTypes.Name)}對你發起了委託交易，請到聊天室查看詳情";

                _notification.CreateNotification(notifierId, recipientId, content);

                await _context.AddAsync(transaction);
                await _context.AddAsync(privateMessage);
                await _context.SaveChangesAsync();

                return RedirectToAction("Index", "Home");
            }
            var commissions = await _context.Commissions
                .Where(c => c.IsDone)
                .Where(c => c.DelegatorId == User.FindFirstValue(ClaimTypes.Email)).ToListAsync();

            ViewData["CommissionId"] = commissions.Select(c => new SelectListItem
            {
                Value = c.CommissionId,
                Text = c.CommissionTitle
            }).ToList();
            ViewData["PayeeId"] = new SelectList(_context.Users.Where(u => u.Email != User.FindFirstValue(ClaimTypes.Email)), "Email", "Email");
            ViewData["PayerId"] = new SelectList(_context.Users, "Email", "Email", transaction.PayerId);
            return View(transaction);
        }

        // GET: Transactions/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var transaction = await _context.Transactions.FindAsync(id);
            if (transaction == null)
            {
                return NotFound();
            }
            ViewData["CommissionId"] = new SelectList(_context.Commissions, "CommissionId", "CommissionId", transaction.CommissionId);
            ViewData["PayeeId"] = new SelectList(_context.Users, "Email", "Email", transaction.PayeeId);
            ViewData["PayerId"] = new SelectList(_context.Users, "Email", "Email", transaction.PayerId);
            return View(transaction);
        }

        // POST: Transactions/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("TransactionId,CommissionId,PayerId,PayeeId,Describe,TransactionStatus,CreateTime")] Transaction transaction)
        {
            if (id != transaction.TransactionId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(transaction);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TransactionExists(transaction.TransactionId))
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
            ViewData["CommissionId"] = new SelectList(_context.Commissions, "CommissionId", "CommissionId", transaction.CommissionId);
            ViewData["PayeeId"] = new SelectList(_context.Users, "Email", "Email", transaction.PayeeId);
            ViewData["PayerId"] = new SelectList(_context.Users, "Email", "Email", transaction.PayerId);
            return View(transaction);
        }

        // GET: Transactions/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var transaction = await _context.Transactions
                .Include(t => t.Commission)
                .Include(t => t.Payee)
                .Include(t => t.Payer)
                .FirstOrDefaultAsync(m => m.TransactionId == id);
            if (transaction == null)
            {
                return NotFound();
            }

            return View(transaction);
        }

        // POST: Transactions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var transaction = await _context.Transactions.FindAsync(id);
            _context.Transactions.Remove(transaction);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TransactionExists(string id)
        {
            return _context.Transactions.Any(e => e.TransactionId == id);
        }

        [HttpPost]
        public async Task<IActionResult> AcceptCommission(string transactionId)
        {
            var transaction = await _context.Transactions
                .Include(t => t.Commission)
                .Include(t => t.Payee)
                .Include(t => t.Payer)
                .FirstOrDefaultAsync(m => m.TransactionId == transactionId);
            if (transaction == null)
            {
                return NotFound();
            }

            var notifierId = transaction.PayeeId;
            var recipientId = transaction.PayerId;
            var content = $"{transaction.Payee.Username}接受了你的委託交易，你可以到委託列表查看進度";

            _notification.CreateNotification(notifierId, recipientId, content);

            var commission = await _context.Commissions.FirstOrDefaultAsync(c => c.CommissionId == transaction.CommissionId);
            commission.CommissionStatusId = "s02";

            transaction.TransactionStatus = true;
            await _context.SaveChangesAsync();

            return Json("/Transactions/Index");
        }

        public async Task<IActionResult> SubmitFile(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var transaction = await _context.Transactions
                .Include(t => t.Commission)
                .Include(t => t.Payee)
                .Include(t => t.Payer)
                .FirstOrDefaultAsync(m => m.TransactionId == id);
            if (transaction == null)
            {
                return NotFound();
            }
            return View(transaction);
        }

        public async Task<IActionResult> ViewFinishedCommission(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var transaction = await _context.Transactions
                .Include(t => t.Commission)
                .Include(t => t.Payee)
                .Include(t => t.Payer)
                .FirstOrDefaultAsync(m => m.TransactionId == id);
            if (transaction == null)
            {
                return NotFound();
            }
            return View(transaction);
        }

        [HttpPost]
        public async Task<IActionResult> UploadFile(IFormFile file, string transactionId)
        {
            var transaction = await _context.Transactions
                .Include(t => t.Commission)
                .Include(t => t.Payee)
                .Include(t => t.Payer)
                .FirstOrDefaultAsync(m => m.TransactionId == transactionId);
            if (transaction == null)
            {
                return NotFound();
            }

            // Create a unique file name with a Guid
            var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);

            var filePath = Path.Combine(_env.WebRootPath, "TransactionFile", uniqueFileName);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // Save the relative path including the unique file name in the transaction's FileURL field
            transaction.FileURL = "/TransactionFile/" + uniqueFileName;
            transaction.IsSubmit = true;
            await _context.SaveChangesAsync();

            var notifierId = transaction.PayeeId;
            var recipientId = transaction.PayerId;
            var content = $"{transaction.Payee.Username}已經提交了你的委託({transaction.Commission.CommissionTitle})的成品<br><a href=\"/Transactions/ViewFinishedCommission/{transaction.TransactionId}\">點此</a>驗收成品";

            _notification.CreateNotification(notifierId, recipientId, content);

            return Ok("/Transactions/Index");
        }

        [HttpPost]
        public async Task<IActionResult> DownloadFile(string transactionId)
        {
            var transaction = await _context.Transactions
                .Include(t => t.Commission)
                .Include(t => t.Payee)
                .Include(t => t.Payer)
                .FirstOrDefaultAsync(m => m.TransactionId == transactionId);
            if (transaction == null)
            {
                return NotFound();
            }

            transaction.IsReceive = true;
            await _context.SaveChangesAsync();

            var notifierId = transaction.PayerId;
            var recipientId = transaction.PayeeId;
            var content = $"{transaction.Payer.Username}已經下載了你提交的成品";

            _notification.CreateNotification(notifierId, recipientId, content);

            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> ConfirmTransaction(string transactionId)
        {
            var transaction = _context.Transactions.FirstOrDefault(e => e.TransactionId == transactionId);
            if (transaction == null)
            {
                return NotFound();
            }

            var payer = await _context.Users.FirstOrDefaultAsync(u => u.Email == transaction.PayerId);
            var payee = await _context.Users.FirstOrDefaultAsync(u => u.Email == transaction.PayeeId);

            payer.ModCoin -= transaction.Budget;
            payee.ModCoin += transaction.Budget;

            transaction.IsConfirm = true;
            await _context.SaveChangesAsync();

            var notifierId = transaction.PayerId;
            var recipientId = transaction.PayeeId;
            var content = $"{transaction.Payer.Username}已經驗收了你的作品<br>並將{transaction.Budget}個Mod Coin移交給你";

            _notification.CreateNotification(notifierId, recipientId, content);

            var commission = await _context.Commissions.FirstOrDefaultAsync(c => c.CommissionId == transaction.CommissionId);
            commission.CommissionStatusId = "s03";

            return Ok("/Transactions/Index");
        }

        [HttpPost]
        public async Task<IActionResult> ConfirmedAmount(string transactionId)
        {
            var transaction = await _context.Transactions
                .Include(t => t.Commission)
                .Include(t => t.Payee)
                .Include(t => t.Payer)
                .FirstOrDefaultAsync(m => m.TransactionId == transactionId);
            if (transaction == null)
            {
                return NotFound();
            }

            if(transaction.Payer.ModCoin >= transaction.Budget)
            {
                return Json("餘額充足");
            }
            else
            {
                return Json("餘額不足");
            }
        }
    }
}
