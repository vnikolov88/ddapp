using DDApp.Extensions;
using System.Collections.Generic;

namespace DDApp.AppStructure
{
    public interface IDataDrivenAppDataType : IGUID
    {
        //string Name { get; set; }
        IList<string> Attributes { get; set; }
        IDictionary<string, DataDrivenAppDataTypeProperty> Properties { get; set; }
        IDictionary<string, string> Methods { get; set; }
    }
}
