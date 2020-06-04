using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Json.Serialization;
using LessPaper.Shared.Enums;
using LessPaper.Shared.Interfaces.GuardApi.Response;
using Microsoft.AspNetCore.Mvc;

namespace LessPaper.Shared.Rest.Models.Dtos
{
    public class PermissionDto : IdentifiableDto, IPermissionResponse
    {
        public PermissionDto()
        {
            
        }

        public PermissionDto(IPermissionResponse data) : base(data)
        {
            Permission = data.Permission;
        }
        
        /// <inheritdoc />
        [Required]
        [JsonPropertyName("permission")]
        [ModelBinder(Name = "permission")]
        public Permission Permission { get; set; }
    }
}
