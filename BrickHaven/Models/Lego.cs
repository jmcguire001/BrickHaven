using System.ComponentModel.DataAnnotations;

namespace BrickHaven.Models
{
    public class Lego
    {
        [Key]
        public int LegoId { get; set; }
        public string LegoName { get; set; }
        public string ProgramName { get; set; }
        public string? LegoType { get; set; }
        public int LegoImpact { get; set; }
        public DateTime LegoInstallation { get; set; }
        public string LegoPhase { get; set; }
    }
}
