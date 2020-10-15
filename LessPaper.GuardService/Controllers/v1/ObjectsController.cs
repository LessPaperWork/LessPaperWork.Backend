using System;
using System.ComponentModel;
using System.Data.SqlTypes;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using LessPaper.Shared.Enums;
using LessPaper.Shared.Helper;
using LessPaper.Shared.Interfaces.Database.Manager;
using LessPaper.Shared.Rest.Models.Dtos;
using LessPaper.Shared.Rest.Models.DtoSwaggerExamples;
using LessPaper.Shared.Rest.Models.RequestDtos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.AspNetCore.Filters;

namespace LessPaper.GuardService.Controllers.v1
{
    [ApiController]
    [Route("v1/[controller]")]
    public class ObjectsController : ControllerBase
    {
        private readonly ILogger<ObjectsController> logger;
        private readonly IDbUserManager userManager;
        private readonly IDbDirectoryManager directoryManager;
        private readonly IDbFileManager fileManager;

        public ObjectsController(
            ILogger<ObjectsController> logger,
            IDbUserManager userManager,
            IDbDirectoryManager directoryManager,
            IDbFileManager fileManager)
        {
            this.logger = logger;
            this.userManager = userManager;
            this.directoryManager = directoryManager;
            this.fileManager = fileManager;
        }

        [HttpGet("permissions/{requestingUserId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [SwaggerRequestExample(typeof(GetObjectsPermissionDto), typeof(GetObjectsPermissionDtoSwaggerExample))]
        public async Task<IActionResult> GetObjectsPermissions(
            [FromRoute] [SwaggerParameterExample("0146613259928c4bd8a58dfd0fca344e47")] string requestingUserId, 
            [FromBody] GetObjectsPermissionDto data)
        {
            if (!IdGenerator.IsType(requestingUserId, IdType.User))
                return BadRequest(new MessageDto("Requesting user id is not of type user"));

            if (!IdGenerator.IsType(data.UserId, IdType.User))
                return BadRequest(new MessageDto("Target user id is not of type user"));

            if (data.ObjectIds.Length > 1000 || data.ObjectIds.Length == 0)
                return BadRequest(new MessageDto("Invalid number of object ids"));

            var directoriesRequests = data.ObjectIds.Where(x => IdGenerator.IsType(x, IdType.Directory)).ToArray();
            var fileRequests = data.ObjectIds.Where(x => IdGenerator.IsType(x, IdType.File)).ToArray();

            if (directoriesRequests.Length == 0 && fileRequests.Length == 0)
                return BadRequest(new MessageDto("List of object ids does not contain any file or directory id"));

            var directoryPermissionsTask =
                directoryManager.GetPermissions(requestingUserId, data.UserId, directoriesRequests);

            var filePermissionsTask =
                directoryManager.GetPermissions(requestingUserId, data.UserId, fileRequests);

            await Task.WhenAll(directoryPermissionsTask, filePermissionsTask);

            var permissions = directoryPermissionsTask.Result.Select(x => new PermissionDto(x)).ToList();
            permissions.AddRange(filePermissionsTask.Result.Select(x => new PermissionDto(x)).ToArray());

            return new OkObjectResult(permissions);
        }

        [HttpPost("directories/{parentDirectoryId}/{directoryName}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> AddDirectory(
            [FromQuery][SwaggerParameterExample("0146613259928c4bd8a58dfd0fca344e47")] string requestingUserId,
            [FromRoute][SwaggerParameterExample("039e6b7c1cda0045b081ba0e96759fb275")] string parentDirectoryId,
            [FromRoute][SwaggerParameterExample("Test Directory")] string directoryName)
        {
            if (!IdGenerator.IsType(requestingUserId, IdType.User))
                return BadRequest(new MessageDto("Requesting user id is not of type user"));

            if (!IdGenerator.IsType(parentDirectoryId, IdType.Directory))
                return BadRequest(new MessageDto("Parent directory id is not of type directory"));

            if (string.IsNullOrWhiteSpace(directoryName))
                return BadRequest(new MessageDto("Invalid directory name"));

            var newDirectoryId = IdGenerator.NewId(IdType.Directory);

            var successful = await directoryManager.InsertDirectory(requestingUserId, parentDirectoryId, directoryName, newDirectoryId);
            if (!successful)
            {
                return new ObjectResult(new MessageDto("Database operation not successful"))
                {
                    StatusCode = StatusCodes.Status500InternalServerError
                };
            }

            return new OkObjectResult(newDirectoryId);
        }

        [HttpDelete("{objectId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteObject(
            [FromQuery][SwaggerParameterExample("0146613259928c4bd8a58dfd0fca344e47")] string requestingUserId, 
            [FromRoute][SwaggerParameterExample("02a6b31c4a65164520b479d68f03c61e21")] string objectId)
        {
            if (!IdGenerator.IsType(requestingUserId, IdType.User))
                return BadRequest(new MessageDto("Requesting user id is not of type user"));

            if (!IdGenerator.TypeFromId(objectId, out var typeOfId) ||
                (typeOfId != IdType.Directory &&
                 typeOfId != IdType.File &&
                 typeOfId != IdType.User))
                return BadRequest(new MessageDto("Object id is neither directory, file or user"));

            var deletedRevisionIds = typeOfId switch
            {
                IdType.User => await userManager.DeleteUser(requestingUserId, objectId),
                IdType.File => await fileManager.Delete(requestingUserId, objectId),
                IdType.Directory => await directoryManager.Delete(requestingUserId, objectId),
                _ => throw new ArgumentOutOfRangeException()
            };

            return deletedRevisionIds == null ? 
                new ObjectResult(new MessageDto("Database operation not successful. No object deleted"))
                {
                    StatusCode = StatusCodes.Status500InternalServerError
                } : 
                new OkObjectResult(deletedRevisionIds);
        }

        [HttpPost("files/{directoryId}/{fileId}/{revisionId}")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [SwaggerRequestExample(typeof(AddFileDto), typeof(AddFileDtoSwaggerExample))]
        public async Task<IActionResult> AddFile(
            [FromQuery][SwaggerParameterExample("0146613259928c4bd8a58dfd0fca344e47")] string requestingUserId,
            [FromRoute][SwaggerParameterExample("039e6b7c1cda0045b081ba0e96759fb275")] string directoryId,
            [FromRoute][SwaggerParameterExample("02a6b31c4a65164520b479d68f03c61e21")] string fileId,
            [FromRoute][SwaggerParameterExample("04d6d97edce0634a93a234280f48ff9640")] string revisionId,
            [FromBody] AddFileDto request)
        {
            if (!IdGenerator.IsType(requestingUserId, IdType.User))
                return BadRequest(new MessageDto("Requesting user id is not of type user"));
            if (!IdGenerator.IsType(revisionId, IdType.FileBlob))
                return BadRequest(new MessageDto("Revision id is not of type file blob"));
            if (!IdGenerator.IsType(directoryId, IdType.Directory))
                return BadRequest(new MessageDto("Directory id is not of type directory"));
            if (!IdGenerator.IsType(fileId, IdType.File))
                return BadRequest(new MessageDto("File id is not of type file"));
            if (request.EncryptedKey.Count == 0 || 
                request.EncryptedKey.Any(x => string.IsNullOrWhiteSpace(x.Key) || string.IsNullOrWhiteSpace(x.Value)))
                return BadRequest(new MessageDto("Encrypted keys are invalid"));
            if (string.IsNullOrWhiteSpace(request.FileName))
                return BadRequest(new MessageDto("No filename given"));
            if(request.FileSize == 0 || request.FileSize > 10_000_000)
                return BadRequest(new MessageDto("Invalid file size"));

            var quickNumber = await fileManager.InsertFile(requestingUserId, directoryId, fileId,
             revisionId, request.FileName, (int)request.FileSize, request.EncryptedKey, request.DocumentLanguage,
                request.FileExtension);

            // Check for invalid quick number
            if (quickNumber <= 0)
            {
                return new ObjectResult(new MessageDto("Database operation not successful"))
                {
                    StatusCode = StatusCodes.Status500InternalServerError
                };
            }
            
            return new ObjectResult(quickNumber)
            {
                StatusCode = StatusCodes.Status201Created
            };
        }

        [HttpPost("{objectId}/{newParentDirectoryId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> MoveObject(
            [FromQuery][SwaggerParameterExample("0146613259928c4bd8a58dfd0fca344e47")] string requestingUserId,
            [FromRoute][SwaggerParameterExample("02a6b31c4a65164520b479d68f03c61e21")] string objectId,
            [FromRoute][SwaggerParameterExample("039e6b7c1cda0045b081ba0e96759fb275")] string newParentDirectoryId)
        {
            if (!IdGenerator.IsType(requestingUserId, IdType.User))
                return BadRequest(new MessageDto("Requesting user id is not of type user"));
            if (!IdGenerator.IsType(newParentDirectoryId, IdType.Directory))
                return BadRequest(new MessageDto("New directory id is not of type directory"));
            if (!IdGenerator.TypeFromId(objectId, out var typeOfId) || (typeOfId != IdType.Directory && typeOfId != IdType.File))
                return BadRequest(new MessageDto("Object id is neither directory or file"));
            
            var successful = typeOfId switch
            {
                IdType.File => await fileManager.Move(requestingUserId, objectId, newParentDirectoryId),
                IdType.Directory => await directoryManager.Move(requestingUserId, objectId, newParentDirectoryId),
                _ => throw new ArgumentOutOfRangeException()
            };

            if (!successful)
            {
                return new ObjectResult(new MessageDto("Database operation not successful"))
                {
                    StatusCode = StatusCodes.Status500InternalServerError
                };
            }

            return new OkResult();
        }

        [HttpPost("{objectId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> RenameObject(
            [FromQuery][SwaggerParameterExample("0146613259928c4bd8a58dfd0fca344e47")] string requestingUserId, 
            [FromRoute][SwaggerParameterExample("02a6b31c4a65164520b479d68f03c61e21")] string objectId, 
            [FromQuery][SwaggerParameterExample("NewDocName")] string newName)
        {
            if (!IdGenerator.IsType(requestingUserId, IdType.User))
                return BadRequest(new MessageDto("Requesting user id is not of type user"));
            if (string.IsNullOrWhiteSpace(newName))
                return BadRequest(new MessageDto("New name is not valid"));
            if (!IdGenerator.TypeFromId(objectId, out var typeOfId) || (typeOfId != IdType.Directory && typeOfId != IdType.File))
                return BadRequest(new MessageDto("Object id is neither directory or file"));
            
            var successful = typeOfId switch
            {
                IdType.File => await fileManager.Rename(requestingUserId, objectId, newName),
                IdType.Directory => await directoryManager.Rename(requestingUserId, objectId, newName),
                _ => throw new ArgumentOutOfRangeException()
            };

            if (!successful)
            {
                return new ObjectResult(new MessageDto("Database operation not successful"))
                {
                    StatusCode = StatusCodes.Status500InternalServerError
                };
            }

            return new OkResult();
        }

        [HttpGet("{objectId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetMetadata(
            [FromQuery][SwaggerParameterExample("0146613259928c4bd8a58dfd0fca344e47")] string requestingUserId,
            [FromRoute][SwaggerParameterExample("02a6b31c4a65164520b479d68f03c61e21")] string objectId,
            [FromQuery] string revisionNumber)
        {
            if (!IdGenerator.IsType(requestingUserId, IdType.User))
                return BadRequest(new MessageDto("Requesting user id is not of type user"));
            if (!IdGenerator.TypeFromId(objectId, out var typeOfId) || (typeOfId != IdType.Directory && typeOfId != IdType.File))
                return BadRequest(new MessageDto("Object id is neither directory or file"));
            
            switch (typeOfId)
            {
                case IdType.File:
                    {
                        var fileMetadata = await fileManager.GetFileMetadata(requestingUserId, objectId, revisionNumber);
                        return new OkObjectResult(new FileMetadataDto(fileMetadata));
                    }
                case IdType.Directory:
                    {
                        var directoryMetadata = await directoryManager.GetDirectoryMetadata(requestingUserId, objectId);
                        return new OkObjectResult(new DirectoryMetadataDto(directoryMetadata));
                    }
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
