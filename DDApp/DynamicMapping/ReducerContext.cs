using System;

namespace DDApp.DynamicMapping
{
    public class ReducerContext<TSource> : IReducerContext
    {
        private readonly Func<TSource, object, object> _reducer;

        public ReducerContext(Func<TSource, object, object> reducer)
        {
            _reducer = reducer ?? throw new ArgumentNullException(nameof(reducer));
        }

        public dynamic Execute(object source, object queryContext) => _reducer((TSource)source, queryContext);
    }
}
