using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using LessPaper.Shared.Interfaces.General;
using Microsoft.AspNetCore.Mvc;

namespace LessPaper.Shared.Rest.Models.Dtos
{
    public class BasicUserInformationDto : MessageDto, IBasicUserInformation
    {
        public BasicUserInformationDto()
        {
            
        }
        public BasicUserInformationDto(IBasicUserInformation data)
        {
            UserId = data.UserId;
            Email = data.Email;
            PublicKey = data.PublicKey;
        }

        [Required]
        [JsonPropertyName("user_id")]
        [ModelBinder(Name = "user_id")]
        public string UserId { get; set; }

        /// <inheritdoc />
        [Required]
        [JsonPropertyName("email")]
        [ModelBinder(Name = "email")]
        public string Email { get; set; }
        
        /// <inheritdoc />
        [Required]
        [JsonPropertyName("public_key")]
        [ModelBinder(Name = "public_key")]
        public string PublicKey { get; set; }
    }
}
