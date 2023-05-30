using System.ComponentModel.DataAnnotations;

namespace GameModCrafters.ViewModels
{
    public class LoginViewModel
    {
        [Key, Required(ErrorMessage = "此欄位為必填"), MaxLength(255), Display(Name = "電子郵件/使用者名稱")]
        public string Text { get; set; }

      

        [Required(ErrorMessage = "此欄位為必填"), MaxLength(255), Display(Name = "密碼")]
        
        public string Password { get; set; }
    }
}
