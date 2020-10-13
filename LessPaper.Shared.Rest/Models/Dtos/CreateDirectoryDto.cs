using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;

namespace LessPaper.Shared.Rest.Models.Dtos
{
    public class CreateDirectoryDto
    {
        [Required]
        [JsonPropertyName("sub_directory_name")]
        [ModelBinder(Name = "sub_directory_name")]
        public string SubDirectoryName { get; set; }
    }
}
