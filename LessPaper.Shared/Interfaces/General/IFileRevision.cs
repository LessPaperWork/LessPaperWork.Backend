using System;
using System.Collections.Generic;
using System.Text;
using LessPaper.Shared.Enums;

namespace LessPaper.Shared.Interfaces.General
{
    public interface IFileRevision : IIdentifiable
    {
        /// <summary>
        /// Unique id set on uploading file
        /// </summary>
        uint QuickNumber { get; }
        
        /// <summary>
        /// Size of the object in Bytes
        /// </summary>
        uint SizeInBytes { get; }

        /// <summary>
        /// Date of last change
        /// </summary>
        DateTime ChangeDate { get; }
        
        /// <summary>
        /// Permissions
        /// 
        /// Key: UserId
        /// Value: Encrypted key
        /// </summary>
        Dictionary<string, IAccessKey> AccessKeys { get; }
    }
}
