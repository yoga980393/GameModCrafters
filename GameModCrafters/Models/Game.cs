using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;
using System;
using System.Collections.Generic;

namespace GameModCrafters.Models
{
    public class Game
    {
        [Key, Required, MaxLength(255), Display(Name = "遊戲ID")]
        public string GameId { get; set; }

        [Required, MaxLength(255), Display(Name = "遊戲名稱")]
        public string GameName { get; set; }

        [Required, Display(Name = "描述")]
        public string Description { get; set; }

        [Display(Name = "縮圖")]
        public string Thumbnail { get; set; }

        [Required, Display(Name = "創建時間")]
        public DateTime CreateTime { get; set; }

        // Navigation property
        public ICollection<Mod> Mods { get; set; }

        public ICollection<Commission> Commission { get; set; }
    }
}
