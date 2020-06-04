namespace LessPaper.Shared.Interfaces.General
{
    public interface ISearchResult
    {
        /// <summary>
        /// Search query
        /// </summary>
        string SearchQuery { get; }
        /// <summary>
        /// Found files
        /// </summary>
        IFileMetadata[] Files { get; }

        /// <summary>
        /// Found directories
        /// </summary>
        IMinimalDirectoryMetadata[] Directories { get; }
    }
}
