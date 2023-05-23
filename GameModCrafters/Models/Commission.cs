namespace GameModCrafters.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class Commission
    {
        [Key, Required, MaxLength(255), Display(Name = "委託ID")]
        public string CommissionId { get; set; }

        [Required, MaxLength(255), Display(Name = "委託者ID")]
        public string DelegatorId { get; set; }

        [MaxLength(255), Display(Name = "遊戲ID")]
        public string GameId { get; set; }

        [MaxLength(255), Display(Name = "委託標題")]
        public string CommissionTitle { get; set; }

        [Display(Name = "委託描述")]
        public string CommissionDescription { get; set; }

        [Display(Name = "預算")]
        public decimal? Budget { get; set; }

        [Display(Name = "截止日期")]
        public DateTime? Deadline { get; set; }

        [MaxLength(255), Display(Name = "委託狀態ID")]
        public string CommissionStatusId { get; set; }

        [Display(Name = "創建時間")]
        public DateTime? CreateTime { get; set; }

        [Display(Name = "更新時間")]
        public DateTime? UpdateTime { get; set; }

        [Display(Name = "是否完成")]
        public bool IsDone { get; set; }

        [Display(Name = "待刪除")]
        public bool Trash { get; set; }

        // Navigation properties and Foreign key settings
        [ForeignKey(nameof(DelegatorId))]
        public User Delegator { get; set; }

        [ForeignKey(nameof(GameId))]
        public Game Game { get; set; }

        [ForeignKey(nameof(CommissionStatusId))]
        public CommissionStatus CommissionStatus { get; set; }

        // Navigation property
        public ICollection<Transaction> Transaction { get; set; }

        public ICollection<CommissionTracking> CommissionTrackings { get; set; }
    }
}
