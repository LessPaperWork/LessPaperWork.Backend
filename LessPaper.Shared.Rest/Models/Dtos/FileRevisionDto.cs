using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Json.Serialization;
using LessPaper.Shared.Interfaces.General;
using Microsoft.AspNetCore.Mvc;

namespace LessPaper.Shared.Rest.Models.Dtos
{
    public class FileRevisionDto : IdentifiableDto, IFileRevision
    {
        public FileRevisionDto()
        {
            
        }

        public FileRevisionDto(IFileRevision data) : base(data)
        {
            QuickNumber = data.QuickNumber;
            SizeInBytes = data.SizeInBytes;
            ChangeDate = data.ChangeDate;
            AccessKeys = data.AccessKeys;
        }

        /// <inheritdoc />
        [Required]
        [JsonPropertyName("quick_number")]
        [ModelBinder(Name = "quick_number")]
        public uint QuickNumber { get; set; }

        /// <inheritdoc />
        [Required]
        [JsonPropertyName("size_in_bytes")]
        [ModelBinder(Name = "size_in_bytes")]
        public uint SizeInBytes { get; set; }

        /// <inheritdoc />
        [Required]
        [JsonPropertyName("change_date")]
        [ModelBinder(Name = "change_date")]
        public DateTime ChangeDate { get; set; }

        /// <inheritdoc />
        [Required]
        [JsonPropertyName("access_keys")]
        [ModelBinder(Name = "access_keys")]
        public Dictionary<string, IAccessKey> AccessKeys { get; set; }
    }
}
