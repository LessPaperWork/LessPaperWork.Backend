
namespace LessPaper.Shared.Interfaces.WriteApi.WriteObjectApi
{
    public interface IMetadataUpdate
    {
        /// <summary>
        /// Object Name
        /// </summary>
        string ObjectName { get; }

        /// <summary>
        /// List of parent directories
        /// </summary>
        string[] ParentDirectoryIds { get; }
    }
}
