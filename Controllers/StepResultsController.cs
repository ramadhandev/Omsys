using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OMSys.Data;
using OMSys.Models;

namespace OMSys.Controllers
{
    [Authorize(Roles = "Admin")]
    public class StepResultsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public StepResultsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(int? stepId, string search, int page = 1, int pageSize = 10)
        {
            var query = _context.StepResults
                .Include(s => s.Step)
                .Include(s => s.Solution)
                .Include(s => s.NextStep)
                .AsQueryable();

            // Filter by Step
            if (stepId.HasValue)
            {
                query = query.Where(sr => sr.StepId == stepId.Value);
            }

            // Search by keyword
            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(sr =>
                 (sr.ResultOption ?? "").Contains(search) ||
                 (sr.Step != null && (sr.Step.Instruction ?? "").Contains(search)) ||
                 (sr.Solution != null && (sr.Solution.IndicationAndRepair ?? "").Contains(search)));

            }

            // Pagination
            var totalItems = await query.CountAsync();
            var stepResults = await query
                .OrderBy(sr => sr.ResultId)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewBag.StepId = stepId;
            ViewBag.Search = search;
            ViewBag.Page = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalItems = totalItems;
            ViewData["Steps"] = new SelectList(_context.DiagnosisSteps.ToList(), "StepId", "Instruction", stepId);

            return View(stepResults);
        }


        // POST: StepResults/BulkDelete
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> BulkDelete(int[] selectedIds)
        {
            if (selectedIds != null && selectedIds.Length > 0)
            {
                var results = await _context.StepResults
                    .Where(sr => selectedIds.Contains(sr.ResultId))
                    .ToListAsync();

                _context.StepResults.RemoveRange(results);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: StepResults/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var stepResult = await _context.StepResults
                .Include(s => s.NextStep)
                .Include(s => s.Solution)
                .Include(s => s.Step)
                .FirstOrDefaultAsync(m => m.ResultId == id);
            if (stepResult == null)
            {
                return NotFound();
            }

            return View(stepResult);
        }

        // GET: StepResults/Create
        public IActionResult Create()
        {
            ViewData["NextStepId"] = new SelectList(_context.DiagnosisSteps, "StepId", "Instruction");
            ViewData["SolutionId"] = new SelectList(_context.Solutions, "SolutionId", "IndicationAndRepair");
            ViewData["StepId"] = new SelectList(_context.DiagnosisSteps, "StepId", "Instruction");
            return View();
        }

        // POST: StepResults/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ResultId,StepId,ResultOption,NextStepId,SolutionId")] StepResult stepResult)
        {
            if (ModelState.IsValid)
            {
                _context.Add(stepResult);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["NextStepId"] = new SelectList(_context.DiagnosisSteps, "StepId", "Instruction", stepResult.NextStepId);
            ViewData["SolutionId"] = new SelectList(_context.Solutions, "SolutionId", "IndicationAndRepair", stepResult.SolutionId);
            ViewData["StepId"] = new SelectList(_context.DiagnosisSteps, "StepId", "Instruction", stepResult.StepId);
            return View(stepResult);
        }

        // GET: StepResults/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var stepResult = await _context.StepResults.FindAsync(id);
            if (stepResult == null)
            {
                return NotFound();
            }
            ViewData["NextStepId"] = new SelectList(_context.DiagnosisSteps, "StepId", "Instruction", stepResult.NextStepId);
            ViewData["SolutionId"] = new SelectList(_context.Solutions, "SolutionId", "IndicationAndRepair", stepResult.SolutionId);
            ViewData["StepId"] = new SelectList(_context.DiagnosisSteps, "StepId", "Instruction", stepResult.StepId);
            return View(stepResult);
        }

        // POST: StepResults/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ResultId,StepId,ResultOption,NextStepId,SolutionId")] StepResult stepResult)
        {
            if (id != stepResult.ResultId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(stepResult);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!StepResultExists(stepResult.ResultId))
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
            ViewData["NextStepId"] = new SelectList(_context.DiagnosisSteps, "StepId", "Instruction", stepResult.NextStepId);
            ViewData["SolutionId"] = new SelectList(_context.Solutions, "SolutionId", "IndicationAndRepair", stepResult.SolutionId);
            ViewData["StepId"] = new SelectList(_context.DiagnosisSteps, "StepId", "Instruction", stepResult.StepId);
            return View(stepResult);
        }

        // GET: StepResults/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var stepResult = await _context.StepResults
                .Include(s => s.NextStep)
                .Include(s => s.Solution)
                .Include(s => s.Step)
                .FirstOrDefaultAsync(m => m.ResultId == id);
            if (stepResult == null)
            {
                return NotFound();
            }

            return View(stepResult);
        }

        // POST: StepResults/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var stepResult = await _context.StepResults.FindAsync(id);
            if (stepResult != null)
            {
                _context.StepResults.Remove(stepResult);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool StepResultExists(int id)
        {
            return _context.StepResults.Any(e => e.ResultId == id);
        }
    }
}
