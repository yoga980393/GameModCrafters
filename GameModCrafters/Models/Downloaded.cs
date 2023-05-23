using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;
using System;

namespace GameModCrafters.Models
{
    public class Downloaded
    {
        [Key, Required, MaxLength(255), Display(Name = "下載ID")]
        public string DownloadId { get; set; }

        [Required, MaxLength(255), Display(Name = "用戶ID")]
        public string UserId { get; set; }

        [Required, MaxLength(255), Display(Name = "Mod ID")]
        public string ModId { get; set; }

        [Required, Display(Name = "下載時間")]
        public DateTime DownloadTime { get; set; }

        // Navigation properties and Foreign key settings
        [ForeignKey(nameof(UserId))]
        public User User { get; set; }

        [ForeignKey(nameof(ModId))]
        public Mod Mod { get; set; }
    }
}
