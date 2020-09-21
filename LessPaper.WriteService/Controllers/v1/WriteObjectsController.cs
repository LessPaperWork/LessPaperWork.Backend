using LessPaper.Shared.Enums;
using LessPaper.Shared.Helper;
using LessPaper.Shared.Interfaces.GuardApi;
using LessPaper.Shared.Queueing.Models.Dto.v1;
using LessPaper.WriteService.Models.Request;
using LessPaper.WriteService.Options;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using System.Threading.Tasks;
using LessPaper.Shared.Interfaces.Bucket;
using LessPaper.Shared.Interfaces.Queuing;
using LessPaper.Shared.Rest.Models.RequestDtos;
using LessPaper.WriteService.Models.Response;

namespace LessPaper.WriteService.Controllers.v1
{
    [Route("v1/objects")]
    [ApiController]
    public class WriteObjectsController : ControllerBase
    {
        private readonly IOptions<AppSettings> config;
        private readonly IGuardApi guardApi;
        private readonly IWritableBucket bucket;
        readonly IQueueSender queueSender;

        public WriteObjectsController(IOptions<AppSettings> config, IGuardApi guardApi, IWritableBucket bucket, IQueueBuilder queueBuilder)
        {
            this.config = config;
            this.guardApi = guardApi;
            this.bucket = bucket;
            queueSender = queueBuilder.Start().Result;
        }

        /// <summary>
        /// Upload a file to a specific location
        /// </summary>
        /// <param name="fileData">Form-data of the file</param>
        /// <param name="directoryId">Target directory id</param>
        /// <param name="requestingUserId">Id of the requesting user</param>
        /// <returns></returns>
        [HttpPost("/files/{directoryId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UploadFile(
            [FromBody] UploadFileDto fileData,
            [FromRoute] string directoryId,
            [FromQuery] string requestingUserId)
        {
            #region - Input data validation

            if (!IdGenerator.TypeFromId(directoryId, out var typOfId) || typOfId != IdType.Directory)
                return BadRequest();

            if (fileData.File == null ||
                fileData.File.Length <= 0 ||
                fileData.File.Length > int.MaxValue ||
                fileData.File.Length > config.Value.ValidationRules.MaxFileSizeInBytes ||
                string.IsNullOrWhiteSpace(fileData.FileName) ||
                fileData.EncryptedKey.Count == 0 ||
                !fileData.EncryptedKey.ContainsKey(requestingUserId) ||
                fileData.EncryptedKey.Any(x => string.IsNullOrWhiteSpace(x.Key) || string.IsNullOrWhiteSpace(x.Value)) ||
                string.IsNullOrWhiteSpace(fileData.PlaintextKey))
            {
                return BadRequest();
            }

            var fileSize = (int)fileData.File.Length;

            var plaintextKeyBytes = Convert.FromBase64String(fileData.PlaintextKey);
            // Make sure the iv is 16 Bytes long and the key has exactly 32 Byte. 
            if (plaintextKeyBytes.Length != 16 + 32)
                return BadRequest();

            #endregion

            var fileId = IdGenerator.NewId(IdType.File);
            var revisionId = IdGenerator.NewId(IdType.FileBlob);

            try
            {
                // Upload file to bucket
                await bucket.UploadEncrypted(
                    config.Value.ExternalServices.MinioBucketName,
                    revisionId,
                    fileSize,
                    plaintextKeyBytes,
                    fileData.File.OpenReadStream());
                
                // Add item to database
                var quickNumber = await guardApi.AddFile(
                                                    requestingUserId,
                                                    directoryId,
                                                    fileId,
                                                    revisionId,
                                                    fileData.FileName,
                                                    fileSize,
                                                    fileData.EncryptedKey,
                                                    DocumentLanguage.German,
                                                    ExtensionType.Docx);


                // Add item to queue
                var queueRequest = new QueueFileMetadataDto
                {
                    FileId = fileId,
                    DirectoryId = directoryId,
                    DocumentLanguage = fileData.DocumentLanguage,
                    FileName = fileData.FileName,
                    PlaintextKey = fileData.PlaintextKey
                };
                await queueSender.Send(queueRequest);


                // Build response 
                // TODO Remove casts and add uint return values in sub-apis
                var response = new UploadFileResponse(
                    fileId,
                    revisionId,
                    (uint)quickNumber);

                return Ok(response);
            }
            catch (Exception e)
            {
                // TODO Remove file if database failed
                // TODO Write queue data to database if queue fails

                Console.Write(e);
                return BadRequest();
            }
        }


        /// <summary>
        /// Create a new directory in a given location
        /// </summary>
        /// <param name="directoryId">Directory id</param>
        /// <param name="createDirectoryRequest"></param>
        /// <param name="requestingUserId"></param>
        /// <returns></returns>
        [HttpPost("/directories/{directoryId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateDirectory(
            [FromRoute] string directoryId,
            [FromQuery] string requestingUserId,
            [FromBody] CreateDirectoryRequest createDirectoryRequest
        )
        {
            #region - Input data validation -

            if (!IdGenerator.TypeFromId(directoryId, out var typOfId) || typOfId != IdType.Directory)
                return BadRequest();

            if (string.IsNullOrWhiteSpace(createDirectoryRequest.SubDirectoryName))
                return BadRequest();

            #endregion


            try
            {
                var newDirectoryId = await guardApi.AddDirectory(
                    requestingUserId,
                    directoryId,
                    createDirectoryRequest.SubDirectoryName);

                if (!IdGenerator.IsType(newDirectoryId, IdType.Directory))
                    return BadRequest();

                return Ok(newDirectoryId);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return BadRequest();
            }
        }


        /// <summary>
        /// Rename a file or a directory
        /// </summary>
        /// <returns></returns>
        [HttpPost("{objectId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> RenameObject(
            [FromQuery]string requestingUserId,
            [FromQuery] string objectId,
            [FromQuery] string newName)
        {
            #region - Input data validation -

            if (!IdGenerator.IsType(requestingUserId, IdType.User) ||
                !IdGenerator.TypeFromId(objectId, out var typOfId) ||
                typOfId != IdType.Directory && typOfId != IdType.File ||
                string.IsNullOrWhiteSpace(newName))
            {
                return BadRequest();
            }

            #endregion

            try
            {
                var successful = await guardApi.RenameObject(requestingUserId, objectId, newName);

                if (!successful)
                    return BadRequest();

                return Ok();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return BadRequest();
            }
        }

        /// <summary>
        /// Move a file or a directory
        /// </summary>
        /// <returns></returns>
        [HttpPost("{objectId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> MoveObject(
            [FromQuery] string requestingUserId,
            [FromRoute] string objectId,
            [FromRoute] string newParentDirectoryId)
        {
            #region - Input data validation -

            if (!IdGenerator.IsType(requestingUserId, IdType.User) ||
                !IdGenerator.IsType(newParentDirectoryId, IdType.Directory) ||
                !IdGenerator.TypeFromId(objectId, out var typeOfId) ||
                (typeOfId != IdType.Directory &&
                 typeOfId != IdType.File))
            {
                return BadRequest();
            }

            #endregion

            try
            {
                var successful = await guardApi.MoveObject(requestingUserId, objectId, newParentDirectoryId);

                if (!successful)
                    return BadRequest();

                return Ok();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return BadRequest();
            }
        }

        /// <summary>
        /// Delete a file or a directory
        /// </summary>
        /// <param name="objectId">Id of the File or directory to delete</param>
        /// <param name="requestingUserId">Requesting user id</param>
        /// <returns></returns>
        [HttpDelete("{objectId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> DeleteObject(
            [FromQuery] string requestingUserId,
            [FromRoute] string objectId)
        {
            #region - Input data validation -

            if (!IdGenerator.IsType(requestingUserId, IdType.User) ||
                !IdGenerator.TypeFromId(objectId, out var typOfId) ||
                (typOfId != IdType.Directory && typOfId != IdType.File && typOfId != IdType.FileBlob || typOfId != IdType.User))
                return BadRequest();

            #endregion

            try
            {
                var revisionIds = await guardApi.DeleteObject(requestingUserId, objectId);
                await bucket.Delete(config.Value.ExternalServices.MinioBucketName, revisionIds);
                
                return Ok();
            }
            catch (Exception e)
            {
                //TODO Remember revision ids and retry delete another time

                Console.WriteLine(e);
                return BadRequest();
            }
        }
    }
}
