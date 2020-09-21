using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using LessPaper.Shared.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LessPaper.Shared.Rest.Models.RequestDtos
{
    public class UploadFileDto
    {
        [Required]
        [JsonPropertyName("plaintext_key")]
        [ModelBinder(Name = "plaintext_key")]
        public string PlaintextKey { get; set; }

        [Required]
        [JsonPropertyName("filename")]
        [ModelBinder(Name = "filename")]
        public string FileName { get; set; }
        
        [Required]
        [JsonPropertyName("encrypted_key")]
        [ModelBinder(Name = "encrypted_key")]
        public Dictionary<string, string> EncryptedKey { get; set; }

        [Required]
        [JsonPropertyName("document_language")]
        [ModelBinder(Name = "document_language")]
        public DocumentLanguage DocumentLanguage { get; set; }

        [Required]
        [JsonPropertyName("file_extension")]
        [ModelBinder(Name = "file_extension")]
        public ExtensionType FileExtension { get; set; }

        [Required]
        [JsonPropertyName("file")]
        [ModelBinder(Name = "file")]
        public IFormFile File { get; set; }
    }
}
