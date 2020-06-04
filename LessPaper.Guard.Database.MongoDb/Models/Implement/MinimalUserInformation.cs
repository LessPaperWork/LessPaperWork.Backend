using LessPaper.Guard.Database.MongoDb.Models.Dtos;
using LessPaper.Shared.Interfaces.General;

namespace LessPaper.Guard.Database.MongoDb.Models.Implement
{
    public class MinimalUserInformation : IBasicUserInformation
    {
        private readonly UserDto dto;

        public MinimalUserInformation(UserDto dto)
        {
            this.dto = dto;
        }

        /// <inheritdoc />
        public string UserId => dto.Id;

        /// <inheritdoc />
        public string Email => dto.Email;

        /// <inheritdoc />
        public string PublicKey => dto.PublicKey;

    }
}
