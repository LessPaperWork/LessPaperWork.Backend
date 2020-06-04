using LessPaper.Shared.Interfaces.General;

namespace LessPaper.Shared.Interfaces.WriteApi.WriteObjectApi
{
    public interface IUploadMetadata: IMetadata
    {
        /// <summary>
        /// Unique id set on uploading file
        /// </summary>
        uint QuickNumber { get;  }
    }
}
