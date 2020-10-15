using LessPaper.ReadService.Options;
using LessPaper.Shared.Interfaces.Bucket;
using LessPaper.Shared.Interfaces.GuardApi;
using LessPaper.Shared.Interfaces.Queuing;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace LessPaper.ReadService.Controllers.v1
{
    [Route("v1/objects")]
    [ApiController]
    public class ReadObjectsController : ControllerBase
    {
        private readonly IOptions<AppSettings> config;
        private readonly IGuardApi guardApi;
        private readonly IReadableBucket bucket;

        public ReadObjectsController(IOptions<AppSettings> config, IGuardApi guardApi, IReadableBucket bucket)
        {
            this.config = config;
            this.guardApi = guardApi;
            this.bucket = bucket;
        }

    }
}
