using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GameModCrafters.ViewModels
{
    public class PersonViewModel
    {
        [Key, Required(ErrorMessage = "此欄位為必填"), MaxLength(255), Display(Name = "電子郵件")]
        public string Email { get; set; }

        [Required(ErrorMessage = "此欄位為必填"), Display(Name = "用戶名")]
        [StringLength(20, MinimumLength = 3, ErrorMessage = "需介於3-20字元")]
        public string Username { get; set; }

        [Display(Name = "頭像")]
        public string Avatar { get; set; }

        [Display(Name = "背景圖片")]
        public string BackgroundImage { get; set; }

        public List<ModViewModel> PublishedMods { get; set; }
        public int PublishedCurrentPage { get; set; }
        public int PublishedTotalPages { get; set; }

        public List<ModViewModel> FavoritedMods { get; set; }
        public int FavoritedCurrentPage { get; set; }
        public int FavoritedTotalPages { get; set; }

        public List<ModViewModel> DownloadedMods { get; set; }
        public int DownloadedCurrentPage { get; set; }
        public int DownloadedTotalPages { get; set; }
    }
}
