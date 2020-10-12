using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Json.Serialization;
using LessPaper.Shared.Interfaces.General;
using Microsoft.AspNetCore.Mvc;

namespace LessPaper.Shared.Rest.Models.Dtos
{
    public class ExtendedUserInformationDto : BasicUserInformationDto, IExtendedUserInformation
    {
        public ExtendedUserInformationDto()
        {
        }

        public ExtendedUserInformationDto(IExtendedUserInformation data) : base(data)
        {
            PasswordHash = data.PasswordHash;
            Salt = data.Salt;
            RootDirectoryId = data.RootDirectoryId;
            EncryptedPrivateKey = data.EncryptedPrivateKey;
        }


        /// <inheritdoc />
        [Required]
        [JsonPropertyName("password_hash")]
        [ModelBinder(Name = "password_hash")]
        public string PasswordHash { get; set; }

        /// <inheritdoc />
        [Required]
        [JsonPropertyName("salt")]
        [ModelBinder(Name = "salt")]
        public string Salt { get; set; }

        /// <inheritdoc />
        [Required]
        [JsonPropertyName("root_directory_id")]
        [ModelBinder(Name = "root_directory_id")]
        public string RootDirectoryId { get; set; }

        /// <inheritdoc />
        [Required]
        [JsonPropertyName("encrypted_private_key")]
        [ModelBinder(Name = "encrypted_private_key")]
        public string EncryptedPrivateKey { get; set; }
    }
}
