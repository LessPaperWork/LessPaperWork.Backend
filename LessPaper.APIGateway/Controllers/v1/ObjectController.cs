using System;
using System.Linq;
using System.Threading.Tasks;
using LessPaper.APIGateway.Models.Request;
using LessPaper.APIGateway.Models.Response;
using LessPaper.APIGateway.Options;
using LessPaper.Shared.Enums;
using LessPaper.Shared.Helper;
using LessPaper.Shared.Interfaces.General;
using LessPaper.Shared.Interfaces.ReadApi;
using LessPaper.Shared.Interfaces.WriteApi;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace LessPaper.APIGateway.Controllers.v1
{
    [Route("v1/objects")]
    [ApiController]
    public class ObjectController : ControllerBase
    {
        private IOptions<AppSettings> config;
        private readonly IWriteApi writeApi;
        private readonly IReadApi readApi;

        public ObjectController(IOptions<AppSettings> config, IWriteApi writeApi, IReadApi readApi)
        {
            this.config = config;
            this.writeApi = writeApi;
            this.readApi = readApi;
        }

        protected string GetUserId()
        {
            var userId = User.Claims.First(i => i.Type == "UserId").Value;
            if (IdGenerator.IsType(userId, IdType.User))
                throw new Exception("Invalid user id");

            return userId;
        }

        /// <summary>
        /// Upload a file to a specific location
        /// </summary>
        /// <param name="fileData">Form-data of the file</param>
        /// <param name="directoryId">Target directory id</param>
        /// <param name="revisionNumber">Revision number of the file. Null if the latest version is meant</param>
        /// <returns></returns>
        [Route("files")]
        [HttpPost("{directoryId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UploadFile(
            [FromForm] UploadFileRequest fileData,
            [FromRoute] string directoryId,
            [FromQuery(Name = "revisionNr")] uint? revisionNumber)
        {
            var requestingUserId = GetUserId();

            try
            {
                var uploadMetadata = await writeApi.ObjectApi.UploadFile(
                    requestingUserId,
                    directoryId,
                    fileData.File.OpenReadStream(),
                    fileData.PlaintextKey,
                    fileData.EncryptedKey,
                    fileData.DocumentLanguage,
                    fileData.FileExtension);

                var responseObject = new UploadFileResponse(uploadMetadata);
                return Ok(responseObject);
            }
            catch (Exception e)
            {
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
            var requestingUserId = GetUserId();

            try
            {
                var createDirectoryMetadata = await writeApi.ObjectApi.CreateDirectory(
                        requestingUserId,
                        directoryId,
                        createDirectoryRequest.SubDirectoryName);

                var responseObject = new DirectoryMetadataResponse(createDirectoryMetadata);
                return Ok(responseObject);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return BadRequest();
            }
        }

        /// <summary>
        /// Download a file or a directory
        /// </summary>
        /// <param name="objectId">File or directory id</param>
        /// <param name="revisionNumber">Revision number of the file. Null for directories or if the latest version is meant</param>
        /// <returns></returns>
        [HttpGet("{objectId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetObject(
            [FromRoute] string objectId,
            [FromQuery(Name = "revisionNr")] uint? revisionNumber)
        {
            var requestingUserId = GetUserId();

            try
            {
                var stream = await readApi.ObjectApi.GetObject(requestingUserId, objectId, revisionNumber);


                if (stream == null)
                    return BadRequest();

                return File(stream, "application/octet-stream");
            }
            catch (Exception e)
            {
                Console.Write(e);
                return BadRequest();
            }
        }

        /// <summary>
        /// Get the metadata of a file or a directory
        /// </summary>
        /// <param name="objectId">File or directory id</param>
        /// <param name="revisionNumber">Revision number of the file. Null for directories or if the latest version is meant</param>
        /// <returns></returns>
        [HttpHead("{objectId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetObjectMetadata(
            [FromRoute] string objectId,
            [FromQuery(Name = "revisionNr")] uint? revisionNumber)
        {
            var requestingUserId = GetUserId();
            try
            {
                var metadata = await readApi.ObjectApi.GetMetadata(requestingUserId, objectId, revisionNumber);
                return metadata switch
                {
                    IFileMetadata fileMetadata => (IActionResult)Ok(new FileMetadataResponse(fileMetadata)),
                    IDirectoryMetadata directoryMetadata => Ok(new DirectoryMetadataResponse(directoryMetadata)),
                    _ => BadRequest()
                };
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
            [FromQuery] string requestingUserId,
            [FromQuery] string objectId,
            [FromQuery] string newName)
        {
            try
            {
                var successful = await writeApi.ObjectApi.RenameObject(requestingUserId, objectId, newName);

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
            try
            {
                var successful = await writeApi.ObjectApi.MoveObject(requestingUserId, objectId, newParentDirectoryId);

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
            var requestingUserId = GetUserId();

            try
            {
                var deleted = await writeApi.ObjectApi.DeleteObject(requestingUserId, objectId);
                if (deleted)
                    return Ok();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return BadRequest();
            }

            return BadRequest();
        }

        /// <summary>
        /// Search for an object
        /// </summary>
        /// <param name="directoryId">Id of the directory where the search starts</param>
        /// <param name="searchQuery">Search query</param>
        /// <param name="count">Number of items to return</param>
        /// <param name="page">Search result page</param>
        /// <returns></returns>
        [HttpGet("{directoryId}")]
        [Route("/search")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> SearchObject(
            [FromRoute] string directoryId,
            [FromQuery(Name = "searchQuery")] string searchQuery,
            [FromQuery(Name = "count")] uint? count,
            [FromQuery(Name = "page")] uint? page)
        {
            var requestingUserId = GetUserId();

            try
            {
                var searchResults = await readApi.ObjectApi.Search(requestingUserId, directoryId, searchQuery, count ?? 10, page ?? 0);
                return Ok(new SearchResponse(searchResults));
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return BadRequest();
            }
        }
    }
}
