using LessPaper.Shared.Interfaces.Database;
using Microsoft.Extensions.Options;

namespace LessPaper.GuardService.Options
{
    public class DatabaseSettings : IDatabaseSettings
    {
        public DatabaseSettings()
        {
            
        }

        public DatabaseSettings(IOptions<AppSettings> settings)
        {
            ConnectionString = settings.Value.DatabaseSettings.ConnectionString;
        }


        /// <inheritdoc />
        public string ConnectionString { get; set; }
    }
}
