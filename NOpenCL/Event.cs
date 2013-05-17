namespace NOpenCL
{
    using System;

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
                throw new ArgumentNullException("handle");

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
