using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using LessPaper.Shared.Interfaces.WriteApi.WriteObjectApi;
using Microsoft.AspNetCore.Mvc;

namespace LessPaper.APIGateway.Models.Request
{
    public class UpdateFileMetaDataRequest : IMetadataUpdate
    {
        [Required]
        [JsonPropertyName("object_name")]
        [ModelBinder(Name = "object_name")]
        public string ObjectName { get; set; }
        
        [Required]
        [JsonPropertyName("parent_directory_ids")]
        [ModelBinder(Name = "parent_directory_ids")]
        public string[] ParentDirectoryIds { get; set; }
    }
}
