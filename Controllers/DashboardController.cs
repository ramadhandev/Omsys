using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OMSys.Data;
using OMSys.Models;
using System.Diagnostics;
using System.Linq;

namespace OMSys.Controllers
{
    [Authorize]
    public class DashboardController(ILogger<DashboardController> logger, ApplicationDbContext context) : Controller
    {
        private readonly ILogger<DashboardController> _logger = logger;
        private readonly ApplicationDbContext _context = context;

        public IActionResult Index()
        {
            // Hitung total data
            var totalUnit = _context.Units.Count();
            var totalComponent = _context.Components.Count();
            var totalSymptom = _context.Symptoms.Count();
            var totalSolution = _context.Solutions.Count();

            // Statistik gejala per komponen
            var symptomStats = _context.Symptoms
                .Include(s => s.Component)
                .Where(s => s.Component != null)
                .GroupBy(s => s.Component!.Name)
                .Select(g => new { ComponentName = g.Key, Count = g.Count() })
                .ToList();

            ViewBag.ChartLabels = symptomStats.Select(s => s.ComponentName).ToList();
            ViewBag.ChartData = symptomStats.Select(s => s.Count).ToList();

            // Top 5 gejala terbaru
            var topSymptoms = _context.Symptoms
                .OrderByDescending(s => s.SymptomId)
                .Take(5)
                .Select(s => s.Description)
                .ToList();

            // lempar ke View
            ViewBag.TotalUnit = totalUnit;
            ViewBag.TotalComponent = totalComponent;
            ViewBag.TotalSymptom = totalSymptom;
            ViewBag.TotalSolution = totalSolution;
            ViewBag.TopSymptoms = topSymptoms;

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}