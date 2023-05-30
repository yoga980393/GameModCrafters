using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GameModCrafters.Models
{
    public class User
    {
        [Key, Required, MaxLength(255), Display(Name = "電子郵件")]
        public string Email { get; set; }

        [Required, MaxLength(255), Display(Name = "用戶名")]
        public string Username { get; set; }

        [Required, MaxLength(255), Display(Name = "密碼")]
        public string Password { get; set; }

        [Display(Name = "頭像")]
        public string Avatar { get; set; }

        [Display(Name = "背景圖片")]
        public string BackgroundImage { get; set; }

        [Required, Display(Name = "是否為管理員")]
        public bool IsAdmin { get; set; }

        [Required, Display(Name = "註冊日期")]
        public DateTime RegistrationDate { get; set; }

        [Required, Display(Name = "最後登入時間")]
        public DateTime LastLogin { get; set; }

        [Required, Display(Name = "是否被封鎖")]
        public bool Baned { get; set; }
        //[Required, Display(Name = "email確認")]
        //public bool EmailConfirmed { get; set; }

        // Navigation property
        public ICollection<Mod> Mods { get; set; }

        public ICollection<ModLike> ModLikes { get; set; }

        public ICollection<ModComment> ModComment { get; set; }

        public ICollection<Log> Log { get; set; }

        public ICollection<Downloaded> Downloaded { get; set; }

        public ICollection<Commission> Commission { get; set; }

        public ICollection<Transaction> Payments { get; set; }
        public ICollection<Transaction> Incomes { get; set; }

        public ICollection<PrivateMessage> SentMessages { get; set; }
        public ICollection<PrivateMessage> ReceivedMessages { get; set; }

        public ICollection<ContactUs> ContactMessages { get; set; }

        public ICollection<CommissionTracking> CommissionTrackings { get; set; }
    }

}
