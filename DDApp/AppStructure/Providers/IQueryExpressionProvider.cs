using Microsoft.Extensions.Primitives;
using System.Collections.Generic;

namespace DDApp.AppStructure.Providers
{
    public interface IQueryExpressionProvider
    {
        string GetQueriedExpresion(string template, IDictionary<string, StringValues> query);
    }
}
