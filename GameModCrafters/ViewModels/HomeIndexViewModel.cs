using GameModCrafters.Models;
using System.Collections.Generic;

namespace GameModCrafters.ViewModels
{
    public class HomeIndexViewModel
    {
        public List<Game> Games { get; set; }
        public List<ModViewModel> Mods { get; set; }
        public List<AuthorViewModel> Author { get; set; }
    }
}
