// Copyright (c) Tunnel Vision Laboratories, LLC. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace NOpenCL
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using NOpenCL.SafeHandles;

    public sealed class Context : IDisposable
    {
        private readonly ContextSafeHandle _context;
        private bool _disposed;

        private Context(ContextSafeHandle context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            _context = context;
        }

        [EditorBrowsable(EditorBrowsableState.Advanced)]
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
            if (devices == null)
                throw new ArgumentNullException(nameof(devices));
            if (devices.Length == 0)
                throw new ArgumentException("No devices specified.", nameof(devices));

            UnsafeNativeMethods.ClDeviceID[] deviceIDs = Array.ConvertAll(devices, device => device.ID);
            ContextSafeHandle handle = UnsafeNativeMethods.CreateContext(deviceIDs, null, IntPtr.Zero);
            return new Context(handle);
        }

        public static Context Create(Platform platform, params Device[] devices)
        {
            if (devices == null)
                throw new ArgumentNullException(nameof(devices));
            if (devices.Length == 0)
                throw new ArgumentException("No devices specified.", nameof(devices));

            UnsafeNativeMethods.ClDeviceID[] deviceIDs = Array.ConvertAll(devices, device => device.ID);
            ContextSafeHandle handle = UnsafeNativeMethods.CreateContext(platform.ID, deviceIDs, null, IntPtr.Zero);
            return new Context(handle);
        }

        public static Context Create(Platform platform, DeviceType deviceType)
        {
            if (platform == null)
                throw new ArgumentNullException(nameof(platform));

            ContextSafeHandle handle = UnsafeNativeMethods.CreateContextFromType(platform.ID, deviceType, null, IntPtr.Zero);
            return new Context(handle);
        }

        public CommandQueue CreateCommandQueue(Device device)
        {
            return CreateCommandQueue(device, CommandQueueProperties.None);
        }

        public CommandQueue CreateCommandQueue(Device device, CommandQueueProperties properties)
        {
            if (device == null)
                throw new ArgumentNullException(nameof(device));

            CommandQueueSafeHandle handle = UnsafeNativeMethods.CreateCommandQueue(Handle, device.ID, properties);
            return new CommandQueue(handle, this, device);
        }

        public Buffer CreateBuffer(MemoryFlags flags, long size)
        {
            return CreateBuffer(flags, size, IntPtr.Zero);
        }

        public Buffer CreateBuffer(MemoryFlags flags, long size, IntPtr hostAddress)
        {
            if (size < 0)
                throw new ArgumentOutOfRangeException(nameof(size));
            else if (size == 0)
                throw new ArgumentException("Invalid buffer size.", nameof(size));

            if (hostAddress == IntPtr.Zero && (flags & (MemoryFlags.UseHostPointer | MemoryFlags.CopyHostPointer)) != 0)
                throw new ArgumentException("Invalid host address.", nameof(hostAddress));
            if (hostAddress != IntPtr.Zero && (flags & (MemoryFlags.UseHostPointer | MemoryFlags.CopyHostPointer)) == 0)
                throw new ArgumentException("Invalid host address.", nameof(hostAddress));

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
                throw new ArgumentException("Invalid host address.", nameof(hostAddress));
            if (hostAddress != IntPtr.Zero && (flags & (MemoryFlags.UseHostPointer | MemoryFlags.CopyHostPointer)) == 0)
                throw new ArgumentException("Invalid host address.", nameof(hostAddress));

            ImageSafeHandle handle = UnsafeNativeMethods.CreateImage(Handle, flags, ref format, ref descriptor, hostAddress);
            return new Image(this, handle);
        }

        /// <summary>
        /// Creates a sampler object. Samplers contros how elements of an <see cref="Image"/> object are read by
        /// <a href="http://www.khronos.org/registry/cl/sdk/1.2/docs/man/xhtml/imageFunctions.html"><c>read_image{f|i|ui}</c></a>
        /// </summary>
        /// <param name="normalizedCoordinates"><c>true</c> to use normalized coordinates, otherwise <c>false</c>.
        /// For more information, see <see cref="Sampler.NormalizedCoordinates"/>.</param>
        /// <param name="addressingMode">Specifies the image addressing-mode, i.e.
        /// how out-of-range image coordinates are handled.</param>
        /// <param name="filterMode">Specifies the filtering mode to use.</param>
        /// <returns>A new sampler.</returns>
        public Sampler CreateSampler(bool normalizedCoordinates, AddressingMode addressingMode, FilterMode filterMode)
        {
            SamplerSafeHandle handle = UnsafeNativeMethods.CreateSampler(Handle, normalizedCoordinates, addressingMode, filterMode);
            return new Sampler(handle, this);
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
