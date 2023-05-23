using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace GameModCrafters.Models
{
    public class Tag
    {
        [Key, Required, MaxLength(255), Display(Name = "標籤ID")]
        public string TagId { get; set; }

        [Required, MaxLength(255), Display(Name = "標籤名稱")]
        public string TagName { get; set; }

        // Navigation property
        public ICollection<ModTag> ModTags { get; set; }
    }
}
