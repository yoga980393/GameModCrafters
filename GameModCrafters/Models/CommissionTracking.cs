using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;
using System;

namespace GameModCrafters.Models
{
    public class CommissionTracking
    {
        [Required, MaxLength(255), Display(Name = "用戶ID")]
        public string UserId { get; set; }

        [Required, MaxLength(255), Display(Name = "委託ID")]
        public string CommissionId { get; set; }

        [Required, Display(Name = "添加時間")]
        public DateTime AddTime { get; set; }

        // Navigation properties and Foreign key settings
        [ForeignKey(nameof(UserId))]
        public User User { get; set; }

        [ForeignKey(nameof(CommissionId))]
        public Commission Commission { get; set; }
    }
}
