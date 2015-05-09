/*
 * Copyright (c) 2013 Sam Harwell, Tunnel Vision Laboratories LLC
 * All rights reserved.
 */

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
                throw new ArgumentNullException("context");

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

        /// <summary>
        /// Creates a program object for a <see cref="Context"/>, and loads specified binary 
        /// data into the <see cref="Program"/> object.
        /// </summary>
        /// <remarks>
        /// OpenCL allows applications to create a program object using the program source or binary 
        /// and build appropriate program executables. This allows applications to determine whether 
        /// they want to use the pre-built offline binary or load and compile the program source 
        /// and use the executable compiled/linked online as the program executable. This can be 
        /// very useful as it allows applications to load and build program executables online on 
        /// its first instance for appropriate OpenCL devices in the system. These executables can 
        /// now be queried and cached by the application. Future instances of the application 
        /// launching will no longer need to compile and build the program executables. The cached 
        /// executables can be read and loaded by the application, which can help significantly 
        /// reduce the application initialization time.
        /// </remarks>
        /// <param name="devices">A pointer to a list of devices that are in context. device_list 
        /// must be a non-NULL value. The binaries are loaded for devices specified in this list.</param>
        /// <param name="bins">An array of pointers to program binaries to be loaded for devices 
        /// specified by device_list. For each device given by device_list[i], the pointer to the 
        /// program binary for that device is given by binaries[i] and the length of this 
        /// corresponding binary is given by lengths[i]. lengths[i] cannot be zero and binaries[i] 
        /// cannot be a NULL pointer.</param>
        /// <returns>
        /// <see cref="Program"/>
        /// </returns>
        public Program CreateProgramWithBinary(byte[][] bins, params Device[] devices)
        {
            UnsafeNativeMethods.ClDeviceID[] deviceIDs = Array.ConvertAll(devices, device => device.ID);
            ProgramSafeHandle handle = UnsafeNativeMethods.CreateProgramWithBinary(_context, deviceIDs, bins);
            return new Program(this, handle);
        }

        public static Context Create(params Device[] devices)
        {
            if (devices == null)
                throw new ArgumentNullException("devices");
            if (devices.Length == 0)
                throw new ArgumentException("No devices specified.");

            UnsafeNativeMethods.ClDeviceID[] deviceIDs = Array.ConvertAll(devices, device => device.ID);
            ContextSafeHandle handle = UnsafeNativeMethods.CreateContext(deviceIDs, null, IntPtr.Zero);
            return new Context(handle);
        }

        public static Context Create(Platform platform, params Device[] devices)
        {
            if (devices == null)
                throw new ArgumentNullException("devices");
            if (devices.Length == 0)
                throw new ArgumentException("No devices specified.");

            UnsafeNativeMethods.ClDeviceID[] deviceIDs = Array.ConvertAll(devices, device => device.ID);
            ContextSafeHandle handle = UnsafeNativeMethods.CreateContext(platform.ID, deviceIDs, null, IntPtr.Zero);
            return new Context(handle);
        }

        public static Context Create(Platform platform, DeviceType deviceType)
        {
            if (platform == null)
                throw new ArgumentNullException("platform");

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
                throw new ArgumentNullException("device");

            CommandQueueSafeHandle handle = UnsafeNativeMethods.CreateCommandQueue(this.Handle, device.ID, properties);
            return new CommandQueue(handle, this, device);
        }

        public Mem CreateBuffer(MemoryFlags flags, long size)
        {
            return CreateBuffer(flags, size, IntPtr.Zero);
        }

        public Mem CreateBuffer(MemoryFlags flags, long size, IntPtr hostAddress)
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
            return new Mem(this, handle);
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

        /// <summary>
        /// Creates a sampler object. Samplers controls how elements of an <see cref="Image"/> object are read by
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
