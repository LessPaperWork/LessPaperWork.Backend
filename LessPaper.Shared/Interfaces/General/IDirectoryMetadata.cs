namespace LessPaper.Shared.Interfaces.General
{
    public interface IDirectoryMetadata: IMinimalDirectoryMetadata
    {
        /// <summary>
        /// Information of file childs
        /// </summary>
        IFileMetadata[] FileChilds { get; }

        /// <summary>
        /// Minimal information of directory childs
        /// </summary>
        IMinimalDirectoryMetadata[] DirectoryChilds { get; }


    }
}
