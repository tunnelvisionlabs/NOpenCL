/*
 * Copyright (c) 2013 Sam Harwell, Tunnel Vision Laboratories LLC
 * All rights reserved.
 */

namespace NOpenCL
{
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    public struct BufferRegion
    {
        public readonly IntPtr Origin;
        public readonly IntPtr Size;

        public BufferRegion(IntPtr origin, IntPtr size)
        {
            Origin = origin;
            Size = size;
        }
    }
}
