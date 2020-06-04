using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Json.Serialization;
using LessPaper.Shared.Interfaces.General;
using Microsoft.AspNetCore.Mvc;

namespace LessPaper.Shared.Rest.Models.Dtos
{
    public class MinimalDirectoryMetadataDto : MetadataDto, IMinimalDirectoryMetadata
    {
        public MinimalDirectoryMetadataDto()
        {
            
        }

        public MinimalDirectoryMetadataDto(IMinimalDirectoryMetadata data)
        {
            NumberOfChilds = data.NumberOfChilds;
        }

        /// <inheritdoc />
        [Required]
        [JsonPropertyName("number_of_childs")]
        [ModelBinder(Name = "number_of_childs")]
        public uint NumberOfChilds { get; set; }
    }
}
