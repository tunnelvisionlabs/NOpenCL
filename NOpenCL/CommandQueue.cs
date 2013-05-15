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
