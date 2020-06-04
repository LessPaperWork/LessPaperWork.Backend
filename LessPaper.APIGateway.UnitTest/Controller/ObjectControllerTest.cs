using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using LessPaper.APIGateway.Controllers.v1;
using LessPaper.APIGateway.Models.Request;
using LessPaper.APIGateway.Models.Response;
using LessPaper.Shared.Enums;
using LessPaper.Shared.Interfaces.General;
using LessPaper.Shared.Interfaces.GuardApi;
using LessPaper.Shared.Interfaces.ReadApi;
using LessPaper.Shared.Interfaces.ReadApi.ReadObjectApi;
using LessPaper.Shared.Interfaces.WriteApi;
using LessPaper.Shared.Interfaces.WriteApi.WriteObjectApi;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace LessPaper.APIGateway.UnitTest.Controller
{
    public class ObjectControllerTest : BaseController
    {
        private readonly byte[] myFile;
        private readonly UploadFileRequest request;
        private readonly MemoryStream stream;
        public ObjectControllerTest()
        {
            // Setup dummy request
            myFile = new byte[] { 0, 1, 2, 3 };
            stream = new MemoryStream(myFile);
            IFormFile file = new FormFile(stream, 0, myFile.Length, "name", "fileName");
            request = new UploadFileRequest()
            {
                PlaintextKey = "MyPlaintextKey",
                EncryptedKey = "MyEncryptedKey",
                File = file,
                Name = "MyDoc.pdf",
                DocumentLanguage = DocumentLanguage.English
            };
        }


        [Fact]
        public async void UploadFileToKnownLocation_Ok()
        {
            // Setup dummy response
            var uploadResponse = new Mock<IUploadMetadata>();
            uploadResponse.SetupGet(x => x.ObjectId).Returns("MyId");
            uploadResponse.SetupGet(x => x.SizeInBytes).Returns((uint)myFile.Length);
            uploadResponse.SetupGet(x => x.QuickNumber).Returns(1);
            uploadResponse.SetupGet(x => x.ObjectName).Returns(request.Name);

            // Mock apis
            var readApiMock = new Mock<IReadApi>();
            var writeApiMock = new Mock<IWriteApi>();
            writeApiMock.Setup(mock =>
                mock.ObjectApi.UploadFile(
                    It.IsAny<string>(),
                    It.IsAny<Stream>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<DocumentLanguage>(), 
                    It.IsAny<ExtensionType>())
            ).ReturnsAsync(uploadResponse.Object);

            var controller = new ObjectController(AppSettings, writeApiMock.Object, readApiMock.Object);

            // Query controller
            var response = await controller.UploadFileToKnownLocation(request, "myDirectoryId", 4);
            var metadataResponseObject = Assert.IsType<OkObjectResult>(response);
            var metadataResponse = Assert.IsType<UploadFileResponse>(metadataResponseObject.Value);

            // Compare values
            Assert.Equal(uploadResponse.Object.ObjectId, metadataResponse.ObjectId);
            Assert.Equal(uploadResponse.Object.QuickNumber, metadataResponse.QuickNumber);
            Assert.Equal(uploadResponse.Object.ObjectName, metadataResponse.ObjectName);
            Assert.Equal(uploadResponse.Object.SizeInBytes, metadataResponse.SizeInBytes);
        }


        [Fact]
        public async void UploadFileToKnownLocation_Throws()
        {
            // Mock apis
            var readApiMock = new Mock<IReadApi>();
            var writeApiMock = new Mock<IWriteApi>();
            writeApiMock.Setup(mock =>
                mock.ObjectApi.UploadFile(
                    It.IsAny<string>(),
                    It.IsAny<Stream>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<DocumentLanguage>(),
                    It.IsAny<ExtensionType>())
            ).Throws<InvalidOperationException>();

            var controller = new ObjectController(AppSettings, writeApiMock.Object, readApiMock.Object);

            var response = await controller.UploadFileToKnownLocation(request, "myDirectoryId", 4);
            Assert.IsType<BadRequestResult>(response);
        }

        [Fact]
        public async void CreateDirectory_Ok()
        {
            // Mock apis
            var readApiMock = new Mock<IReadApi>();
            var writeApiMock = new Mock<IWriteApi>();

            // Setup dummy response
            var directoryMetadataMock = new Mock<IDirectoryMetadata>();
            directoryMetadataMock.SetupGet(x => x.ObjectId).Returns("MyDirectoryId");
            directoryMetadataMock.SetupGet(x => x.SizeInBytes).Returns((uint)myFile.Length);
            directoryMetadataMock.SetupGet(x => x.ObjectName).Returns("MyDirectory");
            directoryMetadataMock.SetupGet(x => x.FileChilds).Returns(new IFileMetadata[0]);
            directoryMetadataMock.SetupGet(x => x.DirectoryChilds).Returns(new IMinimalDirectoryMetadata[0]);
            directoryMetadataMock.SetupGet(x => x.LatestViewDate).Returns(DateTime.MaxValue);
            directoryMetadataMock.SetupGet(x => x.LatestChangeDate).Returns(DateTime.MinValue);
            directoryMetadataMock.SetupGet(x => x.NumberOfChilds).Returns(1);

            var directoryMetadataMockObject = directoryMetadataMock.Object;

            writeApiMock.Setup(mock =>
                mock.ObjectApi.CreateDirectory(
                    It.IsAny<string>(),
                    It.IsAny<string>()
                )).ReturnsAsync(directoryMetadataMockObject);

            var controller = new ObjectController(AppSettings, writeApiMock.Object, readApiMock.Object);

            var response = await controller.CreateDirectory("Test", new CreateDirectoryRequest()
            {
                SubDirectoryName = "SubTest"
            });
            var responseObject = Assert.IsType<OkObjectResult>(response);
            var directoryMetadata = Assert.IsAssignableFrom<IDirectoryMetadata>(responseObject.Value);
            Assert.Equal(directoryMetadataMockObject.ObjectId,directoryMetadata.ObjectId);
            Assert.Equal(directoryMetadataMockObject.SizeInBytes, directoryMetadata.SizeInBytes);
            Assert.Equal(directoryMetadataMockObject.ObjectName, directoryMetadata.ObjectName);
            Assert.Equal(directoryMetadataMockObject.FileChilds, directoryMetadata.FileChilds);
            Assert.Equal(directoryMetadataMockObject.DirectoryChilds, directoryMetadata.DirectoryChilds);
            Assert.Equal(directoryMetadataMockObject.LatestChangeDate, directoryMetadata.LatestChangeDate);
            Assert.Equal(directoryMetadataMockObject.LatestViewDate, directoryMetadata.LatestViewDate);
            Assert.Equal(directoryMetadataMockObject.NumberOfChilds, directoryMetadata.NumberOfChilds);

        }

        [Fact]
        public async void CreateDirectory_Throws()
        {
            // Mock apis
            var readApiMock = new Mock<IReadApi>();
            var writeApiMock = new Mock<IWriteApi>();

            writeApiMock.Setup(mock =>
                mock.ObjectApi.CreateDirectory(
                    It.IsAny<string>(),
                    It.IsAny<string>()
                )).Throws<InvalidOperationException>();

            var controller = new ObjectController(AppSettings, writeApiMock.Object, readApiMock.Object);

            var response = await controller.CreateDirectory("Test", new CreateDirectoryRequest()
            {
                SubDirectoryName = "SubTest"
            });

            Assert.IsType<BadRequestResult>(response);
        }

        [Fact]
        public async void GetObject_Ok()
        {
            // Mock apis
            var writeApiMock = new Mock<IWriteApi>();
            var readApiMock = new Mock<IReadApi>();
            readApiMock.Setup(mock =>
                mock.ObjectApi.GetObject(
                    It.IsAny<string>(),
                    It.IsAny<uint>())
            ).ReturnsAsync(stream);

            var controller = new ObjectController(AppSettings, writeApiMock.Object, readApiMock.Object);

            var response = await controller.GetObject("MyObjId", 4);
            var metadataResponseObject = Assert.IsType<FileStreamResult>(response);
            Assert.Equal(myFile, Stream2Array(metadataResponseObject.FileStream));
        }

        [Fact]
        public async void GetObject_Throws()
        {
            // Mock apis
            var readApiMock = new Mock<IReadApi>();
            var writeApiMock = new Mock<IWriteApi>();
            readApiMock.Setup(mock =>
                mock.ObjectApi.GetObject(
                    It.IsAny<string>(),
                    It.IsAny<uint>())
            ).Throws<InvalidOperationException>();

            var controller = new ObjectController(AppSettings, writeApiMock.Object, readApiMock.Object);

            var response = await controller.GetObject("MyObjId", 4);
            Assert.IsType<BadRequestResult>(response);
        }

        [Fact]
        public async void GetObjectMetadata_Ok()
        {
            // Setup dummy tag
            var tagMock = new Mock<ITag>();
            tagMock.SetupGet(x => x.Value).Returns("MyTag1");
            tagMock.SetupGet(x => x.Relevance).Returns(0.8f);
            tagMock.SetupGet(x => x.Source).Returns(TagSource.User);

            // Setup dummy response
            var fileMetadataMock = new Mock<IFileMetadata>();
            fileMetadataMock.SetupGet(x => x.ObjectId).Returns("MyFileId");
            fileMetadataMock.SetupGet(x => x.SizeInBytes).Returns((uint)myFile.Length);
            fileMetadataMock.SetupGet(x => x.ObjectName).Returns(request.Name);
            fileMetadataMock.SetupGet(x => x.EncryptionKey).Returns("MyEncryptionKey");
            fileMetadataMock.SetupGet(x => x.Extension).Returns(ExtensionType.Pdf);
            fileMetadataMock.SetupGet(x => x.Hash).Returns("MyFileHash");
            fileMetadataMock.SetupGet(x => x.ParentDirectoryIds).Returns(new[] { "MyDirectoryId" });
            fileMetadataMock.SetupGet(x => x.RevisionNumber).Returns(0);
            fileMetadataMock.SetupGet(x => x.Revisions).Returns(new[] { 0u });
            fileMetadataMock.SetupGet(x => x.Tags).Returns(new[] { tagMock.Object });
            fileMetadataMock.SetupGet(x => x.ThumbnailId).Returns("MyThumbnailId");
            fileMetadataMock.SetupGet(x => x.UploadDate).Returns(DateTime.MinValue);

            // Setup dummy response
            var directoryMetadataMock = new Mock<IDirectoryMetadata>();
            directoryMetadataMock.SetupGet(x => x.ObjectId).Returns("MyDirectoryId");
            directoryMetadataMock.SetupGet(x => x.SizeInBytes).Returns((uint)myFile.Length);
            directoryMetadataMock.SetupGet(x => x.ObjectName).Returns("MyDirectory");
            directoryMetadataMock.SetupGet(x => x.FileChilds).Returns(new[] { fileMetadataMock.Object });
            directoryMetadataMock.SetupGet(x => x.DirectoryChilds).Returns(new IMinimalDirectoryMetadata[0]);
            directoryMetadataMock.SetupGet(x => x.LatestViewDate).Returns(DateTime.MaxValue);
            directoryMetadataMock.SetupGet(x => x.LatestChangeDate).Returns(DateTime.MinValue);
            directoryMetadataMock.SetupGet(x => x.NumberOfChilds).Returns(1);


            // Mock apis
            var writeApiMock = new Mock<IWriteApi>();
            var readApiMock = new Mock<IReadApi>();
            readApiMock.Setup(mock =>
                mock.ObjectApi.GetMetadata(
                    It.IsAny<string>(),
                    It.IsAny<uint?>())
            ).ReturnsAsync(fileMetadataMock.Object);
            var controller = new ObjectController(AppSettings, writeApiMock.Object, readApiMock.Object);

            // Query controller
            var response = await controller.GetObjectMetadata("MyFileId", 0);
            var metadataResponseObject = Assert.IsType<OkObjectResult>(response);
            var fileMetadataResponse = Assert.IsType<FileMetadataResponse>(metadataResponseObject.Value);

            Assert.Equal(fileMetadataMock.Object.Extension, fileMetadataResponse.Extension);
            Assert.Equal(fileMetadataMock.Object.Hash, fileMetadataResponse.Hash);
            Assert.Equal(fileMetadataMock.Object.ParentDirectoryIds, fileMetadataResponse.ParentDirectoryIds);
            Assert.Equal(fileMetadataMock.Object.QuickNumber, fileMetadataResponse.QuickNumber);
            Assert.Equal(fileMetadataMock.Object.RevisionNumber, fileMetadataResponse.RevisionNumber);
            Assert.Equal(fileMetadataMock.Object.Revisions, fileMetadataResponse.Revisions);

            Assert.Single(fileMetadataResponse.Tags);
            Assert.Equal(tagMock.Object.Value, fileMetadataResponse.Tags[0].Value);
            Assert.Equal(tagMock.Object.Relevance, fileMetadataResponse.Tags[0].Relevance);
            Assert.Equal(tagMock.Object.Source, fileMetadataResponse.Tags[0].Source);

            Assert.Equal(fileMetadataMock.Object.ThumbnailId, fileMetadataResponse.ThumbnailId);
            Assert.Equal(fileMetadataMock.Object.UploadDate, fileMetadataResponse.UploadDate);
            Assert.Equal(fileMetadataMock.Object.EncryptionKey, fileMetadataResponse.EncryptionKey);
            Assert.Equal(fileMetadataMock.Object.ObjectName, fileMetadataResponse.ObjectName);
            Assert.Equal(fileMetadataMock.Object.ObjectId, fileMetadataResponse.ObjectId);
            Assert.Equal(fileMetadataMock.Object.SizeInBytes, fileMetadataResponse.SizeInBytes);


            readApiMock.Setup(mock =>
                mock.ObjectApi.GetMetadata(
                    It.IsAny<string>(),
                    It.IsAny<uint?>())
            ).ReturnsAsync(directoryMetadataMock.Object);
            controller = new ObjectController(AppSettings, writeApiMock.Object, readApiMock.Object);


            // Query controller
            response = await controller.GetObjectMetadata("MyDirectoryId", 0);
            metadataResponseObject = Assert.IsType<OkObjectResult>(response);
            var directoryMetadataResponse = Assert.IsType<DirectoryMetadataResponse>(metadataResponseObject.Value);

            Assert.Equal(directoryMetadataMock.Object.DirectoryChilds, directoryMetadataResponse.DirectoryChilds);

            Assert.Single(directoryMetadataResponse.FileChilds);
            Assert.Equal(directoryMetadataMock.Object.FileChilds[0].Extension, directoryMetadataResponse.FileChilds[0].Extension);
            Assert.Equal(directoryMetadataMock.Object.FileChilds[0].Hash, directoryMetadataResponse.FileChilds[0].Hash);
            Assert.Equal(directoryMetadataMock.Object.FileChilds[0].ParentDirectoryIds, directoryMetadataResponse.FileChilds[0].ParentDirectoryIds);
            Assert.Equal(directoryMetadataMock.Object.FileChilds[0].QuickNumber, directoryMetadataResponse.FileChilds[0].QuickNumber);
            Assert.Equal(directoryMetadataMock.Object.FileChilds[0].RevisionNumber, directoryMetadataResponse.FileChilds[0].RevisionNumber);
            Assert.Equal(directoryMetadataMock.Object.FileChilds[0].Revisions, directoryMetadataResponse.FileChilds[0].Revisions);

            Assert.Single(directoryMetadataResponse.FileChilds[0].Tags);
            Assert.Equal(tagMock.Object.Value, directoryMetadataResponse.FileChilds[0].Tags[0].Value);
            Assert.Equal(tagMock.Object.Relevance, directoryMetadataResponse.FileChilds[0].Tags[0].Relevance);
            Assert.Equal(tagMock.Object.Source, directoryMetadataResponse.FileChilds[0].Tags[0].Source);

            Assert.Equal(directoryMetadataMock.Object.FileChilds[0].ThumbnailId, directoryMetadataResponse.FileChilds[0].ThumbnailId);
            Assert.Equal(directoryMetadataMock.Object.FileChilds[0].UploadDate, directoryMetadataResponse.FileChilds[0].UploadDate);
            Assert.Equal(directoryMetadataMock.Object.FileChilds[0].EncryptionKey, directoryMetadataResponse.FileChilds[0].EncryptionKey);
            Assert.Equal(directoryMetadataMock.Object.FileChilds[0].ObjectName, directoryMetadataResponse.FileChilds[0].ObjectName);
            Assert.Equal(directoryMetadataMock.Object.FileChilds[0].ObjectId, directoryMetadataResponse.FileChilds[0].ObjectId);
            Assert.Equal(directoryMetadataMock.Object.FileChilds[0].SizeInBytes, directoryMetadataResponse.FileChilds[0].SizeInBytes);

            Assert.Equal(directoryMetadataMock.Object.ObjectName, directoryMetadataResponse.ObjectName);
            Assert.Equal(directoryMetadataMock.Object.LatestChangeDate, directoryMetadataResponse.LatestChangeDate);
            Assert.Equal(directoryMetadataMock.Object.LatestViewDate, directoryMetadataResponse.LatestViewDate);
            Assert.Equal(directoryMetadataMock.Object.NumberOfChilds, directoryMetadataResponse.NumberOfChilds);
            Assert.Equal(directoryMetadataMock.Object.ObjectId, directoryMetadataResponse.ObjectId);
            Assert.Equal(directoryMetadataMock.Object.ObjectName, directoryMetadataResponse.ObjectName);
            Assert.Equal(directoryMetadataMock.Object.SizeInBytes, directoryMetadataResponse.SizeInBytes);
        }

        [Fact]
        public async void GetObjectMetadata_Throws()
        {
            // Setup dummy response
            var directoryMetadataMock = new Mock<IDirectoryMetadata>();
            directoryMetadataMock.SetupGet(x => x.ObjectId).Returns("MyDirectoryId");
            directoryMetadataMock.SetupGet(x => x.SizeInBytes).Returns((uint)myFile.Length);
            directoryMetadataMock.SetupGet(x => x.ObjectName).Returns("MyDirectory");
            directoryMetadataMock.SetupGet(x => x.FileChilds).Returns(new IFileMetadata[0]);
            directoryMetadataMock.SetupGet(x => x.DirectoryChilds).Returns(new IMinimalDirectoryMetadata[0]);
            directoryMetadataMock.SetupGet(x => x.LatestViewDate).Returns(DateTime.MaxValue);
            directoryMetadataMock.SetupGet(x => x.LatestChangeDate).Returns(DateTime.MinValue);
            directoryMetadataMock.SetupGet(x => x.NumberOfChilds).Returns(1);

            // Mock apis
            var writeApiMock = new Mock<IWriteApi>();
            var readApiMock = new Mock<IReadApi>();
            readApiMock.Setup(mock =>
                mock.ObjectApi.GetMetadata(
                    It.IsAny<string>(),
                    It.IsAny<uint?>())
            ).Throws<InvalidOperationException>();
            var controller = new ObjectController(AppSettings, writeApiMock.Object, readApiMock.Object);

            // Query controller
            var response = await controller.GetObjectMetadata("MyDirectoryId", 0);
            Assert.IsType<BadRequestResult>(response);
        }

        [Fact]
        public async void UpdateObjectMetadata_Ok()
        {
            // Mock apis
            var writeApiMock = new Mock<IWriteApi>();
            var readApiMock = new Mock<IReadApi>();
            writeApiMock.Setup(mock =>
                mock.ObjectApi.UpdateMetadata(
                    It.IsAny<string>(),
                    It.IsAny<IMetadataUpdate>())
            ).ReturnsAsync(true);

            var controller = new ObjectController(AppSettings, writeApiMock.Object, readApiMock.Object);

            var updatedMetadata = new UpdateFileMetaDataRequest()
            {
                ObjectName = "NewFilename",
                ParentDirectoryIds = new[] { "Dir1", "Dir2" }
            };

            var response = await controller.UpdateObjectMetadata(updatedMetadata, "ObjId", 4);
            Assert.IsType<OkResult>(response);
        }

        [Fact]
        public async void UpdateObjectMetadata_Throw()
        {
            // Mock apis
            var writeApiMock = new Mock<IWriteApi>();
            var readApiMock = new Mock<IReadApi>();
            writeApiMock.Setup(mock =>
                mock.ObjectApi.UpdateMetadata(
                    It.IsAny<string>(),
                    It.IsAny<IMetadataUpdate>())
            ).Throws<InvalidOperationException>();

            var controller = new ObjectController(AppSettings, writeApiMock.Object, readApiMock.Object);

            var updatedMetadata = new UpdateFileMetaDataRequest()
            {
                ObjectName = "NewFilename",
                ParentDirectoryIds = new[] { "Dir1", "Dir2" }
            };

            var response = await controller.UpdateObjectMetadata(updatedMetadata, "ObjId", 4);
            Assert.IsType<BadRequestResult>(response);
        }

        [Fact]
        public async void DeleteObject_Ok()
        {
            // Mock apis
            var writeApiMock = new Mock<IWriteApi>();
            var readApiMock = new Mock<IReadApi>();
            writeApiMock.Setup(mock =>
                mock.ObjectApi.DeleteObject(It.IsAny<string>())
            ).ReturnsAsync(true);

            var controller = new ObjectController(AppSettings, writeApiMock.Object, readApiMock.Object);

            var response = await controller.DeleteObject("ObjId", 4);
            Assert.IsType<OkResult>(response);
        }

        [Fact]
        public async void DeleteObject_Throw()
        {
            // Mock apis
            var writeApiMock = new Mock<IWriteApi>();
            var readApiMock = new Mock<IReadApi>();
            writeApiMock.Setup(mock =>
                mock.ObjectApi.DeleteObject(It.IsAny<string>())
               ).Throws<InvalidOperationException>();

            var controller = new ObjectController(AppSettings, writeApiMock.Object, readApiMock.Object);

            var response = await controller.DeleteObject("ObjId", 4);
            Assert.IsType<BadRequestResult>(response);
        }

        [Fact]
        public async void Search_Ok()
        {
            // Setup dummy response
            var searchResponseMock = new Mock<ISearchResponse>();
            searchResponseMock.SetupGet(x => x.SearchQuery).Returns("My search");
            searchResponseMock.SetupGet(x => x.Directories).Returns(new IMinimalDirectoryMetadata[0]);
            searchResponseMock.SetupGet(x => x.Files).Returns(new IFileMetadata[0]);


            // Mock apis
            var writeApiMock = new Mock<IWriteApi>();
            var readApiMock = new Mock<IReadApi>();
            readApiMock.Setup(mock =>
                mock.ObjectApi.Search(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<uint>(),
                    It.IsAny<uint>())
            ).ReturnsAsync(searchResponseMock.Object);

            var controller = new ObjectController(AppSettings, writeApiMock.Object, readApiMock.Object);

            var response = await controller.SearchObject("MyDirId", "My search", 10, 4);
            var searchResponseObject = Assert.IsType<OkObjectResult>(response);
            var searchResponse = Assert.IsType<SearchResponse>(searchResponseObject.Value);

            Assert.Equal(searchResponseMock.Object.Directories, searchResponse.Directories);
            Assert.Equal(searchResponseMock.Object.Files, searchResponse.Files);
            Assert.Equal(searchResponseMock.Object.SearchQuery, searchResponse.SearchQuery);
        }

        [Fact]
        public async void Search_Throws()
        {
            // Mock apis
            var writeApiMock = new Mock<IWriteApi>();
            var readApiMock = new Mock<IReadApi>();
            readApiMock.Setup(mock =>
                mock.ObjectApi.Search(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<uint>(),
                    It.IsAny<uint>())
            ).Throws<InvalidOperationException>();

            var controller = new ObjectController(AppSettings, writeApiMock.Object, readApiMock.Object);
            var response = await controller.SearchObject("MyDirId", "My search", 10, 4);

            Assert.IsType<BadRequestResult>(response);
        }

        public static byte[] Stream2Array(Stream input)
        {
            using var ms = new MemoryStream();
            input.CopyTo(ms);
            return ms.ToArray();
        }

    }
}