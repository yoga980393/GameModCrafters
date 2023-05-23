using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;
using System;

namespace GameModCrafters.Models
{
    public class ModLike
    {
        [Required, MaxLength(255), Display(Name = "Mod ID")]
        public string ModId { get; set; }

        [Required, MaxLength(255), Display(Name = "用戶ID")]
        public string UserId { get; set; }

        [Required, Display(Name = "是否喜歡")]
        public bool Liked { get; set; }

        [Required, Display(Name = "評分日期")]
        public DateTime RatingDate { get; set; }

        // Navigation properties and Foreign key settings
        [ForeignKey(nameof(UserId))]
        public User User { get; set; }

        [ForeignKey(nameof(ModId))]
        public Mod Mod { get; set; }
    }
}
