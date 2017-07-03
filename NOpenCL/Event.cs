// Copyright (c) Tunnel Vision Laboratories, LLC. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace NOpenCL
{
    using System;
    using System.ComponentModel;
    using NOpenCL.SafeHandles;

    /// <summary>
    /// To create an instance of <see cref="Event"/>, call <see cref="NOpenCL.Context.CreateUserEvent"/>.
    /// </summary>
    public sealed class Event : IDisposable
    {
        private readonly EventSafeHandle _handle;
        private bool _disposed;

        internal Event(EventSafeHandle handle)
        {
            if (handle == null)
                throw new ArgumentNullException(nameof(handle));

            _handle = handle;
        }

        public CommandQueue CommandQueue
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public Context Context
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public CommandType CommandType
        {
            get
            {
                return (CommandType)UnsafeNativeMethods.GetEventInfo(Handle, UnsafeNativeMethods.EventInfo.CommandType);
            }
        }

        public ExecutionStatus CommandExecutionStatus
        {
            get
            {
                return (ExecutionStatus)UnsafeNativeMethods.GetEventInfo(Handle, UnsafeNativeMethods.EventInfo.CommandExecutionStatus);
            }
        }

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public uint ReferenceCount
        {
            get
            {
                return UnsafeNativeMethods.GetEventInfo(Handle, UnsafeNativeMethods.EventInfo.ReferenceCount);
            }
        }

        public ulong CommandQueuedTime
        {
            get
            {
                return UnsafeNativeMethods.GetEventProfilingInfo(Handle, UnsafeNativeMethods.EventProfilingInfo.CommandQueued);
            }
        }

        public ulong CommandSubmitTime
        {
            get
            {
                return UnsafeNativeMethods.GetEventProfilingInfo(Handle, UnsafeNativeMethods.EventProfilingInfo.CommandSubmit);
            }
        }

        public ulong CommandStartTime
        {
            get
            {
                return UnsafeNativeMethods.GetEventProfilingInfo(Handle, UnsafeNativeMethods.EventProfilingInfo.CommandStart);
            }
        }

        public ulong CommandEndTime
        {
            get
            {
                return UnsafeNativeMethods.GetEventProfilingInfo(Handle, UnsafeNativeMethods.EventProfilingInfo.CommandEnd);
            }
        }

        internal EventSafeHandle Handle
        {
            get
            {
                ThrowIfDisposed();
                return _handle;
            }
        }

        public static void WaitAll(params Event[] events)
        {
            if (events == null)
                throw new ArgumentNullException(nameof(events));
            if (events.Length == 0)
                throw new ArgumentException($"{nameof(events)} cannot be empty", nameof(events));

            EventSafeHandle[] eventHandles = null;
            if (events != null)
                eventHandles = Array.ConvertAll(events, @event => @event.Handle);

            UnsafeNativeMethods.WaitForEvents(eventHandles);
        }

        public void SetUserEventStatus(ExecutionStatus status)
        {
            UnsafeNativeMethods.SetUserEventStatus(Handle, status);
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
