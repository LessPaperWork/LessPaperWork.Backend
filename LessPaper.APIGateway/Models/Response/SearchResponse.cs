using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using LessPaper.Shared.Interfaces.General;
using LessPaper.Shared.Interfaces.ReadApi.ReadObjectApi;

namespace LessPaper.APIGateway.Models.Response
{
    public class SearchResponse : ISearchResult
    {
        public SearchResponse(ISearchResult searchResponse)
        {
            SearchQuery = searchResponse.SearchQuery;
            Files = searchResponse.Files;
            Directories = searchResponse.Directories;
        }

        /// <inheritdoc />
        [JsonPropertyName("search_query")]
        public string SearchQuery { get; }

        /// <inheritdoc />
        [JsonPropertyName("files")]
        public IFileMetadata[] Files { get; }

        /// <inheritdoc />
        [JsonPropertyName("directories")]
        public IMinimalDirectoryMetadata[] Directories { get; }
    }
}
