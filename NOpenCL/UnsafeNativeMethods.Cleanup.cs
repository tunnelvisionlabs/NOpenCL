// Copyright (c) Tunnel Vision Laboratories, LLC. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

#pragma warning disable IDE1006 // Naming Styles

namespace NOpenCL
{
    using System;
    using System.Runtime.InteropServices;
    using NOpenCL.SafeHandles;

    /// <content>
    /// Flush and finish.
    /// </content>
    internal partial class UnsafeNativeMethods
    {
        /// <summary>
        /// Issues all previously queued OpenCL commands in a command-queue to the device associated with the command-queue.
        /// http://www.khronos.org/registry/cl/sdk/1.2/docs/man/xhtml/clFlush.html
        /// http://www.khronos.org/registry/cl/specs/opencl-1.2.pdf#page=193
        /// </summary>
        /// <remarks>
        /// Issues all previously queued OpenCL commands in <paramref name="commandQueue"/>
        /// to the device associated with <paramref name="commandQueue"/>.
        ///
        /// <para>
        /// <see cref="clFlush"/> only guarantees that all queued commands to
        /// <paramref name="commandQueue"/> will eventually be submitted to the appropriate
        /// device. There is no guarantee that they will be complete after <see cref="clFlush"/>
        /// returns.
        /// </para>
        ///
        /// <para>
        /// Any blocking commands queued in a command-queue and <see cref="clReleaseCommandQueue"/>
        /// perform an implicit flush of the command-queue. These blocking commands are
        /// <see cref="clEnqueueReadBuffer"/>, <see cref="clEnqueueReadBufferRect"/>, or
        /// <see cref="clEnqueueReadImage"/> with <em>blockingRead</em> set to <c>true</c>;
        /// <see cref="clEnqueueWriteBuffer"/>, <see cref="clEnqueueWriteBufferRect"/>, or
        /// <see cref="clEnqueueWriteImage"/> with <em>blockingWrite</em> set to <c>true</c>;
        /// <see cref="clEnqueueMapBuffer"/> or <see cref="clEnqueueMapImage"/> with
        /// <em>blockingMap</em> set to <c>true</c>; or <see cref="clWaitForEvents"/>.
        /// </para>
        ///
        /// <para>
        /// To use event objects that refer to commands enqueued in a command-queue as event
        /// objects to wait on by commands enqueued in a different command-queue, the
        /// application must call a <see cref="clFlush"/> or any blocking commands that
        /// perform an implicit flush of the command-queue where the commands that refer
        /// to these event objects are enqueued.
        /// </para>
        /// </remarks>
        /// <param name="commandQueue">A valid command-queue.</param>
        /// <returns>
        /// Returns <see cref="ErrorCode.Success"/> if the function call was executed
        /// successfully. Otherwise, it returns one of the following:
        ///
        /// <list type="bullet">
        /// <item><see cref="ErrorCode.InvalidCommandQueue"/> if <paramref name="commandQueue"/> is not a valid command-queue.</item>
        /// <item><see cref="ErrorCode.OutOfHostMemory"/> if there is a failure to allocate resources required by the OpenCL implementation on the host.</item>
        /// <item><see cref="ErrorCode.OutOfResources"/> if there is a failure to allocate resources required by the OpenCL implementation on the device.</item>
        /// </list>
        /// </returns>
        /// <seealso cref="clFinish"/>
        [DllImport(ExternDll.OpenCL)]
        private static extern ErrorCode clFlush(CommandQueueSafeHandle commandQueue);

        internal static void Flush(CommandQueueSafeHandle commandQueue)
        {
            if (commandQueue == null)
                throw new ArgumentNullException(nameof(commandQueue));

            ErrorHandler.ThrowOnFailure(clFlush(commandQueue));
        }

        /// <summary>
        /// Blocks until all previously queued OpenCL commands in a command-queue are issued to the associated device and have completed.
        /// http://www.khronos.org/registry/cl/sdk/1.2/docs/man/xhtml/clFinish.html
        /// </summary>
        /// <remarks>
        /// Blocks until all previously queued OpenCL commands in
        /// <paramref name="commandQueue"/> are issued to the associated device and
        /// have completed.
        ///
        /// <para>
        /// <see cref="clFinish"/> does not return until all previously queued commands
        /// in <paramref name="commandQueue"/> have been processed and completed.
        /// <see cref="clFinish"/> is also a synchronization point.
        /// </para>
        /// </remarks>
        /// <param name="commandQueue">A valid command-queue.</param>
        /// <returns>
        /// Returns <see cref="ErrorCode.Success"/> if the function call was executed
        /// successfully. Otherwise, it returns one of the following:
        ///
        /// <list type="bullet">
        /// <item><see cref="ErrorCode.InvalidCommandQueue"/> if <paramref name="commandQueue"/> is not a valid command-queue.</item>
        /// <item><see cref="ErrorCode.OutOfHostMemory"/> if there is a failure to allocate resources required by the OpenCL implementation on the host.</item>
        /// <item><see cref="ErrorCode.OutOfResources"/> if there is a failure to allocate resources required by the OpenCL implementation on the device.</item>
        /// </list>
        /// </returns>
        /// <seealso cref="clFlush"/>
        [DllImport(ExternDll.OpenCL)]
        private static extern ErrorCode clFinish(CommandQueueSafeHandle commandQueue);

        internal static void Finish(CommandQueueSafeHandle commandQueue)
        {
            if (commandQueue == null)
                throw new ArgumentNullException(nameof(commandQueue));

            ErrorHandler.ThrowOnFailure(clFinish(commandQueue));
        }
    }
}
