/*
 * Copyright (c) 2013 Sam Harwell, Tunnel Vision Laboratories LLC
 * All rights reserved.
 */

namespace NOpenCL
{
    using System;
    using System.Runtime.InteropServices;
    using NOpenCL.SafeHandles;

    partial class UnsafeNativeMethods
    {
        #region Markers, Barriers, and Waiting

        /// <summary>
        /// Enqueues a marker command which waits for either a list of events to complete, or all previously enqueued commands to complete.
        /// http://www.khronos.org/registry/cl/sdk/1.2/docs/man/xhtml/clEnqueueMarkerWithWaitList.html
        /// http://www.khronos.org/registry/cl/specs/opencl-1.2.pdf#page=186
        /// </summary>
        /// <remarks>
        /// Enqueues a marker command which waits for either a list of events to complete,
        /// or if the list is empty it waits for all commands previously enqueued in
        /// <paramref name="commandQueue"/> to complete before it completes. This command
        /// returns an event which can be waited on, i.e. this event can be waited on to
        /// ensure that all events either in the <paramref name="eventWaitList"/> or all
        /// previously enqueued commands, queued before this command to
        /// <paramref name="commandQueue"/>, have completed.
        /// </remarks>
        /// <param name="commandQueue">A valid command-queue.</param>
        /// <param name="numEventsInWaitList"></param>
        /// <param name="eventWaitList"></param>
        /// <param name="event">Returns an event object that identifies this particular command.</param>
        /// <returns>
        /// Returns <see cref="ErrorCode.Success"/> if the function executed successfully,
        /// or one of the errors below:
        ///
        /// <list type="bullet">
        /// <item><see cref="ErrorCode.InvalidCommandQueue"/> if <paramref name="commandQueue"/> is not a valid command-queue.</item>
        /// <item><see cref="ErrorCode.InvalidEventWaitList"/> if <paramref name="eventWaitList"/> is <c>null</c> and <paramref name="numEventsInWaitList"/> &gt; 0, or <paramref name="eventWaitList"/> is not <c>null</c> and <paramref name="numEventsInWaitList"/> is 0, or if event objects in <paramref name="eventWaitList"/> are not valid events.</item>
        /// <item><see cref="ErrorCode.OutOfResources"/> if there is a failure to allocate resources required by the OpenCL implementation on the device.</item>
        /// <item><see cref="ErrorCode.OutOfHostMemory"/> if there is a failure to allocate resources required by the OpenCL implementation on the host.</item>
        /// </list>
        /// </returns>
        [DllImport(ExternDll.OpenCL)]
        private static extern ErrorCode clEnqueueMarkerWithWaitList(
            CommandQueueSafeHandle commandQueue,
            uint numEventsInWaitList,
            [In, MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(SafeHandleArrayMarshaler))] EventSafeHandle[] eventWaitList,
            out EventSafeHandle @event);

        internal static EventSafeHandle EnqueueMarkerWithWaitList(CommandQueueSafeHandle commandQueue, EventSafeHandle[] eventWaitList)
        {
            if (commandQueue == null)
                throw new ArgumentNullException("commandQueue");

            EventSafeHandle result;
            ErrorHandler.ThrowOnFailure(clEnqueueMarkerWithWaitList(commandQueue, GetNumEventsInWaitList(eventWaitList), GetEventWaitList(eventWaitList), out result));
            return result;
        }

        /// <summary>
        /// A synchronization point that enqueues a barrier operation.
        /// http://www.khronos.org/registry/cl/sdk/1.2/docs/man/xhtml/clEnqueueBarrierWithWaitList.html
        /// http://www.khronos.org/registry/cl/specs/opencl-1.2.pdf#page=187
        /// </summary>
        /// <remarks>
        /// Enqueues a barrier command which waits for either a list of events to complete,
        /// or if the list is empty it waits for all commands previously enqueued in
        /// <paramref name="commandQueue"/> to complete before it completes. This command
        /// blocks command execution, that is, any following commands enqueued after it do
        /// not execute until it completes. This command returns an event which can be
        /// waited on, i.e. this event can be waited on to ensure that all events either
        /// in the <paramref name="eventWaitList"/> or all previously enqueued commands,
        /// queued before this command to <paramref name="commandQueue"/>, have completed.
        /// </remarks>
        /// <param name="commandQueue">A valid command-queue.</param>
        /// <param name="numEventsInWaitList"></param>
        /// <param name="eventWaitList"></param>
        /// <param name="event">Returns an event object that identifies this particular command.</param>
        /// <returns>
        /// Returns <see cref="ErrorCode.Success"/> if the function executed successfully,
        /// or one of the errors below:
        ///
        /// <list type="bullet">
        /// <item><see cref="ErrorCode.InvalidCommandQueue"/> if <paramref name="commandQueue"/> is not a valid command-queue.</item>
        /// <item><see cref="ErrorCode.InvalidEventWaitList"/> if <paramref name="eventWaitList"/> is <c>null</c> and <paramref name="numEventsInWaitList"/> &gt; 0, or <paramref name="eventWaitList"/> is not <c>null</c> and <paramref name="numEventsInWaitList"/> is 0, or if event objects in <paramref name="eventWaitList"/> are not valid events.</item>
        /// <item><see cref="ErrorCode.OutOfResources"/> if there is a failure to allocate resources required by the OpenCL implementation on the device.</item>
        /// <item><see cref="ErrorCode.OutOfHostMemory"/> if there is a failure to allocate resources required by the OpenCL implementation on the host.</item>
        /// </list>
        /// </returns>
        [DllImport(ExternDll.OpenCL)]
        private static extern ErrorCode clEnqueueBarrierWithWaitList(
            CommandQueueSafeHandle commandQueue,
            uint numEventsInWaitList,
            [In, MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(SafeHandleArrayMarshaler))] EventSafeHandle[] eventWaitList,
            out EventSafeHandle @event);

        internal static EventSafeHandle EnqueueBarrierWithWaitList(CommandQueueSafeHandle commandQueue, EventSafeHandle[] eventWaitList)
        {
            if (commandQueue == null)
                throw new ArgumentNullException("commandQueue");

            EventSafeHandle result;
            ErrorHandler.ThrowOnFailure(clEnqueueBarrierWithWaitList(commandQueue, GetNumEventsInWaitList(eventWaitList), GetEventWaitList(eventWaitList), out result));
            return result;
        }

        #endregion

        #region Flush and Finish

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
                throw new ArgumentNullException("commandQueue");

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
                throw new ArgumentNullException("commandQueue");

            ErrorHandler.ThrowOnFailure(clFinish(commandQueue));
        }

        #endregion
    }
}
