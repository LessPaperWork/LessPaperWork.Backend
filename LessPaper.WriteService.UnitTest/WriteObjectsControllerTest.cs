using System;
using System.IO;
using System.Text;
using LessPaper.Shared.Enums;
using LessPaper.Shared.Helper;
using LessPaper.Shared.Interfaces.Bucket;
using LessPaper.Shared.Interfaces.GuardApi;
using LessPaper.Shared.Interfaces.Queuing;
using LessPaper.Shared.Interfaces.WriteApi.WriteObjectApi;
using LessPaper.Shared.Queueing.Interfaces;
using LessPaper.WriteService.Controllers;
using LessPaper.WriteService.Controllers.v1;
using LessPaper.WriteService.Models.Request;
using LessPaper.WriteService.Models.Response;
using LessPaper.WriteService.Options;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace LessPaper.WriteService.UnitTest
{
    public class WriteObjectsControllerTest
    {
        private readonly byte[] payload = new byte[] { 0, 1, 2, 3, 4, 5 };
        private Stream stream;
        private readonly byte[] ivAndKey1 = new byte[16 + 32];
        private readonly byte[] ivAndKey2 = new byte[16 + 32];

        public WriteObjectsControllerTest()
        {
            stream = new MemoryStream(payload);
            for (var i = 0; i < ivAndKey1.Length; i++)
            {
                ivAndKey1[i] = (byte)i;
                ivAndKey2[i] = (byte)(ivAndKey1.Length - i);
            }
        }

        private Mock<IOptions<AppSettings>> GetAppSettingsMock()
        {
            var appSettings = new AppSettings
            {
                ValidationRules = new ValidationRules { MaxFileSizeInBytes = 10 * 1024 * 1024 },
                ExternalServices = new ExternalServices
                {
                    MinioBucketName = "lesspaper"
                }
            };

            var optionsMock = new Mock<IOptions<AppSettings>>();
            optionsMock.Setup(options => options.Value).Returns(appSettings);

            return optionsMock;
        }

        private Mock<IQueueBuilder> GetQueueMock()
        {
            var queueWriterMock = new Mock<IQueueSender>();
            queueWriterMock.Setup(x => x.Send(It.IsAny<object>())).Returns(async () => { });
            var queueMock = new Mock<IQueueBuilder>();
            queueMock.Setup(x => x.Start()).ReturnsAsync(queueWriterMock.Object);

            return queueMock;
        }

        [Fact]
        public async void FileUpload_Correct()
        {
            var optionsMock = GetAppSettingsMock();
            var queueMock = GetQueueMock();

            var guardMock = new Mock<IGuardApi>();
            guardMock.Setup(api => api.AddFile(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<int>(),
                It.IsAny<string>(),
                It.IsAny<DocumentLanguage>(),
                It.IsAny<ExtensionType>())).ReturnsAsync(4);

            var bucketMock = new Mock<IWriteableBucket>();
            bucketMock.Setup(x => x.UploadFileEncrypted(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<int>(),
                It.IsAny<byte[]>(),
                It.IsAny<Stream>())).ReturnsAsync(true);

            var writeObjectsController = new WriteObjectsController(optionsMock.Object, guardMock.Object, bucketMock.Object, queueMock.Object);


            var requestPayload = new UploadFileRequest
            {
                DocumentLanguage = DocumentLanguage.German,
                EncryptedKey = Convert.ToBase64String(ivAndKey1),
                Name = "MyFile.pdf",
                PlaintextKey = Convert.ToBase64String(ivAndKey2),
                File = new FormFile(stream, 0, payload.Length, "", "")
            };

            var response = await writeObjectsController.UploadFile(requestPayload, IdGenerator.NewId(IdType.Directory), 0);
            var metadataResponseObject = Assert.IsType<OkObjectResult>(response);
            var metadataResponse = Assert.IsType<UploadFileResponse>(metadataResponseObject.Value);

            Assert.Equal(requestPayload.Name, metadataResponse.ObjectName);
            Assert.True(IdGenerator.TypeFromId(metadataResponse.ObjectId, out var typeOfId));
            Assert.Equal(IdType.File, typeOfId);
            Assert.True(DateTime.UtcNow - metadataResponse.LatestChangeDate < TimeSpan.FromHours(1));
            Assert.Equal(DateTime.MinValue, metadataResponse.LatestViewDate);
            Assert.Equal((uint)payload.Length, metadataResponse.SizeInBytes);
        }


        [Fact]
        public async void FileUpload_InvalidData()
        {
            var optionsMock = GetAppSettingsMock();
            var queueMock = GetQueueMock();

            var guardMock = new Mock<IGuardApi>();
            guardMock.Setup(api => api.AddFile(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<int>(),
                It.IsAny<string>(),
                It.IsAny<DocumentLanguage>(),
                It.IsAny<ExtensionType>())).ReturnsAsync(4);

            var bucketMock = new Mock<IWriteableBucket>();
            bucketMock.Setup(x => x.UploadFileEncrypted(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<int>(),
                It.IsAny<byte[]>(),
                It.IsAny<Stream>())).ReturnsAsync(true);

            var writeObjectsController = new WriteObjectsController(optionsMock.Object, guardMock.Object, bucketMock.Object, queueMock.Object);

            // Invalid key length
            var invalidKeyPayload = new UploadFileRequest
            {
                DocumentLanguage = DocumentLanguage.German,
                EncryptedKey = Convert.ToBase64String(ivAndKey1),
                Name = "MyFile.pdf",
                PlaintextKey = "MyInvalidKey",
                File = new FormFile(stream, 0, payload.Length, "", "")
            };
            var response = await writeObjectsController.UploadFile(invalidKeyPayload, IdGenerator.NewId(IdType.Directory), 0);
            Assert.IsType<BadRequestResult>(response);

            // No file name
            var noFileNamePayload = new UploadFileRequest
            {
                DocumentLanguage = DocumentLanguage.German,
                EncryptedKey = Convert.ToBase64String(ivAndKey1),
                Name = null,
                PlaintextKey = Convert.ToBase64String(ivAndKey2),
                File = new FormFile(stream, 0, payload.Length, "", "")
            };
            response = await writeObjectsController.UploadFile(noFileNamePayload, IdGenerator.NewId(IdType.Directory), 0);
            Assert.IsType<BadRequestResult>(response);
            
            // No file
            var noFilePayload = new UploadFileRequest
            {
                DocumentLanguage = DocumentLanguage.German,
                EncryptedKey = Convert.ToBase64String(ivAndKey1),
                Name = "MyFile",
                PlaintextKey = Convert.ToBase64String(ivAndKey2),
                File = null
            };
            response = await writeObjectsController.UploadFile(noFilePayload, IdGenerator.NewId(IdType.Directory), 0);
            Assert.IsType<BadRequestResult>(response);

        }


        [Fact]
        public async void FileUpload_throws()
        {
            var optionsMock = GetAppSettingsMock();
            var queueMock = GetQueueMock();

            var guardMock = new Mock<IGuardApi>();
            guardMock.Setup(api => api.AddFile(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<int>(),
                It.IsAny<string>(),
                It.IsAny<DocumentLanguage>(),
                It.IsAny<ExtensionType>())).Throws<InvalidOperationException>();

            var bucketMock = new Mock<IWriteableBucket>();
            bucketMock.Setup(x => x.UploadFileEncrypted(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<int>(),
                It.IsAny<byte[]>(),
                It.IsAny<Stream>())).ReturnsAsync(true);

            var writeObjectsController = new WriteObjectsController(optionsMock.Object, guardMock.Object, bucketMock.Object, queueMock.Object);
            
            var validRequestPayload = new UploadFileRequest
            {
                DocumentLanguage = DocumentLanguage.German,
                EncryptedKey = Convert.ToBase64String(ivAndKey1),
                Name = "MyFile.pdf",
                PlaintextKey = Convert.ToBase64String(ivAndKey2),
                File = new FormFile(stream, 0, payload.Length, "", "")
            };

            var response = await writeObjectsController.UploadFile(validRequestPayload, IdGenerator.NewId(IdType.Directory), 0);
            Assert.IsType<BadRequestResult>(response);
        }
        
        [Fact]
        public async void CreateDirectory_Correct()
        {
            var optionsMock = GetAppSettingsMock();
            var queueMock = GetQueueMock();

            var guardMock = new Mock<IGuardApi>();
            guardMock.Setup(api => api.AddDirectory(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>()))
                .ReturnsAsync(true);

            var writeObjectsController = new WriteObjectsController(optionsMock.Object, guardMock.Object, null, queueMock.Object);

            var request = new CreateDirectoryRequest()
            {
                SubDirectoryName = "MySubDir"
            };

            var response = await writeObjectsController.CreateDirectory(IdGenerator.NewId(IdType.Directory), request);
            var metadataResponseObject = Assert.IsType<OkObjectResult>(response);
            var newDirectoryId = Assert.IsType<string>(metadataResponseObject.Value);

            Assert.True(IdGenerator.TypeFromId(newDirectoryId, out var typeOfId));
            Assert.Equal(IdType.Directory, typeOfId);
        }

        [Fact]
        public async void UpdateMetadata_Correct()
        {
            var optionsMock = GetAppSettingsMock();
            var queueMock = GetQueueMock();

            var guardMock = new Mock<IGuardApi>();
            guardMock.Setup(api => api.UpdateObjectMetadata(
                    It.IsAny<string>(),
                    It.IsAny<IMetadataUpdate>()))
                .ReturnsAsync(true);

            var writeObjectsController = new WriteObjectsController(optionsMock.Object, guardMock.Object, null, queueMock.Object);

            var request = new UpdateObjectMetaDataRequest()
            {
                ParentDirectoryIds = new[] { IdGenerator.NewId(IdType.Directory) },
                ObjectName = "NewDirectoryName"
            };
            
            var response = await writeObjectsController.UpdateObjectMetadata(request, IdGenerator.NewId(IdType.Directory), 0);
            Assert.IsType<OkResult>(response);
        }

        [Fact]
        public async void UpdateMetadata_InvalidData()
        {
            var optionsMock = GetAppSettingsMock();
            var queueMock = GetQueueMock();

            var guardMock = new Mock<IGuardApi>();
            guardMock.Setup(api => api.UpdateObjectMetadata(
                    It.IsAny<string>(),
                    It.IsAny<IMetadataUpdate>()))
                .ReturnsAsync(true);

            var writeObjectsController = new WriteObjectsController(optionsMock.Object, guardMock.Object, null, queueMock.Object);

            // No object name
            var noObjectNameRequest = new UpdateObjectMetaDataRequest
            {
                ParentDirectoryIds = new[] { IdGenerator.NewId(IdType.Directory) },
                ObjectName = null
            };

            var response = await writeObjectsController.UpdateObjectMetadata(noObjectNameRequest, IdGenerator.NewId(IdType.Directory), 0);
            Assert.IsType<BadRequestResult>(response);
            

            // No object name
            var parentDirectoryHasFileIdRequest = new UpdateObjectMetaDataRequest
            {
                ParentDirectoryIds = new[] { IdGenerator.NewId(IdType.File) },
                ObjectName = "Abc"
            };

            response = await writeObjectsController.UpdateObjectMetadata(parentDirectoryHasFileIdRequest, IdGenerator.NewId(IdType.Directory), 0);
            Assert.IsType<BadRequestResult>(response);

            
            // Invalid target object id 
            var validRequest = new UpdateObjectMetaDataRequest
            {
                ParentDirectoryIds = new[] { IdGenerator.NewId(IdType.Directory) },
                ObjectName = "Abc"
            };

            response = await writeObjectsController.UpdateObjectMetadata(validRequest, IdGenerator.NewId(IdType.Undefined), 0);
            Assert.IsType<BadRequestResult>(response);
        }

        [Fact]
        public async void UpdateMetadata_Throws()
        {
            var optionsMock = GetAppSettingsMock();
            var queueMock = GetQueueMock();

            var guardMock = new Mock<IGuardApi>();
            guardMock.Setup(api => api.UpdateObjectMetadata(
                    It.IsAny<string>(),
                    It.IsAny<IMetadataUpdate>()))
                .Throws<InvalidOperationException>();

            var writeObjectsController = new WriteObjectsController(optionsMock.Object, guardMock.Object, null, queueMock.Object);

            var request = new UpdateObjectMetaDataRequest()
            {
                ParentDirectoryIds = new[] { IdGenerator.NewId(IdType.Directory) },
                ObjectName = "NewDirectoryName"
            };

            var response = await writeObjectsController.UpdateObjectMetadata(request, IdGenerator.NewId(IdType.Directory), 0);
            Assert.IsType<BadRequestResult>(response);
        }

        [Fact]
        public async void DeleteObject_Correct()
        {
            var optionsMock = GetAppSettingsMock();
            var queueMock = GetQueueMock();

            var guardMock = new Mock<IGuardApi>();
            guardMock.Setup(api => api.DeleteObject(
                    It.IsAny<string>()))
                .ReturnsAsync(true);

            var writeObjectsController = new WriteObjectsController(optionsMock.Object, guardMock.Object, null, queueMock.Object);
            
            var response = await writeObjectsController.DeleteObject(IdGenerator.NewId(IdType.File), 0);
            Assert.IsType<OkResult>(response);
        }

        [Fact]
        public async void DeleteObject_InvalidData()
        {
            var optionsMock = GetAppSettingsMock();
            var queueMock = GetQueueMock();

            var guardMock = new Mock<IGuardApi>();
            guardMock.Setup(api => api.DeleteObject(
                    It.IsAny<string>()))
                .ReturnsAsync(true);

            var writeObjectsController = new WriteObjectsController(optionsMock.Object, guardMock.Object, null, queueMock.Object);

            // Use invalid id to delete
            var response = await writeObjectsController.DeleteObject(IdGenerator.NewId(IdType.Undefined), 0);
            Assert.IsType<BadRequestResult>(response);
        }

        [Fact]
        public async void DeleteObject_Throws()
        {
            var optionsMock = GetAppSettingsMock();
            var queueMock = GetQueueMock();

            var guardMock = new Mock<IGuardApi>();
            guardMock.Setup(api => api.DeleteObject(
                    It.IsAny<string>()))
                .Throws<InvalidOperationException>();

            var writeObjectsController = new WriteObjectsController(optionsMock.Object, guardMock.Object, null, queueMock.Object);

            var response = await writeObjectsController.DeleteObject(IdGenerator.NewId(IdType.File), 0);
            Assert.IsType<BadRequestResult>(response);
        }
    }
}
