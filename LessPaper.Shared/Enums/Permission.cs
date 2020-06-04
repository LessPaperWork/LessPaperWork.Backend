using System;
using System.Collections.Generic;
using System.Text;

namespace LessPaper.Shared.Enums
{
    [Flags]
    public enum Permission
    {
        Read = 1,
        ReadWrite = 3,

        ReadPermissions = 4,
        ReadWritePermissions = 12,

        Share = 16
    }
}
