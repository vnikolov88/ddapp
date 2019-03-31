using DDApp.Extensions;

namespace DDApp.AppStructure
{
    public interface IDataDrivenAppComponent : IGUID
    {
        string RenderType { get; set; }
        DataProvider Provider { get; set; }
        string Model { get; set; }
    }
}
