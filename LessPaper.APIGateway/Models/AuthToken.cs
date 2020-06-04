using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace LessPaper.APIGateway.Models
{
    public class AuthToken
    {
        [Required]
        [JsonPropertyName("token")]
        [ModelBinder(Name = "token")]
        public string Token { get; set; }

        [Required]
        [JsonPropertyName("refresh_token")]
        [ModelBinder(Name = "refresh_token")]
        public string RefreshToken { get; set; }

    }
}
