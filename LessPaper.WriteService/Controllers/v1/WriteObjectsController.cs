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
using System.Threading.Tasks;
using LessPaper.Shared.Interfaces.Bucket;
using LessPaper.Shared.Interfaces.Queuing;
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

        public WriteObjectsController(IOptions<AppSettings> config,  IGuardApi guardApi, IWritableBucket bucket, IQueueBuilder queueBuilder)
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
        /// <param name="revisionNumber">Revision number of the file. Null if the latest version is meant</param>
        /// <returns></returns>
        [Route("/files")]
        [HttpPost("{directoryId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UploadFile(
            [FromForm] UploadFileRequest fileData,
            [FromRoute] string directoryId,
            [FromQuery(Name = "revisionNr")] uint? revisionNumber)
        {
            #region - Input data validation

            if (!IdGenerator.TypeFromId(directoryId, out var typOfId) || typOfId != IdType.Directory)
                return BadRequest();

            if (fileData.File == null ||
                fileData.File.Length <= 0 ||
                fileData.File.Length > int.MaxValue ||
                fileData.File.Length > config.Value.ValidationRules.MaxFileSizeInBytes ||
                string.IsNullOrWhiteSpace(fileData.Name) ||
                string.IsNullOrWhiteSpace(fileData.EncryptedKey) ||
                string.IsNullOrWhiteSpace(fileData.PlaintextKey))
            {
                return BadRequest();
            }

            var fileSize = (int)fileData.File.Length;

            var plaintextKeyBytes = Convert.FromBase64String(fileData.PlaintextKey);
            // Make sure the iv is 16 Bytes long and the key has exactly 32 Byte. 
            if (plaintextKeyBytes.Length != 16 +32)
                return BadRequest();

            #endregion

            var fileId = IdGenerator.NewId(IdType.File);

            try
            {
                // Upload file to bucket
                var successful = await bucket.UploadFileEncrypted(
                    config.Value.ExternalServices.MinioBucketName,
                    fileId,
                    fileSize,
                    plaintextKeyBytes,
                    fileData.File.OpenReadStream());

                if (!successful)
                    return BadRequest();

                // Add item to database
                var quickNumber = await guardApi.AddFile(
                                                    directoryId, 
                                                    fileId, 
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
                    EncryptedKey = fileData.EncryptedKey,
                    FileName = fileData.Name,
                    PlaintextKey = fileData.PlaintextKey
                };
                await queueSender.Send(queueRequest);
                

                // Build response 
                // TODO Remove casts and add uint return values in sub-apis
                var response = new UploadFileResponse(
                    queueRequest.FileName,
                    fileId, 
                    (uint)fileSize, 
                    DateTime.UtcNow,
                    DateTime.MinValue,
                    (uint)quickNumber);

                return Ok(response);
            }
            catch (Exception e)
            {
                // TODO Remove file if something failed
                Console.Write(e);
                return BadRequest();
            }
        }

        [Route("/directories")]
        [HttpPost("{directoryId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateDirectory(
            [FromRoute] string directoryId,
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
                var newDirectoryId = IdGenerator.NewId(IdType.Directory);
                var successful = await guardApi.AddDirectory(directoryId, createDirectoryRequest.SubDirectoryName, newDirectoryId);

                if (!successful)
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
        /// Update the metadata of a file or a directory
        /// </summary>
        /// <param name="updatedMetadata">Updated metadata</param>
        /// <param name="objectId">File or directory id</param>
        /// <param name="revisionNumber">Revision number of the file. Null for directories or if the latest version is meant</param>
        /// <returns></returns>
        [HttpPatch("{objectId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateObjectMetadata(
            [FromBody] UpdateObjectMetaDataRequest updatedMetadata,
            [FromRoute] string objectId,
            [FromQuery(Name = "revisionNr")] uint? revisionNumber)
        {
            #region - Input data validation -
            
            if (!IdGenerator.TypeFromId(objectId, out var typOfId) || (typOfId != IdType.Directory && typOfId != IdType.File))
                return BadRequest();

            if (string.IsNullOrWhiteSpace(updatedMetadata.ObjectName) ||
                updatedMetadata.ParentDirectoryIds == null ||
                updatedMetadata.ParentDirectoryIds.Length == 0)
            {
                return BadRequest();
            }

            foreach (var updatedMetadataParentDirectoryId in updatedMetadata.ParentDirectoryIds)
            {
                // Check that all ids are well formed
                if (!IdGenerator.TypeFromId(updatedMetadataParentDirectoryId, out var typOfParentDirectoryId) ||
                    typOfParentDirectoryId != IdType.Directory)
                    return BadRequest();
            }

            #endregion
            
            try
            {
                var successful = await guardApi.UpdateObjectMetadata(objectId, updatedMetadata);
     
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
        /// <param name="revisionNumber">Revision number of the file. Null for directories or if the latest version is meant</param>
        /// <returns></returns>
        [HttpDelete("{objectId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> DeleteObject(
            [FromRoute] string objectId,
            [FromQuery(Name = "revisionNr")] uint? revisionNumber)
        {
            #region - Input data validation -

            if (!IdGenerator.TypeFromId(objectId, out var typOfId) || 
                (typOfId != IdType.Directory && typOfId != IdType.File))
                return BadRequest();
            
            #endregion
            
            try
            {
                var successful = await guardApi.DeleteObject(objectId);

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
    }
}
