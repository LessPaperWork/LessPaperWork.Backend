using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;

namespace LessPaper.Shared.Rest.Models.Dtos
{


    public class MessageDto
    {
        public MessageDto(string message)
        {
            Message = message;
        }

        public MessageDto()
        {
        }

        [Required]
        [JsonPropertyName("server_message")]
        [ModelBinder(Name = "server_message")]
        public string Message { get; }
    }
}
