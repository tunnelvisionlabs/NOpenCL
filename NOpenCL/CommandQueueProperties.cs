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
