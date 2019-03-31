using DDApp.Extensions;
using System.Collections.Generic;

namespace DDApp.AppStructure
{
    public interface IDataDrivenAppDataTypeProperty : IGUID
    {
        string Type { get; set; }
        IList<string> Attributes { get; set; }
    }
}
