using DDApp.Extensions;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace DDApp.AppStructure
{
    [JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
    public class DataDrivenAppDataTypeProperty : WithGUID, IDataDrivenAppDataTypeProperty
    {
        public static readonly DataDrivenAppDataTypeProperty Empty = new DataDrivenAppDataTypeProperty
        {
            Type = string.Empty,
            Attributes = new string[0]
        };
        
        public static implicit operator DataDrivenAppDataTypeProperty(string str) => new DataDrivenAppDataTypeProperty { Type = str };
        
        [Display(Name = "Type", Description = "Data type of the property")]
        public string Type { get; set; }
        [Display(Name = "Attributes", Description = "List of attributes to apply to the property")]
        public IList<string> Attributes { get; set; }

        public DataDrivenAppDataTypeProperty()
        {
            Type = string.Empty;
            Attributes = new string[0];
        }

        protected override ulong getGUID()
        {
            unchecked
            {
                ulong result = 0;
                result = (result * 397) ^ Type?.GetGUID() ?? result;
                foreach (var attribute in Attributes ?? Enumerable.Empty<string>())
                {
                    result = (result * 397) ^ attribute?.GetGUID() ?? result;
                }
                return result;
            }
        }
    }
}