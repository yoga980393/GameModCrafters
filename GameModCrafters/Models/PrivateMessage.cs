namespace GameModCrafters.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class PrivateMessage
    {
        [Key, Required, MaxLength(255), Display(Name = "訊息ID")]
        public string MessageId { get; set; }

        [Required, MaxLength(255), Display(Name = "發送者ID")]
        public string SenderId { get; set; }

        [Required, MaxLength(255), Display(Name = "接收者ID")]
        public string ReceiverId { get; set; }

        [Required, Display(Name = "訊息內容")]
        public string MessageContent { get; set; }

        [Required, Display(Name = "訊息時間")]
        public DateTime MessageTime { get; set; }

        [Required, Display(Name = "是否已讀")]
        public bool IsRead { get; set; }

        public bool IsRequestMessage { get; set; }

        // Navigation properties and Foreign key settings
        [ForeignKey(nameof(SenderId))]
        public User Sender { get; set; }

        [ForeignKey(nameof(ReceiverId))]
        public User Receiver { get; set; }
    }

}
