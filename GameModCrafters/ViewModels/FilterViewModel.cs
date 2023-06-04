namespace GameModCrafters.ViewModels
{
    public class FilterViewModel
    {
        public string SearchString { get; set; }
        public int? TimeFilter { get; set; }
        public string SortFilter { get; set; }
        public string OrderFilter { get; set; }
        public int PageSize { get; set; }
    }
}
