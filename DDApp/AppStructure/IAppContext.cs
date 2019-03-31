using DDApp.AppStructure.Providers;

namespace DDApp.AppStructure
{
    public interface IAppContext
    {
        IRuntimeProvider RuntimeProvider { get; }
        IModelProvider ModelProvider { get; }
    }
}
