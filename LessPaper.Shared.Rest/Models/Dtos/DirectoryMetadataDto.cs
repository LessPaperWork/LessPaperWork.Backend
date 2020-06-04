using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using LessPaper.Shared.Interfaces.General;
using Microsoft.AspNetCore.Mvc;

namespace LessPaper.Shared.Rest.Models.Dtos
{
    public class DirectoryMetadataDto : MinimalDirectoryMetadataDto, IDirectoryMetadata
    {
        public DirectoryMetadataDto()
        {
            
        }

        public DirectoryMetadataDto(IDirectoryMetadata data)
        {
            FileChilds = data?.FileChilds.Select(x => (IFileMetadata) new FileMetadataDto(x)).ToArray();
            DirectoryChilds = data?.DirectoryChilds.Select(x => (IMinimalDirectoryMetadata)new MinimalDirectoryMetadataDto(x)).ToArray();
        }

        /// <inheritdoc />
        [Required]
        [JsonPropertyName("file_childs")]
        [ModelBinder(Name = "file_childs")]
        public IFileMetadata[] FileChilds { get; set; }

        /// <inheritdoc />
        [Required]
        [JsonPropertyName("directory_childs")]
        [ModelBinder(Name = "directory_childs")]
        public IMinimalDirectoryMetadata[] DirectoryChilds { get; set; }
    }
}
