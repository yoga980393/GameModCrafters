using GameModCrafters.Data;
using GameModCrafters.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PayPal.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

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
                { "clientId", "Aadk1LCLNrKsRBjTzdr2fMQb-N6ysUo75cq0fqVw-TdCNXXOLvzkaUHret6SvsJf-w6ZIralojS2whkg" },
                { "clientSecret", "EKPCkHpefRADCGdPK5-eMhvI-Ih60XityKEaLyTuBFffrf1LrDndRFBMbEy4L0REyX_h3PFXd-UzoMw6" }
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
                    amount = new Amount { currency = "USD", total = price.ToString() },  // Convert the price from TWD to USD if necessary
                    item_list = new ItemList
                    {
                        items = new List<Item>
                        {
                            new Item
                            {
                                name = " 個Mod Coin",
                                currency = "USD",
                                price = "1", // Convert the price from TWD to USD if necessary
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

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> ConvertModCoin([FromBody] ConvertModCoinViewModel model)
        {
            int amount = model.amount;

            string userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == userEmail);

            // 驗證用戶和金額
            if (user == null || user.ModCoin < amount)
            {
                return BadRequest();
            }

            // 扣除用戶的 Mod Coin
            user.ModCoin -= amount;
            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            // 使用 PayPal 沙盒賬戶模擬轉賬，以美元為例
            var payout = new Payout
            {
                sender_batch_header = new PayoutSenderBatchHeader
                {
                    email_subject = "You have a Payout!",
                },
                items = new List<PayoutItem>
                {
                    new PayoutItem
                    {
                        recipient_type = PayoutRecipientType.EMAIL,
                        amount = new Currency
                        {
                            value = (amount).ToString(), 
                            currency = "USD"  
                        },
                        receiver = user.PayPalAccounts,  
                        note = "Thank you for your business!",
                        sender_item_id = "item_id_" + Guid.NewGuid().ToString(),
                    }
                }
            };

            var createdPayout = payout.Create(apiContext, false);

            if (createdPayout.batch_header.payout_batch_id != null)
            {
                TempData["TransferAmount"] = amount;
                TempData["RemainingModCoins"] = user.ModCoin;
                return Json("success");
            }
            else
            {
                // 還原 Mod Coin
                user.ModCoin += amount;
                _context.Users.Update(user);
                _context.SaveChanges();

                return Json("fail");
            }
        }

        [Authorize]
        public IActionResult TransferMoneySuccess(int amount)
        {
            string userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
            var user = _context.Users.FirstOrDefault(u => u.Email == userEmail);
            ViewBag.CoinCount = user.ModCoin;

            return View(amount);
        }

        [Authorize]
        public IActionResult TransferMoney()
        {
            string userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
            var user = _context.Users.FirstOrDefault(u => u.Email == userEmail);

            return View(user.ModCoin);
        }
    }
}
