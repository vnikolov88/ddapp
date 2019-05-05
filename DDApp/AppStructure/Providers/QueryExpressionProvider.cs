using DDApp.Extensions;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace DDApp.AppStructure.Providers
{
    public class QueryExpressionProvider : IQueryExpressionProvider
    {
        /// <summary>
        /// Get a formated expresion from a template and a query context,
        /// [Valid template components]
        /// {{formater:param}}
        /// {{param}}
        /// {{param?}}
        /// {{formater:param?}}
        /// </summary>
        /// <param name="template">the template for the url</param>
        /// <param name="query">the query context of the current page</param>
        /// <returns>a valid URL</returns>
        public string GetQueriedExpresion(string template,
            IDictionary<string, StringValues> query)
        {
            Match match;
            var result = new string(template);
            while ((match = Regex.Match(result, "{{[^{}]+}}")) != Match.Empty)
            {
                var value = string.Empty;
                var components = Regex.Matches(match.Value, "[^{}:]+");
                var identifier = components.Last().Value.TrimEnd('?');
                var isOptional = components.Last().Value.EndsWith('?');
                var formatter = components.Count == 2 ? components.First().Value : null;

                if (!query.ContainsKey(identifier))
                {
                    if (!isOptional)
                    {
                        throw new ArgumentException($"Missing parameter from query context in [{template}]",
                            identifier, new Exception($"Current match state [{result}]"));
                    }
                }
                else
                {
                    value = GetValueUsingFormater(query[identifier], formatter);
                }
                result = result.Replace(match.Value, value);
            }

            return result;
        }

        private string GetValueUsingFormater(string queryValue, string formatter)
        {
            switch (formatter)
            {
                case "base64":
                    return queryValue.Base64Encode();
                case "base64_decode":
                    return queryValue.Base64Decode();
                case null:
                    return queryValue;
                default:
                    throw new ArgumentException($"Trying to apply unknown formatter [{formatter}] to value [{queryValue}]");
            }
        }
    }
}
