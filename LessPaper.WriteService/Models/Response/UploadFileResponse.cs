using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using LessPaper.Shared.Enums;
using LessPaper.Shared.Interfaces.WriteApi.WriteObjectApi;

namespace LessPaper.WriteService.Models.Response
{
    public class UploadFileResponse : IUploadMetadata
    {
        public UploadFileResponse(string objectName, string objectId, uint quickNumber)
        {
            ObjectName = objectName;
            ObjectId = objectId;
            QuickNumber = quickNumber;
        }

        /// <inheritdoc />
        [JsonPropertyName("object_name")]
        public string ObjectName { get; }

        /// <inheritdoc />
        public Dictionary<string, Permission> Permissions { get;}

        /// <inheritdoc />
        public string Path { get; }

        /// <inheritdoc />
        [JsonPropertyName("object_id")]
        public string ObjectId { get; }
        
        /// <inheritdoc />
        [JsonPropertyName("quick_number")]
        public uint QuickNumber { get; }
    }
}
