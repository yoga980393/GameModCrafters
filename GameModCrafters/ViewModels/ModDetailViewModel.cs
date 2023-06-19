using System.Collections.Generic;
using System;

namespace GameModCrafters.ViewModels
{
    public class ModDetailViewModel
    {
        public string ModId { get; set; }
        public string ModName { get; set; }
        public List<string> Tags { get; set; }
        public DateTime? CreateTime { get; set; }
        public DateTime? UpdateTime { get; set; }
        public string Description { get; set; }
        public string InstallationInstructions { get; set; }
        public int LikeCount { get; set; }
        public int FavoriteCount { get; set; }
        public int DownloadCount { get; set; }
        public decimal? Price { get; set; }
        public string AuthorName { get; set; }
        public int AuthorWorkCount { get; set; } // 作者作品數量
        public int AuthorLikesReceived { get; set; } // 作者獲讚數量
        public string GameId { get; set; }
        public List<ModCommentViewModel> Comments { get; set; }
        public bool UserHasLiked { get; set; }
        public bool UserHasFavorite { get; set; }
    }
}
