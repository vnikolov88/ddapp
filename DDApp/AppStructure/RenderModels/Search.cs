namespace DDApp.AppStructure.RenderModels
{
    public class Search
    {
        public string FirstIcon { set; get; }
        public string SecondIcon { set; get; }
        //Text above input.
        //Example: Suche / Search
        public string Title { set; get; }
        public string Action { get; set; }

        public string AutoSearchText { get; set; }
    }
}
