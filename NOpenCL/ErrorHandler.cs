// Copyright (c) Tunnel Vision Laboratories, LLC. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace NOpenCL
{
    using System;
    using ErrorCode = NOpenCL.UnsafeNativeMethods.ErrorCode;

    internal static class ErrorHandler
    {
        internal static void ThrowOnFailure(ErrorCode errorCode)
        {
            if (errorCode >= 0)
                return;

            switch (errorCode)
            {
            case ErrorCode.OutOfHostMemory:
                throw new OutOfMemoryException("Could not allocate OpenCL resources on the host.");

            case ErrorCode.OutOfResources:
                throw new OutOfMemoryException("Could not allocate OpenCL resources on the device.");

            case ErrorCode.DevicePartitionFailed:
                throw new InvalidOperationException("The device could not be further partitioned.");

            case ErrorCode.InvalidDevicePartitionCount:
                throw new InvalidOperationException("Invalid device partition size.");

            case ErrorCode.InvalidCommandQueue:
                throw new ArgumentException("Invalid command queue.");

            case ErrorCode.InvalidEventWaitList:
                throw new ArgumentException("The event wait list contained an invalid event.");

            case ErrorCode.InvalidDevice:
                throw new ArgumentException("Invalid device.");

            case ErrorCode.InvalidValue:
                throw new ArgumentException("Invalid value.");

            default:
                throw new Exception(string.Format("Unknown error code: {0}", errorCode));
            }
        }
    }
}
