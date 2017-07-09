// Copyright (c) Tunnel Vision Laboratories, LLC. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace NOpenCL
{
    using System;
    using NOpenCL.SafeHandles;

    public sealed class Buffer<T> : MemObject<BufferSafeHandle>
        where T : struct
    {
        private readonly Buffer<T> _parent;

        internal Buffer(Context context, BufferSafeHandle handle)
            : base(context, handle)
        {
        }

        internal Buffer(Context context, Buffer<T> parent, BufferSafeHandle handle)
            : base(context, handle)
        {
            if (parent == null)
                throw new ArgumentNullException(nameof(parent));

            _parent = parent;
        }

        public Buffer<T> AssociatedMemObject
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

        public Buffer<T> CreateSubBuffer(MemoryFlags flags, BufferRegion regionInfo)
        {
            BufferSafeHandle subBuffer = UnsafeNativeMethods.CreateSubBuffer(Handle, flags, regionInfo);
            return new Buffer<T>(Context, this, subBuffer);
        }
    }
}
