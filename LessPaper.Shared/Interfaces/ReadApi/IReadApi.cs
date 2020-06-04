using LessPaper.Shared.Interfaces.ReadApi.ReadObjectApi;

namespace LessPaper.Shared.Interfaces.ReadApi
{
    public interface IReadApi
    {
        /// <summary>
        /// Api for reading objects like files, directories or specific properties
        /// </summary>
        IReadObjectApi ObjectApi { get; }
    }
}
