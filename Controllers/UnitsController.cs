
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OMSys.Data;
using OMSys.Models;

namespace OMSys.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UnitsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public UnitsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Units
        public async Task<IActionResult> Index(string search, string brandFilter, string typeFilter, int page = 1, int pageSize = 10)
        {
            // Ambil query awal
            var query = _context.Units.AsQueryable();

            // 1. Search text filter
            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(u =>
                    (u.UnitName != null && u.UnitName.Contains(search)) ||
                    (u.Brand != null && u.Brand.Contains(search)) ||
                    (u.Type != null && u.Type.Contains(search))
                );
            }

            // 2. Filter by Brand
            if (!string.IsNullOrEmpty(brandFilter))
            {
                query = query.Where(u => u.Brand == brandFilter);
            }

            // 3. Filter by Type
            if (!string.IsNullOrEmpty(typeFilter))
            {
                query = query.Where(u => u.Type == typeFilter);
            }

            // 4. Hitung total item untuk pagination
            var totalItems = await query.CountAsync();

            // 5. Ambil data dengan pagination
            var units = await query
                .OrderBy(u => u.UnitName)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            // 6. ViewBag untuk Razor
            ViewBag.Search = search;
            ViewBag.BrandFilter = brandFilter;
            ViewBag.TypeFilter = typeFilter;
            ViewBag.Page = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalItems = totalItems;
            ViewBag.TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            ViewBag.BrandList = await _context.Units
                .Select(u => u.Brand)
                .Where(b => b != null)          // hanya ambil yang bukan null
                .Distinct()
                .ToListAsync();

            ViewBag.TypeList = await _context.Units
                .Select(u => u.Type)
                .Where(t => t != null)          // hanya ambil yang bukan null
                .Distinct()
                .ToListAsync();

            return View(units);
        }

        // POST: Units/BulkDelete
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> BulkDelete(int[] selectedIds)
        {
            if (selectedIds != null && selectedIds.Length > 0)
            {
                using var transaction = await _context.Database.BeginTransactionAsync();
                try
                {
                    var units = await _context.Units
                        .Where(u => selectedIds.Contains(u.UnitId))
                        .ToListAsync();

                    foreach (var unit in units)
                    {
                        // Set UnitId to null for all related components
                        var components = await _context.Components
                            .Where(c => c.UnitId == unit.UnitId)
                            .ToListAsync();

                        foreach (var component in components)
                        {
                            component.UnitId = null;
                        }
                    }

                    _context.Units.RemoveRange(units);
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    TempData["ErrorMessage"] = $"Error deleting units: {ex.Message}";
                }
            }

            return RedirectToAction(nameof(Index));
        }


        // GET: Units/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var unit = await _context.Units
                .FirstOrDefaultAsync(m => m.UnitId == id);
            if (unit == null)
            {
                return NotFound();
            }

            return View(unit);
        }

        // GET: Units/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Units/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("UnitId,UnitName,Brand,Type")] Unit unit)
        {
            if (ModelState.IsValid)
            {
                _context.Add(unit);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(unit);
        }

        // GET: Units/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var unit = await _context.Units.FindAsync(id);
            if (unit == null)
            {
                return NotFound();
            }
            return View(unit);
        }

        // POST: Units/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("UnitId,UnitName,Brand,Type")] Unit unit)
        {
            if (id != unit.UnitId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(unit);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UnitExists(unit.UnitId))
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
            return View(unit);
        }

        // GET: Units/Delete/5 - TAMBAHKAN INI
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var unit = await _context.Units
                .FirstOrDefaultAsync(m => m.UnitId == id);

            if (unit == null)
            {
                return NotFound();
            }

            // CHECK REFERENCE - INI YANG DITAMBAH
            var hasComponents = await _context.Components.AnyAsync(c => c.UnitId == id);
            ViewBag.HasComponents = hasComponents;

            return View(unit);
        }


        // POST: Units/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var unit = await _context.Units.FindAsync(id);
                if (unit != null)
                {
                    // Set UnitId to null for all components
                    var components = await _context.Components
                        .Where(c => c.UnitId == id)
                        .ToListAsync();

                    foreach (var component in components)
                    {
                        component.UnitId = null; // Ini sekarang bisa karena UnitId nullable
                    }

                    _context.Units.Remove(unit);
                    await _context.SaveChangesAsync();
                }

                await transaction.CommitAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                TempData["ErrorMessage"] = $"Error deleting unit: {ex.Message}";
                return RedirectToAction(nameof(Delete), new { id });
            }
        }
        private bool UnitExists(int id)
        {
            return _context.Units.Any(e => e.UnitId == id);
        }
    }
}
