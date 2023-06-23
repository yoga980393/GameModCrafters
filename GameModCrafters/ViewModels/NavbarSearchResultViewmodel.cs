using GameModCrafters.Models;
using System.Collections.Generic;

namespace GameModCrafters.ViewModels
{
    public class NavbarSearchResultViewmodel
    {
        public List<Game> Games { get; set; }
        public List<ModViewModel> Mods { get; set; }

        public int TotalPages { get; set; }
        public int CurrentPage { get; set; }
    }
}
