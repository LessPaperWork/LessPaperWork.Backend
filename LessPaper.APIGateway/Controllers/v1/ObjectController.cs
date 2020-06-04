using System;
using System.Threading.Tasks;
using LessPaper.APIGateway.Models.Request;
using LessPaper.APIGateway.Models.Response;
using LessPaper.APIGateway.Options;
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
        public async Task<IActionResult> UploadFileToKnownLocation(
            [FromForm] UploadFileRequest fileData,
            [FromRoute] string directoryId,
            [FromQuery(Name = "revisionNr")] uint? revisionNumber)
        {
            try
            {
                var uploadMetadata = await writeApi.ObjectApi.UploadFile(
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
            try
            {
                var createDirectoryMetadata = await writeApi.ObjectApi.CreateDirectory(
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
            try
            {
                var stream = await readApi.ObjectApi.GetObject(objectId, revisionNumber);

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
            try
            {
                var metadata = await readApi.ObjectApi.GetMetadata(objectId, revisionNumber);
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
            [FromBody] UpdateFileMetaDataRequest updatedMetadata,
            [FromRoute] string objectId,
            [FromQuery(Name = "revisionNr")] uint? revisionNumber)
        {
            try
            {
                var updated = await writeApi.ObjectApi.UpdateMetadata(objectId, updatedMetadata);
                if (updated)
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
            try
            {
                var deleted = await writeApi.ObjectApi.DeleteObject(objectId);
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
            try
            {
                var searchResults = await readApi.ObjectApi.Search(directoryId, searchQuery, count ?? 10, page ?? 0);
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
