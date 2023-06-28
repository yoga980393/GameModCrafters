﻿using System;

namespace GameModCrafters.ViewModels
{
    public class CommissionViewModel
    {
        public string CommissionId { get; set; }
        public string GameID { get; set; }
        public string GameName { get; set; }
        public string DelegatorName { get; set; }
        public string CommissionTitle { get; set; }
        public decimal? Budget { get; set; }
        public DateTime? CreateTime { get; set; }
        public DateTime? UpdateTime { get; set; }
        public string Status { get; set; }
    }
}
