using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using LessPaper.Shared.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LessPaper.WriteService.Models.Request
{
    public class UploadFileRequest 
    {
        [Required]
        [JsonPropertyName("name")]
        [ModelBinder(Name = "name")]
        public string Name { get; set; }

        [Required]
        [JsonPropertyName("plaintext_key")]
        [ModelBinder(Name = "plaintext_key")]
        public string PlaintextKey { get; set; }

        [Required]
        [JsonPropertyName("encrypted_key")]
        [ModelBinder(Name = "encrypted_key")]
        public string EncryptedKey { get; set; }

        [Required]
        [JsonPropertyName("document_language")]
        [ModelBinder(Name = "document_language")]
        public DocumentLanguage DocumentLanguage { get; set; }

        [Required]
        [JsonPropertyName("file")]
        [ModelBinder(Name = "file")]
        public IFormFile File { get; set; }


    }
}
