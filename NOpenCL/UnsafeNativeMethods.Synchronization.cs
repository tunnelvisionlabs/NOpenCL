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
            ErrorHandler.ThrowOnFailure(clEnqueueMarkerWithWaitList(commandQueue, eventWaitList != null ? (uint)eventWaitList.Length : 0, eventWaitList != null && eventWaitList.Length > 0 ? eventWaitList : null, out result));
            return result;
        }

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
            ErrorHandler.ThrowOnFailure(clEnqueueBarrierWithWaitList(commandQueue, eventWaitList != null ? (uint)eventWaitList.Length : 0, eventWaitList != null && eventWaitList.Length > 0 ? eventWaitList : null, out result));
            return result;
        }

        #endregion

        #region Flush and Finish

        [DllImport(ExternDll.OpenCL)]
        private static extern ErrorCode clFlush(CommandQueueSafeHandle commandQueue);

        internal static void Flush(CommandQueueSafeHandle commandQueue)
        {
            ErrorHandler.ThrowOnFailure(clFlush(commandQueue));
        }

        [DllImport(ExternDll.OpenCL)]
        private static extern ErrorCode clFinish(CommandQueueSafeHandle commandQueue);

        internal static void Finish(CommandQueueSafeHandle commandQueue)
        {
            ErrorHandler.ThrowOnFailure(clFinish(commandQueue));
        }

        #endregion
    }
}
