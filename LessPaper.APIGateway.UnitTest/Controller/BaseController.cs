using System;
using System.Collections.Generic;
using System.Text;
using LessPaper.APIGateway.Options;
using Microsoft.Extensions.Options;

namespace LessPaper.APIGateway.UnitTest.Controller
{
    public class BaseController
    {
        protected IOptions<AppSettings> AppSettings;

        public BaseController()
        {
            var appSettings = new AppSettings
            {
                ValidationRules = new ValidationRules {MinimumPasswordLength = 7},
                JwtSettings = new JwtSettings {Secret = "my_test_secret"}
            };

            AppSettings = new AppSettingsOptions(appSettings);
        }
    }


    public class AppSettingsOptions : IOptions<AppSettings>
    {
        public AppSettingsOptions(AppSettings appSettings)
        {
            Value = appSettings;
        }

        /// <inheritdoc />
        public AppSettings Value { get; }
    }
}
