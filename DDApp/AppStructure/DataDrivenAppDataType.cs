using DDApp.Extensions;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace DDApp.AppStructure
{
    [JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
    public class DataDrivenAppDataType : WithGUID, IDataDrivenAppDataType
    {
        public static readonly DataDrivenAppDataType Empty = new DataDrivenAppDataType {
            Attributes = new List<string>(),
            Properties = new Dictionary<string, DataDrivenAppDataTypeProperty>(),
            Methods = new Dictionary<string, string>()
        };
        
        [Display(Name = "Attributes", Description = "Class level attributes")]
        public IList<string> Attributes { get; set; }

        [Display(Name = "Properties", Description = "Properties of the data type")]
        public IDictionary<string, DataDrivenAppDataTypeProperty> Properties { get; set; }

        [Display(Name = "Methods", Description = "Methods of the data type")]
        public IDictionary<string, string> Methods { get; set; }

        public DataDrivenAppDataType()
        {
            Attributes = new List<string>();
            Properties = new Dictionary<string, DataDrivenAppDataTypeProperty>();
            Methods = new Dictionary<string, string>();
        }

        protected override ulong getGUID()
        {
            unchecked
            {
                ulong result = 0;
                foreach (var attribute in Attributes ?? Enumerable.Empty<string>())
                {
                    result = (result * 397) ^ attribute?.GetGUID() ?? result;
                }
                foreach (var property in Properties ?? Enumerable.Empty<KeyValuePair<string, DataDrivenAppDataTypeProperty>>())
                {
                    result = (result * 397) ^ property.Key?.GetGUID() ?? result;
                    result = (result * 397) ^ property.Value?.GUID ?? result;
                }
                foreach (var method in Methods ?? Enumerable.Empty<KeyValuePair<string, string>>())
                {
                    result = (result * 397) ^ method.Key?.GetGUID() ?? result;
                    result = (result * 397) ^ method.Value?.GetGUID() ?? result;
                }
                return result;
            }
        }
    }
}
