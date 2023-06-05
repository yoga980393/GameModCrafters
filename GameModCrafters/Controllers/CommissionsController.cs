using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using GameModCrafters.Data;
using GameModCrafters.Models;
using Microsoft.Extensions.Logging;

namespace GameModCrafters.Controllers
{
    public class CommissionsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger _logger;

        public CommissionsController(ApplicationDbContext context, ILogger<CommissionsController> logger)
        {
            _context = context;
            _logger = logger;
        }


        // GET: Commissions
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Commissions.Include(c => c.CommissionStatus).Include(c => c.Delegator).Include(c => c.Game);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Commissions/Details/5
        //public async Task<IActionResult> Details(string id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var commission = await _context.Commissions
        //        .Include(c => c.CommissionStatus)
        //        .Include(c => c.Delegator)
        //        .Include(c => c.Game)
        //        .FirstOrDefaultAsync(m => m.CommissionId == id);
        //    if (commission == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(commission);
        //}

        // GET: Commissions/Create
        public IActionResult Create()
        {
            ViewData["CommissionStatusId"] = new SelectList(_context.CommissionStatuses, "CommissionStatusId", "CommissionStatusId");
            ViewData["DelegatorId"] = new SelectList(_context.Users, "Email", "Email");
            ViewData["GameId"] = new SelectList(_context.Games, "GameId", "GameId");
            return View();
        }

        // POST: Commissions/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CommissionId,GameId,CommissionTitle,CommissionDescription,Budget,Deadline,CommissionStatusId,CreateTime,UpdateTime,IsDone,Trash")] Commission commission)
        {
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
                var counter = await _context.Counters.FindAsync(1);
                string newCommissionId = $"C{counter.Value + 1:D4}";  // Format as 'C0001'
                counter.Value++;  // Increment counter
                _context.Counters.Update(counter);
                await _context.SaveChangesAsync();
                commission.CommissionId = newCommissionId;
                
                commission.CreateTime = DateTime.Now;
                commission.UpdateTime = DateTime.Now;
                var SelectGameId = from gi in _context.Games
                             where commission.GameId == gi.GameName
                             select gi.GameId;
                string gameId = SelectGameId.FirstOrDefault();

                commission.GameId = gameId;
                
                _context.Add(commission);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CommissionStatusId"] = new SelectList(_context.CommissionStatuses, "CommissionStatusId", "CommissionStatusId", commission.CommissionStatusId);
            ViewData["DelegatorId"] = new SelectList(_context.Users, "Email", "Email", commission.DelegatorId);
            ViewData["GameId"] = new SelectList(_context.Games, "GameId", "GameId", commission.GameId);
            return View(commission);
        }

        // GET: Commissions/Edit/5
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
            ViewData["CommissionStatusId"] = new SelectList(_context.CommissionStatuses, "CommissionStatusId", "CommissionStatusId", commission.CommissionStatusId);
            ViewData["DelegatorId"] = new SelectList(_context.Users, "Email", "Email", commission.DelegatorId);
            ViewData["GameId"] = new SelectList(_context.Games, "GameId", "GameId", commission.GameId);
            return View(commission);
        }

        // POST: Commissions/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
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
                    _context.Update(commission);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CommissionExists(commission.CommissionId))
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
            ViewData["CommissionStatusId"] = new SelectList(_context.CommissionStatuses, "CommissionStatusId", "CommissionStatusId", commission.CommissionStatusId);
            ViewData["DelegatorId"] = new SelectList(_context.Users, "Email", "Email", commission.DelegatorId);
            ViewData["GameId"] = new SelectList(_context.Games, "GameId", "GameId", commission.GameId);
            return View(commission);
        }

        // GET: Commissions/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var commission = await _context.Commissions
                .Include(c => c.CommissionStatus)
                .Include(c => c.Delegator)
                .Include(c => c.Game)
                .FirstOrDefaultAsync(m => m.CommissionId == id);
            if (commission == null)
            {
                return NotFound();
            }

            return View(commission);
        }

        // POST: Commissions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var commission = await _context.Commissions.FindAsync(id);
            _context.Commissions.Remove(commission);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CommissionExists(string id)
        {
            return _context.Commissions.Any(e => e.CommissionId == id);
        }
    }
}
