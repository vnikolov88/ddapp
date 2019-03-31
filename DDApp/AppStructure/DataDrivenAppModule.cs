using DDApp.Extensions;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Collections.Generic;
using System.Linq;

namespace DDApp.AppStructure
{
    [JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
    public class DataDrivenAppModule : WithGUID, IDataDrivenAppModule
    {
        public static readonly DataDrivenAppModule Empty = new DataDrivenAppModule
        {
            DataTypes = new Dictionary<string, DataDrivenAppDataType>(),
            Pages = new Dictionary<string, DataDrivenAppPage>()
        };
        public IDictionary<string, DataDrivenAppDataType> DataTypes { get; set; }
        public IDictionary<string, DataDrivenAppPage> Pages { get; set; }

        public DataDrivenAppModule()
        {
            DataTypes = new Dictionary<string, DataDrivenAppDataType>();
            Pages = new Dictionary<string, DataDrivenAppPage>();
        }

        protected override ulong getGUID()
        {
            unchecked
            {
                ulong result = 0;
                foreach (var dataType in DataTypes ?? Enumerable.Empty<KeyValuePair<string, DataDrivenAppDataType>>())
                {
                    result = (result * 397) ^ dataType.Key?.GetGUID() ?? result;
                    result = (result * 397) ^ dataType.Value.GUID;
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
