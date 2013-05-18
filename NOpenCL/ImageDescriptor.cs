/*
 * Copyright (c) 2013 Sam Harwell, Tunnel Vision Laboratories LLC
 * All rights reserved.
 */

namespace NOpenCL
{
    using System;
    using System.Runtime.InteropServices;
    using NOpenCL.SafeHandles;

    [StructLayout(LayoutKind.Sequential)]
    public struct ImageDescriptor
    {
        public readonly UIntPtr Width;
        public readonly UIntPtr Height;
        public readonly UIntPtr Depth;
        public readonly UIntPtr ArraySize;
        public readonly UIntPtr RowPitch;
        public readonly UIntPtr SlicePitch;
        public readonly uint NumMipLevels;
        public readonly uint NumSamples;
        public readonly BufferSafeHandle Buffer;
    }
}
