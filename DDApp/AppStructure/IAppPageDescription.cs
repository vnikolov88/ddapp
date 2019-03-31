using System.Collections.Generic;

namespace DDApp.AppStructure
{
    public interface IAppPageDescription
    {
        string Title { get; set; }
        string TitleIcon { get; set; }
        string TitleImage { get; set; }
        IDictionary<string, uint> TabGroups { get; set; }
    }
}
