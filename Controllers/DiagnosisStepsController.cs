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
        private readonly IWebHostEnvironment _webHostEnvironment;

        public DiagnosisStepsController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment ?? throw new ArgumentNullException(nameof(webHostEnvironment));
        }

        // GET: DiagnosisSteps
        public async Task<IActionResult> Index(string search, string symptomName, int page = 1, int pageSize = 10)
        {
            var query = _context.DiagnosisSteps
                .Include(d => d.Symptom)
                .AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(d =>
                    d.Instruction.Contains(search) ||
                    (d.Symptom != null && d.Symptom.SymptomName.Contains(search)));
            }

            if (!string.IsNullOrEmpty(symptomName))
            {
                query = query.Where(d =>
                    d.Symptom != null &&
                    d.Symptom.SymptomName.Contains(symptomName));
            }

            var symptomNames = await _context.Symptoms
                .OrderBy(s => s.SymptomName)
                .Select(s => s.SymptomName)
                .Distinct()
                .ToListAsync();

            ViewBag.SymptomNames = new SelectList(symptomNames, symptomName);

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
            if (selectedIds?.Length > 0)
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
            if (id == null) return NotFound();

            var diagnosisStep = await _context.DiagnosisSteps
                .Include(d => d.Symptom)
                .FirstOrDefaultAsync(m => m.StepId == id);

            if (diagnosisStep == null) return NotFound();

            return View(diagnosisStep);
        }

        // GET: DiagnosisSteps/Create
        public IActionResult Create()
        {
            ViewData["SymptomId"] = new SelectList(_context.Symptoms, "SymptomId", "SymptomName");
            return View();
        }

        // POST: DiagnosisSteps/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("StepId,SymptomId,StepOrder,Instruction,Diagnosis")] DiagnosisStep diagnosisStep,
            IFormFile? ImageFile)
        {
            if (ModelState.IsValid)
            {
                if (ImageFile != null && ImageFile.Length > 0)
                {
                    var uploadDir = Path.Combine(_webHostEnvironment.WebRootPath, "uploads");
                    if (!Directory.Exists(uploadDir))
                        Directory.CreateDirectory(uploadDir);

                    var fileName = Guid.NewGuid() + Path.GetExtension(ImageFile.FileName);
                    var filePath = Path.Combine(uploadDir, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await ImageFile.CopyToAsync(stream);
                    }

                    diagnosisStep.ImagePath = "/uploads/" + fileName;
                }

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
            if (id == null) return NotFound();

            var diagnosisStep = await _context.DiagnosisSteps.FindAsync(id);
            if (diagnosisStep == null) return NotFound();

            ViewData["SymptomId"] = new SelectList(_context.Symptoms, "SymptomId", "SymptomName", diagnosisStep.SymptomId);
            return View(diagnosisStep);
        }

        // POST: DiagnosisSteps/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("StepId,SymptomId,StepOrder,Instruction,Diagnosis,ImagePath")] DiagnosisStep diagnosisStep, IFormFile? ImageFile)
        {
            if (id != diagnosisStep.StepId) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    if (ImageFile != null && ImageFile.Length > 0)
                    {
                        var uploadDir = Path.Combine(_webHostEnvironment.WebRootPath, "uploads");
                        if (!Directory.Exists(uploadDir))
                            Directory.CreateDirectory(uploadDir);

                        var fileName = Guid.NewGuid() + Path.GetExtension(ImageFile.FileName);
                        var filePath = Path.Combine(uploadDir, fileName);

                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await ImageFile.CopyToAsync(stream);
                        }

                        // Hapus file lama
                        if (!string.IsNullOrEmpty(diagnosisStep.ImagePath))
                        {
                            var oldFilePath = Path.Combine(_webHostEnvironment.WebRootPath, diagnosisStep.ImagePath.TrimStart('/'));
                            if (System.IO.File.Exists(oldFilePath))
                                System.IO.File.Delete(oldFilePath);
                        }

                        diagnosisStep.ImagePath = "/uploads/" + fileName;
                    }

                    _context.Update(diagnosisStep);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DiagnosisStepExists(diagnosisStep.StepId))
                        return NotFound();
                    else
                        throw;
                }

                return RedirectToAction(nameof(Index));
            }

            ViewData["SymptomId"] = new SelectList(_context.Symptoms, "SymptomId", "SymptomName", diagnosisStep.SymptomId);
            return View(diagnosisStep);
        }

        // GET: DiagnosisSteps/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var diagnosisStep = await _context.DiagnosisSteps
                .Include(d => d.Symptom)
                .FirstOrDefaultAsync(m => m.StepId == id);

            if (diagnosisStep == null) return NotFound();

            var hasStepResults = await _context.StepResults.AnyAsync(sr => sr.StepId == id);
            ViewBag.HasStepResults = hasStepResults;

            return View(diagnosisStep);
        }

        // POST: DiagnosisSteps/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var diagnosisStep = await _context.DiagnosisSteps.FindAsync(id);
            if (diagnosisStep != null)
            {
                // hapus file gambar juga
                if (!string.IsNullOrEmpty(diagnosisStep.ImagePath))
                {
                    var oldFilePath = Path.Combine(_webHostEnvironment.WebRootPath, diagnosisStep.ImagePath.TrimStart('/'));
                    if (System.IO.File.Exists(oldFilePath))
                        System.IO.File.Delete(oldFilePath);
                }

                _context.DiagnosisSteps.Remove(diagnosisStep);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private bool DiagnosisStepExists(int id)
        {
            return _context.DiagnosisSteps.Any(e => e.StepId == id);
        }
    }
}