// Copyright (c) Tunnel Vision Laboratories, LLC. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace NOpenCL
{
    using System;
    using System.Runtime.InteropServices;
    using NOpenCL.SafeHandles;

    partial class UnsafeNativeMethods
    {
        #region Program Objects

        [DllImport(ExternDll.OpenCL)]
        private static extern ProgramSafeHandle clCreateProgramWithSource(
            ContextSafeHandle context,
            uint count,
            [In, MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.LPStr)] string[] strings,
            [In, MarshalAs(UnmanagedType.LPArray)] IntPtr[] lengths,
            out ErrorCode errorCode);

        public static ProgramSafeHandle CreateProgramWithSource(ContextSafeHandle context, string[] strings)
        {
            if (context == null)
                throw new ArgumentNullException("context");
            if (strings == null)
                throw new ArgumentNullException("strings");

            ErrorCode errorCode;
            ProgramSafeHandle handle = clCreateProgramWithSource(context, (uint)strings.Length, strings, null, out errorCode);
            ErrorHandler.ThrowOnFailure(errorCode);
            return handle;
        }

        [DllImport(ExternDll.OpenCL)]
        private static extern ProgramSafeHandle clCreateProgramWithBinary(
            ContextSafeHandle context,
            uint numDevices,
            [In, MarshalAs(UnmanagedType.LPArray)] ClDeviceID[] devices,
            [In, MarshalAs(UnmanagedType.LPArray)] IntPtr[] lengths,
            [In, MarshalAs(UnmanagedType.LPArray)] IntPtr[] binaries,
            [Out, MarshalAs(UnmanagedType.LPArray)] ErrorCode[] binaryStatus,
            out ErrorCode errorCode);

        /// <summary>
        /// Creates a program object for a <paramref name="context"/>, and loads specified binary 
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
        /// <param name="context">Must be a valid OpenCL context.</param>
        /// <param name="devices">A pointer to a list of devices that are in context. device_list 
        /// must be a non-NULL value. The binaries are loaded for devices specified in this list.</param>
        /// <param name="bins">An array of pointers to program binaries to be loaded for devices 
        /// specified by device_list. For each device given by device_list[i], the pointer to the 
        /// program binary for that device is given by binaries[i] and the length of this 
        /// corresponding binary is given by lengths[i]. lengths[i] cannot be zero and binaries[i] 
        /// cannot be a NULL pointer.</param>
        /// <returns>
        /// <see cref="clUnloadPlatformCompiler"/> returns <see cref="ErrorCode.Success"/>
        /// if the function is executed successfully. Otherwise, it returns one of the following
        /// errors:
        ///
        /// <list type="bullet">
        /// <item><see cref="ErrorCode.InvalidContext"/> if <paramref name="context"/> is not a valid context.</item>
        /// <item><see cref="ErrorCode.InvalidValue"/> if device_list is NULL or num_devices is zero; or if lengths or binaries are null or if any entry in lengths[i] or binaries[i] is NULL.</item>
        /// <item><see cref="ErrorCode.InvalidDevice"/> if OpenCL devices listed in device_list are not in the list of devices associated with context.</item>
        /// <item><see cref="ErrorCode.InvalidBinary"/> if an invalid program binary was encountered for any device. binary_status will return specific status for each device.</item>
        /// <item><see cref="ErrorCode.OutOfHostMemory"/>  if there is a failure to allocate resources required by the OpenCL implementation on the host.</item>
        /// </list>
        /// </returns>
        public static ProgramSafeHandle CreateProgramWithBinary(ContextSafeHandle context, ClDeviceID[] devices, byte[][] bins)
        {
            if (context == null)
                throw new ArgumentNullException("context");
            if (bins == null)
                throw new ArgumentNullException("bins");
            if (devices == null)
                throw new ArgumentNullException("devices");
            if (devices.Length == 0)
                throw new ArgumentException("No devices specified.");

            uint devCt = (devices != null && devices.Length > 0) ? (uint)devices.Length : 0;

            int binCt = bins.Length;

            IntPtr[] lengths = Array.ConvertAll(bins, b => (IntPtr)b.Length);

            ErrorCode errorCode;
            ErrorCode[] binaryStatus = new ErrorCode[binCt];
            GCHandle[] gh = null;

            ProgramSafeHandle handle;
            try
            {
                gh = Array.ConvertAll(bins, b => GCHandle.Alloc(b, GCHandleType.Pinned));
                IntPtr[] binaries = Array.ConvertAll(gh, h => h.AddrOfPinnedObject());

                handle = clCreateProgramWithBinary(context, devCt, devices, lengths, binaries, binaryStatus, out errorCode);
            }
            finally
            {
                for (int i = 0; i < gh.Length; i++)
                    gh[i].Free();
            }
            
            ErrorHandler.ThrowOnFailure(errorCode);

            return handle;
        }


        [DllImport(ExternDll.OpenCL)]
        private static extern ProgramSafeHandle clCreateProgramWithBuiltInKernels(
            ContextSafeHandle context,
            uint numDevices,
            [In, MarshalAs(UnmanagedType.LPArray)] ClDeviceID[] devices,
            [MarshalAs(UnmanagedType.LPStr)] string kernelNames,
            out ErrorCode errorCode);

        [DllImport(ExternDll.OpenCL)]
        private static extern ErrorCode clRetainProgram(ProgramSafeHandle program);

        [DllImport(ExternDll.OpenCL)]
        public static extern ErrorCode clReleaseProgram(IntPtr program);

        [DllImport(ExternDll.OpenCL)]
        private static extern ErrorCode clBuildProgram(
            ProgramSafeHandle program,
            uint numDevices,
            [In, MarshalAs(UnmanagedType.LPArray)] ClDeviceID[] devices,
            [MarshalAs(UnmanagedType.LPStr)] string options,
            BuildProgramCallback pfnNotify,
            IntPtr userData);

        public static void BuildProgram(ProgramSafeHandle program, ClDeviceID[] devices, string options, BuildProgramCallback notify, IntPtr userData)
        {
            ErrorHandler.ThrowOnFailure(clBuildProgram(program, devices != null && devices.Length > 0 ? (uint)devices.Length : 0, devices, options, notify, userData));
        }

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate void CompileProgramCallback(ProgramSafeHandle program, IntPtr userData);

        [DllImport(ExternDll.OpenCL)]
        private static extern ErrorCode clCompileProgram(
            ProgramSafeHandle program,
            uint numDevices,
            [In, MarshalAs(UnmanagedType.LPArray)] ClDeviceID[] devices,
            string options,
            uint numInputHeaders,
            [In, MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(SafeHandleArrayMarshaler))] ProgramSafeHandle[] inputHeaders,
            [In, MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.LPStr)] string[] headerIncludeNames,
            [MarshalAs(UnmanagedType.FunctionPtr)] CompileProgramCallback callbackPointer, IntPtr userData);

        /// <summary>
        /// Compiles a program’s source for all the devices or a specific device(s) in the OpenCL context associated with program.
        /// </summary>
        /// <param name="program">The program object that is the compilation target.</param>
        /// <param name="devices">A pointer to a list of devices associated with program. If device_list is 
        /// a NULL value, the compile is performed for all devices associated with program. If device_list 
        /// is a non-NULL value, the compile is performed for devices specified in this list.</param>
        /// <param name="options">A pointer to a null-terminated string of characters that describes the 
        /// compilation options to be used for building the program executable. The list of supported options 
        /// is as described below.</param>
        /// <param name="inputHeaders">An array of program embedded headers created with 
        /// <see cref="clCreateProgramWithSource"/>.</param>
        /// <param name="headerIncludeNames">An array that has a one to one correspondence with input_headers. 
        /// Each entry in header_include_names specifies the include name used by source in program that comes 
        /// from an embedded header. The corresponding entry in input_headers identifies the program object 
        /// which contains the header source to be used. The embedded headers are first searched before the 
        /// headers in the list of directories specified by the –I compile option (as described in section 
        /// 5.8.4.1). If multiple entries in header_include_names refer to the same header name, the first one 
        /// encountered will be used.</param>
        /// <param name="pfnNotify">A function pointer to a notification routine. The notification routine is 
        /// a callback function that an application can register and which will be called when the program 
        /// executable has been built (successfully or unsuccessfully). If pfnNotify is not NULL, 
        /// clCompileProgram does not need to wait for the compiler to complete and can return immediately 
        /// once the compilation can begin. The compilation can begin if the context, program whose sources 
        /// are being compiled, list of devices, input headers, programs that describe input headers and 
        /// compiler options specified are all valid and appropriate host and device resources needed to 
        /// perform the compile are available. If pfn_notify is NULL, clCompileProgram does not return until 
        /// the compiler has completed. This callback function may be called asynchronously by the OpenCL 
        /// implementation. It is the application’s responsibility to ensure that the callback function is 
        /// thread-safe.</param>
        /// <param name="userData">Passed as an argument when pfnNotify is called. user_data can be NULL.</param>
        public static void CompileProgram(ProgramSafeHandle program, ClDeviceID[] devices, string options, ProgramSafeHandle[] inputHeaders,
            string[] headerIncludeNames, CompileProgramCallback pfnNotify, IntPtr userData)
        {
            ErrorHandler.ThrowOnFailure(clCompileProgram(
                program,
                (devices != null && devices.Length > 0) ? (uint)devices.Length : 0,
                devices,
                options,
                (uint)inputHeaders.Length,
                inputHeaders,
                headerIncludeNames,
                pfnNotify,
                userData));
        }

        [DllImport(ExternDll.OpenCL)]
        private static extern ProgramSafeHandle clLinkProgram(
            ContextSafeHandle context,
            uint numDevices,
            [In, MarshalAs(UnmanagedType.LPArray)] ClDeviceID[] devices,
            [MarshalAs(UnmanagedType.LPStr)] string options,
            uint numInputPrograms,
            [In, MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(SafeHandleArrayMarshaler))] ProgramSafeHandle[] inputPrograms,
            BuildProgramCallback pfnNotify,
            IntPtr userData,
            out ErrorCode errorCode);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate void BuildProgramCallback(ProgramSafeHandle program, IntPtr userData);

        /// <summary>
        /// Allows the implementation to release the resources allocated by the OpenCL compiler
        /// for <paramref name="platform"/>.
        /// http://www.khronos.org/registry/cl/sdk/1.2/docs/man/xhtml/clUnloadPlatformCompiler.html
        /// http://www.khronos.org/registry/cl/specs/opencl-1.2.pdf#page=149
        /// </summary>
        /// <remarks>
        /// This is a hint from the application and does not guarantee that the compiler will
        /// not be used in the future or that the compiler will actually be unloaded by the
        /// implementation. Calls to <see cref="clBuildProgram"/>, <see cref="clCompileProgram"/>,
        /// or <see cref="clLinkProgram"/> after <see cref="clUnloadPlatformCompiler"/> will
        /// reload the compiler, if necessary, to build the appropriate program executable.
        /// </remarks>
        /// <param name="platform">A valid platform.</param>
        /// <returns>
        /// <see cref="clUnloadPlatformCompiler"/> returns <see cref="ErrorCode.Success"/>
        /// if the function is executed successfully. Otherwise, it returns one of the following
        /// errors:
        ///
        /// <list type="bullet">
        /// <item><see cref="ErrorCode.InvalidPlatform"/> if <paramref name="platform"/> is not a valid platform.</item>
        /// </list>
        /// </returns>
        [DllImport(ExternDll.OpenCL)]
        private static extern ErrorCode clUnloadPlatformCompiler(ClPlatformID platform);

        /// <summary>
        /// Invokes <see cref="clUnloadPlatformCompiler"/>, and throws an exception if
        /// the call does not succeed.
        /// </summary>
        /// <param name="platform">A valid platform.</param>
        /// <seealso cref="Platform.UnloadCompiler"/>
        public static void UnloadPlatformCompiler(ClPlatformID platform)
        {
            ErrorHandler.ThrowOnFailure(clUnloadPlatformCompiler(platform));
        }

        [DllImport(ExternDll.OpenCL)]
        private static extern ErrorCode clGetProgramInfo(
            ProgramSafeHandle program,
            int paramName,
            UIntPtr paramValueSize,
            IntPtr paramValue,
            out UIntPtr paramValueSizeRet);

        [DllImport(ExternDll.OpenCL)]
        private static extern ErrorCode clGetProgramInfo(
            ProgramSafeHandle program,
            int paramName,
            UIntPtr paramValueSize,
            IntPtr[] paramValue,
            out UIntPtr paramValueSizeRet);

        public static T GetProgramInfo<T>(ProgramSafeHandle program, ProgramParameterInfo<T> parameter)
        {
            if (program == null)
                throw new ArgumentNullException("program");
            if (parameter == null)
                throw new ArgumentNullException("parameter");

            int? fixedSize = parameter.ParameterInfo.FixedSize;
#if DEBUG
            bool verifyFixedSize = true;
#else
            bool verifyFixedSize = false;
#endif

            UIntPtr requiredSize;
            if (fixedSize.HasValue && !verifyFixedSize)
                requiredSize = (UIntPtr)fixedSize;
            else
                ErrorHandler.ThrowOnFailure(clGetProgramInfo(program, parameter.ParameterInfo.Name, UIntPtr.Zero, IntPtr.Zero, out requiredSize));

            if (verifyFixedSize && fixedSize.HasValue)
            {
                if (requiredSize.ToUInt64() != (ulong)fixedSize.Value)
                    throw new ArgumentException("The parameter definition includes a fixed size that does not match the required size according to the runtime.");
            }

            IntPtr memory = IntPtr.Zero;
            try
            {
                memory = Marshal.AllocHGlobal((int)requiredSize.ToUInt32());
                UIntPtr actualSize;
                ErrorHandler.ThrowOnFailure(clGetProgramInfo(program, parameter.ParameterInfo.Name, requiredSize, memory, out actualSize));
                return parameter.ParameterInfo.Deserialize(actualSize, memory);
            }
            finally
            {
                Marshal.FreeHGlobal(memory);
            }
        }

        /// <summary>
        /// Returns an array that contains the size in bytes of the program binary for each device associated with program. The size of the array is the number of devices associated with program. If a binary is not available for a device(s), a size of zero is returned.
        /// </summary>
        /// <remarks>
        /// If program is created using clCreateProgramWithBuiltInKernels, the implementation may return zero in any entries of the returned array.
        /// </remarks>
        public static int[] GetBinarySizes(ProgramSafeHandle program)
        {
            IntPtr memory = IntPtr.Zero;
            UIntPtr actualSize;
            try
            {
                memory = Marshal.AllocHGlobal(IntPtr.Size);
                ErrorHandler.ThrowOnFailure(clGetProgramInfo(program, 0x1165, (UIntPtr)UIntPtr.Size, memory, out actualSize));
                IntPtr[] array = new IntPtr[1];
                Marshal.Copy(memory, array, 0, array.Length);
                return Array.ConvertAll(array, p => (int)p);
            }
            finally
            {
                Marshal.FreeHGlobal(memory);
            }
        }

        public static Binary[] GetBinaries(ProgramSafeHandle program)
        {
            int[] sizes = GetBinarySizes(program);
            GCHandle[] gh = Array.ConvertAll(sizes, s => GCHandle.Alloc(new byte[s], GCHandleType.Pinned));
            IntPtr[] binaries2 = Array.ConvertAll(gh, h => h.AddrOfPinnedObject());

            int progCt = sizes.Length;
            UIntPtr actual = UIntPtr.Zero;
            try
            {
                ErrorHandler.ThrowOnFailure(clGetProgramInfo(program, 0x1166, new UIntPtr((uint)(UIntPtr.Size * progCt)), binaries2, out actual));

                if (IntPtr.Size * progCt != actual.ToUInt32())
                    throw new Exception(string.Format("unexpected clGetProgramInfo size: {0}!={1}", actual.ToUInt32(), IntPtr.Size * progCt));

                return Array.ConvertAll(gh, g => new Binary((byte[])g.Target));
            }
            finally
            {
                for (int i = 0; i < progCt; i++)
                    gh[i].Free();
            }
        }

        public static Device[] GetDevices(ProgramSafeHandle program)
        {
            UIntPtr requiredSize;
            int val = 0x1163;
            ErrorHandler.ThrowOnFailure(clGetProgramInfo(program, val, UIntPtr.Zero, IntPtr.Zero, out requiredSize));

            uint size = requiredSize.ToUInt32();
            if (size == 0)
                return new Device[0];

            IntPtr memory = IntPtr.Zero;
            try
            {
                memory = Marshal.AllocHGlobal((int)requiredSize.ToUInt32());
                UIntPtr actualSize;
                ErrorHandler.ThrowOnFailure(clGetProgramInfo(program, val, requiredSize, memory, out actualSize));
                IntPtr[] array = new IntPtr[(int)((long)actualSize.ToUInt64() / IntPtr.Size)];
                Marshal.Copy(memory, array, 0, array.Length);

                return Array.ConvertAll(array, d => new Device(d));
            }
            finally
            {
                Marshal.FreeHGlobal(memory);
            }
        }

        /// <summary>
        /// Specifies the information to query when using GetProgramInfo(..) (aka clGetProgramInfo(..))
        /// </summary>
        public static class ProgramInfo
        {
            /// <summary>
            /// Return the program reference count.
            /// <see href="https://www.khronos.org/registry/cl/sdk/2.0/docs/man/xhtml/clGetProgramInfo.html"/>
            /// </summary>
            /// <remarks>
            /// The reference count returned should be considered immediately stale. It is unsuitable for general use in applications. This feature is provided for identifying memory leaks.
            /// </remarks>
            public static readonly ProgramParameterInfo<uint> ReferenceCount = (ProgramParameterInfo<uint>)new ParameterInfoUInt32(0x1160);
            /// <summary>
            /// Return the context specified when the program object is created.
            /// <see href="https://www.khronos.org/registry/cl/sdk/2.0/docs/man/xhtml/clGetProgramInfo.html"/>
            /// </summary>
            public static readonly ProgramParameterInfo<IntPtr> Context = (ProgramParameterInfo<IntPtr>)new ParameterInfoIntPtr(0x1161);
            /// <summary>
            /// Return the number of devices associated with program.
            /// </summary>
            /// <remarks>
            /// The actual number of characters that represents the program source code including the null terminator is returned in param_value_size_ret.
            /// </remarks>
            /// <see href="https://www.khronos.org/registry/cl/sdk/2.0/docs/man/xhtml/clGetProgramInfo.html"/>
            public static readonly ProgramParameterInfo<uint> NumDevices = (ProgramParameterInfo<uint>)new ParameterInfoUInt32(0x1162);
            /// <summary>
            /// Return the list of devices associated with the program object. This can be the devices associated with context on which the program object has been created or can be a subset of devices that are specified when a progam object is created using clCreateProgramWithBinary.
            /// <see href="https://www.khronos.org/registry/cl/sdk/2.0/docs/man/xhtml/clGetProgramInfo.html"/>
            /// </summary>
            public static readonly ProgramParameterInfo<IntPtr[]> Devices = (ProgramParameterInfo<IntPtr[]>)new ParameterInfoIntPtrArray(0x1163);
            /// <summary>
            /// Return the program source code specified by clCreateProgramWithSource. The source string returned is a concatenation of all source strings specified to clCreateProgramWithSource with a null terminator. The concatenation strips any nulls in the original source strings.
            /// <see href="https://www.khronos.org/registry/cl/sdk/2.0/docs/man/xhtml/clGetProgramInfo.html"/>
            /// </summary>
            /// <remarks>
            /// If program is created using clCreateProgramWithBinary or clCreateProgramWithBuiltInKernels, a null string or the appropriate program source code is returned depending on whether or not the program source code is stored in the binary.
            /// The actual number of characters that represents the program source code including the null terminator is returned in param_value_size_ret.
            /// </remarks>
            public static readonly ProgramParameterInfo<string> Source = (ProgramParameterInfo<string>)new ParameterInfoString(0x1164);
            /// <summary>
            /// Returns an array that contains the size in bytes of the program binary for each device associated with program. The size of the array is the number of devices associated with program. If a binary is not available for a device(s), a size of zero is returned.
            /// <see href="https://www.khronos.org/registry/cl/sdk/2.0/docs/man/xhtml/clGetProgramInfo.html"/>
            /// </summary>
            /// <remarks>
            /// If program is created using clCreateProgramWithBuiltInKernels, the implementation may return zero in any entries of the returned array.
            /// </remarks>
            public static readonly ProgramParameterInfo<UIntPtr[]> BinarySizes = (ProgramParameterInfo<UIntPtr[]>)new ParameterInfoUIntPtrArray(0x1165);
            /// <summary>
            /// Return the program binaries for all devices associated with program. For each device in program, the binary returned can be the binary specified for the device when program is created with clCreateProgramWithBinary or it can be the executable binary generated by clBuildProgram. If program is created with clCreateProgramWithSource, the binary returned is the binary generated by clBuildProgram. The bits returned can be an implementation-specific intermediate representation (a.k.a. IR) or device specific executable bits or both. The decision on which information is returned in the binary is up to the OpenCL implementation.
            /// <see href="https://www.khronos.org/registry/cl/sdk/2.0/docs/man/xhtml/clGetProgramInfo.html"/>
            /// </summary>
            /// <remarks>
            /// param_value points to an array of n pointers where n is the number of devices associated with program. The buffer sizes needed to allocate the memory that these n pointers refer to can be queried using the CL_PROGRAM_BINARY_SIZES query as described in this table.
            /// Each entry in this array is used by the implementation as the location in memory where to copy the program binary for a specific device, if there is a binary available. To find out which device the program binary in the array refers to, use the CL_PROGRAM_DEVICES query to get the list of devices. There is a one-to-one correspondence between the array of n pointers returned by CL_PROGRAM_BINARIES and array of devices returned by CL_PROGRAM_DEVICES.
            /// </remarks>
            public static readonly ProgramParameterInfo<IntPtr[]> Binaries = (ProgramParameterInfo<IntPtr[]>)new ParameterInfoIntPtrArray(0x1166);
            /// <summary>
            /// Returns the number of kernels declared in program that can be created with clCreateKernel. This information is only available after a successful program executable has been built for at least one device in the list of devices associated with program.
            /// <see href="https://www.khronos.org/registry/cl/sdk/2.0/docs/man/xhtml/clGetProgramInfo.html"/>
            /// </summary>
            public static readonly ProgramParameterInfo<IntPtr> NumKernels = (ProgramParameterInfo<IntPtr>)new ParameterInfoIntPtr(0x1167);
            /// <summary>
            /// Returns a semi-colon separated list of kernel names in program that can be created with clCreateKernel. This information is only available after a successful program executable has been built for at least one device in the list of devices associated with program.
            /// <see href="https://www.khronos.org/registry/cl/sdk/2.0/docs/man/xhtml/clGetProgramInfo.html"/>
            /// </summary>
            public static readonly ProgramParameterInfo<string> KernelNames = (ProgramParameterInfo<string>)new ParameterInfoString(0x1168);
        }

        public sealed class ProgramParameterInfo<T>
        {
            private readonly ParameterInfo<T> _parameterInfo;

            public ProgramParameterInfo(ParameterInfo<T> parameterInfo)
            {
                if (parameterInfo == null)
                    throw new ArgumentNullException("parameterInfo");

                _parameterInfo = parameterInfo;
            }

            public static explicit operator ProgramParameterInfo<T>(ParameterInfo<T> parameterInfo)
            {
                return new ProgramParameterInfo<T>(parameterInfo);
            }

            public ParameterInfo<T> ParameterInfo
            {
                get
                {
                    return _parameterInfo;
                }
            }
        }

        [DllImport(ExternDll.OpenCL)]
        private static extern ErrorCode clGetProgramBuildInfo(
            ProgramSafeHandle program,
            ClDeviceID device,
            int paramName,
            UIntPtr paramValueSize,
            IntPtr paramValue,
            out UIntPtr paramValueSizeRet);

        public static T GetProgramBuildInfo<T>(ProgramSafeHandle program, ClDeviceID device, ProgramBuildParameterInfo<T> parameter)
        {
            if (program == null)
                throw new ArgumentNullException("program");
            if (parameter == null)
                throw new ArgumentNullException("parameter");

            int? fixedSize = parameter.ParameterInfo.FixedSize;
#if DEBUG
            bool verifyFixedSize = true;
#else
            bool verifyFixedSize = false;
#endif

            UIntPtr requiredSize;
            if (fixedSize.HasValue && !verifyFixedSize)
                requiredSize = (UIntPtr)fixedSize;
            else
                ErrorHandler.ThrowOnFailure(clGetProgramBuildInfo(program, device, parameter.ParameterInfo.Name, UIntPtr.Zero, IntPtr.Zero, out requiredSize));

            if (verifyFixedSize && fixedSize.HasValue)
            {
                if (requiredSize.ToUInt64() != (ulong)fixedSize.Value)
                    throw new ArgumentException("The parameter definition includes a fixed size that does not match the required size according to the runtime.");
            }

            IntPtr memory = IntPtr.Zero;
            try
            {
                memory = Marshal.AllocHGlobal((int)requiredSize.ToUInt32());
                UIntPtr actualSize;
                ErrorHandler.ThrowOnFailure(clGetProgramBuildInfo(program, device, parameter.ParameterInfo.Name, requiredSize, memory, out actualSize));
                return parameter.ParameterInfo.Deserialize(actualSize, memory);
            }
            finally
            {
                Marshal.FreeHGlobal(memory);
            }
        }

        public static class ProgramBuildInfo
        {
            /// <summary>
            /// Returns the build, compile or link status, whichever was performed last on program for device.
            /// This can be one of the following:
            ///
            /// <list type="bullet">
            /// <item><see cref="NOpenCL.BuildStatus.None"/>. The build status returned if no <see cref="clBuildProgram"/>, <see cref="clCompileProgram"/> or <see cref="clLinkProgram"/> has been performed on the specified program object for device.</item>
            /// <item><see cref="NOpenCL.BuildStatus.Error"/>. The build status returned if <see cref="clBuildProgram"/>, <see cref="clCompileProgram"/> or <see cref="clLinkProgram"/> whichever was performed last on the specified program object for device generated an error.</item>
            /// <item><see cref="NOpenCL.BuildStatus.Success"/>. The build status returned if <see cref="clBuildProgram"/>, <see cref="clCompileProgram"/> or <see cref="clLinkProgram"/> whichever was performed last on the specified program object for device was successful.</item>
            /// <item><see cref="NOpenCL.BuildStatus.InProgress"/>. The build status returned if <see cref="clBuildProgram"/>, <see cref="clCompileProgram"/> or <see cref="clLinkProgram"/> whichever was performed last on the specified program object for device has not finished.</item>
            /// </list>
            /// </summary>
            public static readonly ProgramBuildParameterInfo<uint> BuildStatus =
                (ProgramBuildParameterInfo<uint>)new ParameterInfoUInt32(0x1181);

            /// <summary>
            /// Return the build, compile or link options specified by the options argument in
            /// <see cref="clBuildProgram"/>, <see cref="clCompileProgram"/>, or <see cref="clLinkProgram"/>,
            /// whichever was performed last on program for device.
            /// <para/>
            /// If build status of program for device is <see cref="NOpenCL.BuildStatus.None"/>, an empty string is returned.
            /// </summary>
            public static readonly ProgramBuildParameterInfo<string> BuildOptions =
                (ProgramBuildParameterInfo<string>)new ParameterInfoString(0x1182);

            /// <summary>
            /// Return the build, compile or link log for <see cref="clBuildProgram"/>,
            /// <see cref="clCompileProgram"/>, or <see cref="clLinkProgram"/> whichever
            /// was performed last on program for device.
            /// <para/>
            /// If build status of program for device is <see cref="NOpenCL.BuildStatus.None"/>, an empty string is returned.
            /// </summary>
            public static readonly ProgramBuildParameterInfo<string> BuildLog =
                (ProgramBuildParameterInfo<string>)new ParameterInfoString(0x1183);

            /// <summary>
            /// Return the program binary type for device. This can be one of the following values:
            ///
            /// <list type="bullet">
            /// <item><see cref="NOpenCL.BinaryType.None"/>. There is no binary associated with device.</item>
            /// <item><see cref="NOpenCL.BinaryType.CompiledObject"/>. A compiled binary is associated with device. This is the case if program was created using <see cref="clCreateProgramWithSource"/> and compiled using <see cref="clCompileProgram"/> or a compiled binary is loaded using <see cref="clCreateProgramWithBinary"/>.</item>
            /// <item><see cref="NOpenCL.BinaryType.Library"/>. A library binary is associated with device. This is the case if program was created by <see cref="clLinkProgram"/> which is called with the <c>–createlibrary</c> link option or if a library binary is loaded using <see cref="clCreateProgramWithBinary"/>.</item>
            /// <item><see cref="NOpenCL.BinaryType.Executable"/>. An executable binary is associated with device. This is the case if program was created by <see cref="clLinkProgram"/> without the <c>–createlibrary</c> link option or program was created by <see cref="clBuildProgram"/> or an executable binary is loaded using <see cref="clCreateProgramWithBinary"/>.</item>
            /// </list>
            /// </summary>
            public static readonly ProgramBuildParameterInfo<uint> BinaryType =
                (ProgramBuildParameterInfo<uint>)new ParameterInfoUInt32(0x1184);
        }

        public sealed class ProgramBuildParameterInfo<T>
        {
            private readonly ParameterInfo<T> _parameterInfo;

            public ProgramBuildParameterInfo(ParameterInfo<T> parameterInfo)
            {
                if (parameterInfo == null)
                    throw new ArgumentNullException("parameterInfo");

                _parameterInfo = parameterInfo;
            }

            public static explicit operator ProgramBuildParameterInfo<T>(ParameterInfo<T> parameterInfo)
            {
                return new ProgramBuildParameterInfo<T>(parameterInfo);
            }

            public ParameterInfo<T> ParameterInfo
            {
                get
                {
                    return _parameterInfo;
                }
            }
        }

        #endregion
    }
}
