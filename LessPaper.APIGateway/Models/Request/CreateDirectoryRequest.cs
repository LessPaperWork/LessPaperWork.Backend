using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace LessPaper.APIGateway.Models.Request
{
    public class CreateDirectoryRequest
    {
        [Required]
        [JsonPropertyName("sub_directory_name")]
        [ModelBinder(Name = "sub_directory_name")]
        public string SubDirectoryName { get; set; }
    }
}
