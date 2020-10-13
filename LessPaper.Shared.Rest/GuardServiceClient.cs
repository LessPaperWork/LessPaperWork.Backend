using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using LessPaper.Shared.Enums;
using LessPaper.Shared.Helper;
using LessPaper.Shared.Interfaces.General;
using LessPaper.Shared.Interfaces.GuardApi;
using LessPaper.Shared.Interfaces.GuardApi.Response;
using LessPaper.Shared.Rest.Enums;
using LessPaper.Shared.Rest.Exceptions;
using LessPaper.Shared.Rest.Models.Dtos;
using LessPaper.Shared.Rest.Models.RequestDtos;
using Microsoft.Extensions.Logging;

namespace LessPaper.Shared.Rest
{
    public class GuardServiceClient : IGuardApi
    {
        private readonly IBaseClient client;
        private readonly ILogger<GuardServiceClient> logger;

        public GuardServiceClient(IBaseClient client, ILogger<GuardServiceClient> logger)
        {
            this.client = client;
            this.logger = logger;
        }

        /// <inheritdoc />
        public async Task<bool> RegisterNewUser(string email, string passwordHash, string salt, string userId, string publicKey,
            string encryptedPrivateKey)
        {
            logger.LogTrace($"Entering method {nameof(GuardServiceClient) + "." + nameof(RegisterNewUser)}");

            var (message, statusCode) = await client.ExecuteAsync<MessageDto>(
                HttpRequestMethod.Post,
                "v1/user",
                new UserCreationDto
                {
                    Email = email,
                    HashedPassword = passwordHash,
                    Salt = salt,
                    UserId = userId,
                    PublicKey = publicKey,
                    EncryptedPrivateKey = encryptedPrivateKey
                });

            if (statusCode != HttpStatusCode.Created)
                throw new InvalidStatusCodeException(HttpStatusCode.Created, statusCode, message);

            return true;
        }

        /// <inheritdoc />
        public async Task<IExtendedUserInformation> GetUserInformation(string email)
        {
            logger.LogTrace($"Entering method {nameof(GuardServiceClient) + "." + nameof(GetUserInformation)}");

            var (result, statusCode) =
                await client.ExecuteAsync<ExtendedUserInformationDto>(
                    HttpRequestMethod.Get,
                        "v1/user/{email}",
                        pathParameter: new Dictionary<string, object>
                            {
                                { "email", email }
                            }
                    );

            if (statusCode != HttpStatusCode.OK)
                throw new InvalidStatusCodeException(HttpStatusCode.OK, statusCode);

            return result;
        }

        /// <inheritdoc />
        public async Task<IPermissionResponse[]> GetObjectsPermissions(string requestingUserId, string userId, string[] objectIds)
        {
            logger.LogTrace($"Entering method {nameof(GuardServiceClient) + "." + nameof(GetObjectsPermissions)}");

            var (result, statusCode) =
                await client.ExecuteAsync<PermissionDto[]>(
                    HttpRequestMethod.Get,
                    "v1/permissions/{requestingUserId}",
                    new GetObjectsPermissionDto
                    {
                        ObjectIds = objectIds,
                        UserId = userId
                    },
                    new Dictionary<string, object>
                    {
                        { "requestingUserId", requestingUserId }
                    }
                );

            if (statusCode != HttpStatusCode.OK)
                throw new InvalidStatusCodeException(HttpStatusCode.OK, statusCode);

            return result.Cast<IPermissionResponse>().ToArray();
        }

        /// <inheritdoc />
        public async Task<string> AddDirectory(string requestingUserId, string parentDirectoryId, string directoryName)
        {
            logger.LogTrace($"Entering method {nameof(GuardServiceClient) + "." + nameof(GetObjectsPermissions)}");

            var (result, statusCode) =
                await client.ExecuteAsync<string>(
                    HttpRequestMethod.Post,
                    "v1/Objects/directories/{parentDirectoryId}/{directoryName}",
                    null,
                    new Dictionary<string, object>
                    {
                        { "parentDirectoryId", parentDirectoryId },
                        { "directoryName", directoryName }
                    },
                    new Dictionary<string, object>
                    {
                        { "requestingUserId", requestingUserId }
                    }
                );

            if (statusCode != HttpStatusCode.OK)
                throw new InvalidStatusCodeException(HttpStatusCode.OK, statusCode);

            return result;
        }

        /// <inheritdoc />
        public async Task<string[]> DeleteObject(string requestingUserId, string objectId)
        {
            logger.LogTrace($"Entering method {nameof(GuardServiceClient) + "." + nameof(DeleteObject)}");

            var (result, statusCode) =
                await client.ExecuteAsync<string[]>(
                    HttpRequestMethod.Delete,
                    "v1/Objects/{objectId}",
                    null,
                    new Dictionary<string, object>
                    {
                        { "objectId", objectId }
                    },
                    new Dictionary<string, object>
                    {
                        { "requestingUserId", requestingUserId }
                    }
                );

            if (statusCode != HttpStatusCode.OK)
                throw new InvalidStatusCodeException(HttpStatusCode.OK, statusCode);

            return result;
        }

        /// <inheritdoc />
        public async Task<int> AddFile(string requestingUserId, string directoryId, string fileId, string blobId, string fileName, int fileSize,
            Dictionary<string, string> encryptedKey, DocumentLanguage documentLanguage, ExtensionType fileExtension)
        {
            logger.LogTrace($"Entering method {nameof(GuardServiceClient) + "." + nameof(AddFile)}");

            var (result, statusCode) =
                await client.ExecuteAsync<int>(
                    HttpRequestMethod.Post,
                    "v1/Objects/files/{directoryId}/{fileId}/{revisionId}",
                    new AddFileDto
                    {
                        DocumentLanguage = documentLanguage,
                        EncryptedKey = encryptedKey,
                        FileExtension = fileExtension,
                        FileName = fileName,
                        FileSize = (uint)fileSize
                    },
                    new Dictionary<string, object>
                    {
                        { "directoryId", directoryId },
                        { "fileId", fileId },
                        { "revisionId", blobId }
                    },
                    new Dictionary<string, object>
                    {
                        { "requestingUserId", requestingUserId }
                    }
                );

            if (statusCode != HttpStatusCode.Created)
                throw new InvalidStatusCodeException(HttpStatusCode.Created, statusCode);

            return result;
        }

        /// <inheritdoc />
        public async Task<bool> MoveObject(string requestingUserId, string objectId, string newParentDirectoryId)
        {
            logger.LogTrace($"Entering method {nameof(GuardServiceClient) + "." + nameof(MoveObject)}");

            var statusCode =
                await client.ExecuteAsync(
                    HttpRequestMethod.Delete,
                    "v1/Objects/{objectId}/{newParentDirectoryId}",
                    null,
                    new Dictionary<string, object>
                    {
                        { "objectId", objectId },
                        { "newParentDirectoryId", newParentDirectoryId }
                    },
                    new Dictionary<string, object>
                    {
                        { "requestingUserId", requestingUserId }
                    }
                );

            if (statusCode != HttpStatusCode.OK)
                throw new InvalidStatusCodeException(HttpStatusCode.OK, statusCode);

            return true;
        }

        /// <inheritdoc />
        public async Task<bool> RenameObject(string requestingUserId, string objectId, string newName)
        {
            logger.LogTrace($"Entering method {nameof(GuardServiceClient) + "." + nameof(RenameObject)}");

            var statusCode =
                await client.ExecuteAsync(
                    HttpRequestMethod.Post,
                    "v1/Objects/{objectId}",
                    null,
                    new Dictionary<string, object>
                    {
                        { "objectId", objectId },
                        { "newName", newName }
                    },
                    new Dictionary<string, object>
                    {
                        { "requestingUserId", requestingUserId }
                    }
                );

            if (statusCode != HttpStatusCode.OK)
                throw new InvalidStatusCodeException(HttpStatusCode.OK, statusCode);

            return true;
        }

        /// <inheritdoc />
        public async Task<IMetadata> GetMetadata(string requestingUserId, string objectId, string revisionId)
        {
            logger.LogTrace($"Entering method {nameof(GuardServiceClient) + "." + nameof(GetMetadata)}");

            if (IdGenerator.TypeFromId(objectId, out var typeOfId))
                throw new InvalidOperationException("Non existing id type");

            IMetadata result;
            HttpStatusCode statusCode;

            var pathParameter = new Dictionary<string, object>
            {
                {"objectId", objectId},
                {"revisionId", revisionId},
            };

            var queryParameter = new Dictionary<string, object>
            {
                {"requestingUserId", requestingUserId}
            };



            switch (typeOfId)
            {
                case IdType.Directory:
                    (result, statusCode) =
                        await client.ExecuteAsync<DirectoryMetadataDto>(
                            HttpRequestMethod.Get,
                            "v1/Objects/{objectId}",
                            null,
                            pathParameter,
                            queryParameter
                        );
                    break;
                case IdType.File:
                    (result, statusCode) =
                        await client.ExecuteAsync<FileMetadataDto>(
                            HttpRequestMethod.Get,
                            "v1/Objects/{objectId}",
                            null,
                            pathParameter,
                            queryParameter
                        );
                    break;
                default:
                    throw new InvalidOperationException($"Invalid object id type. Found type {typeOfId}. Following types are allowed types: {IdType.File}, {IdType.Directory}");
            }

            if (statusCode != HttpStatusCode.OK)
                throw new InvalidStatusCodeException(HttpStatusCode.OK, statusCode);

            return result;
        }

        /// <inheritdoc />
        public async Task<ISearchResult> Search(string requestingUserId, string directoryId, string searchQuery, uint count, uint page)
        {
            logger.LogTrace($"Entering method {nameof(GuardServiceClient) + "." + nameof(Search)}");

            throw new NotImplementedException();
        }
    }
}
