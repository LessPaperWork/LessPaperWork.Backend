using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;

namespace LessPaper.Shared.Rest.Models.RequestDtos
{
    public class GetObjectsPermissionDto
    {
        public GetObjectsPermissionDto()
        {
            
        }

        public GetObjectsPermissionDto(string userId, string[] objectIds)
        {
            UserId = userId;
            ObjectIds = objectIds;
        }

        [Required]
        [JsonPropertyName("user_id")]
        [ModelBinder(Name = "user_id")]
        public string UserId { get; set; }

        [Required]
        [JsonPropertyName("object_ids")]
        [ModelBinder(Name = "object_ids")]
        public string[] ObjectIds { get; set; }

    }
}
