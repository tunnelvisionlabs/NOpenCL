// Copyright (c) Tunnel Vision Laboratories, LLC. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

#pragma warning disable IDE1006 // Naming Styles

namespace NOpenCL
{
    using System;
    using System.Runtime.InteropServices;
    using NOpenCL.SafeHandles;

    /// <content>
    /// Program objects.
    /// </content>
    internal partial class UnsafeNativeMethods
    {
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
                throw new ArgumentNullException(nameof(context));
            if (strings == null)
                throw new ArgumentNullException(nameof(strings));

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

        [DllImport(ExternDll.OpenCL)]
        private static extern ErrorCode clCompileProgram(
            ProgramSafeHandle program,
            uint numDevices,
            [In, MarshalAs(UnmanagedType.LPArray)] ClDeviceID[] devices,
            string options,
            uint numInputHeaders,
            [In, MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(SafeHandleArrayMarshaler))] ProgramSafeHandle[] inputHeaders,
            [In, MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.LPStr)] string[] headerIncludeNames,
            BuildProgramCallback pfnNotify,
            IntPtr userData);

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

        public static T GetProgramInfo<T>(ProgramSafeHandle program, ProgramParameterInfo<T> parameter)
        {
            if (program == null)
                throw new ArgumentNullException(nameof(program));
            if (parameter == null)
                throw new ArgumentNullException(nameof(parameter));

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

        public static class ProgramInfo
        {
            public static readonly ProgramParameterInfo<uint> ReferenceCount = (ProgramParameterInfo<uint>)new ParameterInfoUInt32(0x1160);
            public static readonly ProgramParameterInfo<IntPtr> Context = (ProgramParameterInfo<IntPtr>)new ParameterInfoIntPtr(0x1161);
            public static readonly ProgramParameterInfo<uint> NumDevices = (ProgramParameterInfo<uint>)new ParameterInfoUInt32(0x1162);
            public static readonly ProgramParameterInfo<IntPtr[]> Devices = (ProgramParameterInfo<IntPtr[]>)new ParameterInfoIntPtrArray(0x1163);
            public static readonly ProgramParameterInfo<string> Source = (ProgramParameterInfo<string>)new ParameterInfoString(0x1164);
            public static readonly ProgramParameterInfo<UIntPtr[]> BinarySizes = (ProgramParameterInfo<UIntPtr[]>)new ParameterInfoUIntPtrArray(0x1165);
            public static readonly ProgramParameterInfo<IntPtr[]> Binaries = (ProgramParameterInfo<IntPtr[]>)new ParameterInfoIntPtrArray(0x1166);
            public static readonly ProgramParameterInfo<IntPtr> NumKernels = (ProgramParameterInfo<IntPtr>)new ParameterInfoIntPtr(0x1167);
            public static readonly ProgramParameterInfo<string> KernelNames = (ProgramParameterInfo<string>)new ParameterInfoString(0x1168);
        }

        public sealed class ProgramParameterInfo<T>
        {
            private readonly ParameterInfo<T> _parameterInfo;

            public ProgramParameterInfo(ParameterInfo<T> parameterInfo)
            {
                if (parameterInfo == null)
                    throw new ArgumentNullException(nameof(parameterInfo));

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
                throw new ArgumentNullException(nameof(program));
            if (parameter == null)
                throw new ArgumentNullException(nameof(parameter));

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
                    throw new ArgumentNullException(nameof(parameterInfo));

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
    }
}
