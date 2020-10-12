using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using LessPaper.Shared.Rest.Enums;

namespace LessPaper.Shared.Rest
{
    public interface IBaseClient
    {
        /// <summary>
        /// HTTP Get request
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="requestMethod">HTTP method</param>
        /// <param name="endpoint">Api endpoint i.e. /v1/user</param>
        /// <param name="payload">Data</param>
        /// <param name="pathParameter">Path parameters</param>
        /// <param name="queryParameter">Query parameters</param>
        /// <returns>Result and HTTP status code</returns>
        Task<(TResult, HttpStatusCode)> ExecuteAsync<TResult>(
            HttpRequestMethod requestMethod,
            string endpoint,
            object payload = null,
            Dictionary<string, object> pathParameter = null,
            Dictionary<string, object> queryParameter = null);


        /// <summary>
        /// HTTP Get request
        /// </summary>
        /// <param name="requestMethod">HTTP method</param>
        /// <param name="endpoint">Api endpoint i.e. /v1/user</param>
        /// <param name="payload">Data</param>
        /// <param name="pathParameter">Path parameters</param>
        /// <param name="queryParameter">Query parameters</param>
        /// <returns>HTTP status code</returns>
        Task<HttpStatusCode> ExecuteAsync(
            HttpRequestMethod requestMethod, 
            string endpoint,
            object payload = null,
            Dictionary<string, object> pathParameter = null,
            Dictionary<string, object> queryParameter = null);

    }
}
