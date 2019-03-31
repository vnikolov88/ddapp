using DDApp.Extensions;
using System.Collections.Generic;

namespace DDApp.AppStructure
{
    public interface IDataDrivenAppModule : IGUID
    {
        IDictionary<string, DataDrivenAppDataType> DataTypes { get; set; }
        IDictionary<string, DataDrivenAppPage> Pages { get; set; }
    }
}
