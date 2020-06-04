using System;
using System.Collections.Generic;
using LessPaper.Shared.Enums;

namespace LessPaper.Shared.Interfaces.General
{
    public interface IMinimalDirectoryMetadata: IMetadata
    {
        /// <summary>
        /// Number of total childs in the directory
        /// </summary>
        uint NumberOfChilds { get; }


    }
}
