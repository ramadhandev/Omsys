
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OMSys.Data;
using OMSys.Models;

namespace OMSys.Controllers
{
    [Authorize(Roles = "Admin")]
    public class DiagnosisStepsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DiagnosisStepsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: DiagnosisSteps
        public async Task<IActionResult> Index(string search, string symptomName, int page = 1, int pageSize = 10)
        {
            var query = _context.DiagnosisSteps
                .Include(d => d.Symptom)
                .AsQueryable();

            // Filtering (search)
            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(d =>
                d.Instruction.Contains(search) ||
                (d.Symptom != null && d.Symptom.SymptomName.Contains(search)));

            }

            // Filter by symptom name
            if (!string.IsNullOrEmpty(symptomName))
            {
                query = query.Where(d =>
                    d.Symptom != null &&
                    d.Symptom.SymptomName.Contains(symptomName));
            }


            // Get distinct symptom names for dropdown
            var symptomNames = await _context.Symptoms
                .OrderBy(s => s.SymptomName)
                .Select(s => s.SymptomName)
                .Distinct()
                .ToListAsync();

            ViewBag.SymptomNames = new SelectList(symptomNames, symptomName);

            // Pagination
            var totalItems = await query.CountAsync();
            var diagnosisSteps = await query
                .OrderBy(d => d.StepOrder)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewBag.Search = search;
            ViewBag.SymptomName = symptomName;
            ViewBag.Page = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalItems = totalItems;

            return View(diagnosisSteps);
        }

        // POST: DiagnosisSteps/BulkDelete
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> BulkDelete(int[] selectedIds)
        {
            if (selectedIds != null && selectedIds.Length > 0)
            {
                var steps = await _context.DiagnosisSteps
                    .Where(d => selectedIds.Contains(d.StepId))
                    .ToListAsync();

                _context.DiagnosisSteps.RemoveRange(steps);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }


        // GET: DiagnosisSteps/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var diagnosisStep = await _context.DiagnosisSteps
                .Include(d => d.Symptom)
                .FirstOrDefaultAsync(m => m.StepId == id);
            if (diagnosisStep == null)
            {
                return NotFound();
            }

            return View(diagnosisStep);
        }

        // GET: DiagnosisSteps/Create
        public IActionResult Create()
        {
            ViewData["SymptomId"] = new SelectList(_context.Symptoms, "SymptomId", "SymptomName");
            return View();
        }

        // POST: DiagnosisSteps/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("StepId,SymptomId,StepOrder,Instruction,Diagnosis")] DiagnosisStep diagnosisStep)
        {
            if (ModelState.IsValid)
            {
                _context.Add(diagnosisStep);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["SymptomId"] = new SelectList(_context.Symptoms, "SymptomId", "SymptomName", diagnosisStep.SymptomId);
            return View(diagnosisStep);
        }

        // GET: DiagnosisSteps/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var diagnosisStep = await _context.DiagnosisSteps.FindAsync(id);
            if (diagnosisStep == null)
            {
                return NotFound();
            }
            ViewData["SymptomId"] = new SelectList(_context.Symptoms, "SymptomId", "SymptomName", diagnosisStep.SymptomId);
            return View(diagnosisStep);
        }

        // POST: DiagnosisSteps/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("StepId,SymptomId,StepOrder,Instruction,Diagnosis")] DiagnosisStep diagnosisStep)
        {
            if (id != diagnosisStep.StepId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(diagnosisStep);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DiagnosisStepExists(diagnosisStep.StepId))
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
            ViewData["SymptomId"] = new SelectList(_context.Symptoms, "SymptomId", "SymptomName", diagnosisStep.SymptomId);
            return View(diagnosisStep);
        }

        // GET: DiagnosisSteps/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var diagnosisStep = await _context.DiagnosisSteps
                .Include(d => d.Symptom)
                .FirstOrDefaultAsync(m => m.StepId == id);

            if (diagnosisStep == null)
            {
                return NotFound();
            }

            // Check if step has step results
            var hasStepResults = await _context.StepResults.AnyAsync(sr => sr.StepId == id);
            ViewBag.HasStepResults = hasStepResults;

            return View(diagnosisStep);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var diagnosisStep = await _context.DiagnosisSteps.FindAsync(id);
            if (diagnosisStep != null)
            {
                _context.DiagnosisSteps.Remove(diagnosisStep);
                await _context.SaveChangesAsync();
            }

            // Perbaiki redirect agar tidak ke Delete/Index
            return RedirectToAction(nameof(Index));
        }


        private bool DiagnosisStepExists(int id)
        {
            return _context.DiagnosisSteps.Any(e => e.StepId == id);
        }
    }
}
