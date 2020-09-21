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
            QuickNumber = uploadMetadata.QuickNumber;
            FileId = uploadMetadata.FileId;
            RevisionId = uploadMetadata.RevisionId;
        }
        
        /// <inheritdoc />
        [JsonPropertyName("quick_number")]
        public uint QuickNumber { get; }

        /// <inheritdoc />
        public string FileId { get; set; }

        /// <inheritdoc />
        public string RevisionId { get; set; }
    }
}
