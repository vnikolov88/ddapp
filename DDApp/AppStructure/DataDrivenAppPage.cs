using DDApp.Extensions;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace DDApp.AppStructure
{
    [JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
    public class DataDrivenAppPage : WithGUID, IAppPageDescription
    {
        public static readonly DataDrivenAppPage Empty = new DataDrivenAppPage { Components = new DataDrivenAppComponent[0], Title = string.Empty };

        [Display(Name = "Page Title", Description = "The visible title of a page")]
        public string Title { get; set; }
        public string TitleIcon { get; set; }
        public string TitleImage { get; set; }
        [Display(Name = "Tab Groups", Description = "Name of the tab and number of elements it contains")]
        public IDictionary<string, uint> TabGroups { get; set; }
        public IList<DataDrivenAppComponent> Components { get; set; }

        protected override ulong getGUID()
        {
            unchecked
            {
                ulong result = 0;
                result = (result * 397) ^ Title?.GetGUID() ?? result;
                result = (result * 397) ^ TitleIcon?.GetGUID() ?? result;
                result = (result * 397) ^ TitleImage?.GetGUID() ?? result;
                foreach (var tabGroup in TabGroups ?? Enumerable.Empty<KeyValuePair<string, uint>>())
                {
                    result = (result * 397) ^ tabGroup.Key?.GetGUID() ?? result;
                    result = (result * 397) ^ tabGroup.Value.GetGUID();
                }
                foreach (var component in Components ?? Enumerable.Empty<DataDrivenAppComponent>())
                {
                    result = (result * 397) ^ component.GUID;
                }
                return result;
            }
        }
    }
}
