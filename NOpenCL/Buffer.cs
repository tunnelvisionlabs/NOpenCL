/*
 * Copyright (c) 2013 Sam Harwell, Tunnel Vision Laboratories LLC
 * All rights reserved.
 */

namespace NOpenCL
{
    using System;
    using NOpenCL.SafeHandles;

    public sealed class Mem : MemObject<BufferSafeHandle>
    {
        private readonly Mem _parent;

        internal Mem(Context context, BufferSafeHandle handle)
            : base(context, handle)
        {
        }

        internal Mem(Context context, Mem parent, BufferSafeHandle handle)
            : base(context, handle)
        {
            if (parent == null)
                throw new ArgumentNullException("parent");

            _parent = parent;
        }

        public Mem AssociatedMemObject
        {
            get
            {
                ThrowIfDisposed();
                return _parent;
            }
        }

        public UIntPtr Offset
        {
            get
            {
                return UnsafeNativeMethods.GetMemObjectInfo(Handle, UnsafeNativeMethods.MemObjectInfo.Offset);
            }
        }

        public Mem CreateSubBuffer(MemoryFlags flags, BufferRegion regionInfo)
        {
            BufferSafeHandle subBuffer = UnsafeNativeMethods.CreateSubBuffer(Handle, flags, regionInfo);
            return new Mem(Context, this, subBuffer);
        }
    }
}
