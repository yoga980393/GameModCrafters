using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;
using System;

namespace GameModCrafters.Models
{
    public class ContactUs
    {
        [Key, Required, MaxLength(255), Display(Name = "聯絡ID")]
        public string ContactId { get; set; }

        [Required, MaxLength(255), Display(Name = "用戶ID")]
        public string UserId { get; set; }

        [Required, MaxLength(255), Display(Name = "主題")]
        public string Subject { get; set; }

        [Required, Display(Name = "訊息")]
        public string Message { get; set; }

        [Required, Display(Name = "狀態")]
        public bool Status { get; set; }

        [Required, Display(Name = "提交時間")]
        public DateTime SubmitTime { get; set; }

        [Required, Display(Name = "更新時間")]
        public DateTime UpdateTime { get; set; }

        // Navigation properties and Foreign key settings
        [ForeignKey(nameof(UserId))]
        public User User { get; set; }
    }
}
