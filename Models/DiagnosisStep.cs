using System.ComponentModel.DataAnnotations;

namespace OMSys.Models
{
    public class DiagnosisStep
    {
        [Key] // 👈 tambahkan attribute Key
        public int StepId { get; set; }

        public int? SymptomId { get; set; }
        public int StepOrder { get; set; }

        [Required]
        public string Instruction { get; set; } = string.Empty;

        [Required]
        public string Diagnosis { get; set; } = string.Empty;
        public string? ImagePath { get; set; }
        public Symptom? Symptom { get; set; }
        public ICollection<StepResult> StepResults { get; set; } = new List<StepResult>();
    }
}
