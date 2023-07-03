using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;
using System;

namespace GameModCrafters.Models
{
    public class Transaction
    {
        [Key, MaxLength(255), Display(Name = "交易ID")]
        public string TransactionId { get; set; }

        [Required, MaxLength(255), Display(Name = "委託ID")]
        public string CommissionId { get; set; }

        [MaxLength(255), Display(Name = "發起人")]
        public string PayerId { get; set; }

        [Required, MaxLength(255), Display(Name = "接收人")]
        public string PayeeId { get; set; }

        [Display(Name = "委託細節")]
        public string Describe { get; set; }

        [Display(Name = "交易狀態")]
        public bool TransactionStatus { get; set; }

        public string FileURL { get; set; }
        [Display(Name = "是否提交")]
        public bool IsSubmit { get; set; }
        [Display(Name = "是否收取")]
        public bool IsReceive { get; set; }
        [Display(Name = "是否確認")]
        public bool IsConfirm { get; set; }

        [Display(Name = "交易完成")]
        public bool Isdone { get; set; }

        [Display(Name = "創建時間")]
        public DateTime CreateTime { get; set; }

        [Required, Display(Name = "預算")]
        public int Budget { get; set; }

        // Navigation properties and Foreign key settings
        [ForeignKey(nameof(CommissionId))]
        public Commission Commission { get; set; }

        [ForeignKey(nameof(PayerId))]
        public User Payer { get; set; }

        [ForeignKey(nameof(PayeeId))]
        public User Payee { get; set; }
    }
}
