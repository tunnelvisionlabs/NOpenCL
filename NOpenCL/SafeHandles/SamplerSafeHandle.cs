/*
 * Copyright (c) 2013 Sam Harwell, Tunnel Vision Laboratories LLC
 * All rights reserved.
 */

namespace NOpenCL.SafeHandles
{
    using Microsoft.Win32.SafeHandles;

    public sealed class SamplerSafeHandle : SafeHandleZeroOrMinusOneIsInvalid
    {
        public SamplerSafeHandle()
            : base(true)
        {
        }

        protected override bool ReleaseHandle()
        {
            ErrorCode result = UnsafeNativeMethods.clReleaseSampler(handle);
            return result == ErrorCode.Success;
        }
    }
}
