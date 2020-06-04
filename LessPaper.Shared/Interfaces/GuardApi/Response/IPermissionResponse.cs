using LessPaper.Shared.Enums;

namespace LessPaper.Shared.Interfaces.GuardApi.Response
{
    public interface IPermissionResponse
    {
        string ObjectId { get; }
        
        Permission Permission { get; }
    }
}
