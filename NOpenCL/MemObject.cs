/*
 * Copyright (c) 2013 Sam Harwell, Tunnel Vision Laboratories LLC
 * All rights reserved.
 */

namespace NOpenCL
{
    using System;
    using NOpenCL.SafeHandles;

    public abstract class MemObject : IDisposable
    {
        internal MemObject()
        {
        }

        internal abstract MemObjectSafeHandle BaseHandle
        {
            get;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
        }
    }
}
