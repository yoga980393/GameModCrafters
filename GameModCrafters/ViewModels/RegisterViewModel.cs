using System.ComponentModel.DataAnnotations;

namespace GameModCrafters.ViewModels
{
    public class RegisterViewModel
    {
        [Key, Required(ErrorMessage = "此欄位為必填"), MaxLength(255), Display(Name = "電子郵件")]
        public string Email { get; set; }

        [Required(ErrorMessage = "此欄位為必填"), MaxLength(255), Display(Name = "用戶名")]
        public string Username { get; set; }

        [Required(ErrorMessage = "此欄位為必填"), MaxLength(255), Display(Name = "密碼")]

        public string Password { get; set; }
        

    }
}
