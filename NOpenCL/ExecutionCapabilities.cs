namespace NOpenCL
{
    using System;

    [Flags]
    public enum ExecutionCapabilities
    {
        None = 0,
        Kernel = 1 << 0,
        NativeKernel = 1 << 1,
    }
}
