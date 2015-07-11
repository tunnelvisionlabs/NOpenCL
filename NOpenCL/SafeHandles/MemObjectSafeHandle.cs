// Copyright (c) Tunnel Vision Laboratories, LLC. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace NOpenCL.SafeHandles
{
    using Microsoft.Win32.SafeHandles;
    using ErrorCode = NOpenCL.UnsafeNativeMethods.ErrorCode;

    public abstract class MemObjectSafeHandle : SafeHandleZeroOrMinusOneIsInvalid
    {
        protected MemObjectSafeHandle()
            : base(true)
        {
        }

        protected override bool ReleaseHandle()
        {
            ErrorCode result = UnsafeNativeMethods.clReleaseMemObject(handle);
            return result == ErrorCode.Success;
        }
    }
}
