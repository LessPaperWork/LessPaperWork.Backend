using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using LessPaper.Shared.Enums;
using LessPaper.Shared.Interfaces.General;
using Microsoft.AspNetCore.Mvc;

namespace LessPaper.Shared.Rest.Models.Dtos
{
    public class FileMetadataDto : MetadataDto, IFileMetadata
    {
        public FileMetadataDto() 
        {
            
        }

        public FileMetadataDto(IFileMetadata data) : base(data)
        {
            Extension = data.Extension;
            ThumbnailId = data.ThumbnailId;
            Revisions = data.Revisions?.Select(x => (IFileRevision)new FileRevisionDto(x)).ToArray();
            Tags = data.Tags?.Select(x => (ITag) new TagDto(x)).ToArray();
        }

        /// <inheritdoc />
        [Required]
        [JsonPropertyName("extension")]
        [ModelBinder(Name = "extension")]
        public ExtensionType Extension { get; set; }

        /// <inheritdoc />
        [Required]
        [JsonPropertyName("thumbnail_id")]
        [ModelBinder(Name = "thumbnail_id")]
        public string ThumbnailId { get; set; }

        /// <inheritdoc />
        [Required]
        [JsonPropertyName("revisions")]
        [ModelBinder(Name = "revisions")]
        public IFileRevision[] Revisions { get; set; }

        /// <inheritdoc />
        [Required]
        [JsonPropertyName("tags")]
        [ModelBinder(Name = "tags")]
        public ITag[] Tags { get; set; }

        /// <inheritdoc />
        [Required]
        [JsonPropertyName("language")]
        [ModelBinder(Name = "language")]
        public DocumentLanguage Language { get; set; }
    }
}
