using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using LessPaper.Shared.Interfaces.General;
using LessPaper.Shared.Interfaces.ReadApi.ReadObjectApi;

namespace LessPaper.APIGateway.Models.Response
{
    public class MinimalDirectoryMetadataResponse : ObjectResponse, IMinimalDirectoryMetadata
    {
        /// <inheritdoc />
        public MinimalDirectoryMetadataResponse(IMinimalDirectoryMetadata minimalDirectoryMetadata) : base(minimalDirectoryMetadata)
        {
            NumberOfChilds = minimalDirectoryMetadata.NumberOfChilds;
        }

        /// <inheritdoc />
        [JsonPropertyName("number_of_childs")]
        public uint NumberOfChilds { get; }
    }
}
