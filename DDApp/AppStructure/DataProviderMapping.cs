using DDApp.Extensions;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace DDApp.AppStructure
{
    [JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
    public class DataProviderMapping : WithGUID
    {
        [Display(Name = "Source Type", Description = "The Type of data we are mapping from")]
        public string SourceType { get; set; }
        [Display(Name = "Destination Type", Description = "The Type of data we are mapping to")]
        public string DestinationType { get; set; }
        [Display(Name = "Mapping configuration", Description = "Dictionary of lambda expressions for destination and source mappings")]
        public Dictionary<string, string> Mapping { get; set; }
        
        protected override ulong getGUID()
        {
            unchecked
            {
                ulong _guid = 0;
                _guid = (_guid * 397) ^ SourceType.GetGUID();
                _guid = (_guid * 397) ^ DestinationType.GetGUID();
                foreach (var mapping in Mapping ?? Enumerable.Empty<KeyValuePair<string, string>>())
                {
                    _guid = (_guid * 397) ^ mapping.Key.GetGUID();
                    _guid = (_guid * 397) ^ mapping.Value.GetGUID();
                }
                return _guid;
            }
        }
    }

}
