using GameModCrafters.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using GameModCrafters.Models;
using System.Security.Claims;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using GameModCrafters.ViewModels;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace GameModCrafters.Services
{
    public class NotificationService
    {
        private readonly ApplicationDbContext _context;

        public NotificationService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task CreateNotificationAsync(string notifierId, string recipientId, string content)
        {
            var notification = new Notification()
            {
                NotificationId = Guid.NewGuid().ToString(),
                NotifierId = notifierId,
                RecipientId = recipientId,
                NotificationContent = content,
                NotificationTime = DateTime.Now,
                IsRead = false
            };

            await _context.Notifications.AddAsync(notification);
            await _context.SaveChangesAsync();
        }

        public void CreateNotification(string notifierId, string recipientId, string content)
        {
            var notification = new Notification()
            {
                NotificationId = Guid.NewGuid().ToString(),
                NotifierId = notifierId,
                RecipientId = recipientId,
                NotificationContent = content,
                NotificationTime = DateTime.Now,
                IsRead = false
            };

            _context.Notifications.Add(notification);
            _context.SaveChanges();
        }

        public async Task<NotificationInfo> GetUnreadNotificationsAsync(string userId)
        {
            var notifications = await _context.Notifications
                .Where(n => n.RecipientId == userId && !n.IsRead)
                .ToListAsync();

            var notificationInfo = new NotificationInfo
            {
                UnreadCount = notifications.Count,
                Notifications = notifications.Select(n => new NotificationDetails
                {
                    Id = n.NotificationId,
                    Content = n.NotificationContent,
                    Time = n.NotificationTime
                }).ToList()
            };

            return notificationInfo;
        }

        public async Task MarkNotificationsAsReadAsync(string userId)
        {
            var notifications = await _context.Notifications
                .Where(n => n.RecipientId == userId && !n.IsRead)
                .ToListAsync();

            foreach (var notification in notifications)
            {
                notification.IsRead = true;
            }

            await _context.SaveChangesAsync();
        }

        public async Task<List<NotificationDetails>> GetAllNotificationsAsync(string userId)
        {
            var notifications = await _context.Notifications
                .Where(n => n.RecipientId == userId)
                .Select(n => new NotificationDetails
                {
                    Id = n.NotificationId,
                    Content = n.NotificationContent,
                    Time = n.NotificationTime
                })
                .ToListAsync();

            return notifications;
        }
    }
}
