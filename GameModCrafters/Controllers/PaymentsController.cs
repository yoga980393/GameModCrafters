using GameModCrafters.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PayPal.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace GameModCrafters.Controllers
{
    public class PaymentsController : Controller
    {
        private APIContext apiContext;

        private readonly ApplicationDbContext _context;

        public PaymentsController(ApplicationDbContext context)
        {
            var config = new Dictionary<string, string>
            {
                { "mode", "sandbox" },
                { "clientId", "AUL-Ku1UaDTQLEwqFSwBsEY041alKdAOCneCel9ESJo2XyI2qdcGMcHtSk_3FhP-dkEeWw7CR84O4rMA" },
                { "clientSecret", "EB39_sJHBx3UWDA_9Fcpkr8IFMI2QhMM4gsTUhU8zoijv2f1XX0ZSLdWSmUGm9xWFPF0at0hQEgGm7GW" }
            };

            var accessToken = new OAuthTokenCredential(config).GetAccessToken();
            apiContext = new APIContext(accessToken);

            _context = context;
        }

        [Authorize]
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult CreatePayment(int price, int quantity)
        {
            HttpContext.Session.SetInt32("purchasedModCoins", quantity);

            var payment = new Payment
            {
                intent = "sale",
                payer = new Payer { payment_method = "paypal" },
                transactions = new List<Transaction>
            {
                new Transaction
                {
                    description = "Transaction description.",
                    invoice_number = Guid.NewGuid().ToString(),
                    amount = new Amount { currency = "TWD", total = price.ToString() },  // Convert the price from TWD to USD if necessary
                    item_list = new ItemList
                    {
                        items = new List<Item>
                        {
                            new Item
                            {
                                name = " 個Mod Coin",
                                currency = "TWD",
                                price = "30", // Convert the price from TWD to USD if necessary
                                quantity = quantity.ToString(),
                                sku = "sku"
                            }
                        }
                    }
                }
            },
                redirect_urls = new RedirectUrls
                {
                    return_url = "https://localhost:44347/payments/success",
                    cancel_url = "https://localhost:44347/payments/cancel"
                }
            };

            var createdPayment = payment.Create(apiContext);
            var approvalUrl = createdPayment.links.FirstOrDefault(l => l.rel == "approval_url").href;

            return Json(approvalUrl);
        }

        [Authorize]
        public IActionResult Success(string paymentId, string payerId)
        {
            var paymentExecution = new PaymentExecution() { payer_id = payerId };
            var payment = new Payment() { id = paymentId };
            var executedPayment = payment.Execute(apiContext, paymentExecution);

            string userEmail = User.FindFirst(ClaimTypes.Email)?.Value;

            var user = _context.Users.FirstOrDefault(u => u.Email == userEmail);

            var CoinCount = 0;

            if (user != null)
            {
                int? purchasedModCoins = HttpContext.Session.GetInt32("purchasedModCoins");

                user.ModCoin += (int)purchasedModCoins;
                CoinCount = user.ModCoin;

                _context.Users.Update(user);
                _context.SaveChanges();
            }

            return View(CoinCount);
        }

        [Authorize]
        public IActionResult Cancel()
        {
            return View("Cancel");
        }

        [Authorize]
        public IActionResult StoredValue()
        {
            return View();
        }
    }
}
