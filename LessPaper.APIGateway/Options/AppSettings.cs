using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LessPaper.APIGateway.Options
{
    public class AppSettings
    {
        public JwtSettings JwtSettings { get; set; }

        public ValidationRules ValidationRules { get; set; }
    }

    public class JwtSettings {

        // TODO Determine better way to handle the jwt secret
        public string Secret { get; set; }
    }

    public class ValidationRules
    {
        public uint MinimumPasswordLength { get; set; }
    }
}
