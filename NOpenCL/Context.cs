namespace NOpenCL
{
    using System;
    using System.Collections.Generic;

    public sealed class Context : IDisposable
    {
        private readonly ContextSafeHandle _context;
        private bool _disposed;

        private Context(ContextSafeHandle context)
        {
            if (context == null)
                throw new ArgumentNullException("context");

            _context = context;
        }

        public uint ReferenceCount
        {
            get
            {
                return UnsafeNativeMethods.GetContextInfo(Handle, UnsafeNativeMethods.ContextInfo.ReferenceCount);
            }
        }

        public uint NumDevices
        {
            get
            {
                return UnsafeNativeMethods.GetContextInfo(Handle, UnsafeNativeMethods.ContextInfo.NumDevices);
            }
        }

        public IReadOnlyList<Device> Devices
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public IReadOnlyList<IntPtr> Properties
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        internal ContextSafeHandle Handle
        {
            get
            {
                ThrowIfDisposed();
                return _context;
            }
        }

        public static Context Create(params Device[] devices)
        {
            UnsafeNativeMethods.ClDeviceID[] deviceIDs = Array.ConvertAll(devices, device => device.ID);
            ContextSafeHandle handle = UnsafeNativeMethods.CreateContext(deviceIDs, null, IntPtr.Zero);
            return new Context(handle);
        }

        public void Dispose()
        {
            _context.Dispose();
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
