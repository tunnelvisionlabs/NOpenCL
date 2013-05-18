/*
 * Copyright (c) 2013 Sam Harwell, Tunnel Vision Laboratories LLC
 * All rights reserved.
 */

namespace NOpenCL
{
    using System;

    [Flags]
    public enum CommandQueueProperties : ulong
    {
        None = 0,
        OutOfOrderExecutionModeEnable = 1 << 0,
        ProfilingEnable = 1 << 1,
    }
}
