/*
 * Copyright (c) 2013 Sam Harwell, Tunnel Vision Laboratories LLC
 * All rights reserved.
 */

namespace NOpenCL.SafeHandles
{
    using Microsoft.Win32.SafeHandles;

    public sealed class CommandQueueSafeHandle : SafeHandleZeroOrMinusOneIsInvalid
    {
        public CommandQueueSafeHandle()
            : base(true)
        {
        }

        protected override bool ReleaseHandle()
        {
            ErrorCode result = UnsafeNativeMethods.clReleaseCommandQueue(handle);
            return result == ErrorCode.Success;
        }
    }
}
