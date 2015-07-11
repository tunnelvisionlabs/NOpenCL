// Copyright (c) Tunnel Vision Laboratories, LLC. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace NOpenCL
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using NOpenCL.SafeHandles;
    using System.Runtime.InteropServices;

    public sealed class Program : IDisposable
    {
        private readonly Context _context;
        private readonly ProgramSafeHandle _handle;
        private bool _disposed;

        internal Program(Context context, ProgramSafeHandle handle)
        {
            if (context == null)
                throw new ArgumentNullException("context");
            if (handle == null)
                throw new ArgumentNullException("handle");
            
            _context = context;
            _handle = handle;
        }

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public uint ReferenceCount
        {
            get
            {
                return UnsafeNativeMethods.GetProgramInfo(Handle, UnsafeNativeMethods.ProgramInfo.ReferenceCount);
            }
        }

        /// <summary>
        /// Compiles a program’s source for specific device(s) in the OpenCL context associated with program.
        /// </summary>
        /// <param name="devices">A pointer to a list of devices associated with program. If device_list is 
        /// a NULL value, the compile is performed for all devices associated with program. If device_list 
        /// is a non-NULL value, the compile is performed for devices specified in this list.</param>
        /// <param name="options">A pointer to a null-terminated string of characters that describes the 
        /// compilation options to be used for building the program executable. The list of supported options 
        /// is as described below.</param>
        /// <param name="inputHeaders">An array of program embedded headers created with 
        /// <see cref="UnsafeNativeMethods.clCreateProgramWithSource"/>.</param>
        /// <param name="headerNames">An array that has a one to one correspondence with input_headers. 
        /// Each entry in header_include_names specifies the include name used by source in program that comes 
        /// from an embedded header. The corresponding entry in input_headers identifies the program object 
        /// which contains the header source to be used. The embedded headers are first searched before the 
        /// headers in the list of directories specified by the –I compile option (as described in section 
        /// 5.8.4.1). If multiple entries in header_include_names refer to the same header name, the first one 
        /// encountered will be used.</param>
        public void Compile(Device[] devices, string options, ProgramSafeHandle[] inputHeaders, string[] headerNames)
        {
            UnsafeNativeMethods.ClDeviceID[] deviceIDs = null;
            if (devices != null)
                deviceIDs = Array.ConvertAll(devices, device => device.ID);
            
            UnsafeNativeMethods.CompileProgram(_handle, deviceIDs, options, inputHeaders, headerNames, null, IntPtr.Zero);
        }

        /// <summary>
        /// Compiles a program’s source for specific device(s) in the OpenCL context associated with program.
        /// </summary>
        /// <param name="devices">A pointer to a list of devices associated with program. If device_list is 
        /// a NULL value, the compile is performed for all devices associated with program. If device_list 
        /// is a non-NULL value, the compile is performed for devices specified in this list.</param>
        /// <param name="inputHeaders">An array of program embedded headers created with 
        /// <see cref="UnsafeNativeMethods.clCreateProgramWithSource"/>.</param>
        /// <param name="headerNames">An array that has a one to one correspondence with input_headers. 
        /// Each entry in header_include_names specifies the include name used by source in program that comes 
        /// from an embedded header. The corresponding entry in input_headers identifies the program object 
        /// which contains the header source to be used. The embedded headers are first searched before the 
        /// headers in the list of directories specified by the –I compile option (as described in section 
        /// 5.8.4.1). If multiple entries in header_include_names refer to the same header name, the first one 
        /// encountered will be used.</param>
        public void Compile(Device[] devices, ProgramSafeHandle[] inputHeaders, string[] headerNames)
        {
            UnsafeNativeMethods.ClDeviceID[] deviceIDs = null;
            if (devices != null)
                deviceIDs = Array.ConvertAll(devices, device => device.ID);

            UnsafeNativeMethods.CompileProgram(_handle, deviceIDs, "", inputHeaders, headerNames, null, IntPtr.Zero);
        }

        /// <summary>
        /// Compiles a program’s source for all the devices.
        /// </summary>
        /// <param name="options">A pointer to a null-terminated string of characters that describes the 
        /// compilation options to be used for building the program executable. The list of supported options 
        /// is as described below.</param>
        /// <param name="inputHeaders">An array of program embedded headers created with 
        /// <see cref="UnsafeNativeMethods.clCreateProgramWithSource"/>.</param>
        /// <param name="headerNames">An array that has a one to one correspondence with input_headers. 
        /// Each entry in header_include_names specifies the include name used by source in program that comes 
        /// from an embedded header. The corresponding entry in input_headers identifies the program object 
        /// which contains the header source to be used. The embedded headers are first searched before the 
        /// headers in the list of directories specified by the –I compile option (as described in section 
        /// 5.8.4.1). If multiple entries in header_include_names refer to the same header name, the first one 
        /// encountered will be used.</param>
        public void Compile(string options, ProgramSafeHandle[] inputHeaders, string[] headerNames)
        {
            UnsafeNativeMethods.CompileProgram(_handle, null, options, inputHeaders, headerNames, null, IntPtr.Zero);
        }

        /// <summary>
        /// Compiles a program’s source for all the devices.
        /// </summary>
        /// <param name="inputHeaders">An array of program embedded headers created with 
        /// <see cref="UnsafeNativeMethods.clCreateProgramWithSource"/>.</param>
        /// <param name="headerNames">An array that has a one to one correspondence with input_headers. 
        /// Each entry in header_include_names specifies the include name used by source in program that comes 
        /// from an embedded header. The corresponding entry in input_headers identifies the program object 
        /// which contains the header source to be used. The embedded headers are first searched before the 
        /// headers in the list of directories specified by the –I compile option (as described in section 
        /// 5.8.4.1). If multiple entries in header_include_names refer to the same header name, the first one 
        /// encountered will be used.</param>
        public void Compile(ProgramSafeHandle[] inputHeaders, string[] headerNames)
        {
            UnsafeNativeMethods.CompileProgram(_handle, null, "", inputHeaders, headerNames, null, IntPtr.Zero);
        }

        /// <summary>
        /// Compiles a program’s source for all the devices.
        /// </summary>
        public void Compile()
        {
            ProgramSafeHandle[] inputHeaders = new ProgramSafeHandle[0];
            string[] headerNames = new string[0];
            UnsafeNativeMethods.CompileProgram(_handle, null, "", inputHeaders, headerNames, null, IntPtr.Zero);
        }

        /// <summary>
        /// Gets the current programs Context.
        /// </summary>
        public Context Context
        {
            get
            {
                ThrowIfDisposed();
                return _context;
            }
        }

        /// <summary>
        /// Gets the number of devices for the given program.
        /// </summary>
        public uint NumDevices
        {
            get
            {
                return UnsafeNativeMethods.GetProgramInfo(Handle, UnsafeNativeMethods.ProgramInfo.NumDevices);
            }
        }

        public IReadOnlyList<Device> Devices
        {
            get
            {
                //return UnsafeNativeMethods.GetProgramDevices(Handle);

                IntPtr[] devices = UnsafeNativeMethods.GetProgramInfo(Handle, UnsafeNativeMethods.ProgramInfo.Devices);
                return Array.ConvertAll(devices, device => new Device(device));
            }
        }

        public string Source
        {
            get
            {
                return UnsafeNativeMethods.GetProgramInfo(Handle, UnsafeNativeMethods.ProgramInfo.Source);
            }
        }

        public IReadOnlyList<UIntPtr> BinarySizes
        {
            get
            {
                return UnsafeNativeMethods.GetProgramInfo(Handle, UnsafeNativeMethods.ProgramInfo.BinarySizes);
            }
        }

        /// <summary>
        /// Returns an array of binaries.
        /// </summary>
        public Binary[] Binaries
        {
            get
            {
                Binary[] bins = UnsafeNativeMethods.GetBinaries(Handle);
                
                return bins;
            }
        }

        public IReadOnlyList<IntPtr> BinariesAsIntPtrList
        {
            get
            {
                return UnsafeNativeMethods.GetProgramInfo(Handle, UnsafeNativeMethods.ProgramInfo.Binaries);
            }
        }

        public IntPtr NumKernels
        {
            get
            {
                return UnsafeNativeMethods.GetProgramInfo(Handle, UnsafeNativeMethods.ProgramInfo.NumKernels);
            }
        }

        public IReadOnlyList<string> KernelNames
        {
            get
            {
                return UnsafeNativeMethods.GetProgramInfo(Handle, UnsafeNativeMethods.ProgramInfo.KernelNames).Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            }
        }

        internal ProgramSafeHandle Handle
        {
            get
            {
                ThrowIfDisposed();
                return _handle;
            }
        }

        public BuildStatus GetBuildStatus(Device device)
        {
            if (device == null)
                throw new ArgumentNullException("device");

            return (BuildStatus)UnsafeNativeMethods.GetProgramBuildInfo(Handle, device.ID, UnsafeNativeMethods.ProgramBuildInfo.BuildStatus);
        }

        public string GetBuildOptions(Device device)
        {
            if (device == null)
                throw new ArgumentNullException("device");

            return UnsafeNativeMethods.GetProgramBuildInfo(Handle, device.ID, UnsafeNativeMethods.ProgramBuildInfo.BuildOptions);
        }

        public string GetBuildLog(Device device)
        {
            if (device == null)
                throw new ArgumentNullException("device");

            return UnsafeNativeMethods.GetProgramBuildInfo(Handle, device.ID, UnsafeNativeMethods.ProgramBuildInfo.BuildLog);
        }

        public BinaryType GetBinaryType(Device device)
        {
            if (device == null)
                throw new ArgumentNullException("device");

            return (BinaryType)UnsafeNativeMethods.GetProgramBuildInfo(Handle, device.ID, UnsafeNativeMethods.ProgramBuildInfo.BinaryType);
        }

        public Kernel CreateKernel(string name)
        {
            KernelSafeHandle kernel = UnsafeNativeMethods.CreateKernel(Handle, name);
            return new Kernel(kernel, this);
        }

        public void Build()
        {
            Build(null, null);
        }

        public void Build(string options)
        {
            Build(null, options);
        }

        public void Build(Device[] devices)
        {
            Build(devices, null);
        }

        public void Build(Device[] devices, string options)
        {
            UnsafeNativeMethods.ClDeviceID[] deviceIDs = null;
            if (devices != null)
                deviceIDs = Array.ConvertAll(devices, device => device.ID);

            UnsafeNativeMethods.BuildProgram(Handle, deviceIDs, options, null, IntPtr.Zero);
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

    /// <summary>
    /// Contains a Binary in a simple byte array.
    /// </summary>
    public struct Binary
    {
        public byte[] bytes;

        public Binary(byte[] value)
        {
            this.bytes = value;
        }

        public Binary(int size)
        {
            this.bytes = new byte[size];
        }

        public static implicit operator Binary(byte[] bin)
        {
            return new Binary(bin);
        }

        public static implicit operator byte[] (Binary bin)
        {
            return bin.bytes;
        }
    }
}
