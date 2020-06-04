using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LessPaper.Shared.Interfaces.ReadApi;
using LessPaper.Shared.Interfaces.ReadApi.ReadObjectApi;

namespace LessPaper.APIGateway.Models
{
    public class ReadApi : IReadApi
    {
        /// <inheritdoc />
        public IReadObjectApi ObjectApi { get; }
    }
}
