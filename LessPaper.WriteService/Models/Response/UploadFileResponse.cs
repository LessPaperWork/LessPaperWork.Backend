using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using LessPaper.Shared.Enums;
using LessPaper.Shared.Interfaces.WriteApi.WriteObjectApi;

namespace LessPaper.WriteService.Models.Response
{
    public class UploadFileResponse : IUploadMetadata
    {
        public UploadFileResponse(string fileId, string revisionId, uint quickNumber)
        {
            FileId = fileId;
            RevisionId = revisionId;
            QuickNumber = quickNumber;
        }

        /// <inheritdoc />
        [JsonPropertyName("object_id")]
        public string FileId { get; }

        /// <inheritdoc />
        [JsonPropertyName("revision_id")]
        public string RevisionId { get; }

        /// <inheritdoc />
        [JsonPropertyName("quick_number")]
        public uint QuickNumber { get; }
    }
}
