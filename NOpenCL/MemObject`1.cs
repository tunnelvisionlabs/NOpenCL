// Copyright (c) Tunnel Vision Laboratories, LLC. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace NOpenCL
{
    using System;
    using System.ComponentModel;
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
                throw new ArgumentNullException(nameof(context));
            if (handle == null)
                throw new ArgumentNullException(nameof(handle));

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

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public uint MapCount
        {
            get
            {
                return UnsafeNativeMethods.GetMemObjectInfo(Handle, UnsafeNativeMethods.MemObjectInfo.MapCount);
            }
        }

        [EditorBrowsable(EditorBrowsableState.Advanced)]
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
                ThrowIfDisposed();
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

        protected void ThrowIfDisposed()
        {
            if (_disposed)
                throw new ObjectDisposedException(GetType().FullName);
        }
    }
}
