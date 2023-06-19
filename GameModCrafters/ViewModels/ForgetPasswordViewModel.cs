using System.ComponentModel.DataAnnotations;

namespace GameModCrafters.ViewModels
{
    public class ForgetPasswordViewModel
    {
        [Required(ErrorMessage = "此欄位為必填"), Display(Name = "輸入新的密碼")]
        [StringLength(20, MinimumLength = 8, ErrorMessage = "需介於8-20字元，並且包含至少一個大寫字母和一個數字")]


        public string Password1 { get; set; }

        [Required(ErrorMessage = "此欄位為必填"), Display(Name = "確認新的密碼")]
        [StringLength(20, MinimumLength = 8, ErrorMessage = "需介於8-20字元，並且包含至少一個大寫字母和一個數字")]
        [Compare(nameof(Password1))]

        public string Password2 { get; set; }
    }
}
