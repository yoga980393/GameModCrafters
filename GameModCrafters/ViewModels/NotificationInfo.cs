using System.Collections.Generic;

namespace GameModCrafters.ViewModels
{
    public class NotificationInfo
    {
        public int UnreadCount { get; set; }
        public List<NotificationDetails> Notifications { get; set; }
    }
}
