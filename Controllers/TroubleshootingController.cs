using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OMSys.Data;
using OMSys.Models;
using System.Linq;

namespace OMSys.Controllers
{
    public class TroubleshootingController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TroubleshootingController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Index: menampilkan list gejala
        public IActionResult Index()
        {
            var data = _context.Symptoms
                .Include(s => s.Component)
                    .ThenInclude(c => c.Unit)
                .Include(s => s.DiagnosisSteps)
                    .ThenInclude(ds => ds.StepResults)
                        .ThenInclude(sr => sr.Solution) // pastikan Solution dari DB
                .ToList();

            if (!data.Any())
            {
                return View(new List<TroubleshootingView>
        {
            new TroubleshootingView
            {
                ComponentId = 0,
                UnitName = "Belum ada data",
                Brand = "-",
                ComponentName = "-",
                SymptomDescription = "-"
            }
        });
            }

            var vmList = data.Select(symptom => new TroubleshootingView
            {
                ComponentId = symptom.ComponentId,
                UnitName = symptom.Component?.Unit?.UnitName ?? "-",
                Brand = symptom.Component?.Unit?.Brand ?? "-",
                ComponentName = symptom.Component?.Name ?? "-",
                SymptomDescription = symptom.Description ?? "-",
                Steps = symptom.DiagnosisSteps
                    .SelectMany((ds, index) => ds.StepResults.Select(sr => new StepView
                    {
                        StepNumber = index + 1,
                        Instruction = ds.Instruction ?? "-",
                        Result = sr.ResultOption ?? "-",
                        Diagnosis = ds.Diagnosis ?? "-",   // ambil dari database
                        Solution = sr.Solution?.Description ?? "-" // ambil dari master data Solution
                    }))
                    .ToList()
            }).ToList();

            return View(vmList);
        }


        public IActionResult Details(int id)
        {
            var symptom = _context.Symptoms
                .Include(s => s.Component)
                    .ThenInclude(c => c.Unit)
                .Include(s => s.DiagnosisSteps)
                    .ThenInclude(ds => ds.StepResults)
                        .ThenInclude(sr => sr.Solution)
                .FirstOrDefault(s => s.ComponentId == id);

            if (symptom == null)
                return NotFound();

            var vm = new TroubleshootingView
            {
                ComponentId = symptom.ComponentId,
                UnitName = symptom.Component?.Unit?.UnitName ?? "-",
                Brand = symptom.Component?.Unit?.Brand ?? "-",
                ComponentName = symptom.Component?.Name ?? "-",
                SymptomDescription = symptom.Description ?? "-",
                Steps = symptom.DiagnosisSteps
                    .SelectMany(ds => ds.StepResults.Select((sr, index) => new StepView
                    {
                        StepNumber = index + 1,
                        Instruction = ds.Instruction ?? "-",
                        Result = sr.ResultOption ?? "-",
                        Diagnosis = sr.Solution?.Description ?? "-", // ambil dari Description
                        Solution = sr.Solution?.Title ?? "-"        // ambil dari Title
                    }))
                    .ToList()
            };

            return View(vm);
        }




    }
}
