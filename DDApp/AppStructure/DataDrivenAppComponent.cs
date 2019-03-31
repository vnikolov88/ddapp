using DDApp.Extensions;
using DDApp.Infrastructure;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.ComponentModel.DataAnnotations;

namespace DDApp.AppStructure
{
    [JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
    public class DataDrivenAppComponent : WithGUID, IDataDrivenAppComponent
    {
        public static readonly DataDrivenAppComponent Empty = new DataDrivenAppComponent { RenderType = string.Empty };

        [Display(Name = "Render Type", Description = "The type of component this will render")]
        public string RenderType { get; set; }

        [Display(Name = "Data Provider", Description = "Provides external data using GET requests")]
        public DataProvider Provider { get; set; }        

        [JsonConverter(typeof(PassTroughtJsonConverter))]
        [Display(Name = "Model", Description = "The static view model used when there is not DataProvider")]
        public string Model { get; set; }

        protected override ulong getGUID()
        {
            unchecked
            {
                ulong result = 0;
                result = (result * 397) ^ RenderType?.GetGUID() ?? result;
                result = (result * 397) ^ Provider?.GUID ?? result;
                result = (result * 397) ^ Model?.GetGUID() ?? result;
                return result;
            }
        }
    }
}
