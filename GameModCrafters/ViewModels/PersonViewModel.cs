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
    }
}
