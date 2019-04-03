using DDApp.Extensions;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Collections.Generic;
using System.Linq;

namespace DDApp.AppStructure
{
    [JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
    public class DataDrivenApp : WithGUID, IDataDrivenAppModule
    {
        public static readonly DataDrivenApp Empty = new DataDrivenApp {
            Modules = new List<string>(),
            DataTypes = new Dictionary<string, DataDrivenAppDataType>(),
            Pages = new Dictionary<string, DataDrivenAppPage>(),
            Logo = string.Empty
        };
        
        public string Logo { get; set; }
        public string QuickCallNumber { get; set; }
        public string QuickCallNumberIcon { get; set; }
        public string QuickCallNumberText { get; set; }
        public IList<string> Modules { get; set; }
        public IDictionary<string, DataDrivenAppDataType> DataTypes { get; set; }
        public IDictionary<string, string> Navigation { get; set; }
        public IDictionary<string, DataDrivenAppPage> Pages { get; set; }

        public DataDrivenApp()
        {
            Modules = new List<string>();
            DataTypes = new Dictionary<string, DataDrivenAppDataType>();
            Navigation = new Dictionary<string, string>();
            Pages = new Dictionary<string, DataDrivenAppPage>();
            Logo = string.Empty;
        }

        public void LoadAppModule(IDataDrivenAppModule appModule)
        {
            appModule.DataTypes.ForEach(x => {
                if (DataTypes.Any(dt => dt.Key == x.Key))
                    return;
                DataTypes.Add(x);
            });
            appModule.Pages.ForEach(x => {
                if (Pages.Any(page => page.Key == x.Key))
                    return;
                Pages.Add(x);
            });
        }

        protected override ulong getGUID()
        {
            unchecked
            {
                ulong result = 0;
                result = (result * 397) ^ Logo?.GetGUID() ?? result;
                result = (result * 397) ^ QuickCallNumber?.GetGUID() ?? result;
                result = (result * 397) ^ QuickCallNumberIcon?.GetGUID() ?? result;
                result = (result * 397) ^ QuickCallNumberText?.GetGUID() ?? result;
                foreach (var dataType in DataTypes ?? Enumerable.Empty<KeyValuePair<string, DataDrivenAppDataType>>())
                {
                    result = (result * 397) ^ dataType.Key?.GetGUID() ?? result;
                    result = (result * 397) ^ dataType.Value.GUID;
                }
                foreach (var navigation in Navigation ?? Enumerable.Empty<KeyValuePair<string, string>>())
                {
                    result = (result * 397) ^ navigation.Key?.GetGUID() ?? result;
                    result = (result * 397) ^ navigation.Value?.GetGUID() ?? result;
                }
                foreach (var page in Pages ?? Enumerable.Empty<KeyValuePair<string, DataDrivenAppPage>>())
                {
                    result = (result * 397) ^ page.Key?.GetGUID() ?? result;
                    result = (result * 397) ^ page.Value.GUID;
                }
                return result;
            }
        }
    }
}
