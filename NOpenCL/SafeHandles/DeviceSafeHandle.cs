/*
 * Copyright (c) 2013 Sam Harwell, Tunnel Vision Laboratories LLC
 * All rights reserved.
 */

namespace NOpenCL.SafeHandles
{
    using Microsoft.Win32.SafeHandles;

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
