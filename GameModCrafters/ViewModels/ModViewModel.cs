using System;
using System.Collections.Generic;

namespace GameModCrafters.ViewModels
{
    public class ModViewModel
    {
        public string ModId { get; set; }
        public string Thumbnail { get; set; }
        public string ModName { get; set; }
        public decimal? Price { get; set; }
        public string GameName { get; set; }
        public string AuthorName { get; set; }
        public DateTime? CreateTime { get; set; }
        public DateTime? UpdateTime { get; set; }
        public string Description { get; set; }
        public double Capacity { get; set; }
        public int LikeCount { get; set; }
        public int FavoriteCount { get; set; }
        public int DownloadCount { get; set; }
        public List<string> TagNames { get; set; }
    }
}
