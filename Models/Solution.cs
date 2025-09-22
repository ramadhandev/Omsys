using System.ComponentModel.DataAnnotations;

namespace OMSys.Models
{
    public class Solution
    {
        public int SolutionId { get; set; }
        public int? SymptomId { get; set; }
        [Display(Name = "Indication & Repair")]
        public string IndicationAndRepair { get; set; } = string.Empty;

        // Navigation
        public Symptom? Symptom { get; set; }
        public ICollection<StepResult> StepResults { get; set; } = new List<StepResult>();
    }
}
