using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using LessPaper.Shared.Enums;
using LessPaper.Shared.Interfaces.General;
using LessPaper.Shared.Interfaces.WriteApi.WriteObjectApi;

namespace LessPaper.APIGateway.Models.Response
{
    public class UploadFileResponse : IUploadMetadata
    {
        public UploadFileResponse(IUploadMetadata uploadMetadata)
        {
            ObjectName = uploadMetadata.ObjectName;
            ObjectId = uploadMetadata.ObjectId;
            QuickNumber = uploadMetadata.QuickNumber;
        }

        /// <inheritdoc />
        [JsonPropertyName("object_name")]
        public string ObjectName { get; }

        /// <inheritdoc />
        public Dictionary<string, Permission> Permissions { get; set; }

        /// <inheritdoc />
        public string Path { get; set; }

        /// <inheritdoc />
        [JsonPropertyName("object_id")]
        public string ObjectId { get; }

        /// <inheritdoc />
        [JsonPropertyName("quick_number")]
        public uint QuickNumber { get; }
    }
}
