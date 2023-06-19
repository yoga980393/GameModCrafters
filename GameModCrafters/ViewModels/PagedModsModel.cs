using GameModCrafters.Models;
using System.Collections.Generic;

namespace GameModCrafters.ViewModels
{
    public class PagedModsModel
    {
        public List<ModViewModel> Mods { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public string GameId { get; set; }

        public List<CommissionViewModel> Commissions { get; set; }
    }
}
