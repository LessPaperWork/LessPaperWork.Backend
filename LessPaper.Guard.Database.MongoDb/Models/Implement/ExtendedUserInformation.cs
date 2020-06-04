using System;
using System.Collections.Generic;
using System.Text;
using LessPaper.Guard.Database.MongoDb.Models.Dtos;
using LessPaper.Shared.Interfaces.General;

namespace LessPaper.Guard.Database.MongoDb.Models.Implement
{
    public class ExtendedUserInformation : MinimalUserInformation, IExtendedUserInformation
    {
        private readonly UserDto dto;

        public ExtendedUserInformation(UserDto dto) : base(dto)
        {
            this.dto = dto;
        }

        /// <inheritdoc />
        public string PasswordHash => dto.PasswordHash;

        /// <inheritdoc />
        public string Salt => dto.Salt;

        /// <inheritdoc />
        public string RootDirectoryId => dto.RootDirectory;

        /// <inheritdoc />
        public string EncryptedPrivateKey => dto.EncryptedPrivateKey;
    }
}
