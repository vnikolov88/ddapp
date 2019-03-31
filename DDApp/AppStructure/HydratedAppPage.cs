using System.Collections.Generic;

namespace DDApp.AppStructure
{
    public class HydratedAppPage : IAppPageDescription
    {
        public static readonly HydratedAppPage Empty = new HydratedAppPage { Components = new dynamic[0] };

        private HydratedAppPage() { }

        public HydratedAppPage(IAppPageDescription page)
        {
            CanCache = true;
            Title = page.Title;
            TitleIcon = page.TitleIcon;
            TitleImage = page.TitleImage;

            TabGroups = page.TabGroups;
            Components = new List<dynamic>();
        }
        public bool CanCache { get; set; }
        public IList<dynamic> Components { get; set; }

        public string Title { get; set; }
        public string TitleIcon { get; set; }
        public string TitleImage { get; set; }

        public IDictionary<string, uint> TabGroups { get; set; }

        #region Pass-down propertoes
        public string AppLogo { get; set; }
        public string AppQuickCallNumber { get; set; }
        public string AppQuickCallNumberIcon { get; set; }
        public string AppQuickCallNumberText { get; set; }
        public IDictionary<string, string> Navigation { get; set; }
        #endregion Pass-down propertoes
    }
}
