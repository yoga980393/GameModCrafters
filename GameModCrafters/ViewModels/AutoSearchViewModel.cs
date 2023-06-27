using System.Collections.Generic;
using System;
using GameModCrafters.Models;

namespace GameModCrafters.ViewModels
{
    public class AutoSearchViewModel
    {
       

        public List<Game> Games { get; set; }
        public List<ModViewModel> Mods { get; set; }

        public int Count { get; set; }
    }
}
