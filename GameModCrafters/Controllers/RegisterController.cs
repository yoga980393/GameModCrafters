using GameModCrafters.Data;
using GameModCrafters.Models;
using GameModCrafters.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace GameModCrafters.Controllers
{
    public class RegisterController : Controller
    {
        private readonly ApplicationDbContext _context;
        public RegisterController(ApplicationDbContext context)
        {
            _context = context;
        }
        [HttpGet]
        public IActionResult RegisterPage()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RegisterPage(RegisterViewModel register)
        {
            if(ModelState.IsValid)
            {
                var user = new User
                {
                    Email = register.Email,
                    Username = register.Username,
                    Password = register.Password,
                };
                string password1 = Request.Form["password1"];
                string password2 = Request.Form["password2"];

            }
            return View(register);
        }
    }
}
