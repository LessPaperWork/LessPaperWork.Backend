using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;

namespace LessPaper.WriteService.Models.Request
{
    public class CreateDirectoryRequest
    {
        [Required]
        [JsonPropertyName("sub_directory_name")]
        [ModelBinder(Name = "sub_directory_name")]
        public string SubDirectoryName { get; set; }
    }
}
