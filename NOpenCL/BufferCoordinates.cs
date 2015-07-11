// Copyright (c) Tunnel Vision Laboratories, LLC. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace NOpenCL
{
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    public struct BufferCoordinates
    {
        public readonly IntPtr X;
        public readonly IntPtr Y;
        public readonly IntPtr Z;

        public BufferCoordinates(IntPtr x, IntPtr y)
        {
            X = x;
            Y = y;
            Z = IntPtr.Zero;
        }

        public BufferCoordinates(IntPtr x, IntPtr y, IntPtr z)
        {
            X = x;
            Y = y;
            Z = y;
        }
    }
}
