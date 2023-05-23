using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace GameModCrafters.Models
{
    public class CommissionStatus
    {
        [Key, Required, MaxLength(255), Display(Name = "委託狀態ID")]
        public string CommissionStatusId { get; set; }

        [Required, MaxLength(255), Display(Name = "狀態")]
        public string Status { get; set; }

        // Navigation properties
        public ICollection<Commission> Commissions { get; set; }
    }
}
