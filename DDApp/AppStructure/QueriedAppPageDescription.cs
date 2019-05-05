using DDApp.AppStructure.Providers;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;

namespace DDApp.AppStructure
{
    public class QueriedAppPageDescription : IAppPageDescription
    {
        private QueriedAppPageDescription() { }

        public static IAppPageDescription Create(
            IAppPageDescription appPageDescription,
            IQueryExpressionProvider queryExpressionProvider,
            IDictionary<string, StringValues> queryDictionary)
        {
            if (queryExpressionProvider == null)
                throw new ArgumentNullException(nameof(queryExpressionProvider));

            if (queryDictionary == null)
                throw new ArgumentNullException(nameof(queryDictionary));

            return new QueriedAppPageDescription
            {
                Title = queryExpressionProvider.GetQueriedExpresion(appPageDescription.Title, queryDictionary),
                TitleIcon = queryExpressionProvider.GetQueriedExpresion(appPageDescription.TitleIcon, queryDictionary),
                TitleImage = queryExpressionProvider.GetQueriedExpresion(appPageDescription.TitleImage, queryDictionary),
                TabGroups = appPageDescription.TabGroups
            };
        }

        public string Title { get; set; }
        public string TitleIcon { get; set; }
        public string TitleImage { get; set; }
        public IDictionary<string, uint> TabGroups { get; set; }
    }
}
