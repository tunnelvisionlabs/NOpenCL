// Copyright (c) Tunnel Vision Laboratories, LLC. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

#pragma warning disable IDE1006 // Naming Styles

namespace NOpenCL
{
    using System;
    using System.Runtime.InteropServices;
    using NOpenCL.SafeHandles;

    /// <summary>
    /// Contexts.
    /// </summary>
    internal partial class UnsafeNativeMethods
    {
        private static class ContextProperties
        {
            /// <summary>
            /// Specifies the platform to use.
            /// </summary>
            internal static readonly IntPtr Platform = (IntPtr)0x1084;

            /// <summary>
            /// Specifies whether the user is responsible for synchronization between OpenCL and
            /// other APIs. Please refer to the specific sections in the OpenCL 1.2 extension
            /// specification that describe sharing with other APIs for restrictions on using
            /// this flag. If <see cref="InteropUserSync"/> is not specified, a default of
            /// <c>false</c> is assumed.
            /// </summary>
            internal static readonly IntPtr InteropUserSync = (IntPtr)0x1085;
        }

        [DllImport(ExternDll.OpenCL)]
        private static extern ContextSafeHandle clCreateContext(
            [In, MarshalAs(UnmanagedType.LPArray)] IntPtr[] properties,
            uint numDevices,
            [In, MarshalAs(UnmanagedType.LPArray)] ClDeviceID[] devices,
            CreateContextCallback pfnNotify,
            IntPtr userData,
            out ErrorCode errorCode);

        public static ContextSafeHandle CreateContext(ClDeviceID[] devices, CreateContextCallback pfnNotify, IntPtr userData)
        {
            ErrorCode errorCode = ErrorCode.Success;
            ContextSafeHandle result = clCreateContext(null, (uint)devices.Length, devices, pfnNotify, userData, out errorCode);
            ErrorHandler.ThrowOnFailure(errorCode);
            return result;
        }

        public static ContextSafeHandle CreateContext(ClPlatformID platform, ClDeviceID[] devices, CreateContextCallback pfnNotify, IntPtr userData)
        {
            IntPtr[] properties = { ContextProperties.Platform, platform.Handle, IntPtr.Zero };
            ErrorCode errorCode = ErrorCode.Success;
            ContextSafeHandle result = clCreateContext(properties, (uint)devices.Length, devices, pfnNotify, userData, out errorCode);
            ErrorHandler.ThrowOnFailure(errorCode);
            return result;
        }

        [DllImport(ExternDll.OpenCL)]
        private static extern ContextSafeHandle clCreateContextFromType(
            [In, MarshalAs(UnmanagedType.LPArray)] IntPtr[] properties,
            DeviceType deviceType,
            CreateContextCallback pfnNotify,
            IntPtr userData,
            out ErrorCode errorCode);

        public static ContextSafeHandle CreateContextFromType(DeviceType deviceType, CreateContextCallback pfnNotify, IntPtr userData)
        {
            ErrorCode errorCode = ErrorCode.Success;
            ContextSafeHandle result = clCreateContextFromType(null, deviceType, pfnNotify, userData, out errorCode);
            ErrorHandler.ThrowOnFailure(errorCode);
            return result;
        }

        public static ContextSafeHandle CreateContextFromType(ClPlatformID platform, DeviceType deviceType, CreateContextCallback pfnNotify, IntPtr userData)
        {
            IntPtr[] properties = { ContextProperties.Platform, platform.Handle, IntPtr.Zero };

            ErrorCode errorCode = ErrorCode.Success;
            ContextSafeHandle result = clCreateContextFromType(properties, deviceType, pfnNotify, userData, out errorCode);
            ErrorHandler.ThrowOnFailure(errorCode);
            return result;
        }

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate void CreateContextCallback([MarshalAs(UnmanagedType.LPStr)] string errorInfo, IntPtr privateInfo, UIntPtr cb, IntPtr userData);

        [DllImport(ExternDll.OpenCL)]
        private static extern ErrorCode clRetainContext(ContextSafeHandle context);

        [DllImport(ExternDll.OpenCL)]
        public static extern ErrorCode clReleaseContext(IntPtr context);

        [DllImport(ExternDll.OpenCL)]
        private static extern ErrorCode clGetContextInfo(
            ContextSafeHandle context,
            int paramName,
            UIntPtr paramValueSize,
            IntPtr paramValue,
            out UIntPtr paramValueSizeRet);

        public static T GetContextInfo<T>(ContextSafeHandle context, ContextParameterInfo<T> parameter)
        {
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
                ErrorHandler.ThrowOnFailure(clGetContextInfo(context, parameter.ParameterInfo.Name, UIntPtr.Zero, IntPtr.Zero, out requiredSize));

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
                ErrorHandler.ThrowOnFailure(clGetContextInfo(context, parameter.ParameterInfo.Name, requiredSize, memory, out actualSize));
                return parameter.ParameterInfo.Deserialize(actualSize, memory);
            }
            finally
            {
                Marshal.FreeHGlobal(memory);
            }
        }

        public static class ContextInfo
        {
            /// <summary>
            /// Return the context reference count. The reference count returned should be considered
            /// immediately stale. It is unsuitable for general use in applications. This feature is
            /// provided for identifying memory leaks.
            /// </summary>
            public static readonly ContextParameterInfo<uint> ReferenceCount = (ContextParameterInfo<uint>)new ParameterInfoUInt32(0x1080);

            /// <summary>
            /// Return the number of devices in context.
            /// </summary>
            public static readonly ContextParameterInfo<uint> NumDevices = (ContextParameterInfo<uint>)new ParameterInfoUInt32(0x1083);

            /// <summary>
            /// Return the list of devices in context.
            /// </summary>
            public static readonly ContextParameterInfo<IntPtr[]> Devices = (ContextParameterInfo<IntPtr[]>)new ParameterInfoIntPtrArray(0x1081);

            /// <summary>
            /// Return the properties argument specified in <see cref="clCreateContext"/> or
            /// <see cref="clCreateContextFromType"/>.
            /// <para/>
            /// If the properties argument specified in <see cref="clCreateContext"/> or
            /// <see cref="clCreateContextFromType"/> used to create context is not null,
            /// the implementation must return the values specified in the properties argument.
            /// <para/>
            /// If the properties argument specified in <see cref="clCreateContext"/> or
            /// <see cref="clCreateContextFromType"/> used to create context is null, the
            /// implementation may return either a <em>paramValueSizeRet</em> of 0, i.e. there
            /// is no context property value to be returned or can return a context property
            /// value of 0 (where 0 is used to terminate the context properties list) in the
            /// memory that <em>paramValue</em> points to.
            /// </summary>
            public static readonly ContextParameterInfo<IntPtr[]> Properties = (ContextParameterInfo<IntPtr[]>)new ParameterInfoIntPtrArray(0x1082);
        }

        public sealed class ContextParameterInfo<T>
        {
            private readonly ParameterInfo<T> _parameterInfo;

            public ContextParameterInfo(ParameterInfo<T> parameterInfo)
            {
                if (parameterInfo == null)
                    throw new ArgumentNullException(nameof(parameterInfo));

                _parameterInfo = parameterInfo;
            }

            public static explicit operator ContextParameterInfo<T>(ParameterInfo<T> parameterInfo)
            {
                return new ContextParameterInfo<T>(parameterInfo);
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
