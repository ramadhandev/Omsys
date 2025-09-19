namespace OMSys.Models
{
    public class Solution
    {
        public int SolutionId { get; set; }
        public int? SymptomId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? SpareParts { get; set; }

        // Navigation
        public Symptom? Symptom { get; set; }
        public ICollection<StepResult> StepResults { get; set; } = new List<StepResult>();
    }
}
