// Copyright (c) Tunnel Vision Laboratories, LLC. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace NOpenCL.SafeHandles
{
    using Microsoft.Win32.SafeHandles;
    using ErrorCode = NOpenCL.UnsafeNativeMethods.ErrorCode;

    public sealed class DeviceSafeHandle : SafeHandleZeroOrMinusOneIsInvalid
    {
        public DeviceSafeHandle()
            : base(true)
        {
        }

        internal DeviceSafeHandle(UnsafeNativeMethods.ClDeviceID device)
            : base(true)
        {
            SetHandle(device.Handle);
        }

        protected override bool ReleaseHandle()
        {
            ErrorCode result = UnsafeNativeMethods.clReleaseDevice(handle);
            return result == ErrorCode.Success;
        }
    }
}
