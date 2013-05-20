/*
 * Copyright (c) 2013 Sam Harwell, Tunnel Vision Laboratories LLC
 * All rights reserved.
 */

namespace NOpenCL
{
    /// <summary>
    /// Represents the execution status of a command.
    /// </summary>
    public enum ExecutionStatus
    {
        /// <summary>
        /// The command has completed.
        /// </summary>
        Complete = 0,

        /// <summary>
        /// The device is currently executing this command.
        /// </summary>
        Running = 1,

        /// <summary>
        /// The enqueued command has been submitted by the host to the device
        /// associated with the command-queue.
        /// </summary>
        Submitted = 2,

        /// <summary>
        /// The command has been enqueued in the command-queue.
        /// </summary>
        Queued = 3,
    }
}
