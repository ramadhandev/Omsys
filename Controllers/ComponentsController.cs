using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OMSys.Data;
using OMSys.Models;

namespace OMSys.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ComponentsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ComponentsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Components (dengan search & pagination)
        public async Task<IActionResult> Index(int? unitId, int pageNumber = 1)
        {
            int pageSize = 10;

            var query = _context.Components
                .Include(c => c.Unit)
                .AsQueryable();

            // Filter by unit
            if (unitId.HasValue && unitId > 0)
            {
                query = query.Where(c => c.UnitId == unitId);
            }

            int totalItems = await query.CountAsync();
            var components = await query
                .OrderBy(c => c.Name)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            // Get units for filter dropdown
            ViewBag.Units = new SelectList(_context.Units, "UnitId", "UnitName", unitId);

            ViewBag.SelectedUnitId = unitId;
            ViewBag.CurrentPage = pageNumber;
            ViewBag.TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            return View(components);
        }

        // POST: Components/DeleteSelected
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteSelected(int[] selectedIds)
        {
            if (selectedIds != null && selectedIds.Length > 0)
            {
                // Cek apakah masih ada Symptom terkait
                var hasSymptoms = await _context.Symptoms
                    .AnyAsync(s => selectedIds.Contains(s.ComponentId));

                if (hasSymptoms)
                {
                    TempData["ErrorMessage"] = "Some components cannot be deleted because they have associated symptoms.";
                    return RedirectToAction(nameof(Index));
                }

                var items = await _context.Components
                    .Where(c => selectedIds.Contains(c.ComponentId))
                    .ToListAsync();

                _context.Components.RemoveRange(items);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }


        // GET: Components/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var component = await _context.Components
                .Include(c => c.Unit)
                .FirstOrDefaultAsync(m => m.ComponentId == id);
            if (component == null)
            {
                return NotFound();
            }

            return View(component);
        }

        // GET: Components/Create
        public IActionResult Create()
        {
            ViewData["UnitId"] = new SelectList(_context.Units, "UnitId", "UnitName");
            return View();
        }

        // POST: Components/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ComponentId,UnitId,Name,Description")] Component component)
        {
            if (ModelState.IsValid)
            {
                _context.Add(component);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["UnitId"] = new SelectList(_context.Units, "UnitId", "UnitId", component.UnitId);
            return View(component);
        }

        // GET: Components/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var component = await _context.Components.FindAsync(id);
            if (component == null)
            {
                return NotFound();
            }
            ViewData["UnitId"] = new SelectList(_context.Units, "UnitId", "UnitName", component.UnitId);
            return View(component);
        }

        // POST: Components/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ComponentId,UnitId,Name,Description")] Component component)
        {
            if (id != component.ComponentId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(component);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ComponentExists(component.ComponentId))
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
            ViewData["UnitId"] = new SelectList(_context.Units, "UnitId", "UnitId", component.UnitId);
            return View(component);
        }

        // GET: Components/Delete/5 - TAMBAHKAN INI
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var component = await _context.Components
                .Include(c => c.Unit)
                .FirstOrDefaultAsync(m => m.ComponentId == id);

            if (component == null)
            {
                return NotFound();
            }

            // CHECK REFERENCE - INI YANG DITAMBAH
            var hasSymptoms = await _context.Symptoms.AnyAsync(s => s.ComponentId == id);
            ViewBag.HasSymptoms = hasSymptoms;

            return View(component);
        }

        // POST: Components/Delete/5 - UBAH INI
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var component = await _context.Components.FindAsync(id);
                if (component != null)
                {
                    // CHECK REFERENCE - INI YANG DITAMBAH
                    var hasSymptoms = await _context.Symptoms.AnyAsync(s => s.ComponentId == id);

                    if (hasSymptoms)
                    {
                        TempData["ErrorMessage"] = "Cannot delete this component because it has associated symptoms. Please delete the symptoms first.";
                        return RedirectToAction(nameof(Delete), new { id });
                    }

                    _context.Components.Remove(component);
                    await _context.SaveChangesAsync();
                }

                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException ex)
            {
                TempData["ErrorMessage"] = "Error deleting component: " + ex.InnerException?.Message;
                return RedirectToAction(nameof(Delete), new { id });
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "An unexpected error occurred.";
               
                Console.WriteLine(ex);
                return RedirectToAction(nameof(Delete), new { id });
            }

        }

        private bool ComponentExists(int id)
        {
            return _context.Components.Any(e => e.ComponentId == id);
        }
    }
}
