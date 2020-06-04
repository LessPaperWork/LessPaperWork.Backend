using System;
using System.Collections.Generic;
using System.Text;

namespace LessPaper.Shared.Interfaces.General
{
    public interface IIdentifiable
    {
        /// <summary>
        /// Unique object id
        /// </summary>
        string ObjectId { get; }
    }
}
