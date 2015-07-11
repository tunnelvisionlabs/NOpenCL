// Copyright (c) Tunnel Vision Laboratories, LLC. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace NOpenCL
{
    using System;
    using System.Runtime.CompilerServices;
    using NOpenCL.SafeHandles;

    public abstract class MemObject : IDisposable
    {
        private static readonly ConditionalWeakTable<MemObject, UnsafeNativeMethods.MemObjectDestructorCallback> _callbacks =
            new ConditionalWeakTable<MemObject, UnsafeNativeMethods.MemObjectDestructorCallback>();

        private EventHandler _destroyed;

        internal MemObject()
        {
        }

        public event EventHandler Destroyed
        {
            add
            {
                if (_destroyed == null && value != null)
                {
                    bool created = false;
                    UnsafeNativeMethods.MemObjectDestructorCallback callback = _callbacks.GetValue(this, key =>
                    {
                        created = true;
                        return (memObject, data) => key.OnDestroyed();
                    });

                    if (created)
                        UnsafeNativeMethods.SetMemObjectDestructorCallback(BaseHandle, callback, IntPtr.Zero);
                }

                _destroyed += value;
            }

            remove
            {
                _destroyed -= value;
            }
        }

        internal abstract MemObjectSafeHandle BaseHandle
        {
            get;
        }

        private static UnsafeNativeMethods.MemObjectDestructorCallback CreateCallback(MemObject key)
        {
            return (memObject, data) => key.OnDestroyed();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
        }

        protected virtual void OnDestroyed()
        {
            EventHandler destroyed = _destroyed;
            if (destroyed != null)
                destroyed(this, EventArgs.Empty);
        }
    }
}
