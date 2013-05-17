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
