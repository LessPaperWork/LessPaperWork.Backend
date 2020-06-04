using LessPaper.Shared.Enums;
using LessPaper.Shared.Interfaces.General;

namespace LessPaper.Shared.Interfaces.GuardApi.Response
{
    public interface IPermissionResponse : IIdentifiable
    {
        Permission Permission { get; }
    }
}
