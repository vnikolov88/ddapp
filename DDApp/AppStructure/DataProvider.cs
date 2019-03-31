using DDApp.Extensions;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace DDApp.AppStructure
{
    [JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
    public class DataProvider : WithGUID
    {
        [Display(Name = "Data Type", Description = "The type of data the provider expects")]
        public string Type { get; set; }

        [Display(Name = "Provider URL", Description = "URL template for the data provider")]
        public string Url { get; set; }

        [Display(Name = "Reducer", Description = "LINQ expression to select part of the Data Type object")]
        public string Reducer { get; set; }

        [Display(Name = "Mapper providers", Description = "Collection of mappings local to the DataProvider")]
        public ICollection<DataProviderMapping> Mapper { get; set; }

        protected override ulong getGUID()
        {
            unchecked
            {
                ulong result = 0;
                result = (result * 397) ^ Type?.GetGUID() ?? result;
                result = (result * 397) ^ Url?.GetGUID() ?? result;
                result = (result * 397) ^ Reducer?.GetGUID() ?? result;
                result = (result * 397) ^ Mapper.Select(x => x.GUID).Aggregate(result, (current, next) => current * next);
                return result;
            }
        }
    }
}
