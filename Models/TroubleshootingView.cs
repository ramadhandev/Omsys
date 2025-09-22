namespace OMSys.Models
{
    public class TroubleshootingView
    {
       
        public int SymptomId { get; set; }
        public int ComponentId { get; set; }
        public string UnitName { get; set; } = string.Empty;
        public string Brand { get; set; } = string.Empty;
        public string ComponentName { get; set; } = string.Empty;
        public string SymptomName { get; set; } = string.Empty;
        public List<StepView> Steps { get; set; } = new();
    }

    public class StepView
    {
        public int StepNumber { get; set; }
        public string Instruction { get; set; } = string.Empty;
        public string Result { get; set; } = string.Empty;
        public string Diagnosis { get; set; } = string.Empty;
        public string Solution { get; set; } = string.Empty;
    }


}
