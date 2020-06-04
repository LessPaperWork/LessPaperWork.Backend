using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using LessPaper.Shared.Enums;
using LessPaper.Shared.Interfaces.General;
using LessPaper.Shared.Interfaces.ReadApi.ReadObjectApi;

namespace LessPaper.APIGateway.Models.Response
{
    public class FileMetadataResponse : ObjectResponse, IFileMetadata
    {
        public FileMetadataResponse(IFileMetadata fileMetadata) : base(fileMetadata)
        {
            Revisions = fileMetadata.Revisions;
            Extension = fileMetadata.Extension;
            ThumbnailId = fileMetadata.ThumbnailId;
            Revisions = fileMetadata.Revisions;
            Tags = fileMetadata.Tags;
            Language = fileMetadata.Language;
        }


        /// <inheritdoc />
        [JsonPropertyName("extension")]
        public ExtensionType Extension { get; }

        /// <inheritdoc />
        [JsonPropertyName("thumbnail_id")]
        public string ThumbnailId { get; }

        /// <inheritdoc />
        [JsonPropertyName("revisions")]
        public IFileRevision[] Revisions { get; }
        
        /// <inheritdoc />
        [JsonPropertyName("tags")]
        public ITag[] Tags { get; }

        /// <inheritdoc />
        [JsonPropertyName("language")]
        public DocumentLanguage Language { get; }
    }

    public class Tag : ITag
    {
        public Tag(ITag tag)
        {
            Value = tag.Value;
            Relevance = tag.Relevance;
            Source = tag.Source;
        }

        /// <inheritdoc />
        [JsonPropertyName("value")]
        public string Value { get; }

        /// <inheritdoc />
        [JsonPropertyName("relevance")]
        public float Relevance { get; }

        /// <inheritdoc />
        [JsonPropertyName("source")]
        public TagSource Source { get; }
    }
}
