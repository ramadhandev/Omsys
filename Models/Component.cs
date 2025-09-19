namespace OMSys.Models
{
    public class Component
    {
        public int ComponentId { get; set; }
        public int? UnitId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }

        // Navigation
        public Unit? Unit { get; set; }
        public ICollection<Symptom> Symptoms { get; set; } = new List<Symptom>();
    }
}
