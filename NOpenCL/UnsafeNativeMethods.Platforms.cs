// Copyright (c) Tunnel Vision Laboratories, LLC. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

#pragma warning disable IDE1006 // Naming Styles

namespace NOpenCL
{
    using System;
    using System.Runtime.InteropServices;

    /// <content>
    /// Platforms.
    /// </content>
    internal partial class UnsafeNativeMethods
    {
        [DllImport(ExternDll.OpenCL)]
        private static extern ErrorCode clGetPlatformIDs(
            uint numEntries,
            [Out, MarshalAs(UnmanagedType.LPArray)] ClPlatformID[] platforms,
            out uint numPlatforms);

        public static ClPlatformID[] GetPlatformIDs()
        {
            uint required;
            ErrorHandler.ThrowOnFailure(clGetPlatformIDs(0, null, out required));
            if (required == 0)
                return new ClPlatformID[0];

            ClPlatformID[] platforms = new ClPlatformID[required];
            uint actual;
            ErrorHandler.ThrowOnFailure(clGetPlatformIDs(required, platforms, out actual));
            Array.Resize(ref platforms, (int)actual);
            return platforms;
        }

        [DllImport(ExternDll.OpenCL)]
        private static extern ErrorCode clGetPlatformInfo(
            ClPlatformID platform,
            int paramName,
            UIntPtr paramValueSize,
            IntPtr paramValue,
            out UIntPtr paramValueSizeRet);

        public static T GetPlatformInfo<T>(ClPlatformID platform, PlatformParameterInfo<T> parameter)
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
                ErrorHandler.ThrowOnFailure(clGetPlatformInfo(platform, parameter.ParameterInfo.Name, UIntPtr.Zero, IntPtr.Zero, out requiredSize));

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
                ErrorHandler.ThrowOnFailure(clGetPlatformInfo(platform, parameter.ParameterInfo.Name, requiredSize, memory, out actualSize));
                return parameter.ParameterInfo.Deserialize(actualSize, memory);
            }
            finally
            {
                Marshal.FreeHGlobal(memory);
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct ClPlatformID
        {
            internal readonly IntPtr Handle;
        }

        public static class PlatformInfo
        {
            /// <summary>
            /// OpenCL profile string. Returns the profile name supported by the implementation.
            /// The profile name returned can be one of the following strings:
            ///
            /// <list type="bullet">
            /// <item>FULL_PROFILE - if the implementation supports the OpenCL specification
            /// (functionality defined as part of the core specification and does not require
            /// any extensions to be supported).</item>
            /// <item>EMBEDDED_PROFILE - if the implementation supports the OpenCL embedded
            /// profile. The embedded profile is defined to be a subset for each version of
            /// OpenCL.</item>
            /// </list>
            /// </summary>
            public static readonly PlatformParameterInfo<string> Profile = (PlatformParameterInfo<string>)new ParameterInfoString(0x0900);

            /// <summary>
            /// OpenCL version string. Returns the OpenCL version supported by the implementation.
            /// This version string has the following format:
            /// <para />
            /// OpenCL<em>&lt;space&gt;</em><em>&lt;major_version.minor_version&gt;</em><em>&lt;space&gt;</em><em>&lt;platform-specific information&gt;</em>
            /// <para />
            /// The <em>major_version.minor_version</em> value returned will be 1.2.
            /// </summary>
            public static readonly PlatformParameterInfo<string> Version = (PlatformParameterInfo<string>)new ParameterInfoString(0x0901);

            /// <summary>
            /// Platform name string.
            /// </summary>
            public static readonly PlatformParameterInfo<string> Name = (PlatformParameterInfo<string>)new ParameterInfoString(0x0902);

            /// <summary>
            /// Platform vendor string.
            /// </summary>
            public static readonly PlatformParameterInfo<string> Vendor = (PlatformParameterInfo<string>)new ParameterInfoString(0x0903);

            /// <summary>
            /// Returns a space-separated list of extension names (the extension names themselves
            /// do not contain any spaces) supported by the platform. Extensions defined here must
            /// be supported by all devices associated with this platform.
            /// </summary>
            public static readonly PlatformParameterInfo<string> Extensions = (PlatformParameterInfo<string>)new ParameterInfoString(0x0904);
        }

        public sealed class PlatformParameterInfo<T>
        {
            private readonly ParameterInfo<T> _parameterInfo;

            public PlatformParameterInfo(ParameterInfo<T> parameterInfo)
            {
                if (parameterInfo == null)
                    throw new ArgumentNullException(nameof(parameterInfo));

                _parameterInfo = parameterInfo;
            }

            public static explicit operator PlatformParameterInfo<T>(ParameterInfo<T> parameterInfo)
            {
                return new PlatformParameterInfo<T>(parameterInfo);
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
