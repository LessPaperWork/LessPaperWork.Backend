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
    public class TagDto : ITag
    {
        public TagDto()
        {
        }

        public TagDto(ITag data)
        {
            Value = data.Value;
            Relevance = data.Relevance;
            Source = data.Source;
        }

        /// <inheritdoc />
        [Required]
        [JsonPropertyName("value")]
        [ModelBinder(Name = "value")]
        public string Value { get; set; }

        /// <inheritdoc />
        [Required]
        [JsonPropertyName("relevance")]
        [ModelBinder(Name = "relevance")]
        public float Relevance { get; set; }

        /// <inheritdoc />
        [Required]
        [JsonPropertyName("source")]
        [ModelBinder(Name = "source")]
        public TagSource Source { get; set; }
    }
}
