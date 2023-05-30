using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

namespace GameModCrafters.Models
{
    public class Mod
    {
        [Key, MaxLength(255), Display(Name = "Mod ID")]
        public string ModId { get; set; }

        [Display(Name = "遊戲ID")]
        public string GameId { get; set; }

        [Required, Display(Name = "作者ID")]
        public string AuthorId { get; set; }

        [MaxLength(255), Display(Name = "Mod名稱")]
        [Required(ErrorMessage = "必須輸入一個名字")]
        public string ModName { get; set; }

        [Display(Name = "描述")]
        public string Description { get; set; }

        [Display(Name = "安裝說明")]
        public string InstallationInstructions { get; set; }

        [Display(Name = "下載連結")]
        public string DownloadLink { get; set; }

        [Display(Name = "價格")]
        public decimal? Price { get; set; }

        [Display(Name = "縮圖")]
        public string Thumbnail { get; set; }

        [Display(Name = "創建時間")]
        public DateTime? CreateTime { get; set; }

        [Display(Name = "更新時間")]
        public DateTime? UpdateTime { get; set; }

        [Required, Display(Name = "是否已完成")]
        public bool IsDone { get; set; }

        // Navigation properties and Foreign key settings
        [ForeignKey(nameof(AuthorId))]
        public User Author { get; set; }

        [ForeignKey(nameof(GameId))]
        public Game Game { get; set; }

        // Navigation property
        public ICollection<ModTag> ModTags { get; set; }

        public ICollection<ModLike> ModLikes { get; set; }

        public ICollection<ModComment> ModComment { get; set; }

        public ICollection<Favorite> Favorite { get; set; }

        public ICollection<Log> Log { get; set; }

        public ICollection<Downloaded> Downloaded { get; set; }
    }
}
