using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OMSys.Data;
using OMSys.Models;

namespace OMSys.Controllers
{
    public class TroubleshootingController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TroubleshootingController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index(string? search)
        {
            var dataQuery = _context.Symptoms
                .Include(s => s.Component!)
                    .ThenInclude(c => c.Unit!)
                .Include(s => s.DiagnosisSteps)
                    .ThenInclude(ds => ds.StepResults)
                        .ThenInclude(sr => sr.Solution)
                .AsQueryable();

            // Filtering search
            if (!string.IsNullOrEmpty(search))
            {
                dataQuery = dataQuery.Where(s =>
                    (s.Description != null && s.Description.Contains(search)) ||
                    (s.Component != null && s.Component.Name.Contains(search)) ||
                    (s.Component != null && s.Component.Unit != null && s.Component.Unit.UnitName.Contains(search)) ||
                    (s.Component != null && s.Component.Unit != null && s.Component.Unit.Brand.Contains(search))
                );
            }

            var data = dataQuery.ToList();

            if (data.Count == 0)
            {
                return View(new List<TroubleshootingView>
        {
            new TroubleshootingView
            {
                ComponentId = 0,
                UnitName = "Belum ada data",
                Brand = "-",
                ComponentName = "-",
                SymptomName = "-",
                Steps = new List<StepView>()
            }
        });
            }

            var vmList = data.Select(symptom => new TroubleshootingView
            {
                SymptomId = symptom.SymptomId,
                ComponentId = symptom.ComponentId,
                UnitName = symptom.Component?.Unit?.UnitName ?? "-",
                Brand = symptom.Component?.Unit?.Brand ?? "-",
                ComponentName = symptom.Component?.Name ?? "-",
                SymptomName = symptom.SymptomName ?? "-",
                Steps = symptom.DiagnosisSteps
                    .SelectMany((ds, index) => ds.StepResults.Select(sr => new StepView
                    {
                        StepNumber = index + 1,
                        Instruction = ds.Instruction ?? "-",
                        Result = sr.ResultOption ?? "-",
                        Diagnosis = ds.Diagnosis ?? "-",
                        Solution = sr.Solution?.IndicationAndRepair ?? "-"
                    }))
                    .ToList()
            }).ToList();

            ViewBag.Search = search; // supaya input tetap muncul di view
            return View(vmList);
        }



        public IActionResult Details(int id)
        {
            var symptom = _context.Symptoms
                .Include(s => s.Component!)
                    .ThenInclude(c => c.Unit!)
                .Include(s => s.DiagnosisSteps)
                    .ThenInclude(ds => ds.StepResults)
                        .ThenInclude(sr => sr.Solution)
                .FirstOrDefault(s => s.SymptomId == id);

            if (symptom == null)
                return NotFound();

            var vm = new TroubleshootingView
            {
                ComponentId = symptom.ComponentId,
                UnitName = symptom.Component?.Unit?.UnitName ?? "-",
                Brand = symptom.Component?.Unit?.Brand ?? "-",
                ComponentName = symptom.Component?.Name ?? "-",
                SymptomName = symptom.SymptomName ?? "-",
                Steps = symptom.DiagnosisSteps
                    .SelectMany(ds => ds.StepResults.Select((sr, index) => new StepView
                    {
                        StepNumber = index + 1,
                        Instruction = ds.Instruction ?? "-",
                        Result = sr.ResultOption ?? "-",
                        Diagnosis = ds.Diagnosis ?? "-",
                        Solution = sr.Solution?.IndicationAndRepair ?? "-",
                        ImagePath = ds.ImagePath
                    }))
                    .ToList()
            };

            return View(vm);
        }




    }
}
