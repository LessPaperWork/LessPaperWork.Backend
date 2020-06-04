using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;

namespace LessPaper.APIGateway.Models.Request
{
    public class UserLoginRequest
    {
        [Required]
        [JsonPropertyName("email")]
        [ModelBinder(Name = "email")]
        public string Email { get; set; }

        [Required]
        [JsonPropertyName("password")]
        [ModelBinder(Name = "password")]
        public string Password { get; set; }
    }
}
