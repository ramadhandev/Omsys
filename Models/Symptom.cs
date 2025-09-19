namespace OMSys.Models
{
    public class Symptom
    {
        public int SymptomId { get; set; }
        public int ComponentId { get; set; }
        public string SymptomCode { get; set; } = string.Empty;
        public string SymptomName { get; set; } = string.Empty;
        public string? Description { get; set; }

        // Navigation
        public Component? Component { get; set; }
        public ICollection<DiagnosisStep> DiagnosisSteps { get; set; } = new List<DiagnosisStep>();
        public ICollection<Solution> Solutions { get; set; } = new List<Solution>();
    }
}
