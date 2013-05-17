namespace NOpenCL
{
    using System;

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

        public Event EnqueueMarker(params Event[] eventWaitList)
        {
            EventSafeHandle[] eventHandles = null;
            if (eventWaitList != null)
                eventHandles = Array.ConvertAll(eventWaitList, @event => @event.Handle);

            EventSafeHandle handle = UnsafeNativeMethods.EnqueueMarkerWithWaitList(Handle, eventHandles);
            return new Event(handle);
        }

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
