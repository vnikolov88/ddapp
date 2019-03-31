namespace DDApp.DynamicMapping
{
    public interface IReducerContext
    {
        dynamic Execute(object source, object queryContext);
    }
}
