using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using LessPaper.Shared.Rest.Interface;
using Microsoft.Extensions.Logging;
using RestSharp;
using System.Text.Json;
using LessPaper.Shared.Rest.Enums;
using RestSharp.Serializers.SystemTextJson;

namespace LessPaper.Shared.Rest
{
    public class RestSharpBaseClient : IBaseClient
    {
        private readonly ILogger<RestSharpBaseClient> logger;
        private readonly IRestClient client;

        public RestSharpBaseClient(IClientSettings clientSettings, ILogger<RestSharpBaseClient> logger)
        {
            this.logger = logger;
            client = new RestClient(clientSettings.BaseUrl).UseSystemTextJson();
           
            if (logger == null)
                throw new Exception("Logger instance is null");
        }

        private void PrettyLogOnDebug(IRestRequest request, IRestResponse response)
        {
            logger.LogTrace($"Entering method {nameof(BuildRestRequest)}");

            if (!logger.IsEnabled(LogLevel.Debug))
                return;

            var requestToLog = new
            {
                Method = request.Method.ToString(),
                Resource = request.Resource,
                Uri = client.BuildUri(request),
                Parameters = request.Parameters.Select(parameter => new
                {
                    name = parameter.Name,
                    value = parameter.Value,
                    type = parameter.Type.ToString()
                }),
                Body = JsonSerializer.Serialize(request.Body.Value),
            };

            var responseToLog = new
            {
                StatusCode = response.StatusCode,
                Headers = response.Headers,
                ResponseUri = response.ResponseUri,
                ErrorMessage = response.ErrorMessage,
                Content = response.Content,
            };

            logger.LogDebug(
                $"REST Request:\n" +
                      $"Request: { JsonSerializer.Serialize(requestToLog)}, \n" +
                      $"Response: { JsonSerializer.Serialize(responseToLog)}");
        }

        private Method MapHttpMethod(HttpRequestMethod method)
        {
            logger.LogTrace($"Entering method {nameof(MapHttpMethod)}");

            switch (method)
            {
                case HttpRequestMethod.Get:
                    return Method.GET;
                case HttpRequestMethod.Post:
                    return Method.POST;
                case HttpRequestMethod.Head:
                    return Method.HEAD;
                case HttpRequestMethod.Put:
                    return Method.PUT;
                case HttpRequestMethod.Patch:
                    return Method.PATCH;
                case HttpRequestMethod.Delete:
                    return Method.DELETE;
                default:
                    throw new ArgumentOutOfRangeException(nameof(method), method, null);
            }
        }

        private RestRequest BuildRestRequest(
            string endpoint,
            Method restMethod,
            object payload,
            Dictionary<string, object> pathParameter,
            Dictionary<string, object> queryParameter)
        {
            logger.LogTrace($"Entering method {nameof(BuildRestRequest)}");

            var request = new RestRequest(endpoint, restMethod);

            if (pathParameter != null)
                foreach (var o in pathParameter)
                    request.AddUrlSegment(o.Key, o.Value);

            if (queryParameter != null)
                foreach (var o in queryParameter)
                    request.AddParameter(o.Key, o.Value);

            if (payload != null)
                request.AddJsonBody(payload);
            
            return request;
        }

        /// <inheritdoc />
        public async Task<(TResult, HttpStatusCode)> ExecuteAsync<TResult>(
            HttpRequestMethod requestMethod,
            string endpoint, 
            object payload = null, 
            Dictionary<string, object> pathParameter = null, 
            Dictionary<string, object> queryParameter = null)
        {
            logger.LogTrace($"Entering method {nameof(ExecuteAsync)} with response payload");

            var request = BuildRestRequest(
                endpoint, 
                MapHttpMethod(requestMethod), 
                payload, 
                pathParameter, 
                queryParameter);

            var result = await client.ExecuteAsync<TResult>(request);
            PrettyLogOnDebug(request, result);
            return (result.Data, result.StatusCode);
        }

        /// <inheritdoc />
        public async Task<HttpStatusCode> ExecuteAsync(
            HttpRequestMethod requestMethod,
            string endpoint, 
            object payload = null, 
            Dictionary<string, object> pathParameter = null, 
            Dictionary<string, object> queryParameter = null)
        {
            logger.LogTrace($"Entering method {nameof(ExecuteAsync)} without response payload");

            var request = BuildRestRequest(
                endpoint, 
                MapHttpMethod(requestMethod), 
                payload, 
                pathParameter, 
                queryParameter);

            var result = await client.ExecuteAsync(request);
            PrettyLogOnDebug(request, result);
            return result.StatusCode;
        }

    }
}