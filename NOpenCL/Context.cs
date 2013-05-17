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

        public Buffer CreateBuffer(MemoryFlags flags, long size)
        {
            return CreateBuffer(flags, size, IntPtr.Zero);
        }

        public Buffer CreateBuffer(MemoryFlags flags, long size, IntPtr hostAddress)
        {
            if (size < 0)
                throw new ArgumentOutOfRangeException("size");
            else if (size == 0)
                throw new ArgumentException("Invalid buffer size.");

            if (hostAddress == IntPtr.Zero && (flags & (MemoryFlags.UseHostPointer | MemoryFlags.CopyHostPointer)) != 0)
                throw new ArgumentException("Invalid host address.");
            if (hostAddress != IntPtr.Zero && (flags & (MemoryFlags.UseHostPointer | MemoryFlags.CopyHostPointer)) == 0)
                throw new ArgumentException("Invalid host address.");

            BufferSafeHandle handle = UnsafeNativeMethods.CreateBuffer(Handle, flags, (IntPtr)size, hostAddress);
            return new Buffer(this, handle);
        }

        public Image CreateImage(MemoryFlags flags, ImageFormat format, ImageDescriptor descriptor)
        {
            return CreateImage(flags, format, descriptor, IntPtr.Zero);
        }

        public Image CreateImage(MemoryFlags flags, ImageFormat format, ImageDescriptor descriptor, IntPtr hostAddress)
        {
            if (hostAddress == IntPtr.Zero && (flags & (MemoryFlags.UseHostPointer | MemoryFlags.CopyHostPointer)) != 0)
                throw new ArgumentException("Invalid host address.");
            if (hostAddress != IntPtr.Zero && (flags & (MemoryFlags.UseHostPointer | MemoryFlags.CopyHostPointer)) == 0)
                throw new ArgumentException("Invalid host address.");

            ImageSafeHandle handle = UnsafeNativeMethods.CreateImage(Handle, flags, ref format, ref descriptor, hostAddress);
            return new Image(this, handle);
        }

        public Sampler CreateSampler(bool normalizedCoordinates, AddressingMode addressingMode, FilterMode filterMode)
        {
            SamplerSafeHandle handle = UnsafeNativeMethods.CreateSampler(Handle, normalizedCoordinates, addressingMode, filterMode);
            return new Sampler(handle);
        }

        public Program CreateProgramWithSource(params string[] sources)
        {
            ProgramSafeHandle handle = UnsafeNativeMethods.CreateProgramWithSource(Handle, sources);
            return new Program(this, handle);
        }

        public Event CreateUserEvent()
        {
            EventSafeHandle handle = UnsafeNativeMethods.CreateUserEvent(Handle);
            return new Event(handle);
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
