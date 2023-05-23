using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;
using System;

namespace GameModCrafters.Models
{
    public class ModComment
    {
        [Key, Required, MaxLength(255), Display(Name = "留言ID")]
        public string CommentId { get; set; }

        [Required, MaxLength(255), Display(Name = "Mod ID")]
        public string ModId { get; set; }

        [Required, MaxLength(255), Display(Name = "用戶ID")]
        public string UserId { get; set; }

        [Required, Display(Name = "留言內容")]
        public string CommentContent { get; set; }

        [Required, Display(Name = "留言日期")]
        public DateTime CommentDate { get; set; }

        // Navigation properties and Foreign key settings
        [ForeignKey(nameof(UserId))]
        public User User { get; set; }

        [ForeignKey(nameof(ModId))]
        public Mod Mod { get; set; }
    }
}
