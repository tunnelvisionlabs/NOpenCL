// Copyright (c) Tunnel Vision Laboratories, LLC. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace NOpenCL
{
    using System;

    /// <summary>
    /// Specifies additional properties for the <see cref="CommandQueue"/>.
    /// </summary>
    [Flags]
    public enum CommandQueueProperties : ulong
    {
        /// <summary>
        /// No additional properties are specified.
        /// </summary>
        None = 0,

        /// <summary>
        /// If set, the commands in the command-queue are executed out-of-order. Otherwise,
        /// commands are executed in-order.
        /// </summary>
        OutOfOrderExecutionModeEnable = 1 << 0,

        /// <summary>
        /// If set, the profiling of commands is enabled. Otherwise profiling of commands is disabled.
        /// </summary>
        ProfilingEnable = 1 << 1,
    }
}
