/*
 * Copyright (c) 2013 Sam Harwell, Tunnel Vision Laboratories LLC
 * All rights reserved.
 */

namespace NOpenCL
{
    using System;
    using NOpenCL.SafeHandles;

    public sealed class CommandQueue : IDisposable
    {
        private readonly CommandQueueSafeHandle _handle;
        private bool _disposed;

        private CommandQueue(CommandQueueSafeHandle handle)
        {
            if (handle == null)
                throw new ArgumentNullException("handle");

            _handle = handle;
        }

        public Context Context
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public Device Device
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public uint ReferenceCount
        {
            get
            {
                return UnsafeNativeMethods.GetCommandQueueInfo(_handle, UnsafeNativeMethods.CommandQueueInfo.ReferenceCount);
            }
        }

        public CommandQueueProperties Properties
        {
            get
            {
                return (CommandQueueProperties)UnsafeNativeMethods.GetCommandQueueInfo(_handle, UnsafeNativeMethods.CommandQueueInfo.Properties);
            }
        }

        internal CommandQueueSafeHandle Handle
        {
            get
            {
                ThrowIfDisposed();
                return _handle;
            }
        }

        public static CommandQueue Create(Context context, Device device, CommandQueueProperties properties)
        {
            if (context == null)
                throw new ArgumentNullException("context");
            if (device == null)
                throw new ArgumentNullException("device");

            CommandQueueSafeHandle handle = UnsafeNativeMethods.CreateCommandQueue(context.Handle, device.ID, properties);
            return new CommandQueue(handle);
        }

        public Event EnqueueReadBuffer(Buffer buffer, bool blocking, long offset, long size, IntPtr destination, params Event[] eventWaitList)
        {
            EventSafeHandle[] eventHandles = null;
            if (eventWaitList != null)
                eventHandles = Array.ConvertAll(eventWaitList, @event => @event.Handle);

            EventSafeHandle handle = UnsafeNativeMethods.EnqueueReadBuffer(this.Handle, buffer.Handle, blocking, (IntPtr)offset, (IntPtr)size, destination, eventHandles);
            return new Event(handle);
        }

        public Event EnqueueReadBufferRect(Buffer buffer, bool blocking, BufferCoordinates bufferOrigin, BufferCoordinates hostOrigin, BufferSize region, long bufferRowPitch, long bufferSlicePitch, long hostRowPitch, long hostSlicePitch, IntPtr destination, params Event[] eventWaitList)
        {
            EventSafeHandle[] eventHandles = null;
            if (eventWaitList != null)
                eventHandles = Array.ConvertAll(eventWaitList, @event => @event.Handle);

            EventSafeHandle handle = UnsafeNativeMethods.EnqueueReadBufferRect(this.Handle, buffer.Handle, blocking, ref bufferOrigin, ref hostOrigin, ref region, (IntPtr)bufferRowPitch, (IntPtr)bufferSlicePitch, (IntPtr)hostRowPitch, (IntPtr)hostSlicePitch, destination, eventHandles);
            return new Event(handle);
        }

        public Event EnqueueWriteBuffer(Buffer buffer, bool blocking, long offset, long size, IntPtr source, params Event[] eventWaitList)
        {
            EventSafeHandle[] eventHandles = null;
            if (eventWaitList != null)
                eventHandles = Array.ConvertAll(eventWaitList, @event => @event.Handle);

            EventSafeHandle handle = UnsafeNativeMethods.EnqueueWriteBuffer(this.Handle, buffer.Handle, blocking, (IntPtr)offset, (IntPtr)size, source, eventHandles);
            return new Event(handle);
        }

        public Event EnqueueWriteBufferRect(Buffer buffer, bool blocking, BufferCoordinates bufferOrigin, BufferCoordinates hostOrigin, BufferSize region, long bufferRowPitch, long bufferSlicePitch, long hostRowPitch, long hostSlicePitch, IntPtr source, params Event[] eventWaitList)
        {
            EventSafeHandle[] eventHandles = null;
            if (eventWaitList != null)
                eventHandles = Array.ConvertAll(eventWaitList, @event => @event.Handle);

            EventSafeHandle handle = UnsafeNativeMethods.EnqueueWriteBufferRect(this.Handle, buffer.Handle, blocking, ref bufferOrigin, ref hostOrigin, ref region, (IntPtr)bufferRowPitch, (IntPtr)bufferSlicePitch, (IntPtr)hostRowPitch, (IntPtr)hostSlicePitch, source, eventHandles);
            return new Event(handle);
        }

        public Event EnqueueCopyBuffer(Buffer source, Buffer destination, long sourceOffset, long destinationOffset, long size, params Event[] eventWaitList)
        {
            EventSafeHandle[] eventHandles = null;
            if (eventWaitList != null)
                eventHandles = Array.ConvertAll(eventWaitList, @event => @event.Handle);

            EventSafeHandle handle = UnsafeNativeMethods.EnqueueCopyBuffer(this.Handle, source.Handle, destination.Handle, (IntPtr)sourceOffset, (IntPtr)destinationOffset, (IntPtr)size, eventHandles);
            return new Event(handle);
        }

        public Event EnqueueCopyBufferRect(Buffer source, Buffer destination, BufferCoordinates sourceOrigin, BufferCoordinates destinationOrigin, BufferSize region, long sourceRowPitch, long sourceSlicePitch, long destinationRowPitch, long destinationSlicePitch, params Event[] eventWaitList)
        {
            EventSafeHandle[] eventHandles = null;
            if (eventWaitList != null)
                eventHandles = Array.ConvertAll(eventWaitList, @event => @event.Handle);

            EventSafeHandle handle = UnsafeNativeMethods.EnqueueCopyBufferRect(this.Handle, source.Handle, destination.Handle, ref sourceOrigin, ref destinationOrigin, ref region, (IntPtr)sourceRowPitch, (IntPtr)sourceSlicePitch, (IntPtr)destinationRowPitch, (IntPtr)destinationSlicePitch, eventHandles);
            return new Event(handle);
        }

        public Event EnqueueMapBuffer(Buffer buffer, bool blocking, MapFlags mapFlags, long offset, long size, out IntPtr mappedPointer, params Event[] eventWaitList)
        {
            EventSafeHandle[] eventHandles = null;
            if (eventWaitList != null)
                eventHandles = Array.ConvertAll(eventWaitList, @event => @event.Handle);

            EventSafeHandle handle = UnsafeNativeMethods.EnqueueMapBuffer(this.Handle, buffer.Handle, blocking, mapFlags, (IntPtr)offset, (IntPtr)size, out mappedPointer, eventHandles);
            return new Event(handle);
        }

        public Event EnqueueUnmapMemObject<THandle>(MemObject<THandle> memObject, IntPtr mappedPointer, params Event[] eventWaitList)
            where THandle : MemObjectSafeHandle
        {
            EventSafeHandle[] eventHandles = null;
            if (eventWaitList != null)
                eventHandles = Array.ConvertAll(eventWaitList, @event => @event.Handle);

            EventSafeHandle handle = UnsafeNativeMethods.EnqueueUnmapMemObject(this.Handle, memObject.Handle, mappedPointer, eventHandles);
            return new Event(handle);
        }

        public Event EnqueueMigrateMemObjects(MemObject[] memObjects, MigrationFlags flags, params Event[] eventWaitList)
        {
            MemObjectSafeHandle[] memHandles = null;
            if (memObjects != null)
                memHandles = Array.ConvertAll(memObjects, mem => mem.BaseHandle);

            EventSafeHandle[] eventHandles = null;
            if (eventWaitList != null)
                eventHandles = Array.ConvertAll(eventWaitList, @event => @event.Handle);

            EventSafeHandle handle = UnsafeNativeMethods.EnqueueMigrateMemObjects(this.Handle, memHandles, flags, eventHandles);
            return new Event(handle);
        }

        public Event EnqueueNDRangeKernel(Kernel kernel, IntPtr globalWorkSize, IntPtr localWorkSize, params Event[] eventWaitList)
        {
            return EnqueueNDRangeKernel(kernel, null, new[] { globalWorkSize }, new[] { localWorkSize }, eventWaitList);
        }

        public Event EnqueueNDRangeKernel(Kernel kernel, IntPtr globalWorkOffset, IntPtr globalWorkSize, IntPtr localWorkSize, params Event[] eventWaitList)
        {
            return EnqueueNDRangeKernel(kernel, new[] { globalWorkOffset }, new[] { globalWorkSize }, new[] { localWorkSize }, eventWaitList);
        }

        public Event EnqueueNDRangeKernel(Kernel kernel, IntPtr[] globalWorkSize, IntPtr[] localWorkSize, params Event[] eventWaitList)
        {
            return EnqueueNDRangeKernel(kernel, null, globalWorkSize, localWorkSize, eventWaitList);
        }

        public Event EnqueueNDRangeKernel(Kernel kernel, IntPtr[] globalWorkOffset, IntPtr[] globalWorkSize, IntPtr[] localWorkSize, params Event[] eventWaitList)
        {
            EventSafeHandle[] eventHandles = null;
            if (eventWaitList != null)
                eventHandles = Array.ConvertAll(eventWaitList, @event => @event.Handle);

            EventSafeHandle handle = UnsafeNativeMethods.EnqueueNDRangeKernel(this.Handle, kernel.Handle, globalWorkOffset, globalWorkSize, localWorkSize, eventHandles);
            return new Event(handle);
        }

        /// <summary>
        /// Enqueues a command to execute a <see cref="Kernel"/> on this command queue's <see cref="Device"/>.
        /// </summary>
        /// <remarks>
        /// The kernel is executed using a single work-item.
        ///
        /// <para><see cref="EnqueueTask"/> is equivalent to calling
        /// <see cref="EnqueueNDRangeKernel(Kernel, IntPtr, IntPtr, Event[])"/> with
        /// <em>globalWorkSize</em> set to 1, and <em>localWorkSize</em> set to 1.</para>
        /// </remarks>
        /// <param name="kernel">A valid <see cref="Kernel"/> object.</param>
        /// <param name="eventWaitList">The events that need to be complete before this
        /// command is executed. If the list is null or empty, this command does not
        /// wait on any event to complete.</param>
        /// <returns>Returns an event object that identifies this particular kernel execution instance.</returns>
        public Event EnqueueTask(Kernel kernel, params Event[] eventWaitList)
        {
            EventSafeHandle[] eventHandles = null;
            if (eventWaitList != null)
                eventHandles = Array.ConvertAll(eventWaitList, @event => @event.Handle);

            EventSafeHandle handle = UnsafeNativeMethods.EnqueueTask(this.Handle, kernel.Handle, eventHandles);
            return new Event(handle);
        }

        /// <summary>
        /// Enqueues a marker command which waits for either a list of events to complete,
        /// or all previously enqueued commands to complete.
        /// </summary>
        /// <remarks>
        /// Enqueues a marker command which waits for all events in
        /// <paramref name="eventWaitList"/> to complete, or if
        /// <paramref name="eventWaitList"/> is empty it waits for all previously enqueued
        /// commands to complete before it completes. This command returns an event which
        /// can be waited on, i.e. this event can be waited on to ensure that all events
        /// either in <paramref name="eventWaitList"/> or all previously enqueued commands,
        /// queued before this command, have completed.
        /// </remarks>
        /// <param name="eventWaitList">The events that need to be complete before this
        /// command is executed. If the list is null or empty, this command waits until
        /// all previous enqueued commands have completed.</param>
        /// <returns>Returns an event object that identifies this particular command.</returns>
        public Event EnqueueMarker(params Event[] eventWaitList)
        {
            EventSafeHandle[] eventHandles = null;
            if (eventWaitList != null)
                eventHandles = Array.ConvertAll(eventWaitList, @event => @event.Handle);

            EventSafeHandle handle = UnsafeNativeMethods.EnqueueMarkerWithWaitList(Handle, eventHandles);
            return new Event(handle);
        }

        /// <summary>
        /// A synchronization point that enqueues a barrier operation.
        /// </summary>
        /// <remarks>
        /// Enqueues a barrier command which waits for all events in
        /// <paramref name="eventWaitList"/> to complete, or if
        /// <paramref name="eventWaitList"/> is empty it waits for all previously enqueued
        /// commands to complete before it completes. This command blocks command execution,
        /// that is, any commands enqueued after it do not execute until it completes. This
        /// command returns an event which can be waited on, i.e. this event can be waited
        /// on to ensure that all events either in the <paramref name="eventWaitList"/>
        /// or all previously enqueued commands, queued before this command, have completed.
        /// </remarks>
        /// <param name="eventWaitList">The events that need to be complete before this
        /// command is executed. If the list is null or empty, this command waits until
        /// all previous enqueued commands have completed.</param>
        /// <returns>Returns an event object that identifies this particular command.</returns>
        public Event EnqueueBarrier(params Event[] eventWaitList)
        {
            EventSafeHandle[] eventHandles = null;
            if (eventWaitList != null)
                eventHandles = Array.ConvertAll(eventWaitList, @event => @event.Handle);

            EventSafeHandle handle = UnsafeNativeMethods.EnqueueBarrierWithWaitList(Handle, eventHandles);
            return new Event(handle);
        }

        /// <summary>
        /// Blocks until all previously queued OpenCL commands in the command-queue are
        /// issued to the associated device and have completed.
        /// </summary>
        /// <remarks>
        /// <see cref="Finish"/> does not return until all previously queued commands in
        /// the command queue have been processed and completed. <see cref="Finish"/> is
        /// also a synchronization point.
        /// </remarks>
        public void Finish()
        {
            ThrowIfDisposed();
            UnsafeNativeMethods.Finish(Handle);
        }

        /// <summary>
        /// Issues all previously queued OpenCL commands in the command-queue to the device
        /// associated with the command-queue.
        /// </summary>
        /// <remarks>
        /// <see cref="Flush"/> only guarantees that all queued commands will eventually be
        /// submitted to the appropriate device. There is no guarantee that they will be
        /// complete after <see cref="Flush"/> returns.
        ///
        /// <para>Any blocking commands queued in a command-queue and <see cref="Dispose"/>
        /// perform an implicit flush of the command-queue. These blocking commands are
        /// <see cref="EnqueueReadBuffer"/>, <see cref="EnqueueReadBufferRect"/>, or
        /// <see cref="EnqueueReadImage"/> with <c>blocking</c> set to <c>true</c>;
        /// <see cref="EnqueueWriteBuffer"/>, <see cref="EnqueueWriteBufferRect"/>, or
        /// <see cref="EnqueueWriteImage"/> with <c>blocking_write</c> set to <c>true</c>;
        /// <see cref="EnqueueMapBuffer"/> or <see cref="EnqueueMapImage"/> with
        /// <c>blocking_map</c> set to <c>true</c>; or <see cref="Event.WaitAll"/>.</para>
        ///
        /// <para>To use event objects that refer to commands enqueued in a command-queue
        /// as event objects to wait on by commands enqueued in a different command-queue,
        /// the application must call a <see cref="Flush"/> or any blocking commands that
        /// perform an implicit flush of the command-queue where the commands that refer
        /// to these event objects are enqueued.</para>
        /// </remarks>
        public void Flush()
        {
            ThrowIfDisposed();
            UnsafeNativeMethods.Flush(Handle);
        }

        public void Dispose()
        {
            _handle.Dispose();
            _disposed = true;
            GC.SuppressFinalize(this);
        }

        private void ThrowIfDisposed()
        {
            if (_disposed)
                throw new ObjectDisposedException(GetType().FullName);
        }
    }
}
