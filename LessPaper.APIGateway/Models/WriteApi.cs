using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LessPaper.Shared.Interfaces.WriteApi;
using LessPaper.Shared.Interfaces.WriteApi.WriteObjectApi;

namespace LessPaper.APIGateway.Models
{
    public class WriteApi : IWriteApi
    {
        /// <inheritdoc />
        public IWriteObjectApi ObjectApi { get; }
    }
}
