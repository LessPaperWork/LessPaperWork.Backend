using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Json.Serialization;
using LessPaper.Shared.Interfaces.General;
using Microsoft.AspNetCore.Mvc;

namespace LessPaper.Shared.Rest.Models.Dtos
{
    public class IdentifiableDto : IIdentifiable
    {
        public IdentifiableDto()
        {
            
        }

        public IdentifiableDto(IIdentifiable data)
        {
            ObjectId = data.ObjectId;
        }

        /// <inheritdoc />
        [Required]
        [JsonPropertyName("object_id")]
        [ModelBinder(Name = "object_id")]
        public string ObjectId { get; set; }
    }
}
