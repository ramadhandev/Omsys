using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace OMSys.Models
{
    public class Unit
    {
        public int UnitId { get; set; }
        public string UnitName { get; set; } = string.Empty;
        public string Brand { get; set; } = string.Empty;
        public string? Type { get; set; }

        // Navigation
        public ICollection<Component> Components { get; set; } = new List<Component>();
    }
}
