using System.ComponentModel.DataAnnotations;

namespace OMSys.Models
{
    public class StepResult
    {
        [Key]
        public int ResultId { get; set; }

        public int? StepId { get; set; }
        public string ResultOption { get; set; } = string.Empty;

        public int? NextStepId { get; set; }
        public int? SolutionId { get; set; }

        // Navigation
        public DiagnosisStep? Step { get; set; }
        public Solution? Solution { get; set; }
        public DiagnosisStep? NextStep { get; set; }
    }
}
