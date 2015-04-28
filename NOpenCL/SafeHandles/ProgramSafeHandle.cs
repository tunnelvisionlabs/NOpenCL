/*
 * Copyright (c) 2013 Sam Harwell, Tunnel Vision Laboratories LLC
 * All rights reserved.
 */

namespace NOpenCL.SafeHandles
{
    using Microsoft.Win32.SafeHandles;

    public sealed class ProgramSafeHandle : SafeHandleZeroOrMinusOneIsInvalid
    {
        public ProgramSafeHandle()
            : base(true)
        {
        }

        protected override bool ReleaseHandle()
        {
            ErrorCode result = UnsafeNativeMethods.clReleaseProgram(handle);
            return result == ErrorCode.Success;
        }
    }
}
