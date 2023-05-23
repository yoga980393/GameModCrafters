using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;
using System;

namespace GameModCrafters.Models
{
    public class Transaction
    {
        [Key, Required, MaxLength(255), Display(Name = "交易ID")]
        public string TransactionId { get; set; }

        [Required, MaxLength(255), Display(Name = "委託ID")]
        public string CommissionId { get; set; }

        [Required, MaxLength(255), Display(Name = "付款者ID")]
        public string PayerId { get; set; }

        [Required, MaxLength(255), Display(Name = "收款者ID")]
        public string PayeeId { get; set; }

        [Required, Display(Name = "交易狀態")]
        public bool TransactionStatus { get; set; }

        [Required, Display(Name = "創建時間")]
        public DateTime CreateTime { get; set; }

        // Navigation properties and Foreign key settings
        [ForeignKey(nameof(CommissionId))]
        public Commission Commission { get; set; }

        [ForeignKey(nameof(PayerId))]
        public User Payer { get; set; }

        [ForeignKey(nameof(PayeeId))]
        public User Payee { get; set; }
    }
}
