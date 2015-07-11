// Copyright (c) Tunnel Vision Laboratories, LLC. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

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
