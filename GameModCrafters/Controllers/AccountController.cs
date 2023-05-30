using GameModCrafters.Data;
using GameModCrafters.Models;
using GameModCrafters.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
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
        public async Task<IActionResult> LoginPage(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                
                var user =_context.Users.FirstOrDefault(x => x.Username == model.Text && x.Password==model.Password);
                if(user==null)
                {
                    user = _context.Users.FirstOrDefault(x => x.Email == model.Text && x.Password == model.Password);
                    if (user == null)
                    {
                        ModelState.AddModelError("Text", "帳號或密碼錯誤");
                        return View(model);//失敗
                    }
                    return RedirectToAction("Index", "Home");//成功
                }
                else
                {
                    return RedirectToAction("Index", "Home");//成功
                }
                
                
                
            }
            return View(model);//失敗
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
                bool isEmailExists = _context.Users.Any(u => u.Email == register.Email);
                
                bool isUsernameExists = _context.Users.Any(m=>m.Username == register.Username);
                if (isUsernameExists)
                {
                    if (isEmailExists)
                    {
                        ModelState.AddModelError("Email", "該 Email 已被使用。");
                    
                    }
                    ModelState.AddModelError("Username", "該 Username 已被使用。");
                    return View(register);
                }
                if (register.Password1==register.Password2)
                {
                    bool isPasswordValid = IsPasswordValid(register.Password1);
                    if (isPasswordValid==false)
                    {
                        ModelState.AddModelError("Password1","密碼至少包含 8-20 個字符，並且包含至少一個大寫字母和一個數字");
                        return View(register);
                    }
                    var user = new User
                    {
                        Email = register.Email,
                        Username = register.Username,
                        Password = register.Password1,
                    };
                    _context.Users.Add(user);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("LoginPage");//成功
                }
                else
                {
                    ModelState.AddModelError("Password1", "密碼和確認密碼不一樣");
                    ModelState.AddModelError("Password2", "密碼和確認密碼不一樣");
                    return View(register);
                }
            
                

            }
            return View(register);
        }
        private bool IsPasswordValid(string password)
        {
            
            // 以下是一個示例：要求密碼至少包含 8 個字符，並且包含至少一個大寫字母和一個數字
            return password.Length <= 20 && password.Length >= 8 && password.Any(char.IsUpper) && password.Any(char.IsDigit);
        }
    }
}
