namespace NOpenCL
{
    using System;
    using NOpenCL.SafeHandles;

    public abstract class MemObject<THandle> : MemObject
        where THandle : MemObjectSafeHandle
    {
        private readonly Context _context;
        private readonly THandle _handle;
        private bool _disposed;

        internal MemObject(Context context, THandle handle)
        {
            if (context == null)
                throw new ArgumentNullException("context");
            if (handle == null)
                throw new ArgumentNullException("handle");

            _context = context;
            _handle = handle;
        }

        public MemObjectType ObjectType
        {
            get
            {
                return (MemObjectType)UnsafeNativeMethods.GetMemObjectInfo(Handle, UnsafeNativeMethods.MemObjectInfo.Type);
            }
        }

        public MemoryFlags Flags
        {
            get
            {
                return (MemoryFlags)UnsafeNativeMethods.GetMemObjectInfo(Handle, UnsafeNativeMethods.MemObjectInfo.Flags);
            }
        }

        public UIntPtr Size
        {
            get
            {
                return UnsafeNativeMethods.GetMemObjectInfo(Handle, UnsafeNativeMethods.MemObjectInfo.Size);
            }
        }

        public IntPtr HostPointer
        {
            get
            {
                return UnsafeNativeMethods.GetMemObjectInfo(Handle, UnsafeNativeMethods.MemObjectInfo.HostAddress);
            }
        }

        public uint MapCount
        {
            get
            {
                return UnsafeNativeMethods.GetMemObjectInfo(Handle, UnsafeNativeMethods.MemObjectInfo.MapCount);
            }
        }

        public uint ReferenceCount
        {
            get
            {
                return UnsafeNativeMethods.GetMemObjectInfo(Handle, UnsafeNativeMethods.MemObjectInfo.ReferenceCount);
            }
        }

        public Context Context
        {
            get
            {
                return _context;
            }
        }

        internal THandle Handle
        {
            get
            {
                ThrowIfDisposed();
                return _handle;
            }
        }

        internal override MemObjectSafeHandle BaseHandle
        {
            get
            {
                return Handle;
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _handle.Dispose();
            }

            _disposed = true;
        }

        private void ThrowIfDisposed()
        {
            if (_disposed)
                throw new ObjectDisposedException(GetType().FullName);
        }
    }
}
