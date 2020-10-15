using LessPaper.Shared.Rest.Interface;
using Microsoft.Extensions.Options;

namespace LessPaper.ReadService.Options
{
    public class GuardClientSettings : IGuardClientSettings
    {
        public GuardClientSettings()
        {
        }
        
        public GuardClientSettings(IOptions<AppSettings> settings)
        {
            BaseUrl = settings.Value.GuardService.BaseUrl;
        }

        public string BaseUrl { get; set; }
    }
}
