using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LessPaper.Shared.Rest.Interface;
using Microsoft.Extensions.Options;

namespace LessPaper.WriteService.Options
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
