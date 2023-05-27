 using GameModCrafters.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace GameModCrafters.Controllers
{
    public class AccountController : Controller
    {
        [HttpGet]
        public IActionResult Login()
        {
            // 登入頁面
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                return Content("帳密登入成功");
            }
            // 處理登入邏輯
            // 檢查使用者名稱和密碼，進行身份驗證等等
            // 返回結果
            return View();
        }
    }
}
