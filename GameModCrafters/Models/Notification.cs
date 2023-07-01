using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;
using System;

namespace GameModCrafters.Models
{
    public class Notification
    {
        [Key, Required, MaxLength(255), Display(Name = "訊息ID")]
        public string NotificationId { get; set; }

        [Required, MaxLength(255), Display(Name = "發送者ID")]
        public string NotifierId { get; set; }

        [Required, MaxLength(255), Display(Name = "接收者ID")]
        public string RecipientId { get; set; }

        [Required, Display(Name = "訊息內容")]
        public string NotificationContent { get; set; }

        [Required, Display(Name = "訊息時間")]
        public DateTime NotificationTime { get; set; }

        [Required, Display(Name = "是否已讀")]
        public bool IsRead { get; set; }

        // Navigation properties and Foreign key settings
        [ForeignKey(nameof(NotifierId))]
        public User Notifier { get; set; }

        [ForeignKey(nameof(RecipientId))]
        public User Recipient { get; set; }
    }
}
