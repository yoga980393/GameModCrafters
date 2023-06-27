using GameModCrafters.Models;
using System.Collections.Generic;

namespace GameModCrafters.ViewModels
{
    public class NavbarSearchResultViewmodel
    {
        public List<Game> Games { get; set; }
        public List<ModViewModel> Mods { get; set; }

        public int ModTotalPages { get; set; }
        public int ModCurrentPage { get; set; }

        public int GameTotalPages { get; set; }
        public int GameCurrentPage { get; set; }


        public string SearchString { get; set; }
    }
}
