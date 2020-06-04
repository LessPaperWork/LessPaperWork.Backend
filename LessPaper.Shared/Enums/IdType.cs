using System;
using System.Collections.Generic;
using System.Text;

namespace LessPaper.Shared.Enums
{
    public enum IdType
    {
        Undefined = 00,

        User      = 01,

        File      = 02,
        Directory = 03,

        FileBlob = 04,

        Tag        = 05,
        Permission = 06
    }
}
