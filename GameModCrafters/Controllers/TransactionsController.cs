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

namespace GameModCrafters.Controllers
{
    public class TransactionsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;

        public TransactionsController(ApplicationDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        // GET: Transactions
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Transactions
                .Include(t => t.Commission).Include(t => t.Payee).Include(t => t.Payer)
                .Where(c => c.PayeeId == User.FindFirstValue(ClaimTypes.Email) || c.PayerId == User.FindFirstValue(ClaimTypes.Email));

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
                .Where(c => c.DelegatorId == User.FindFirstValue(ClaimTypes.Email)).ToListAsync();

            ViewData["CommissionId"] = commissions.Select(c => new SelectListItem
            {
                Value = c.CommissionId,
                Text = c.CommissionTitle
            }).ToList();
            ViewData["PayeeId"] = new SelectList(_context.Users, "Email", "Email");
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
            ViewData["PayeeId"] = new SelectList(_context.Users, "Email", "Email", transaction.PayeeId);
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
            var transaction = _context.Transactions.FirstOrDefault(e => e.TransactionId == transactionId);
            if (transaction == null)
            {
                return NotFound();
            }

            // TODO: 驗證交易確實屬於當前使用者

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

            var transaction = await _context.Transactions.FindAsync(id);
            if (transaction == null)
            {
                return NotFound();
            }
            return View(transaction);
        }

        [HttpPost]
        public async Task<IActionResult> UploadFile(IFormFile file, string transactionId)
        {
            var transaction = _context.Transactions.FirstOrDefault(e => e.TransactionId == transactionId);
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

            return Ok("/Transactions/Index");
        }

    }
}
