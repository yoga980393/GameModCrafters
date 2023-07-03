using GameModCrafters.Data;
using GameModCrafters.Models;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;
using System;
using System.Threading.Tasks;

namespace GameModCrafters.Hubs
{
    public class ChatHub : Hub
    {
        private readonly ApplicationDbContext _context; // 假设 ApplicationDbContext 是你的 EF Core DbContext

        public ChatHub(ApplicationDbContext context)
        {
            _context = context;
        }

        
        public async Task SendMessage(string receiverEmail, string messageContent, bool IsRequestMessage)
        {
            var senderEmail = Context.User.FindFirstValue(ClaimTypes.Email); // 从授权的用户中获取发送者的 email
            if (string.IsNullOrEmpty(senderEmail))
            {
                throw new Exception("未授权的用户");
            }

            // 创建新的消息
            var message = new PrivateMessage
            {
                MessageId = Guid.NewGuid().ToString(),
                SenderId = senderEmail,
                ReceiverId = receiverEmail,
                MessageContent = messageContent,
                MessageTime = DateTime.Now,
                IsRequestMessage = IsRequestMessage
            };

            // 将消息存入数据库
            _context.PrivateMessages.Add(message);
            await _context.SaveChangesAsync();

            // 将消息发送到客户端
            await Clients.All.SendAsync("ReceiveMessage", senderEmail, receiverEmail, messageContent);
        }
    }
}
