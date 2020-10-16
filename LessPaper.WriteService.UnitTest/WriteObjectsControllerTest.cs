using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using LessPaper.Shared.Enums;
using LessPaper.Shared.Helper;
using LessPaper.Shared.Interfaces.Bucket;
using LessPaper.Shared.Interfaces.GuardApi;
using LessPaper.Shared.Interfaces.Queuing;
using LessPaper.Shared.Interfaces.WriteApi.WriteObjectApi;
using LessPaper.Shared.Queueing.Interfaces;
using LessPaper.Shared.Rest.Models.Dtos;
using LessPaper.Shared.Rest.Models.RequestDtos;
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
        private string user1Id = IdGenerator.NewId(IdType.User);
        private CryptoHelper.RsaKeyPair user1PubKey = CryptoHelper.GenerateRsaKeyPair();
        private string user1EncFileKey;
        public WriteObjectsControllerTest()
        {
            user1EncFileKey = CryptoHelper.RsaEncrypt(user1PubKey.PublicKey,CryptoHelper.GetRandomString(16));
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
                Minio = new MinioSettings
                {
                    BucketName = "lesspaper"
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
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<int>(),
                It.IsAny<Dictionary<string, string>>(),
                It.IsAny<DocumentLanguage>(),
                It.IsAny<ExtensionType>())).ReturnsAsync(4);

            var bucketMock = new Mock<IWritableBucket>();
            bucketMock.Setup(x => x.UploadEncrypted(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<int>(),
                It.IsAny<byte[]>(),
                It.IsAny<Stream>()));

            var writeObjectsController = new WriteObjectsController(optionsMock.Object, guardMock.Object, bucketMock.Object, queueMock.Object);


            var requestPayload = new UploadFileDto
            {
                DocumentLanguage = DocumentLanguage.German,
                EncryptedKey = new Dictionary<string, string>()
                {
                    {user1Id, user1EncFileKey}
                },
                FileName = "MyFile.pdf",
                PlaintextKey = Convert.ToBase64String(ivAndKey2),
                FileExtension = ExtensionType.Pdf,
                File = new FormFile(stream, 0, payload.Length, "", "")
            };

            var response = await writeObjectsController.UploadFile(requestPayload, IdGenerator.NewId(IdType.Directory), user1Id);
            var metadataResponseObject = Assert.IsType<OkObjectResult>(response);
            var metadataResponse = Assert.IsType<UploadFileResponse>(metadataResponseObject.Value);

            Assert.True(IdGenerator.IsType(metadataResponse.FileId,IdType.File));
            Assert.True(IdGenerator.IsType(metadataResponse.RevisionId, IdType.FileBlob));
            Assert.Equal(4u,metadataResponse.QuickNumber);
            
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
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<int>(),
                It.IsAny<Dictionary<string, string>>(),
                It.IsAny<DocumentLanguage>(),
                It.IsAny<ExtensionType>())).ReturnsAsync(4);

            var bucketMock = new Mock<IWritableBucket>();
            bucketMock.Setup(x => x.UploadEncrypted(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<int>(),
                It.IsAny<byte[]>(),
                It.IsAny<Stream>()));

            var writeObjectsController = new WriteObjectsController(optionsMock.Object, guardMock.Object, bucketMock.Object, queueMock.Object);


            var requestPayload = new UploadFileDto
            {
                DocumentLanguage = DocumentLanguage.German,
                EncryptedKey = new Dictionary<string, string>()
                {
                    {user1Id, user1EncFileKey.Substring(0,8)}
                },
                FileName = "MyFile.pdf",
                PlaintextKey = Convert.ToBase64String(ivAndKey2),
                FileExtension = ExtensionType.Pdf,
                File = new FormFile(stream, 0, payload.Length, "", "")
            };

            var response = await writeObjectsController.UploadFile(requestPayload, IdGenerator.NewId(IdType.Directory), user1Id);
            Assert.IsType<BadRequestObjectResult>(response);

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
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<int>(),
                It.IsAny<Dictionary<string, string>>(),
                It.IsAny<DocumentLanguage>(),
                It.IsAny<ExtensionType>())).Throws<InvalidOperationException>();

            var bucketMock = new Mock<IWritableBucket>();
            bucketMock.Setup(x => x.UploadEncrypted(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<int>(),
                It.IsAny<byte[]>(),
                It.IsAny<Stream>()));

            var writeObjectsController = new WriteObjectsController(optionsMock.Object, guardMock.Object, bucketMock.Object, queueMock.Object);


            var requestPayload = new UploadFileDto
            {
                DocumentLanguage = DocumentLanguage.German,
                EncryptedKey = new Dictionary<string, string>()
                {
                    {user1Id, user1EncFileKey}
                },
                FileName = "MyFile.pdf",
                PlaintextKey = Convert.ToBase64String(ivAndKey2),
                FileExtension = ExtensionType.Pdf,
                File = new FormFile(stream, 0, payload.Length, "", "")
            };

            var response = await writeObjectsController.UploadFile(requestPayload, IdGenerator.NewId(IdType.Directory), user1Id);
            Assert.IsType<ObjectResult>(response);
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
                .ReturnsAsync(IdGenerator.NewId(IdType.Directory));

            var writeObjectsController = new WriteObjectsController(optionsMock.Object, guardMock.Object, null, queueMock.Object);

            var request = new CreateDirectoryDto()
            {
                SubDirectoryName = "MySubDir"
            };

            var response = await writeObjectsController.CreateDirectory(IdGenerator.NewId(IdType.Directory),user1Id, request);
            var metadataResponseObject = Assert.IsType<OkObjectResult>(response);
            var newDirectoryId = Assert.IsType<string>(metadataResponseObject.Value);

            Assert.True(IdGenerator.TypeFromId(newDirectoryId, out var typeOfId));
            Assert.Equal(IdType.Directory, typeOfId);
        }
        
        [Fact]
        public async void RenameObject_Correct()
        {
            var optionsMock = GetAppSettingsMock();
            var queueMock = GetQueueMock();

            var guardMock = new Mock<IGuardApi>();
            guardMock.Setup(api => api.RenameObject(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>())).ReturnsAsync(true);

            var writeObjectsController = new WriteObjectsController(optionsMock.Object, guardMock.Object, null, queueMock.Object);

            var request = new UpdateObjectMetaDataRequest()
            {
                ParentDirectoryIds = new[] { IdGenerator.NewId(IdType.Directory) },
                ObjectName = "NewDirectoryName"
            };
            
            var response = await writeObjectsController.RenameObject(user1Id,IdGenerator.NewId(IdType.Directory),  "newDirectoryName");
            Assert.IsType<OkResult>(response);
        }
        
        [Fact]
        public async void RenameObject_InvalidData()
        {
            var optionsMock = GetAppSettingsMock();
            var queueMock = GetQueueMock();

            var guardMock = new Mock<IGuardApi>();
            guardMock.Setup(api => api.RenameObject(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>())).ReturnsAsync(true);

            var writeObjectsController = new WriteObjectsController(optionsMock.Object, guardMock.Object, null, queueMock.Object);

            // No object name
            var response = await writeObjectsController.RenameObject(user1Id,IdGenerator.NewId(IdType.Directory), null);
            Assert.IsType<BadRequestResult>(response);
            
            // Invalid target object id 
            response = await writeObjectsController.RenameObject( user1Id, IdGenerator.NewId(IdType.Undefined),"Abc");
            Assert.IsType<BadRequestResult>(response);
        }

        
        [Fact]
        public async void RenameObject_Throws()
        {
            var optionsMock = GetAppSettingsMock();
            var queueMock = GetQueueMock();

            var guardMock = new Mock<IGuardApi>();
            guardMock.Setup(api => api.RenameObject(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>()))
                .Throws<InvalidOperationException>();

            var writeObjectsController = new WriteObjectsController(optionsMock.Object, guardMock.Object, null, queueMock.Object);

            var response = await writeObjectsController.RenameObject(user1Id, IdGenerator.NewId(IdType.Directory),"Abc");
            Assert.IsType<BadRequestResult>(response);
        }

        /*
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
        */
        
    }
}
