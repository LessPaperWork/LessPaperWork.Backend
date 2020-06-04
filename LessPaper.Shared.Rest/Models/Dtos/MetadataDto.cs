using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Json.Serialization;
using LessPaper.Shared.Enums;
using LessPaper.Shared.Interfaces.General;
using Microsoft.AspNetCore.Mvc;

namespace LessPaper.Shared.Rest.Models.Dtos
{
     public class MetadataDto : IdentifiableDto, IMetadata
    {
        public MetadataDto()
        {
            
        }

        public MetadataDto(IMetadata data) : base(data)
        {
            ObjectName = data.ObjectName;
            Permissions = data.Permissions;
            Path = data.Path;
        }

        /// <inheritdoc />
        [Required]
        [JsonPropertyName("object_name")]
        [ModelBinder(Name = "object_name")]
        public string ObjectName { get; set; }

        /// <inheritdoc />
        [Required]
        [JsonPropertyName("permission")]
        [ModelBinder(Name = "permission")]
        public Dictionary<string, Permission> Permissions { get; set; }

        /// <inheritdoc />
        [Required]
        [JsonPropertyName("path")]
        [ModelBinder(Name = "path")]
        public string Path { get; set; }
    }
}
