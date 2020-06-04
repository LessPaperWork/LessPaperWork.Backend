using System;
using System.Collections.Generic;
using System.Text;

namespace LessPaper.Shared.Interfaces.Database
{
    public interface IDatabaseSettings
    {
        /// <summary>
        /// Database connection information
        /// </summary>
        string ConnectionString { get; }
    }
}
