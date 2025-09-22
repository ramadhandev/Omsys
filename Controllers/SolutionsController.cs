using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OMSys.Data;
using OMSys.Models;

namespace OMSys.Controllers
{
    [Authorize(Roles = "Admin")]
    public class SolutionsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SolutionsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Solutions
        public async Task<IActionResult> Index(int? symptomId, int page = 1, int pageSize = 10)
        {
            var query = _context.Solutions
                .Include(s => s.Symptom)
                .AsQueryable();

            // Filtering berdasarkan SymptomId
            if (symptomId.HasValue && symptomId.Value > 0)
            {
                query = query.Where(s => s.SymptomId == symptomId.Value);
            }

            // Pagination
            var totalItems = await query.CountAsync();
            var solutions = await query
                .OrderBy(s => s.SolutionId)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            // Dropdown data
            ViewData["Symptoms"] = new SelectList(_context.Symptoms.OrderBy(s => s.SymptomName), "SymptomId", "SymptomName", symptomId);

            ViewData["SymptomId"] = symptomId;
            ViewData["Page"] = page;
            ViewData["PageSize"] = pageSize;
            ViewData["TotalItems"] = totalItems;

            return View(solutions);
        }


        // POST: Solutions/BulkDelete
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> BulkDelete(int[] selectedIds)
        {
            if (selectedIds != null && selectedIds.Length > 0)
            {
                var solutions = await _context.Solutions
                    .Where(s => selectedIds.Contains(s.SolutionId))
                    .ToListAsync();

                _context.Solutions.RemoveRange(solutions);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }


        // GET: Solutions/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var solution = await _context.Solutions
                .Include(s => s.Symptom)
                .FirstOrDefaultAsync(m => m.SolutionId == id);
            if (solution == null)
            {
                return NotFound();
            }

            return View(solution);
        }

        // GET: Solutions/Create
        public IActionResult Create()
        {
            ViewData["SymptomId"] = new SelectList(_context.Symptoms, "SymptomId", "SymptomName");
            return View();
        }

        // POST: Solutions/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("SolutionId,SymptomId,IndicationAndRepair")] Solution solution)
        {
            if (ModelState.IsValid)
            {
                _context.Add(solution);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["SymptomId"] = new SelectList(_context.Symptoms, "SymptomId", "SymptomName", solution.SymptomId);
            return View(solution);
        }

        // GET: Solutions/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var solution = await _context.Solutions.FindAsync(id);
            if (solution == null)
            {
                return NotFound();
            }
            ViewData["SymptomId"] = new SelectList(_context.Symptoms, "SymptomId", "SymptomName", solution.SymptomId);
            return View(solution);
        }

        // POST: Solutions/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("SolutionId,SymptomId,IndicationAndRepair")] Solution solution)
        {
            if (id != solution.SolutionId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(solution);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SolutionExists(solution.SolutionId))
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
            ViewData["SymptomId"] = new SelectList(_context.Symptoms, "SymptomId", "SymptomName", solution.SymptomId);
            return View(solution);
        }
        // GET: Solutions/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var solution = await _context.Solutions
                .Include(s => s.Symptom)
                .FirstOrDefaultAsync(m => m.SolutionId == id);

            if (solution == null)
            {
                return NotFound();
            }

            // Check if solution is referenced in step results
            var hasStepResults = await _context.StepResults.AnyAsync(sr => sr.SolutionId == id);
            ViewBag.HasReferences = hasStepResults;

            return View(solution);
        }

        // POST: Solutions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var solution = await _context.Solutions.FindAsync(id);
                if (solution != null)
                {
                    // Check if solution is referenced in step results
                    var hasStepResults = await _context.StepResults.AnyAsync(sr => sr.SolutionId == id);

                    if (hasStepResults)
                    {
                        TempData["ErrorMessage"] = "Cannot delete this solution because it is referenced in step results. Please update the step results first.";
                        return RedirectToAction(nameof(Delete), new { id });
                    }

                    _context.Solutions.Remove(solution);
                    await _context.SaveChangesAsync();
                }

                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException ex)
            {
                TempData["ErrorMessage"] = "Error deleting solution: " + ex.InnerException?.Message;
                return RedirectToAction(nameof(Delete), new { id });
            }
        }

        private bool SolutionExists(int id)
        {
            return _context.Solutions.Any(e => e.SolutionId == id);
        }
    }
}
