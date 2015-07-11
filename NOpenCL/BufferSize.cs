// Copyright (c) Tunnel Vision Laboratories, LLC. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace NOpenCL
{
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    public struct BufferSize
    {
        public readonly IntPtr Width;
        public readonly IntPtr Height;
        public readonly IntPtr Depth;

        public BufferSize(IntPtr width, IntPtr height)
        {
            Width = width;
            Height = height;
            Depth = (IntPtr)1;
        }

        public BufferSize(IntPtr width, IntPtr height, IntPtr depth)
        {
            Width = width;
            Height = height;
            Depth = depth;
        }
    }
}
