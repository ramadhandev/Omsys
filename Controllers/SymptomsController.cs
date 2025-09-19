using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OMSys.Data;
using OMSys.Models;

namespace OMSys.Controllers
{
    [Authorize(Roles = "Admin")]
    public class SymptomsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SymptomsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Symptoms
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Symptoms.Include(s => s.Component);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Symptoms/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var symptom = await _context.Symptoms
                .Include(s => s.Component)
                .FirstOrDefaultAsync(m => m.SymptomId == id);
            if (symptom == null)
            {
                return NotFound();
            }

            return View(symptom);
        }

        // GET: Symptoms/Create
        public IActionResult Create()
        {
            ViewData["ComponentId"] = new SelectList(_context.Components, "ComponentId", "Name");
            return View();
        }

        // POST: Symptoms/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("SymptomId,ComponentId,SymptomCode,SymptomName,Description")] Symptom symptom)
        {
            if (ModelState.IsValid)
            {
                _context.Add(symptom);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ComponentId"] = new SelectList(_context.Components, "ComponentId", "ComponentId", symptom.ComponentId);
            return View(symptom);
        }

        // GET: Symptoms/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var symptom = await _context.Symptoms.FindAsync(id);
            if (symptom == null)
            {
                return NotFound();
            }
            ViewData["ComponentId"] = new SelectList(_context.Components, "ComponentId", "Name", symptom.ComponentId);
            return View(symptom);
        }

        // POST: Symptoms/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("SymptomId,ComponentId,SymptomCode,SymptomName,Description")] Symptom symptom)
        {
            if (id != symptom.SymptomId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(symptom);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SymptomExists(symptom.SymptomId))
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
            ViewData["ComponentId"] = new SelectList(_context.Components, "ComponentId", "ComponentId", symptom.ComponentId);
            return View(symptom);
        }

        // GET: Symptoms/Delete/5 - TAMBAHKAN INI
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var symptom = await _context.Symptoms
                .Include(s => s.Component)
                .FirstOrDefaultAsync(m => m.SymptomId == id);

            if (symptom == null)
            {
                return NotFound();
            }

            // CHECK REFERENCES - INI YANG DITAMBAH
            var hasDiagnosisSteps = await _context.DiagnosisSteps.AnyAsync(ds => ds.SymptomId == id);
            var hasSolutions = await _context.Solutions.AnyAsync(s => s.SymptomId == id);

            ViewBag.HasReferences = hasDiagnosisSteps || hasSolutions;

            return View(symptom);
        }

        // POST: Symptoms/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var symptom = await _context.Symptoms.FindAsync(id);
                if (symptom != null)
                {
                    // 1. First, set all diagnosis steps' SymptomId to null
                    var diagnosisSteps = await _context.DiagnosisSteps
                        .Where(ds => ds.SymptomId == id)
                        .ToListAsync();

                    foreach (var step in diagnosisSteps)
                    {
                        step.SymptomId = null;
                    }

                    // 2. Set all solutions' SymptomId to null
                    var solutions = await _context.Solutions
                        .Where(s => s.SymptomId == id)
                        .ToListAsync();

                    foreach (var solution in solutions)
                    {
                        solution.SymptomId = null;
                    }

                    // 3. Then delete the symptom
                    _context.Symptoms.Remove(symptom);
                    await _context.SaveChangesAsync();
                }

                await transaction.CommitAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                TempData["ErrorMessage"] = $"Error deleting symptom: {ex.Message}";
                return RedirectToAction(nameof(Delete), new { id });
            }
        }

        private bool SymptomExists(int id)
        {
            return _context.Symptoms.Any(e => e.SymptomId == id);
        }
    }
}
