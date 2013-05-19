/*
 * Copyright (c) 2013 Sam Harwell, Tunnel Vision Laboratories LLC
 * All rights reserved.
 */

namespace NOpenCL
{
    using System;

    /// <summary>
    /// Describes the execution capabilities of the device.
    /// The mandated minimum capability is <see cref="Kernel"/>.
    /// </summary>
    /// <seealso cref="Device.ExecutionCapabilities"/>
    [Flags]
    public enum ExecutionCapabilities : ulong
    {
        /// <summary>
        /// No capabilities are specified.
        /// </summary>
        None = 0,

        /// <summary>
        /// The OpenCL device can execute OpenCL kernels.
        /// </summary>
        Kernel = 1 << 0,

        /// <summary>
        /// The OpenCL device can execute native kernels.
        /// </summary>
        NativeKernel = 1 << 1,
    }
}
