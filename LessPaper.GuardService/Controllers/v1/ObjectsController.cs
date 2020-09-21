using System;
using System.Linq;
using System.Threading.Tasks;
using LessPaper.GuardService.Models.Api;
using LessPaper.Shared.Enums;
using LessPaper.Shared.Helper;
using LessPaper.Shared.Interfaces.Database.Manager;
using LessPaper.Shared.Rest.Models.Dtos;
using LessPaper.Shared.Rest.Models.RequestDtos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

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
        public async Task<IActionResult> GetObjectsPermissions([FromRoute] string requestingUserId, [FromBody] GetObjectsPermissionDto data)
        {
            if (!IdGenerator.IsType(requestingUserId, IdType.User) ||
                !IdGenerator.IsType(data.UserId, IdType.User) ||
                data.ObjectIds.Length > 1000 ||
                data.ObjectIds.Length == 0)
                return BadRequest();

            var directoriesRequests = data.ObjectIds.Where(x => IdGenerator.IsType(x, IdType.Directory)).ToArray();
            var fileRequests = data.ObjectIds.Where(x => IdGenerator.IsType(x, IdType.File)).ToArray();

            if (directoriesRequests.Length == 0 && fileRequests.Length == 0)
                return BadRequest();

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
        public async Task<IActionResult> AddDirectory(
            [FromQuery] string requestingUserId, 
            [FromRoute] string parentDirectoryId, 
            [FromRoute] string directoryName)
        {
            if (!IdGenerator.IsType(requestingUserId, IdType.User) ||
                !IdGenerator.IsType(parentDirectoryId, IdType.Directory) ||
                string.IsNullOrWhiteSpace(directoryName))
                return BadRequest();

            var newDirectoryId = IdGenerator.NewId(IdType.Directory);

            var successful = await directoryManager.InsertDirectory(requestingUserId, parentDirectoryId, directoryName, newDirectoryId);
            if (!successful)
                return BadRequest();

            return new OkObjectResult(newDirectoryId);
        }
        
        [HttpDelete("{objectId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> DeleteObject([FromQuery] string requestingUserId, [FromRoute] string objectId)
        {
            if (!IdGenerator.IsType(requestingUserId, IdType.User) ||
                !IdGenerator.TypeFromId(objectId, out var typeOfId) ||
                (typeOfId != IdType.Directory &&
                 typeOfId != IdType.File &&
                 typeOfId != IdType.User))
                return BadRequest();

            var deletedRevisionIds = typeOfId switch
            {
                IdType.User => await userManager.DeleteUser(requestingUserId, objectId),
                IdType.File => await fileManager.Delete(requestingUserId, objectId),
                IdType.Directory => await directoryManager.Delete(requestingUserId, objectId),
                _ => throw new ArgumentOutOfRangeException()
            };

            if (deletedRevisionIds == null)
                return BadRequest();

            return new OkObjectResult(deletedRevisionIds);
        }

        [HttpPost("files/{directoryId}/{fileId}/{revisionId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> AddFile(
            [FromQuery] string requestingUserId, 
            [FromRoute] string directoryId,
            [FromRoute] string fileId,
            [FromRoute] string revisionId,
            [FromBody] AddFileDto request)
        {
            if (!IdGenerator.IsType(requestingUserId, IdType.User) ||
                !IdGenerator.IsType(revisionId, IdType.FileBlob) ||
                !IdGenerator.IsType(directoryId, IdType.Directory) ||
                !IdGenerator.IsType(fileId, IdType.File) ||
                request.EncryptedKey.Count == 0 ||
                request.EncryptedKey.Any(x =>  string.IsNullOrWhiteSpace(x.Key) || string.IsNullOrWhiteSpace(x.Value)) ||
                string.IsNullOrWhiteSpace(request.FileName) ||
                request.FileSize == 0 ||
                request.FileSize > 10_000_000)
            {
                return BadRequest();
            }

            var quickNumber = await fileManager.InsertFile(requestingUserId, directoryId, fileId,
             revisionId, request.FileName, (int)request.FileSize, request.EncryptedKey, request.DocumentLanguage,
                request.FileExtension);

            // Check for invalid quick number
            if (quickNumber <= 0)
                return BadRequest();

            return new OkObjectResult(quickNumber);
        }

        [HttpPost("{objectId}/{newParentDirectoryId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> MoveObject(
            [FromQuery] string requestingUserId,
            [FromRoute] string objectId,
            [FromRoute] string newParentDirectoryId)
        {
            if (!IdGenerator.IsType(requestingUserId, IdType.User) ||
                !IdGenerator.IsType(newParentDirectoryId, IdType.Directory) ||
                !IdGenerator.TypeFromId(objectId, out var typeOfId) ||
                (typeOfId != IdType.Directory &&
                 typeOfId != IdType.File))
            {
                return BadRequest();
            }

            var successful = typeOfId switch
            {
                IdType.File => await fileManager.Move(requestingUserId, objectId, newParentDirectoryId),
                IdType.Directory => await directoryManager.Move(requestingUserId, objectId, newParentDirectoryId),
                _ => throw new ArgumentOutOfRangeException()
            };

            if (!successful)
            {
                return BadRequest();
            }

            return new OkResult();
        }

        [HttpPost("{objectId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> RenameObject([FromQuery]string requestingUserId, [FromQuery] string objectId, [FromQuery] string newName)
        {
            if (!IdGenerator.IsType(requestingUserId, IdType.User) ||
                string.IsNullOrWhiteSpace(newName) ||
                !IdGenerator.TypeFromId(objectId, out var typeOfId) ||
                (typeOfId != IdType.Directory &&
                 typeOfId != IdType.File))
            {
                return BadRequest();
            }

            var successful = typeOfId switch
            {
                IdType.File => await fileManager.Rename(requestingUserId, objectId, newName),
                IdType.Directory => await directoryManager.Rename(requestingUserId, objectId, newName),
                _ => throw new ArgumentOutOfRangeException()
            };

            if (!successful)
                return BadRequest();

            return new OkResult();
        }

        [HttpGet("{objectId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetMetadata(string requestingUserId, string objectId, string revisionNumber)
        {
            if (!IdGenerator.IsType(requestingUserId, IdType.User) ||
                !IdGenerator.TypeFromId(objectId, out var typeOfId) ||
                (typeOfId != IdType.Directory &&
                 typeOfId != IdType.File))
            {
                return BadRequest();
            }

            if (typeOfId == IdType.File)
            {
                var fileMetadata = await fileManager.GetFileMetadata(requestingUserId, objectId, revisionNumber);
                return new OkObjectResult(new FileMetadataDto(fileMetadata));
            }

            if (typeOfId == IdType.Directory)
            {
                var directoryMetadata = await directoryManager.GetDirectoryMetadata(requestingUserId, objectId);
                return new OkObjectResult(new DirectoryMetadataDto(directoryMetadata));
            }

            return BadRequest();
        }

    }
}
