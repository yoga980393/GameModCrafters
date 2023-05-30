using GameModCrafters.Data;
using GameModCrafters.Models;
using GameModCrafters.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace GameModCrafters.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;
        public AccountController(ApplicationDbContext context)
        {
            _context = context;
        }
        [HttpGet]
        public IActionResult LoginPage()
        {
            // 登入頁面
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult LoginPage(LoginViewModel model)
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
        [HttpGet]
        public IActionResult RegisterPage()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RegisterPage([Bind("Email, Username, Password1,Password2")] RegisterViewModel register)
        {
            if (ModelState.IsValid)
            {
                var user = new User
                {
                    Email = register.Email,
                    Username = register.Username,
                    Password = register.Password1,
                };
                //string password1 = Request.Form["password1"];
                //string password2 = Request.Form["password2"];
                _context.Users.Add(user);
                await _context.SaveChangesAsync();

            }
            return View(register);
        }
    }
}
