using System;

namespace DDApp.DynamicMapping
{
    public class MappingContext<TSource>
    {
        public dynamic _GetExecutionTree(TSource SourceContext, object QueryContext, Func<TSource, dynamic> mappingFunc)
        {
            return mappingFunc(SourceContext);
        }

        public dynamic _GetExecutionTree(TSource SourceContext, object QueryContext, Func<TSource, dynamic, dynamic> mappingFunc)
        {
            return mappingFunc(SourceContext, QueryContext);
        }
    }
}
