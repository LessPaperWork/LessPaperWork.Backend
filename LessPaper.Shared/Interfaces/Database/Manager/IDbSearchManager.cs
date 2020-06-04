using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using LessPaper.Shared.Interfaces.General;

namespace LessPaper.Shared.Interfaces.Database.Manager
{
    public interface IDbSearchManager
    { 
        /// <summary>
        /// Search for files and directories
        /// </summary>
        /// <param name="requestingUserId">Id of the requesting user</param>
        /// <param name="directoryId">Search root directory</param>
        /// <param name="searchQuery">Search query</param>
        /// <param name="count">Limit of results</param>
        /// <param name="page">Result page</param>
        /// <returns>List of files and directories</returns>
        /// <exception cref="InvalidOperationException"></exception>
        Task<ISearchResult> Search(string requestingUserId, string directoryId, string searchQuery, uint count, uint page);
    }
}
