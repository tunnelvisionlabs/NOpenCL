/*
 * Copyright (c) 2013 Sam Harwell, Tunnel Vision Laboratories LLC
 * All rights reserved.
 */

namespace NOpenCL
{
    using System;

    [Flags]
    public enum ExecutionCapabilities : ulong
    {
        None = 0,
        Kernel = 1 << 0,
        NativeKernel = 1 << 1,
    }
}
