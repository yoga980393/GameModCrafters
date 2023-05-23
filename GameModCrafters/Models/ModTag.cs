using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace GameModCrafters.Models
{
    public class ModTag
    {
        [Required, MaxLength(255), Display(Name = "Mod ID")]
        public string ModId { get; set; }

        [Required, MaxLength(255), Display(Name = "標籤ID")]
        public string TagId { get; set; }

        // Navigation properties and Foreign key settings
        [ForeignKey(nameof(ModId))]
        public Mod Mod { get; set; }

        [ForeignKey(nameof(TagId))]
        public Tag Tag { get; set; }
    }
}
