// Copyright (c) Tunnel Vision Laboratories, LLC. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace NOpenCL.SafeHandles
{
    using Microsoft.Win32.SafeHandles;
    using ErrorCode = NOpenCL.UnsafeNativeMethods.ErrorCode;

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
