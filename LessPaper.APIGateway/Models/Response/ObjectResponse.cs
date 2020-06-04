using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using LessPaper.Shared.Enums;
using LessPaper.Shared.Interfaces.General;

namespace LessPaper.APIGateway.Models.Response
{
    public class ObjectResponse : IMetadata
    {
        public ObjectResponse(IMetadata metadata)
        {
            ObjectName = metadata.ObjectName;
            ObjectId = metadata.ObjectId;
            Permissions = metadata.Permissions;
            Path = metadata.Path;
        }

        /// <inheritdoc />
        [JsonPropertyName("object_name")]
        public string ObjectName { get; }

        /// <inheritdoc />
        public Dictionary<string, Permission> Permissions { get; set; }

        /// <inheritdoc />
        public string Path { get; set; }

        /// <inheritdoc />
        [JsonPropertyName("object_id")]
        public string ObjectId { get; }

    }
}
