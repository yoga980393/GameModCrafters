using System.Collections.Generic;
using System;

namespace GameModCrafters.ViewModels
{
    public class AutoSearchViewModel
    {
        public string ModId { get; set; }
        public string GameId { get; set; }
        public string GameName { get; set; }
        public string ModName { get; set; }
        public decimal? Price { get; set; }
        public string AuthorName { get; set; }
        public string ModThumbnail { get; set; }
        public string GameThumbnail { get; set; }

        public DateTime? CreateTime { get; set; }
        public DateTime? UpdateTime { get; set; }
        public string Description { get; set; }
      
        public int LikeCount { get; set; }
        public int FavoriteCount { get; set; }
        public int DownloadCount { get; set; }
        public List<string> TagNames { get; set; }
    }
}
