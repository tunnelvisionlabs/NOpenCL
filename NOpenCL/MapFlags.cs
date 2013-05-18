/*
 * Copyright (c) 2013 Sam Harwell, Tunnel Vision Laboratories LLC
 * All rights reserved.
 */

namespace NOpenCL
{
    using System;

    [Flags]
    public enum MapFlags : ulong
    {
        None = 0,
        Read = 1 << 0,
        Write = 1 << 1,
        WriteInvalidateRegion = 1 << 2,
    }
}
