namespace NOpenCL
{
    using System;
    using System.Runtime.InteropServices;

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
