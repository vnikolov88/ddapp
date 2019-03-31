using AutoMapper;
using DDApp.DynamicMapping;
using DDApp.Extensions;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Xml.Serialization;

namespace DDApp.AppStructure.Providers
{
    public class ModelProvider : IModelProvider
    {
        private readonly TimeSpan _getRequestTTL = TimeSpan.FromMinutes(360);
        private readonly TimeSpan _mapperTTL = TimeSpan.FromHours(100);
        private const string RenderModelsNamespace = "DDApp.AppStructure.RenderModels";
        private readonly ILogger _logger;
        private readonly IMemoryCache _memoryCache;
        private readonly IRuntimeProvider _runtimeProvider;

        public IList<string> RenderModels { get; }

        public ModelProvider(
            ILogger logger,
            IMemoryCache memoryCache,
            IRuntimeProvider runtimeProvider)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _memoryCache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));
            _runtimeProvider = runtimeProvider ?? throw new ArgumentNullException(nameof(runtimeProvider));

            RenderModels = Assembly.GetExecutingAssembly().GetTypes().Where(type =>
                type.Namespace == RenderModelsNamespace && type.IsClass && !type.IsNested).Select(type => type.Name).ToList();
        }

        public async Task<dynamic> HydrateAsync(DataDrivenAppComponent component, IDictionary<string, StringValues> query)
        {
            var destinationType = GetDestinationType(component.RenderType);

            // Check if Model is defined inline
            if (component.Provider?.Url == null && component.Model != null)
                return JsonConvert.DeserializeObject(component.Model, destinationType);

            var sourceType = GetSourceType(component.Provider?.Type);

            // Load external data and map data provider model to render model
            var providerUrl = GetProviderUrl(component.Provider, query);
            var response = await GetAsync(providerUrl, sourceType);

            var queryContext = query.CreateQueryContext();

            // Execute reducer
            if (component.Provider?.Reducer != null)
            {
                var reducerContext = await GetReducerContextAsync(component.Provider, sourceType);
                response = reducerContext.Execute(response, queryContext);
                if (response == null) return Activator.CreateInstance(destinationType);
                sourceType = response.GetType();
            }

            // Build custom mapper of get the default one
            var mapper = await GetCustomMapperAsync(component.Provider, sourceType, destinationType)
                .ConfigureAwait(false);

            return mapper.MapWithQueryContext(
                response,
                sourceType,
                destinationType,
                queryContext
                );
        }

        public void BuildAndCacheMappings(DataDrivenApp app)
        {
#if !DEBUG
            var emptyQuery = new Dictionary<string, StringValues>();
            Parallel.ForEach(app.Pages, page =>
            {
               foreach (var component in page.Value.Components)
               {
                   try
                   {
                       HydrateAsync(component, emptyQuery).GetAwaiter().GetResult();
                   }
                   catch (Exception ex)
                   {
                       _logger.LogWarning(ex, $"Unable to pre-cache component in {page.Key}");
                   }
               }
            });
#endif
        }

        private IMapper BuildCustomMapper(
            DataProvider provider,
            Type providerSourceType,
            Type providerDestinationType)
        {
            var config = new MapperConfiguration(cfg => {
                cfg.CreateMissingTypeMaps = true;
                cfg.Advanced.AllowAdditiveTypeMapCreation = true;
                // Create root map
                cfg.CreateProfile(provider.GUID.ToString(), async profile => {
                    profile.CreateMap(providerSourceType, providerDestinationType, MemberList.None);
                    
                    foreach (var mapping in provider.Mapper)
                    {
                        var sourceType = GetSourceType(mapping.SourceType);
                        var destinationType = GetDestinationType(mapping.DestinationType);

                        // Create custom mappings and root map for sub-types
                        await profile.CreateMapAsync(mapping, sourceType, destinationType, _runtimeProvider);
                    }
                });

            });
            config.AssertConfigurationIsValid(provider.GUID.ToString());
            config.CompileMappings();

            return config.CreateMapper();
        }

#region Auxiliary methods

        private Type GetDestinationType(string destinationType)
        {
            var typePath = $"{RenderModelsNamespace}.{destinationType?.Replace('.', '+')}";
            return Type.GetType(typePath) ?? throw new ArgumentNullException(nameof(destinationType), typePath);
        }

        private Type GetSourceType(string sourceType) => _runtimeProvider.GetTypeInApp(sourceType);

        private async Task<IMapper> GetCustomMapperAsync(
            DataProvider provider,
            Type sourceType,
            Type destinationType)
        {
            if (provider.Mapper == null)
                return Mapper.Instance;

            var response = await _memoryCache.GetOrCreateAsync($"{provider.GUID}:{sourceType.GetGUID()}:{destinationType.GetGUID()}", entry =>
            {
                entry.SlidingExpiration = _mapperTTL;

                return Task.FromResult(BuildCustomMapper(provider, sourceType, destinationType));
            });

            return response;
        }
        
        private async Task<IReducerContext> GetReducerContextAsync(DataProvider provider, Type instanceType)
        {
            if (string.IsNullOrWhiteSpace(provider.Reducer))
                return null;

            var response = await _memoryCache.GetOrCreateAsync($"{provider.Reducer.GetGUID()}:{instanceType.GetGUID()}", async entry =>
            {
                entry.SlidingExpiration = _mapperTTL;

                return await provider.CreateReducerContextAsync(instanceType, _runtimeProvider);
            });

            return response;
        }

        private Uri GetProviderUrl(DataProvider provider,
            IDictionary<string, StringValues> query)
        {
            var routeString = GetQueriedExpresion(provider.Url, query);
            return new Uri(routeString);
        }

        private string GetValueUsingFormater(string queryValue, string formatter)
        {
            switch (formatter)
            {
                case "base64":
                    return queryValue.Base64Encode();
                case null:
                    return queryValue;
                default:
                    throw new ArgumentException($"Trying to apply unknown formatter [{formatter}] to value [{queryValue}]");
            }
        }

        private string GetQueriedExpresion(string template,
            IDictionary<string, StringValues> query)
        {
            Match match;
            var result = new string(template);
            while ((match = Regex.Match(result, "{{[^{}]+}}")) != Match.Empty)
            {
                var components = Regex.Matches(match.Value, "[^{}:]+");
                var identifier = components.Last().Value;
                var formatter = components.Count == 2 ? components.First().Value: null;

                if (!query.ContainsKey(identifier))
                    throw new ArgumentException($"Missing parameter from query context in [{template}]",
                        identifier, new Exception($"Current match state [{result}]"));

                var value = GetValueUsingFormater(query[identifier], formatter);
                result = result.Replace(match.Value, value);
            }

            return result;
        }

        private object GetFromQueryString(string queryString, Type dataType)
        {
            var obj = Activator.CreateInstance(dataType);
            var properties = dataType.GetProperties();
            var values = HttpUtility.ParseQueryString(queryString);
            foreach (var property in properties)
            {
                var valueAsString = values.Get(property.Name);
                var value = Parse(valueAsString, property.PropertyType);

                if (value == null)
                    continue;

                property.SetValue(obj, value, null);
            }
            return obj;
        }

        private object Parse(string valueToConvert, Type dataType)
        {
            TypeConverter obj = TypeDescriptor.GetConverter(dataType);
            object value = obj.ConvertFromString(null, CultureInfo.InvariantCulture, valueToConvert);
            return value;
        }

        private async Task<object> GetAsync(Uri uri, Type responseType)
        {
            var response = await _memoryCache.GetOrCreateAsync($"{uri.AbsoluteUri}:{responseType.GetGUID()}", async entry =>
            {
                entry.SlidingExpiration = _getRequestTTL;
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
                request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

                try
                {
                    return await GetResponseAsync(request, responseType);
                }
                catch(Exception ex)
                {
                    // Run validations
                    var matches = Regex.Matches(uri.OriginalString, "{[^{}]+}");
                    if (matches.Count > 0)
                        throw new ArgumentException($"Unable to get data for [{uri.OriginalString}] did you forget to add double brackets ?", string.Join(",", matches.Select(x => x.Value)));

                    throw new WebException($"Unable to get data for [ {uri.OriginalString} ]", ex);
                }
            });

            return response;
        }

        private async Task<object> PostAsync(Uri uri, string data, string contentType, Type responseType, string method = "POST")
        {
            byte[] dataBytes = Encoding.UTF8.GetBytes(data);

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
            request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
            request.ContentLength = dataBytes.Length;
            request.ContentType = contentType;
            request.Method = method;

            using (Stream requestBody = request.GetRequestStream())
            {
                await requestBody.WriteAsync(dataBytes, 0, dataBytes.Length);
            }
            
            return await GetResponseAsync(request, responseType);
        }

        private async Task<object> GetResponseAsync(HttpWebRequest request, Type responseType)
        {
            using (HttpWebResponse response = (HttpWebResponse)await request.GetResponseAsync())
            using (Stream stream = response.GetResponseStream())
            {
                object result;
                if (stream != null && response.ContentType.Contains("xml")) // application/xml
                {
                    var deserializer = new XmlSerializer(responseType);
                    result = deserializer.Deserialize(stream);
                }
                else if (stream != null && response.ContentType.Contains("json")) // application/json
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        var responseContent = await reader.ReadToEndAsync();
                        result = JsonConvert.DeserializeObject(responseContent, responseType);
                    }
                }
                else if (stream != null && response.ContentType.Contains("form")
                ) // multipart/form-data or application/x-www-form-urlencoded
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        var responseContent = await reader.ReadToEndAsync();
                        result = GetFromQueryString(responseContent, responseType);
                    }
                }
                else // Default empty object
                {
                    result = Activator.CreateInstance(responseType);
                }
                return result;
            }
        }

#endregion Auxiliary methods
    }

}
