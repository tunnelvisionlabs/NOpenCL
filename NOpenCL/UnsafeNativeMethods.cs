namespace NOpenCL
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;

    internal static partial class UnsafeNativeMethods
    {
        #region Platforms

        [DllImport(ExternDll.OpenCL)]
        private static extern ErrorCode clGetPlatformIDs(uint numEntries, [Out, MarshalAs(UnmanagedType.LPArray)] ClPlatformID[] platforms, out uint numPlatforms);

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
        private static extern ErrorCode clGetPlatformInfo(ClPlatformID platform, int paramName, UIntPtr paramValueSize, IntPtr paramValue, out UIntPtr paramValueSizeRet);

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
            private readonly IntPtr _handle;
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
                    throw new ArgumentNullException("parameterInfo");

                _parameterInfo = parameterInfo;
            }

            public static explicit operator PlatformParameterInfo<T>(ParameterInfo<T> parameterInfo)
            {
                PlatformParameterInfo<T> result = parameterInfo as PlatformParameterInfo<T>;
                if (result != null)
                    return result;

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

        #endregion

        #region Devices

        [DllImport(ExternDll.OpenCL)]
        private static extern ErrorCode clGetDeviceIDs(ClPlatformID platform, DeviceType deviceType, uint numEntries, [Out, MarshalAs(UnmanagedType.LPArray)] ClDeviceID[] devices, out uint numDevices);

        public static ClDeviceID[] GetDeviceIDs(ClPlatformID platform, DeviceType deviceType)
        {
            uint required;
            ErrorHandler.ThrowOnFailure(clGetDeviceIDs(platform, deviceType, 0, null, out required));
            if (required == 0)
                return new ClDeviceID[0];

            ClDeviceID[] devices = new ClDeviceID[required];
            uint actual;
            ErrorHandler.ThrowOnFailure(clGetDeviceIDs(platform, deviceType, required, devices, out actual));
            Array.Resize(ref devices, (int)actual);
            return devices;
        }

        [DllImport(ExternDll.OpenCL)]
        private static extern ErrorCode clGetDeviceInfo(ClDeviceID device, int paramName, UIntPtr paramValueSize, IntPtr paramValue, out UIntPtr paramValueSizeRet);

        public static T GetDeviceInfo<T>(ClDeviceID device, DeviceParameterInfo<T> parameter)
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
                ErrorHandler.ThrowOnFailure(clGetDeviceInfo(device, parameter.ParameterInfo.Name, UIntPtr.Zero, IntPtr.Zero, out requiredSize));

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
                ErrorHandler.ThrowOnFailure(clGetDeviceInfo(device, parameter.ParameterInfo.Name, requiredSize, memory, out actualSize));
                return parameter.ParameterInfo.Deserialize(actualSize, memory);
            }
            finally
            {
                Marshal.FreeHGlobal(memory);
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct ClDeviceID
        {
            private readonly IntPtr _handle;

            public IntPtr Handle
            {
                get
                {
                    return _handle;
                }
            }
        }

        public static class DeviceInfo
        {
            /// <summary>
            /// The default compute device address space size specified as an unsigned integer
            /// value in bits. Currently supported values are 32 or 64 bits.
            /// </summary>
            public static readonly DeviceParameterInfo<uint> AddressBits = (DeviceParameterInfo<uint>)new ParameterInfoUInt32(0x100D);

            /// <summary>
            /// Is true if the device is available and false if the device is not available.
            /// </summary>
            public static readonly DeviceParameterInfo<bool> Available = (DeviceParameterInfo<bool>)new ParameterInfoBoolean(0x1027);

            /// <summary>
            /// A semi-colon separated list of built-in kernels supported by the device. An
            /// empty string is returned if no built-in kernels are supported by the device.
            /// </summary>
            public static readonly DeviceParameterInfo<string> BuiltInKernels = (DeviceParameterInfo<string>)new ParameterInfoString(0x103F);

            /// <summary>
            /// Is false if the implementation does not have a compiler available to compile
            /// the program source. Is true if the compiler is available. This can be false
            /// for the embedded platform profile only.
            /// </summary>
            public static readonly DeviceParameterInfo<bool> CompilerAvailable = (DeviceParameterInfo<bool>)new ParameterInfoBoolean(0x1028);

            /// <summary>
            /// Describes double precision floating-point capability of the OpenCL device.
            /// This is a bit-field that describes one or more of the following values:
            ///
            /// <list type="bullet">
            /// <item><see cref="FloatingPointConfiguration.Denorm"/> - denorms are supported.</item>
            /// <item><see cref="FloatingPointConfiguration.InfNaN"/> - INF and NaNs are supported.</item>
            /// <item><see cref="FloatingPointConfiguration.RoundToNearest"/> - round to nearest even rounding mode supported.</item>
            /// <item><see cref="FloatingPointConfiguration.RoundToZero"/> - round to zero rounding mode supported.</item>
            /// <item><see cref="FloatingPointConfiguration.RoundToInf"/> - round to positive and negative infinity rounding modes supported.</item>
            /// <item><see cref="FloatingPointConfiguration.Fma"/> - IEEE754-2008 fused multiply-add is supported.</item>
            /// <item><see cref="FloatingPointConfiguration.SoftFloat"/> - Basic floating-point operations (such as addition, subtraction, multiplication) are implemented in software.</item>
            /// </list>
            /// <para/>
            /// Double precision is an optional feature so the mandated minimum double
            /// precision floating-point capability is 0.
            /// <para/>
            /// If double precision is supported by the device, then the minimum double
            /// precision floatingpoint capability must be:
            /// <c><see cref="FloatingPointConfiguration.Fma"/> | <see cref="FloatingPointConfiguration.RoundToNearest"/> | <see cref="FloatingPointConfiguration.RoundToZero"/> | <see cref="FloatingPointConfiguration.RoundToInf"/> | <see cref="FloatingPointConfiguration.InfNaN"/> | <see cref="FloatingPointConfiguration.Denorm"/></c>
            /// .
            /// </summary>
            public static readonly DeviceParameterInfo<ulong> DoubleFloatingPointConfiguration = (DeviceParameterInfo<ulong>)new ParameterInfoUInt64(0x1032);

            /// <summary>
            /// Is true if the OpenCL device is a little endian device and false otherwise.
            /// </summary>
            public static readonly DeviceParameterInfo<bool> LittleEndian = (DeviceParameterInfo<bool>)new ParameterInfoBoolean(0x1026);

            /// <summary>
            /// Is true if the device implements error correction for all accesses to compute
            /// device memory (global and constant). Is false if the device does not implement
            /// such error correction.
            /// </summary>
            public static readonly DeviceParameterInfo<bool> ErrorCorrectionSupport = (DeviceParameterInfo<bool>)new ParameterInfoBoolean(0x1024);

            /// <summary>
            /// Describes the execution capabilities of the device. This is a bit-field that
            /// describes one or more of the following values:
            ///
            /// <list type="bullet">
            /// <item><see cref="NOpenCL.ExecutionCapabilities.Kernel"/> - The OpenCL device can execute OpenCL kernels.</item>
            /// <item><see cref="NOpenCL.ExecutionCapabilities.NativeKernel"/> - The OpenCL device can execute native kernels.</item>
            /// </list>
            ///
            /// The mandated minimum capability is <see cref="NOpenCL.ExecutionCapabilities.Kernel"/>.
            /// </summary>
            public static readonly DeviceParameterInfo<ulong> ExecutionCapabilities = (DeviceParameterInfo<ulong>)new ParameterInfoUInt64(0x1029);

            /// <summary>
            /// Returns a space separated list of extension names (the extension names
            /// themselves do not contain any spaces) supported by the device. The list
            /// of extension names returned can be vendor supported extension names and
            /// one or more of the following Khronos approved extension names:
            ///
            /// <list type="bullet">
            /// <item>cl_khr_int64_base_atomics</item>
            /// <item>cl_khr_int64_extended_atomics</item>
            /// <item>cl_khr_fp16</item>
            /// <item>cl_khr_gl_sharing</item>
            /// <item>cl_khr_gl_event</item>
            /// <item>cl_khr_d3d10_sharing</item>
            /// <item>cl_khr_dx9_media_sharing</item>
            /// <item>cl_khr_d3d11_sharing</item>
            /// </list>
            /// </summary>
            public static readonly DeviceParameterInfo<string> Extensions = (DeviceParameterInfo<string>)new ParameterInfoString(0x1030);

            /// <summary>
            /// Size of global memory cache in bytes.
            /// </summary>
            public static readonly DeviceParameterInfo<ulong> GlobalCacheSize = (DeviceParameterInfo<ulong>)new ParameterInfoUInt64(0x101E);

            /// <summary>
            /// Type of global memory cache supported. Valid values are:
            /// <see cref="CacheType.None"/>, <see cref="CacheType.ReadOnly"/>,
            /// and <see cref="CacheType.ReadWrite"/>.
            /// </summary>
            public static readonly DeviceParameterInfo<uint> GlobalCacheType = (DeviceParameterInfo<uint>)new ParameterInfoUInt32(0x101C);

            /// <summary>
            /// Size of global memory cache line in bytes.
            /// </summary>
            public static readonly DeviceParameterInfo<uint> GlobalCacheLineSize = (DeviceParameterInfo<uint>)new ParameterInfoUInt32(0x101D);

            /// <summary>
            /// Size of global device memory in bytes.
            /// </summary>
            public static readonly DeviceParameterInfo<ulong> GlobalMemorySize = (DeviceParameterInfo<ulong>)new ParameterInfoUInt64(0x101F);

            /// <summary>
            /// Describes the OPTIONAL half precision floating-point capability of the
            /// OpenCL device. This is a bit-field that describes one or more of the
            /// following values:
            ///
            /// <list type="bullet">
            /// <item><see cref="FloatingPointConfiguration.Denorm"/> - denorms are supported.</item>
            /// <item><see cref="FloatingPointConfiguration.InfNaN"/> - INF and NaNs are supported.</item>
            /// <item><see cref="FloatingPointConfiguration.RoundToNearest"/> - round to nearest even rounding mode supported.</item>
            /// <item><see cref="FloatingPointConfiguration.RoundToZero"/> - round to zero rounding mode supported.</item>
            /// <item><see cref="FloatingPointConfiguration.RoundToInf"/> - round to +ve and -ve infinity rounding modes supported.</item>
            /// <item><see cref="FloatingPointConfiguration.Fma"/> - IEEE754-2008 fused multiply-add is supported.</item>
            /// <item><see cref="FloatingPointConfiguration.SoftFloat"/> - Basic floating-point operations (such as addition, subtraction, multiplication) are implemented in software.</item>
            /// </list>
            /// <para/>
            /// The required minimum half precision floating-point capability as implemented by this extension is
            /// <c><see cref="FloatingPointConfiguration.RoundToZero"/></c> or <c><see cref="FloatingPointConfiguration.RoundToInf"/> | <see cref="FloatingPointConfiguration.InfNaN"/></c>
            /// .
            /// </summary>
            public static readonly DeviceParameterInfo<ulong> HalfFloatingPointConfiguration = (DeviceParameterInfo<ulong>)new ParameterInfoUInt64(0x1033);

            /// <summary>
            /// Is true if the device and the host have a unified memory subsystem and is false otherwise.
            /// </summary>
            public static readonly DeviceParameterInfo<bool> HostUnifiedMemory = (DeviceParameterInfo<bool>)new ParameterInfoBoolean(0x1035);

            /// <summary>
            /// Is true if images are supported by the OpenCL device and false otherwise.
            /// </summary>
            public static readonly DeviceParameterInfo<bool> ImageSupport = (DeviceParameterInfo<bool>)new ParameterInfoBoolean(0x1016);

            /// <summary>
            /// Max height of 2D image in pixels. The minimum value is 8192 if <see cref="ImageSupport"/> is true.
            /// </summary>
            public static readonly DeviceParameterInfo<UIntPtr> Image2DMaxHeight = (DeviceParameterInfo<UIntPtr>)new ParameterInfoUIntPtr(0x1012);

            /// <summary>
            /// Max width of 2D image or 1D image not created from a buffer object in pixels. The minimum value is 8192 if <see cref="ImageSupport"/> is true.
            /// </summary>
            public static readonly DeviceParameterInfo<UIntPtr> Image2DMaxWidth = (DeviceParameterInfo<UIntPtr>)new ParameterInfoUIntPtr(0x1011);

            /// <summary>
            /// Max depth of 3D image in pixels. The minimum value is 2048 if <see cref="ImageSupport"/> is true.
            /// </summary>
            public static readonly DeviceParameterInfo<UIntPtr> Image3DMaxDepth = (DeviceParameterInfo<UIntPtr>)new ParameterInfoUIntPtr(0x1015);

            /// <summary>
            /// Max height of 3D image in pixels. The minimum value is 2048 if <see cref="ImageSupport"/> is true.
            /// </summary>
            public static readonly DeviceParameterInfo<UIntPtr> Image3DMaxHeight = (DeviceParameterInfo<UIntPtr>)new ParameterInfoUIntPtr(0x1014);

            /// <summary>
            /// Max width of 3D image in pixels. The minimum value is 2048 if <see cref="ImageSupport"/> is true.
            /// </summary>
            public static readonly DeviceParameterInfo<UIntPtr> Image3DMaxWidth = (DeviceParameterInfo<UIntPtr>)new ParameterInfoUIntPtr(0x1013);

            /// <summary>
            /// Max number of pixels for a 1D image created from a buffer object. The minimum value is 65536 if <see cref="ImageSupport"/> is true.
            /// </summary>
            public static readonly DeviceParameterInfo<UIntPtr> ImageMaxBufferSize = (DeviceParameterInfo<UIntPtr>)new ParameterInfoUIntPtr(0x1040);

            /// <summary>
            /// Max number of images in a 1D or 2D image array. The minimum value is 2048 if <see cref="ImageSupport"/> is true.
            /// </summary>
            public static readonly DeviceParameterInfo<UIntPtr> ImageMaxArraySize = (DeviceParameterInfo<UIntPtr>)new ParameterInfoUIntPtr(0x1041);

            /// <summary>
            /// Is false if the implementation does not have a linker available. Is true
            /// if the linker is available. This can be false for the embedded platform
            /// profile only. This must be true if <see cref="CompilerAvailable"/> is true.
            /// </summary>
            public static readonly DeviceParameterInfo<bool> LinkerAvailable = (DeviceParameterInfo<bool>)new ParameterInfoBoolean(0x103E);

            /// <summary>
            /// Size of local memory arena in bytes. The minimum value is 32 KB for devices
            /// that are not of type <see cref="NOpenCL.DeviceType.Custom"/>.
            /// </summary>
            public static readonly DeviceParameterInfo<ulong> LocalMemorySize = (DeviceParameterInfo<ulong>)new ParameterInfoUInt64(0x1023);

            /// <summary>
            /// Type of local memory supported. This can be set to <see cref="NOpenCL.LocalMemoryType.Local"/>
            /// implying dedicated local memory storage such as SRAM, or <see cref="NOpenCL.LocalMemoryType.Global"/>.
            /// For custom devices, <see cref="NOpenCL.LocalMemoryType.None"/> can also be returned indicating
            /// no local memory support.
            /// </summary>
            public static readonly DeviceParameterInfo<uint> LocalMemoryType = (DeviceParameterInfo<uint>)new ParameterInfoUInt32(0x1022);

            /// <summary>
            /// Maximum configured clock frequency of the device in MHz.
            /// </summary>
            public static readonly DeviceParameterInfo<uint> MaxClockFrequency = (DeviceParameterInfo<uint>)new ParameterInfoUInt32(0x100C);

            /// <summary>
            /// The number of parallel compute units on the OpenCL device. A work-group
            /// executes on a single compute unit. The minimum value is 1.
            /// </summary>
            public static readonly DeviceParameterInfo<uint> MaxComputeUnits = (DeviceParameterInfo<uint>)new ParameterInfoUInt32(0x1002);

            /// <summary>
            /// Max number of arguments declared with the <c>__constant</c> qualifier in
            /// a kernel. The minimum value is 8 for devices that are not of type <see cref="NOpenCL.DeviceType.Custom"/>.
            /// </summary>
            public static readonly DeviceParameterInfo<uint> MaxConstantArguments = (DeviceParameterInfo<uint>)new ParameterInfoUInt32(0x1021);

            /// <summary>
            /// Max size in bytes of a constant buffer allocation. The minimum value is 64
            /// KB for devices that are not of type <see cref="NOpenCL.DeviceType.Custom"/>.
            /// </summary>
            public static readonly DeviceParameterInfo<ulong> MaxConstantBufferSize = (DeviceParameterInfo<ulong>)new ParameterInfoUInt64(0x1020);

            /// <summary>
            /// Max size of memory object allocation in bytes. The minimum value is max
            /// (1/4th of <see cref="GlobalMemorySize"/>, 128*1024*1024) for devices that are
            /// not of type <see cref="NOpenCL.DeviceType.Custom"/>.
            /// </summary>
            public static readonly DeviceParameterInfo<ulong> MaxMemoryAllocationSize = (DeviceParameterInfo<ulong>)new ParameterInfoUInt64(0x1010);

            /// <summary>
            /// Max size in bytes of the arguments that can be passed to a kernel. The
            /// minimum value is 1024 for devices that are not of type <see cref="NOpenCL.DeviceType.Custom"/>.
            /// For this minimum value, only a maximum of 128 arguments can be passed to a kernel.
            /// </summary>
            public static readonly DeviceParameterInfo<UIntPtr> MaxParameterSize = (DeviceParameterInfo<UIntPtr>)new ParameterInfoUIntPtr(0x1017);

            /// <summary>
            /// Max number of simultaneous image objects that can be read by a kernel.
            /// The minimum value is 128 if <see cref="ImageSupport"/> is true.
            /// </summary>
            public static readonly DeviceParameterInfo<uint> MaxReadImageArguments = (DeviceParameterInfo<uint>)new ParameterInfoUInt32(0x100E);

            /// <summary>
            /// Maximum number of samplers that can be used in a kernel. The minimum value
            /// is 16 if <see cref="ImageSupport"/> is true.
            /// </summary>
            /// <seealso cref="Sampler"/>
            public static readonly DeviceParameterInfo<uint> MaxSamplers = (DeviceParameterInfo<uint>)new ParameterInfoUInt32(0x1018);

            /// <summary>
            /// Maximum number of work-items in a work-group executing a kernel on a single
            /// compute unit, using the data parallel execution model.
            /// (Refer to <see cref="clEnqueueNDRangeKernel"/>). The minimum value is 1.
            /// </summary>
            public static readonly DeviceParameterInfo<UIntPtr> MaxWorkGroupSize = (DeviceParameterInfo<UIntPtr>)new ParameterInfoUIntPtr(0x1004);

            /// <summary>
            /// Maximum dimensions that specify the global and local work-item IDs used by
            /// the data parallel execution model. (Refer to <see cref="clEnqueueNDRangeKernel"/>). The
            /// minimum value is 3 for devices that are not of type <see cref="NOpenCL.DeviceType.Custom"/>.
            /// </summary>
            public static readonly DeviceParameterInfo<uint> MaxWorkItemDimensions = (DeviceParameterInfo<uint>)new ParameterInfoUInt32(0x1003);

            /// <summary>
            /// Maximum number of work-items that can be specified in each dimension of
            /// the work-group to <see cref="clEnqueueNDRangeKernel"/>.
            /// <para/>
            /// Returns <em>n</em> <see cref="IntPtr"/> entries, where <em>n</em> is the
            /// value returned by the query for <see cref="MaxWorkItemDimensions"/>.
            /// <para/>
            /// The minimum value is (1, 1, 1) for devices that are not of type <see cref="NOpenCL.DeviceType.Custom"/>.
            /// </summary>
            public static readonly DeviceParameterInfo<UIntPtr[]> MaxWorkItemSizes = (DeviceParameterInfo<UIntPtr[]>)new ParameterInfoUIntPtrArray(0x1005);

            /// <summary>
            /// Max number of simultaneous image objects that can be written to by a
            /// kernel. The minimum value is 8 if <see cref="ImageSupport"/> is true.
            /// </summary>
            public static readonly DeviceParameterInfo<uint> MaxWriteImageArguments = (DeviceParameterInfo<uint>)new ParameterInfoUInt32(0x100F);

            /// <summary>
            /// The minimum value is the size (in bits) of the largest OpenCL built-in
            /// data type supported by the device (long16 in FULL profile, long16 or
            /// int16 in EMBEDDED profile) for devices that are not of type <see cref="NOpenCL.DeviceType.Custom"/>.
            /// </summary>
            public static readonly DeviceParameterInfo<uint> MemoryBaseAddressAlignment = (DeviceParameterInfo<uint>)new ParameterInfoUInt32(0x1019);

            /// <summary>
            /// Deprecated in OpenCL 1.2. The smallest alignment in bytes which can be
            /// used for any data type.
            /// </summary>
            [Obsolete("Deprecated in OpenCL 1.2")]
            public static readonly DeviceParameterInfo<uint> MinDataTypeAlignment = (DeviceParameterInfo<uint>)new ParameterInfoUInt32(0x101A);

            /// <summary>
            /// Device name string.
            /// </summary>
            public static readonly DeviceParameterInfo<string> Name = (DeviceParameterInfo<string>)new ParameterInfoString(0x102B);

            /// <summary>
            /// Returns the native ISA vector width. The vector width is defined as
            /// the number of scalar elements that can be stored in the vector.
            /// </summary>
            public static readonly DeviceParameterInfo<uint> NativeVectorWidthChar = (DeviceParameterInfo<uint>)new ParameterInfoUInt32(0x1036);

            /// <summary>
            /// Returns the native ISA vector width. The vector width is defined as
            /// the number of scalar elements that can be stored in the vector.
            /// </summary>
            public static readonly DeviceParameterInfo<uint> NativeVectorWidthShort = (DeviceParameterInfo<uint>)new ParameterInfoUInt32(0x1037);

            /// <summary>
            /// Returns the native ISA vector width. The vector width is defined as
            /// the number of scalar elements that can be stored in the vector.
            /// </summary>
            public static readonly DeviceParameterInfo<uint> NativeVectorWidthInt = (DeviceParameterInfo<uint>)new ParameterInfoUInt32(0x1038);

            /// <summary>
            /// Returns the native ISA vector width. The vector width is defined as
            /// the number of scalar elements that can be stored in the vector.
            /// </summary>
            public static readonly DeviceParameterInfo<uint> NativeVectorWidthLong = (DeviceParameterInfo<uint>)new ParameterInfoUInt32(0x1039);

            /// <summary>
            /// Returns the native ISA vector width. The vector width is defined as
            /// the number of scalar elements that can be stored in the vector.
            /// </summary>
            public static readonly DeviceParameterInfo<uint> NativeVectorWidthFloat = (DeviceParameterInfo<uint>)new ParameterInfoUInt32(0x103A);

            /// <summary>
            /// Returns the native ISA vector width. The vector width is defined as
            /// the number of scalar elements that can be stored in the vector.
            /// <para/>
            /// If double precision is not supported, this must return 0.
            /// </summary>
            public static readonly DeviceParameterInfo<uint> NativeVectorWidthDouble = (DeviceParameterInfo<uint>)new ParameterInfoUInt32(0x103B);

            /// <summary>
            /// Returns the native ISA vector width. The vector width is defined as
            /// the number of scalar elements that can be stored in the vector.
            /// <para/>
            /// If the <c>cl_khr_fp16</c> extension is not supported, this must return 0.
            /// </summary>
            public static readonly DeviceParameterInfo<uint> NativeVectorWidthHalf = (DeviceParameterInfo<uint>)new ParameterInfoUInt32(0x103C);

            /// <summary>
            /// OpenCL C version string. Returns the highest OpenCL C version supported
            /// by the compiler for this device that is not of type <see cref="NOpenCL.DeviceType.Custom"/>.
            /// This version string has the following format:
            /// <para/>
            /// <em>OpenCL&lt;space&gt;C&lt;space&gt;&lt;major_version.minor_version&gt;&lt;space&gt;&lt;vendor-specific information&gt;</em>
            /// <para/>
            /// The <em>major_version.minor_version</em> value returned must be 1.2 if <see cref="Version"/> is OpenCL 1.2.
            /// <para/>
            /// The <em>major_version.minor_version</em> value returned must be 1.1 if <see cref="Version"/> is OpenCL 1.1.
            /// </summary>
            public static readonly DeviceParameterInfo<string> OpenCLVersion = (DeviceParameterInfo<string>)new ParameterInfoString(0x103D);

            /// <summary>
            /// Returns the <see cref="ClDeviceID"/> of the parent device to which
            /// this sub-device belongs. If device is a root-level device,
            /// <see cref="IntPtr.Zero"/> is returned.
            /// </summary>
            public static readonly DeviceParameterInfo<IntPtr> ParentDevice = (DeviceParameterInfo<IntPtr>)new ParameterInfoIntPtr(0x1042);

            /// <summary>
            /// Returns the maximum number of sub-devices that can be created when a
            /// device is partitioned. The value returned cannot exceed <see cref="MaxComputeUnits"/>.
            /// </summary>
            public static readonly DeviceParameterInfo<uint> PartitionMaxSubDevices = (DeviceParameterInfo<uint>)new ParameterInfoUInt32(0x1043);

            /// <summary>
            /// Returns the list of partition types supported by device. This is an array
            /// of <see cref="PartitionProperty"/> values drawn from the following list:
            ///
            /// <list type="bullet">
            /// <item><see cref="PartitionProperty.PartitionEqually"/></item>
            /// <item><see cref="PartitionProperty.PartitionByCounts"/></item>
            /// <item><see cref="PartitionProperty.PartitionByAffinityDomain"/></item>
            /// </list>
            ///
            /// If the device does not support any partition types, a value of 0 will be returned.
            /// </summary>
            public static readonly DeviceParameterInfo<IntPtr[]> PartitionProperties = (DeviceParameterInfo<IntPtr[]>)new ParameterInfoIntPtrArray(0x1044);

            /// <summary>
            /// Returns the list of supported affinity domains for partitioning the device
            /// using <see cref="PartitionProperty.PartitionByAffinityDomain"/>. This is a
            /// bit-field that describes one or more of the following values:
            ///
            /// <list type="bullet">
            /// <item><see cref="AffinityDomain.Numa"/></item>
            /// <item><see cref="AffinityDomain.L4Cache"/></item>
            /// <item><see cref="AffinityDomain.L3Cache"/></item>
            /// <item><see cref="AffinityDomain.L2Cache"/></item>
            /// <item><see cref="AffinityDomain.L1Cache"/></item>
            /// <item><see cref="AffinityDomain.NextPartitionable"/></item>
            /// </list>
            ///
            /// If the device does not support any affinity domains, <see cref="AffinityDomain.None"/> will be returned.
            /// </summary>
            public static readonly DeviceParameterInfo<ulong> PartitionAffinityDomain = (DeviceParameterInfo<ulong>)new ParameterInfoUInt64(0x1045);

            /// <summary>
            /// Returns the properties argument specified in <see cref="clCreateSubDevices"/>
            /// if device is a subdevice. Otherwise the implementation may either return a
            /// <em>param_value_size_ret</em> of 0 i.e. there is no partition type associated
            /// with device or can return a property value of 0 (where 0 is used to terminate
            /// the partition property list) in the memory that <em>param_value</em> points to.
            /// </summary>
            public static readonly DeviceParameterInfo<IntPtr[]> PartitionType = (DeviceParameterInfo<IntPtr[]>)new ParameterInfoIntPtrArray(0x1046);

            /// <summary>
            /// The platform associated with this device.
            /// </summary>
            public static readonly DeviceParameterInfo<IntPtr> Platform = (DeviceParameterInfo<IntPtr>)new ParameterInfoIntPtr(0x1031);

            /// <summary>
            /// Preferred native vector width size for built-in scalar types that can be put
            /// into vectors. The vector width is defined as the number of scalar elements
            /// that can be stored in the vector.
            /// </summary>
            public static readonly DeviceParameterInfo<uint> PreferredVectorWidthChar = (DeviceParameterInfo<uint>)new ParameterInfoUInt32(0x1006);

            /// <summary>
            /// Preferred native vector width size for built-in scalar types that can be put
            /// into vectors. The vector width is defined as the number of scalar elements
            /// that can be stored in the vector.
            /// </summary>
            public static readonly DeviceParameterInfo<uint> PreferredVectorWidthShort = (DeviceParameterInfo<uint>)new ParameterInfoUInt32(0x1007);

            /// <summary>
            /// Preferred native vector width size for built-in scalar types that can be put
            /// into vectors. The vector width is defined as the number of scalar elements
            /// that can be stored in the vector.
            /// </summary>
            public static readonly DeviceParameterInfo<uint> PreferredVectorWidthInt = (DeviceParameterInfo<uint>)new ParameterInfoUInt32(0x1008);

            /// <summary>
            /// Preferred native vector width size for built-in scalar types that can be put
            /// into vectors. The vector width is defined as the number of scalar elements
            /// that can be stored in the vector.
            /// </summary>
            public static readonly DeviceParameterInfo<uint> PreferredVectorWidthLong = (DeviceParameterInfo<uint>)new ParameterInfoUInt32(0x1009);

            /// <summary>
            /// Preferred native vector width size for built-in scalar types that can be put
            /// into vectors. The vector width is defined as the number of scalar elements
            /// that can be stored in the vector.
            /// </summary>
            public static readonly DeviceParameterInfo<uint> PreferredVectorWidthFloat = (DeviceParameterInfo<uint>)new ParameterInfoUInt32(0x100A);

            /// <summary>
            /// Preferred native vector width size for built-in scalar types that can be put
            /// into vectors. The vector width is defined as the number of scalar elements
            /// that can be stored in the vector.
            /// <para/>
            /// If double precision is not supported, this must return 0.
            /// </summary>
            public static readonly DeviceParameterInfo<uint> PreferredVectorWidthDouble = (DeviceParameterInfo<uint>)new ParameterInfoUInt32(0x100B);

            /// <summary>
            /// Preferred native vector width size for built-in scalar types that can be put
            /// into vectors. The vector width is defined as the number of scalar elements
            /// that can be stored in the vector.
            /// <para/>
            /// If the <c>cl_khr_fp16</c> extension is not supported, this must return 0.
            /// </summary>
            public static readonly DeviceParameterInfo<uint> PreferredVectorWidthHalf = (DeviceParameterInfo<uint>)new ParameterInfoUInt32(0x1034);

            /// <summary>
            /// Maximum size of the internal buffer that holds the output of printf calls from
            /// a kernel. The minimum value for the FULL profile is 1 MB.
            /// </summary>
            public static readonly DeviceParameterInfo<UIntPtr> PrintfBufferSize = (DeviceParameterInfo<UIntPtr>)new ParameterInfoUIntPtr(0x1049);

            /// <summary>
            /// Is true if the device's preference is for the user to be responsible for
            /// synchronization, when sharing memory objects between OpenCL and other APIs
            /// such as DirectX, false if the device / implementation has a performant path
            /// for performing synchronization of memory object shared between OpenCL and
            /// other APIs such as DirectX.
            /// </summary>
            public static readonly DeviceParameterInfo<bool> PreferredInteropUserSync = (DeviceParameterInfo<bool>)new ParameterInfoBoolean(0x1048);

            /// <summary>
            /// OpenCL profile string. Returns the profile name supported by the device
            /// (see note). The profile name returned can be one of the following strings:
            ///
            /// <list type="bullet">
            /// <item>FULL_PROFILE - if the device supports the OpenCL specification (functionality defined as part of the core specification and does not require any extensions to be supported).</item>
            /// <item>EMBEDDED_PROFILE - if the device supports the OpenCL embedded profile.</item>
            /// </list>
            /// </summary>
            public static readonly DeviceParameterInfo<string> Profile = (DeviceParameterInfo<string>)new ParameterInfoString(0x102E);

            /// <summary>
            /// Describes the resolution of device timer. This is measured in nanoseconds.
            /// </summary>
            public static readonly DeviceParameterInfo<UIntPtr> ProfilingTimerResolution = (DeviceParameterInfo<UIntPtr>)new ParameterInfoUIntPtr(0x1025);

            /// <summary>
            /// Describes the command-queue properties supported by the device. This is
            /// a bit-field that describes one or more of the following values:
            ///
            /// <list type="bullet">
            /// <item><see cref="CommandQueueProperties.OutOfOrderExecutionModeEnable"/></item>
            /// <item><see cref="CommandQueueProperties.ProfilingEnable"/></item>
            /// </list>
            ///
            /// These properties are described in the table for <see cref="clCreateCommandQueue"/>.
            /// The mandated minimum capability is <see cref="CommandQueueProperties.ProfilingEnable"/>.
            /// </summary>
            public static readonly DeviceParameterInfo<ulong> QueueProperties = (DeviceParameterInfo<ulong>)new ParameterInfoUInt64(0x102A);

            /// <summary>
            /// Returns the device reference count. If the device is a root-level device,
            /// a reference count of one is returned.
            /// </summary>
            public static readonly DeviceParameterInfo<uint> ReferenceCount = (DeviceParameterInfo<uint>)new ParameterInfoUInt32(0x1047);

            /// <summary>
            /// Describes single precision floating-point capability of the device.
            /// This is a bit-field that describes one or more of the following values:
            ///
            /// <list type="bullet">
            /// <item><see cref="FloatingPointConfiguration.Denorm"/> - denorms are supported.</item>
            /// <item><see cref="FloatingPointConfiguration.InfNaN"/> - INF and quiet NaNs are supported.</item>
            /// <item><see cref="FloatingPointConfiguration.RoundToNearest"/> - round to nearest even rounding mode supported.</item>
            /// <item><see cref="FloatingPointConfiguration.RoundToZero"/> - round to zero rounding mode supported.</item>
            /// <item><see cref="FloatingPointConfiguration.RoundToInf"/> - round to +ve and -ve infinity rounding modes supported.</item>
            /// <item><see cref="FloatingPointConfiguration.Fma"/> - IEEE754-2008 fused multiply-add is supported.</item>
            /// <item><see cref="FloatingPointConfiguration.CorrectlyRoundedDivideSqrt"/> - divide and sqrt are correctly rounded as defined by the IEEE754 specification.</item>
            /// <item><see cref="FloatingPointConfiguration.SoftFloat"/> - Basic floating-point operations (such as addition, subtraction, multiplication) are implemented in software.</item>
            /// </list>
            ///
            /// The mandated minimum floating-point capability for devices that are
            /// not of type <see cref="NOpenCL.DeviceType.Custom"/> is
            /// <see cref="FloatingPointConfiguration.RoundToNearest"/> |
            /// <see cref="FloatingPointConfiguration.InfNaN"/>.
            /// </summary>
            public static readonly DeviceParameterInfo<ulong> SingleFloatingPointConfiguration = (DeviceParameterInfo<ulong>)new ParameterInfoUInt64(0x101B);

            /// <summary>
            /// The OpenCL device type. Currently supported values are one of or a
            /// combination of: <see cref="NOpenCL.DeviceType.Cpu"/>, <see cref="NOpenCL.DeviceType.Gpu"/>,
            /// <see cref="NOpenCL.DeviceType.Accelerator"/>, <see cref="NOpenCL.DeviceType.Default"/>, a
            /// combination of the above types, or <see cref="NOpenCL.DeviceType.Custom"/>.
            /// </summary>
            public static readonly DeviceParameterInfo<ulong> DeviceType = (DeviceParameterInfo<ulong>)new ParameterInfoUInt64(0x1000);

            /// <summary>
            /// Vendor name string.
            /// </summary>
            public static readonly DeviceParameterInfo<string> Vendor = (DeviceParameterInfo<string>)new ParameterInfoString(0x102C);

            /// <summary>
            /// A unique device vendor identifier. An example of a unique device identifier
            /// could be the PCIe ID.
            /// </summary>
            public static readonly DeviceParameterInfo<uint> VendorID = (DeviceParameterInfo<uint>)new ParameterInfoUInt32(0x1001);

            /// <summary>
            /// OpenCL version string. Returns the OpenCL version supported by the device.
            /// This version string has the following format:
            /// <para/>
            /// <em>OpenCL&lt;space&gt;&lt;major_version.minor_version&gt;&lt;space&gt;&lt;vendor-specific information&gt;</em>
            /// <para/>
            /// The <em>major_version.minor_version</em> value returned will be 1.1.
            /// </summary>
            public static readonly DeviceParameterInfo<string> Version = (DeviceParameterInfo<string>)new ParameterInfoString(0x102F);

            /// <summary>
            /// OpenCL software driver version string in the form <em>major_number.minor_number</em>.
            /// </summary>
            public static readonly DeviceParameterInfo<string> DriverVersion = (DeviceParameterInfo<string>)new ParameterInfoString(0x102D);
        }

        public sealed class DeviceParameterInfo<T>
        {
            private readonly ParameterInfo<T> _parameterInfo;

            public DeviceParameterInfo(ParameterInfo<T> parameterInfo)
            {
                if (parameterInfo == null)
                    throw new ArgumentNullException("parameterInfo");

                _parameterInfo = parameterInfo;
            }

            public static explicit operator DeviceParameterInfo<T>(ParameterInfo<T> parameterInfo)
            {
                DeviceParameterInfo<T> result = parameterInfo as DeviceParameterInfo<T>;
                if (result != null)
                    return result;

                return new DeviceParameterInfo<T>(parameterInfo);
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

        #region Partition a Device

        [DllImport(ExternDll.OpenCL)]
        private static extern ErrorCode clCreateSubDevices(ClDeviceID device, [In, MarshalAs(UnmanagedType.LPArray)] IntPtr[] properties, uint numDevices, [Out, MarshalAs(UnmanagedType.LPArray)] ClDeviceID[] devices, out uint numDevicesRet);

        private static DisposableCollection<Device> CreateSubDevices(ClDeviceID device, IntPtr[] properties)
        {
            uint required;
            ErrorHandler.ThrowOnFailure(clCreateSubDevices(device, properties, 0, null, out required));

            ClDeviceID[] devices = new ClDeviceID[required];
            uint actual;
            ErrorHandler.ThrowOnFailure(clCreateSubDevices(device, properties, required, devices, out actual));

            DisposableCollection<Device> result = new DisposableCollection<Device>();
            for (int i = 0; i < actual; i++)
                result.Add(new Device(devices[i], new DeviceSafeHandle(devices[i])));

            return result;
        }

        public static DisposableCollection<Device> PartitionEqually(ClDeviceID device, int partitionSize)
        {
            IntPtr[] properties = { (IntPtr)PartitionProperty.PartitionEqually, (IntPtr)partitionSize, IntPtr.Zero };
            return CreateSubDevices(device, properties);
        }

        public static DisposableCollection<Device> PartitionByCounts(ClDeviceID device, int[] partitionSizes)
        {
            if (partitionSizes == null)
                throw new ArgumentNullException("partitionSizes");

            List<IntPtr> propertiesList = new List<IntPtr>();
            propertiesList.Add((IntPtr)PartitionProperty.PartitionByCounts);
            foreach (int partitionSize in partitionSizes)
            {
                if (partitionSize < 0)
                    throw new ArgumentOutOfRangeException("partitionSizes", "Partition size cannot be negative.");

                propertiesList.Add((IntPtr)partitionSize);
            }

            propertiesList.Add((IntPtr)PartitionProperty.PartitionByCountsListEnd);
            propertiesList.Add(IntPtr.Zero);

            IntPtr[] properties = propertiesList.ToArray();
            return CreateSubDevices(device, properties);
        }

        public static DisposableCollection<Device> PartitionByAffinityDomain(ClDeviceID device, AffinityDomain affinityDomain)
        {
            IntPtr[] properties = { (IntPtr)PartitionProperty.PartitionByAffinityDomain, (IntPtr)affinityDomain, IntPtr.Zero };
            return CreateSubDevices(device, properties);
        }

        [DllImport(ExternDll.OpenCL)]
        private static extern ErrorCode clRetainDevice(DeviceSafeHandle device);

        [DllImport(ExternDll.OpenCL)]
        public static extern ErrorCode clReleaseDevice(IntPtr device);

        #endregion

        #region Contexts

        [DllImport(ExternDll.OpenCL)]
        private static extern ContextSafeHandle clCreateContext([In, MarshalAs(UnmanagedType.LPArray)] IntPtr[] properties, uint numDevices, [In, MarshalAs(UnmanagedType.LPArray)] ClDeviceID[] devices, CreateContextCallback pfnNotify, IntPtr userData, out ErrorCode errorCode);

        [DllImport(ExternDll.OpenCL)]
        private static extern ContextSafeHandle clCreateContextFromType([In, MarshalAs(UnmanagedType.LPArray)] IntPtr[] properties, DeviceType deviceType, CreateContextCallback pfnNotify, IntPtr userData, out ErrorCode errorCode);

        public static ContextSafeHandle CreateContext(ClDeviceID[] devices, CreateContextCallback pfnNotify, IntPtr userData)
        {
            ErrorCode errorCode = ErrorCode.Success;
            ContextSafeHandle result = clCreateContext(null, (uint)devices.Length, devices, pfnNotify, userData, out errorCode);
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
        private static extern ErrorCode clGetContextInfo(ContextSafeHandle context, int paramName, UIntPtr paramValueSize, IntPtr paramValue, out UIntPtr paramValueSizeRet);

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
                    throw new ArgumentNullException("parameterInfo");

                _parameterInfo = parameterInfo;
            }

            public static explicit operator ContextParameterInfo<T>(ParameterInfo<T> parameterInfo)
            {
                ContextParameterInfo<T> result = parameterInfo as ContextParameterInfo<T>;
                if (result != null)
                    return result;

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

        #endregion

        #region Command Queues

        [DllImport(ExternDll.OpenCL)]
        private static extern CommandQueueSafeHandle clCreateCommandQueue(ContextSafeHandle context, ClDeviceID device, CommandQueueProperties properties, out ErrorCode errorCode);

        public static CommandQueueSafeHandle CreateCommandQueue(ContextSafeHandle context, ClDeviceID device, CommandQueueProperties properties)
        {
            ErrorCode errorCode;
            CommandQueueSafeHandle result = clCreateCommandQueue(context, device, properties, out errorCode);
            ErrorHandler.ThrowOnFailure(errorCode);
            return result;
        }

        [DllImport(ExternDll.OpenCL)]
        private static extern ErrorCode clRetainCommandQueue(CommandQueueSafeHandle commandQueue);

        [DllImport(ExternDll.OpenCL)]
        public static extern ErrorCode clReleaseCommandQueue(IntPtr commandQueue);

        [DllImport(ExternDll.OpenCL)]
        private static extern ErrorCode clGetCommandQueueInfo(CommandQueueSafeHandle commandQueue, int paramName, UIntPtr paramValueSize, IntPtr paramValue, out UIntPtr paramValueSizeRet);

        public static T GetCommandQueueInfo<T>(CommandQueueSafeHandle commandQueue, CommandQueueParameterInfo<T> parameter)
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
                ErrorHandler.ThrowOnFailure(clGetCommandQueueInfo(commandQueue, parameter.ParameterInfo.Name, UIntPtr.Zero, IntPtr.Zero, out requiredSize));

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
                ErrorHandler.ThrowOnFailure(clGetCommandQueueInfo(commandQueue, parameter.ParameterInfo.Name, requiredSize, memory, out actualSize));
                return parameter.ParameterInfo.Deserialize(actualSize, memory);
            }
            finally
            {
                Marshal.FreeHGlobal(memory);
            }
        }

        public static class CommandQueueInfo
        {
            /// <summary>
            /// Return the context specified when the command-queue is created.
            /// </summary>
            public static CommandQueueParameterInfo<IntPtr> Context = (CommandQueueParameterInfo<IntPtr>)new ParameterInfoIntPtr(0x1090);

            /// <summary>
            /// Return the device specified when the command-queue is created.
            /// </summary>
            public static CommandQueueParameterInfo<IntPtr> Device = (CommandQueueParameterInfo<IntPtr>)new ParameterInfoIntPtr(0x1091);

            /// <summary>
            /// Return the command-queue reference count.
            /// <para/>
            /// The reference count returned with <see cref="ReferenceCount"/> should be considered
            /// immediately stale. It is unsuitable for general use in applications. This feature
            /// is provided for identifying memory leaks.
            /// </summary>
            public static CommandQueueParameterInfo<uint> ReferenceCount = (CommandQueueParameterInfo<uint>)new ParameterInfoUInt32(0x1092);

            /// <summary>
            /// Return the currently specified properties for the command-queue. These
            /// properties are specified by the <em>properties</em> argument in <see cref="clCreateCommandQueue"/>.
            /// </summary>
            public static CommandQueueParameterInfo<ulong> Properties = (CommandQueueParameterInfo<ulong>)new ParameterInfoUInt64(0x1093);
        }

        public sealed class CommandQueueParameterInfo<T>
        {
            private readonly ParameterInfo<T> _parameterInfo;

            public CommandQueueParameterInfo(ParameterInfo<T> parameterInfo)
            {
                if (parameterInfo == null)
                    throw new ArgumentNullException("parameterInfo");

                _parameterInfo = parameterInfo;
            }

            public static explicit operator CommandQueueParameterInfo<T>(ParameterInfo<T> parameterInfo)
            {
                CommandQueueParameterInfo<T> result = parameterInfo as CommandQueueParameterInfo<T>;
                if (result != null)
                    return result;

                return new CommandQueueParameterInfo<T>(parameterInfo);
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

        #region Buffer Objects

        [DllImport(ExternDll.OpenCL)]
        private static extern BufferSafeHandle clCreateBuffer(
            ContextSafeHandle context,
            MemoryFlags flags,
            IntPtr size,
            IntPtr hostPointer,
            out ErrorCode errorCode);

        public static BufferSafeHandle CreateBuffer(ContextSafeHandle context, MemoryFlags flags, IntPtr size, IntPtr hostPointer)
        {
            if (context == null)
                throw new ArgumentNullException("context");

            ErrorCode errorCode;
            BufferSafeHandle handle = clCreateBuffer(context, flags, size, hostPointer, out errorCode);
            ErrorHandler.ThrowOnFailure(errorCode);
            return handle;
        }

        [DllImport(ExternDll.OpenCL)]
        private static extern BufferSafeHandle clCreateSubBuffer(
            BufferSafeHandle buffer,
            MemoryFlags flags,
            BufferCreateType mustBeRegion,
            [In] ref BufferRegion regionInfo,
            out ErrorCode errorCode);

        public static BufferSafeHandle CreateSubBuffer(BufferSafeHandle buffer, MemoryFlags flags, BufferRegion regionInfo)
        {
            if (buffer == null)
                throw new ArgumentNullException("buffer");

            ErrorCode errorCode;
            BufferSafeHandle handle = clCreateSubBuffer(buffer, flags, BufferCreateType.Region, ref regionInfo, out errorCode);
            ErrorHandler.ThrowOnFailure(errorCode);
            return handle;
        }

        [DllImport(ExternDll.OpenCL)]
        private static extern ErrorCode clEnqueueReadBuffer(
            CommandQueueSafeHandle commandQueue,
            BufferSafeHandle buffer,
            [MarshalAs(UnmanagedType.Bool)] bool blockingRead,
            IntPtr offset,
            IntPtr size,
            IntPtr destination,
            uint numEventsInWaitList,
            [In, MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(SafeHandleArrayMarshaler))] EventSafeHandle[] eventWaitList,
            out EventSafeHandle @event);

        public static EventSafeHandle EnqueueReadBuffer(CommandQueueSafeHandle commandQueue, BufferSafeHandle buffer, bool blocking, IntPtr offset, IntPtr size, IntPtr destination, EventSafeHandle[] eventWaitList)
        {
            if (commandQueue == null)
                throw new ArgumentNullException("commandQueue");
            if (buffer == null)
                throw new ArgumentNullException("buffer");
            if (destination == IntPtr.Zero)
                throw new ArgumentNullException("destination");

            EventSafeHandle result;
            ErrorHandler.ThrowOnFailure(clEnqueueReadBuffer(commandQueue, buffer, blocking, offset, size, destination, eventWaitList != null ? (uint)eventWaitList.Length : 0, eventWaitList != null && eventWaitList.Length > 0 ? eventWaitList : null, out result));
            return result;
        }

        [DllImport(ExternDll.OpenCL)]
        private static extern ErrorCode clEnqueueWriteBuffer(
            CommandQueueSafeHandle commandQueue,
            BufferSafeHandle buffer,
            [MarshalAs(UnmanagedType.Bool)] bool blockingWrite,
            IntPtr offset,
            IntPtr size,
            IntPtr source,
            uint numEventsInWaitList,
            [In, MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(SafeHandleArrayMarshaler))] EventSafeHandle[] eventWaitList,
            out EventSafeHandle @event);

        public static EventSafeHandle EnqueueWriteBuffer(CommandQueueSafeHandle commandQueue, BufferSafeHandle buffer, bool blocking, IntPtr offset, IntPtr size, IntPtr source, EventSafeHandle[] eventWaitList)
        {
            if (commandQueue == null)
                throw new ArgumentNullException("commandQueue");
            if (buffer == null)
                throw new ArgumentNullException("buffer");
            if (source == IntPtr.Zero)
                throw new ArgumentNullException("destination");

            EventSafeHandle result;
            ErrorHandler.ThrowOnFailure(clEnqueueWriteBuffer(commandQueue, buffer, blocking, offset, size, source, eventWaitList != null ? (uint)eventWaitList.Length : 0, eventWaitList != null && eventWaitList.Length > 0 ? eventWaitList : null, out result));
            return result;
        }

        [DllImport(ExternDll.OpenCL)]
        private static extern ErrorCode clEnqueueReadBufferRect(
            CommandQueueSafeHandle commandQueue,
            BufferSafeHandle buffer,
            [MarshalAs(UnmanagedType.Bool)] bool blockingRead,
            [In] ref BufferCoordinates bufferOrigin,
            [In] ref BufferCoordinates hostOrigin,
            [In] ref BufferSize region,
            IntPtr bufferRowPitch,
            IntPtr bufferSlicePitch,
            IntPtr hostRowPitch,
            IntPtr hostSlicePitch,
            IntPtr destination,
            uint numEventsInWaitList,
            [In, MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(SafeHandleArrayMarshaler))] EventSafeHandle[] eventWaitList,
            out EventSafeHandle @event);

        public static EventSafeHandle EnqueueReadBufferRect(
            CommandQueueSafeHandle commandQueue,
            BufferSafeHandle buffer,
            bool blocking,
            ref BufferCoordinates bufferOrigin,
            ref BufferCoordinates hostOrigin,
            ref BufferSize region,
            IntPtr bufferRowPitch,
            IntPtr bufferSlicePitch,
            IntPtr hostRowPitch,
            IntPtr hostSlicePitch,
            IntPtr destination,
            EventSafeHandle[] eventWaitList)
        {
            if (commandQueue == null)
                throw new ArgumentNullException("commandQueue");
            if (buffer == null)
                throw new ArgumentNullException("buffer");
            if (destination == IntPtr.Zero)
                throw new ArgumentNullException("destination");

            EventSafeHandle result;
            ErrorHandler.ThrowOnFailure(clEnqueueReadBufferRect(commandQueue, buffer, blocking, ref bufferOrigin, ref hostOrigin, ref region, bufferRowPitch, bufferSlicePitch, hostRowPitch, hostSlicePitch, destination, eventWaitList != null ? (uint)eventWaitList.Length : 0, eventWaitList != null && eventWaitList.Length > 0 ? eventWaitList : null, out result));
            return result;
        }

        [DllImport(ExternDll.OpenCL)]
        private static extern ErrorCode clEnqueueWriteBufferRect(
            CommandQueueSafeHandle commandQueue,
            BufferSafeHandle buffer,
            [MarshalAs(UnmanagedType.Bool)] bool blockingWrite,
            [In] ref BufferCoordinates bufferOrigin,
            [In] ref BufferCoordinates hostOrigin,
            [In] ref BufferSize region,
            IntPtr bufferRowPitch,
            IntPtr bufferSlicePitch,
            IntPtr hostRowPitch,
            IntPtr hostSlicePitch,
            IntPtr source,
            uint numEventsInWaitList,
            [In, MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(SafeHandleArrayMarshaler))] EventSafeHandle[] eventWaitList,
            out EventSafeHandle @event);

        public static EventSafeHandle EnqueueWriteBufferRect(
            CommandQueueSafeHandle commandQueue,
            BufferSafeHandle buffer,
            bool blocking,
            ref BufferCoordinates bufferOrigin,
            ref BufferCoordinates hostOrigin,
            ref BufferSize region,
            IntPtr bufferRowPitch,
            IntPtr bufferSlicePitch,
            IntPtr hostRowPitch,
            IntPtr hostSlicePitch,
            IntPtr source,
            EventSafeHandle[] eventWaitList)
        {
            if (commandQueue == null)
                throw new ArgumentNullException("commandQueue");
            if (buffer == null)
                throw new ArgumentNullException("buffer");
            if (source == IntPtr.Zero)
                throw new ArgumentNullException("source");

            EventSafeHandle result;
            ErrorHandler.ThrowOnFailure(clEnqueueWriteBufferRect(commandQueue, buffer, blocking, ref bufferOrigin, ref hostOrigin, ref region, bufferRowPitch, bufferSlicePitch, hostRowPitch, hostSlicePitch, source, eventWaitList != null ? (uint)eventWaitList.Length : 0, eventWaitList != null && eventWaitList.Length > 0 ? eventWaitList : null, out result));
            return result;
        }

        [DllImport(ExternDll.OpenCL)]
        private static extern ErrorCode clEnqueueFillBuffer(
            CommandQueueSafeHandle commandQueue,
            BufferSafeHandle buffer,
            IntPtr pattern,
            IntPtr patternSize,
            IntPtr offset,
            IntPtr size,
            uint numEventsInWaitList,
            [In, MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(SafeHandleArrayMarshaler))] EventSafeHandle[] eventWaitList,
            out EventSafeHandle @event);

        [DllImport(ExternDll.OpenCL)]
        private static extern ErrorCode clEnqueueCopyBuffer(
            CommandQueueSafeHandle commandQueue,
            BufferSafeHandle sourceBuffer,
            BufferSafeHandle destinationBuffer,
            IntPtr sourceOffset,
            IntPtr destinationOffset,
            IntPtr size,
            uint numEventsInWaitList,
            [In, MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(SafeHandleArrayMarshaler))] EventSafeHandle[] eventWaitList,
            out EventSafeHandle @event);

        public static EventSafeHandle EnqueueCopyBuffer(CommandQueueSafeHandle commandQueue, BufferSafeHandle sourceBuffer, BufferSafeHandle destinationBuffer, IntPtr sourceOffset, IntPtr destinationOffset, IntPtr size, EventSafeHandle[] eventWaitList)
        {
            if (commandQueue == null)
                throw new ArgumentNullException("commandQueue");
            if (sourceBuffer == null)
                throw new ArgumentNullException("sourceBuffer");
            if (destinationBuffer == null)
                throw new ArgumentNullException("destinationBuffer");

            EventSafeHandle result;
            ErrorHandler.ThrowOnFailure(clEnqueueCopyBuffer(commandQueue, sourceBuffer, destinationBuffer, sourceOffset, destinationOffset, size, eventWaitList != null ? (uint)eventWaitList.Length : 0, eventWaitList != null && eventWaitList.Length > 0 ? eventWaitList : null, out result));
            return result;
        }

        [DllImport(ExternDll.OpenCL)]
        private static extern ErrorCode clEnqueueCopyBufferRect(
            CommandQueueSafeHandle commandQueue,
            BufferSafeHandle sourceBuffer,
            BufferSafeHandle destinationBuffer,
            [In] ref BufferCoordinates sourceOrigin,
            [In] ref BufferCoordinates destinationOrigin,
            [In] ref BufferSize region,
            IntPtr sourceRowPitch,
            IntPtr sourceSlicePitch,
            IntPtr destinationRowPitch,
            IntPtr destinationSlicePitch,
            uint numEventsInWaitList,
            [In, MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(SafeHandleArrayMarshaler))] EventSafeHandle[] eventWaitList,
            out EventSafeHandle @event);

        public static EventSafeHandle EnqueueCopyBufferRect(
            CommandQueueSafeHandle commandQueue,
            BufferSafeHandle sourceBuffer,
            BufferSafeHandle destinationBuffer,
            [In] ref BufferCoordinates sourceOrigin,
            [In] ref BufferCoordinates destinationOrigin,
            [In] ref BufferSize region,
            IntPtr sourceRowPitch,
            IntPtr sourceSlicePitch,
            IntPtr destinationRowPitch,
            IntPtr destinationSlicePitch,
            EventSafeHandle[] eventWaitList)
        {
            if (commandQueue == null)
                throw new ArgumentNullException("commandQueue");
            if (sourceBuffer == null)
                throw new ArgumentNullException("sourceBuffer");
            if (destinationBuffer == null)
                throw new ArgumentNullException("destinationBuffer");

            EventSafeHandle result;
            ErrorHandler.ThrowOnFailure(clEnqueueCopyBufferRect(commandQueue, sourceBuffer, destinationBuffer, ref sourceOrigin, ref destinationOrigin, ref region, sourceRowPitch, sourceSlicePitch, destinationRowPitch, destinationSlicePitch, eventWaitList != null ? (uint)eventWaitList.Length : 0, eventWaitList != null && eventWaitList.Length > 0 ? eventWaitList : null, out result));
            return result;
        }

        [DllImport(ExternDll.OpenCL)]
        private static extern IntPtr clEnqueueMapBuffer(
            CommandQueueSafeHandle commandQueue,
            BufferSafeHandle buffer,
            [MarshalAs(UnmanagedType.Bool)] bool blockingMap,
            MapFlags mapFlags,
            IntPtr offset,
            IntPtr size,
            uint numEventsInWaitList,
            [In, MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(SafeHandleArrayMarshaler))] EventSafeHandle[] eventWaitList,
            out EventSafeHandle @event,
            out ErrorCode errorCode);

        public static EventSafeHandle EnqueueMapBuffer(
            CommandQueueSafeHandle commandQueue,
            BufferSafeHandle buffer,
            bool blocking,
            MapFlags mapFlags,
            IntPtr offset,
            IntPtr size,
            out IntPtr mappedPointer,
            EventSafeHandle[] eventWaitList)
        {
            if (commandQueue == null)
                throw new ArgumentNullException("commandQueue");
            if (buffer == null)
                throw new ArgumentNullException("buffer");

            EventSafeHandle result;
            ErrorCode errorCode;
            mappedPointer = clEnqueueMapBuffer(commandQueue, buffer, blocking, mapFlags, offset, size, eventWaitList != null ? (uint)eventWaitList.Length : 0, eventWaitList != null && eventWaitList.Length > 0 ? eventWaitList : null, out result, out errorCode);
            ErrorHandler.ThrowOnFailure(errorCode);
            return result;
        }

        [DllImport(ExternDll.OpenCL)]
        private static extern ImageSafeHandle clCreateImage(
            ContextSafeHandle context,
            MemoryFlags flags,
            [In] ref ImageFormat imageFormat,
            [In] ref ImageDescriptor imageDescriptor,
            IntPtr hostAddress,
            out ErrorCode errorCode);

        public static ImageSafeHandle CreateImage(ContextSafeHandle context, MemoryFlags flags, ref ImageFormat imageFormat, ref ImageDescriptor imageDescriptor, IntPtr hostAddress)
        {
            if (context == null)
                throw new ArgumentNullException("context");

            ErrorCode errorCode;
            ImageSafeHandle result = clCreateImage(context, flags, ref imageFormat, ref imageDescriptor, hostAddress, out errorCode);
            ErrorHandler.ThrowOnFailure(errorCode);
            return result;
        }

        [DllImport(ExternDll.OpenCL)]
        private static extern ErrorCode clGetSupportedImageFormats(ContextSafeHandle context, MemoryFlags flags, MemObjectType imageType, uint numEntries, [Out] ImageFormat[] imageFormats, out uint numImageFormats);

        [DllImport(ExternDll.OpenCL)]
        private static extern ErrorCode clEnqueueReadImage(
            CommandQueueSafeHandle commandQueue,
            ImageSafeHandle image,
            [MarshalAs(UnmanagedType.Bool)] bool blockingRead,
            [In] ref BufferCoordinates origin,
            [In] ref BufferSize region,
            IntPtr rowPitch,
            IntPtr slicePitch,
            IntPtr destination,
            uint numEventsInWaitList,
            [In, MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(SafeHandleArrayMarshaler))] EventSafeHandle[] eventWaitList,
            out EventSafeHandle @event);

        [DllImport(ExternDll.OpenCL)]
        private static extern ErrorCode clEnqueueWriteImage(
            CommandQueueSafeHandle commandQueue,
            ImageSafeHandle image,
            [MarshalAs(UnmanagedType.Bool)] bool blockingWrite,
            [In] ref BufferCoordinates origin,
            [In] ref BufferSize region,
            IntPtr inputRowPtch,
            IntPtr inputSlicePitch,
            IntPtr source,
            uint numEventsInWaitList,
            [In, MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(SafeHandleArrayMarshaler))] EventSafeHandle[] eventWaitList,
            out EventSafeHandle @event);

        [DllImport(ExternDll.OpenCL)]
        private static extern ErrorCode clEnqueueFillImage(
            CommandQueueSafeHandle commandQueue,
            ImageSafeHandle image,
            IntPtr fillColor,
            [In] ref BufferCoordinates origin,
            [In] ref BufferSize region,
            uint numEventsInWaitList,
            [In, MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(SafeHandleArrayMarshaler))] EventSafeHandle[] eventWaitList,
            out EventSafeHandle @event);

        [DllImport(ExternDll.OpenCL)]
        private static extern ErrorCode clEnqueueCopyImage(
            CommandQueueSafeHandle commandQueue,
            ImageSafeHandle sourceImage,
            ImageSafeHandle destinationImage,
            [In] ref BufferCoordinates sourceOrigin,
            [In] ref BufferCoordinates destinationOrigin,
            [In] ref BufferSize region,
            uint numEventsInWaitList,
            [In, MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(SafeHandleArrayMarshaler))] EventSafeHandle[] eventWaitList,
            out EventSafeHandle @event);

        [DllImport(ExternDll.OpenCL)]
        private static extern ErrorCode clEnqueueCopyImageToBuffer(
            CommandQueueSafeHandle commandQueue,
            ImageSafeHandle sourceImage,
            BufferSafeHandle destinationBuffer,
            [In] ref BufferCoordinates sourceOrigin,
            [In] ref BufferSize region,
            IntPtr destinationOffset,
            uint numEventsInWaitList,
            [In, MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(SafeHandleArrayMarshaler))] EventSafeHandle[] eventWaitList,
            out EventSafeHandle @event);

        [DllImport(ExternDll.OpenCL)]
        private static extern ErrorCode clEnqueueCopyBufferToImage(
            CommandQueueSafeHandle commandQueue,
            BufferSafeHandle sourceBuffer,
            ImageSafeHandle destinationImage,
            IntPtr sourceOffset,
            [In] ref BufferCoordinates destinationOrigin,
            [In] ref BufferSize region,
            uint numEventsInWaitList,
            [In, MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(SafeHandleArrayMarshaler))] EventSafeHandle[] eventWaitList,
            out EventSafeHandle @event);

        [DllImport(ExternDll.OpenCL)]
        private static extern IntPtr clEnqueueMapImage(
            CommandQueueSafeHandle commandQueue,
            ImageSafeHandle image,
            [MarshalAs(UnmanagedType.Bool)] bool blockingMap,
            MapFlags mapFlags,
            [In] ref BufferCoordinates origin,
            [In] ref BufferSize region,
            out IntPtr imageRowPitch,
            out IntPtr imageSlicePitch,
            uint numEventsInWaitList,
            [In, MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(SafeHandleArrayMarshaler))] EventSafeHandle[] eventWaitList,
            out EventSafeHandle @event,
            out ErrorCode errorCode);

        [DllImport(ExternDll.OpenCL)]
        private static extern ErrorCode clEnqueueUnmapMemObject(
            CommandQueueSafeHandle commandQueue,
            MemObjectSafeHandle memObject,
            IntPtr mappedPointer,
            uint numEventsInWaitList,
            [In, MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(SafeHandleArrayMarshaler))] EventSafeHandle[] eventWaitList,
            out EventSafeHandle @event);

        public static EventSafeHandle EnqueueUnmapMemObject(
            CommandQueueSafeHandle commandQueue,
            MemObjectSafeHandle memObject,
            IntPtr mappedPointer,
            EventSafeHandle[] eventWaitList)
        {
            if (commandQueue == null)
                throw new ArgumentNullException("commandQueue");
            if (memObject == null)
                throw new ArgumentNullException("memObject");

            EventSafeHandle result;
            ErrorHandler.ThrowOnFailure(clEnqueueUnmapMemObject(commandQueue, memObject, mappedPointer, eventWaitList != null ? (uint)eventWaitList.Length : 0, eventWaitList != null && eventWaitList.Length > 0 ? eventWaitList : null, out result));
            return result;
        }

        [DllImport(ExternDll.OpenCL)]
        private static extern ErrorCode clGetMemObjectInfo(MemObjectSafeHandle memObject, int paramName, UIntPtr paramValueSize, IntPtr paramValue, out UIntPtr paramValueSizeRet);

        public static T GetMemObjectInfo<T>(MemObjectSafeHandle memObject, MemObjectParameterInfo<T> parameter)
        {
            if (memObject == null)
                throw new ArgumentNullException("memObject");
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
                ErrorHandler.ThrowOnFailure(clGetMemObjectInfo(memObject, parameter.ParameterInfo.Name, UIntPtr.Zero, IntPtr.Zero, out requiredSize));

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
                ErrorHandler.ThrowOnFailure(clGetMemObjectInfo(memObject, parameter.ParameterInfo.Name, requiredSize, memory, out actualSize));
                return parameter.ParameterInfo.Deserialize(actualSize, memory);
            }
            finally
            {
                Marshal.FreeHGlobal(memory);
            }
        }

        public static class MemObjectInfo
        {
            public static readonly MemObjectParameterInfo<uint> Type =
                (MemObjectParameterInfo<uint>)new ParameterInfoUInt32(0x1100);
            public static readonly MemObjectParameterInfo<ulong> Flags =
                (MemObjectParameterInfo<ulong>)new ParameterInfoUInt64(0x1101);
            public static readonly MemObjectParameterInfo<UIntPtr> Size =
                (MemObjectParameterInfo<UIntPtr>)new ParameterInfoUIntPtr(0x1102);
            public static readonly MemObjectParameterInfo<IntPtr> HostAddress =
                (MemObjectParameterInfo<IntPtr>)new ParameterInfoIntPtr(0x1103);
            public static readonly MemObjectParameterInfo<uint> MapCount =
                (MemObjectParameterInfo<uint>)new ParameterInfoUInt32(0x1104);
            public static readonly MemObjectParameterInfo<uint> ReferenceCount =
                (MemObjectParameterInfo<uint>)new ParameterInfoUInt32(0x1105);
            public static readonly MemObjectParameterInfo<IntPtr> Context =
                (MemObjectParameterInfo<IntPtr>)new ParameterInfoIntPtr(0x1106);
            public static readonly MemObjectParameterInfo<IntPtr> AssociatedMemObject =
                (MemObjectParameterInfo<IntPtr>)new ParameterInfoIntPtr(0x1107);
            public static readonly MemObjectParameterInfo<UIntPtr> Offset =
                (MemObjectParameterInfo<UIntPtr>)new ParameterInfoUIntPtr(0x1108);
        }

        public sealed class MemObjectParameterInfo<T>
        {
            private readonly ParameterInfo<T> _parameterInfo;

            public MemObjectParameterInfo(ParameterInfo<T> parameterInfo)
            {
                if (parameterInfo == null)
                    throw new ArgumentNullException("parameterInfo");

                _parameterInfo = parameterInfo;
            }

            public static explicit operator MemObjectParameterInfo<T>(ParameterInfo<T> parameterInfo)
            {
                MemObjectParameterInfo<T> result = parameterInfo as MemObjectParameterInfo<T>;
                if (result != null)
                    return result;

                return new MemObjectParameterInfo<T>(parameterInfo);
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
        private static extern ErrorCode clGetImageInfo(ImageSafeHandle image, int paramName, UIntPtr paramValueSize, IntPtr paramValue, out UIntPtr paramValueSizeRet);

        public static T GetImageInfo<T>(ImageSafeHandle image, ImageParameterInfo<T> parameter)
        {
            if (image == null)
                throw new ArgumentNullException("image");
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
                ErrorHandler.ThrowOnFailure(clGetImageInfo(image, parameter.ParameterInfo.Name, UIntPtr.Zero, IntPtr.Zero, out requiredSize));

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
                ErrorHandler.ThrowOnFailure(clGetImageInfo(image, parameter.ParameterInfo.Name, requiredSize, memory, out actualSize));
                return parameter.ParameterInfo.Deserialize(actualSize, memory);
            }
            finally
            {
                Marshal.FreeHGlobal(memory);
            }
        }

        public static class ImageInfo
        {
            public static ImageParameterInfo<IntPtr[]> Format = (ImageParameterInfo<IntPtr[]>)new ParameterInfoIntPtrArray(0x1110);
            public static ImageParameterInfo<UIntPtr> ElementSize = (ImageParameterInfo<UIntPtr>)new ParameterInfoUIntPtr(0x1111);
            public static ImageParameterInfo<UIntPtr> RowPitch = (ImageParameterInfo<UIntPtr>)new ParameterInfoUIntPtr(0x1112);
            public static ImageParameterInfo<UIntPtr> SlicePitch = (ImageParameterInfo<UIntPtr>)new ParameterInfoUIntPtr(0x1113);
            public static ImageParameterInfo<UIntPtr> Width = (ImageParameterInfo<UIntPtr>)new ParameterInfoUIntPtr(0x1114);
            public static ImageParameterInfo<UIntPtr> Height = (ImageParameterInfo<UIntPtr>)new ParameterInfoUIntPtr(0x1115);
            public static ImageParameterInfo<UIntPtr> Depth = (ImageParameterInfo<UIntPtr>)new ParameterInfoUIntPtr(0x1116);
            public static ImageParameterInfo<UIntPtr> ArraySize = (ImageParameterInfo<UIntPtr>)new ParameterInfoUIntPtr(0x1117);
            public static ImageParameterInfo<IntPtr> Buffer = (ImageParameterInfo<IntPtr>)new ParameterInfoIntPtr(0x1118);
            public static ImageParameterInfo<uint> NumMipLevels = (ImageParameterInfo<uint>)new ParameterInfoUInt32(0x1119);
            public static ImageParameterInfo<uint> NumSamples = (ImageParameterInfo<uint>)new ParameterInfoUInt32(0x111A);
        }

        public sealed class ImageParameterInfo<T>
        {
            private readonly ParameterInfo<T> _parameterInfo;

            public ImageParameterInfo(ParameterInfo<T> parameterInfo)
            {
                if (parameterInfo == null)
                    throw new ArgumentNullException("parameterInfo");

                _parameterInfo = parameterInfo;
            }

            public static explicit operator ImageParameterInfo<T>(ParameterInfo<T> parameterInfo)
            {
                ImageParameterInfo<T> result = parameterInfo as ImageParameterInfo<T>;
                if (result != null)
                    return result;

                return new ImageParameterInfo<T>(parameterInfo);
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
        private static extern ErrorCode clRetainMemObject(MemObjectSafeHandle memObject);

        [DllImport(ExternDll.OpenCL)]
        public static extern ErrorCode clReleaseMemObject(IntPtr memObject);

        #endregion

        #region Sampler Objects

        [DllImport(ExternDll.OpenCL)]
        private static extern SamplerSafeHandle clCreateSampler(ContextSafeHandle context, [MarshalAs(UnmanagedType.Bool)] bool normalizedCoordinates, AddressingMode addressingMode, FilterMode filterMode, out ErrorCode errorCode);

        public static SamplerSafeHandle CreateSampler(ContextSafeHandle context, bool normalizedCoordinates, AddressingMode addressingMode, FilterMode filterMode)
        {
            ErrorCode errorCode;
            SamplerSafeHandle handle = clCreateSampler(context, normalizedCoordinates, addressingMode, filterMode, out errorCode);
            ErrorHandler.ThrowOnFailure(errorCode);
            return handle;
        }

        [DllImport(ExternDll.OpenCL)]
        private static extern ErrorCode clRetainSampler(SamplerSafeHandle sampler);

        [DllImport(ExternDll.OpenCL)]
        public static extern ErrorCode clReleaseSampler(IntPtr sampler);

        [DllImport(ExternDll.OpenCL)]
        public static extern ErrorCode clGetSamplerInfo(SamplerSafeHandle sampler, int paramName, UIntPtr paramValueSize, IntPtr paramValue, out UIntPtr paramValueSizeRet);

        public static T GetSamplerInfo<T>(SamplerSafeHandle sampler, SamplerParameterInfo<T> parameter)
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
                ErrorHandler.ThrowOnFailure(clGetSamplerInfo(sampler, parameter.ParameterInfo.Name, UIntPtr.Zero, IntPtr.Zero, out requiredSize));

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
                ErrorHandler.ThrowOnFailure(clGetSamplerInfo(sampler, parameter.ParameterInfo.Name, requiredSize, memory, out actualSize));
                return parameter.ParameterInfo.Deserialize(actualSize, memory);
            }
            finally
            {
                Marshal.FreeHGlobal(memory);
            }
        }

        public static class SamplerInfo
        {
            /// <summary>
            /// Return the sampler reference count. The reference count returned should be
            /// considered immediately stale. It is unsuitable for general use in applications.
            /// This feature is provided for identifying memory leaks.
            /// </summary>
            public static readonly SamplerParameterInfo<uint> ReferenceCount = (SamplerParameterInfo<uint>)new ParameterInfoUInt32(0x1150);

            /// <summary>
            /// Return the context specified when the sampler is created.
            /// </summary>
            public static readonly SamplerParameterInfo<IntPtr> Context = (SamplerParameterInfo<IntPtr>)new ParameterInfoIntPtr(0x1151);

            /// <summary>
            /// Return the normalized coords value associated with sampler.
            /// </summary>
            public static readonly SamplerParameterInfo<bool> NormalizedCoordinates = (SamplerParameterInfo<bool>)new ParameterInfoBoolean(0x1152);

            /// <summary>
            /// Return the addressing mode value associated with sampler.
            /// </summary>
            public static readonly SamplerParameterInfo<uint> AddressingMode = (SamplerParameterInfo<uint>)new ParameterInfoUInt32(0x1153);

            /// <summary>
            /// Return the filter mode value associated with sampler.
            /// </summary>
            public static readonly SamplerParameterInfo<uint> FilterMode = (SamplerParameterInfo<uint>)new ParameterInfoUInt32(0x1154);
        }

        public sealed class SamplerParameterInfo<T>
        {
            private readonly ParameterInfo<T> _parameterInfo;

            public SamplerParameterInfo(ParameterInfo<T> parameterInfo)
            {
                if (parameterInfo == null)
                    throw new ArgumentNullException("parameterInfo");

                _parameterInfo = parameterInfo;
            }

            public static explicit operator SamplerParameterInfo<T>(ParameterInfo<T> parameterInfo)
            {
                SamplerParameterInfo<T> result = parameterInfo as SamplerParameterInfo<T>;
                if (result != null)
                    return result;

                return new SamplerParameterInfo<T>(parameterInfo);
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

        #region Program Objects

        [DllImport(ExternDll.OpenCL)]
        private static extern ErrorCode clRetainProgram(ProgramSafeHandle program);

        [DllImport(ExternDll.OpenCL)]
        public static extern ErrorCode clReleaseProgram(IntPtr program);

        #endregion

        #region Kernel Objects

        [DllImport(ExternDll.OpenCL)]
        private static extern ErrorCode clRetainKernel(KernelSafeHandle kernel);

        [DllImport(ExternDll.OpenCL)]
        public static extern ErrorCode clReleaseKernel(IntPtr kernel);

        #endregion

        #region Event Objects

        [DllImport(ExternDll.OpenCL)]
        private static extern EventSafeHandle clCreateUserEvent(ContextSafeHandle context, out ErrorCode errorCode);

        public static EventSafeHandle CreateUserEvent(ContextSafeHandle context)
        {
            if (context == null)
                throw new ArgumentNullException("context");

            ErrorCode errorCode;
            EventSafeHandle handle = clCreateUserEvent(context, out errorCode);
            ErrorHandler.ThrowOnFailure(errorCode);
            return handle;
        }

        [DllImport(ExternDll.OpenCL)]
        private static extern ErrorCode clSetUserEventStatus(EventSafeHandle @event, ExecutionStatus executionStatus);

        public static void SetUserEventStatus(EventSafeHandle @event, ExecutionStatus executionStatus)
        {
            if (@event == null)
                throw new ArgumentNullException("event");

            ErrorHandler.ThrowOnFailure(clSetUserEventStatus(@event, executionStatus));
        }

        [DllImport(ExternDll.OpenCL)]
        private static extern ErrorCode clWaitForEvents(
            uint numEvents,
            [In, MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(SafeHandleArrayMarshaler))] EventSafeHandle[] eventWaitList);

        public static void WaitForEvents(EventSafeHandle[] eventWaitList)
        {
            if (eventWaitList == null)
                throw new ArgumentNullException("eventWaitList");
            if (eventWaitList.Length == 0)
                throw new ArgumentException();

            ErrorHandler.ThrowOnFailure(clWaitForEvents((uint)eventWaitList.Length, eventWaitList));
        }

        [DllImport(ExternDll.OpenCL)]
        private static extern ErrorCode clGetEventInfo(EventSafeHandle @event, int paramName, UIntPtr paramValueSize, IntPtr paramValue, out UIntPtr paramValueSizeRet);

        public static T GetEventInfo<T>(EventSafeHandle @event, EventParameterInfo<T> parameter)
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
                ErrorHandler.ThrowOnFailure(clGetEventInfo(@event, parameter.ParameterInfo.Name, UIntPtr.Zero, IntPtr.Zero, out requiredSize));

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
                ErrorHandler.ThrowOnFailure(clGetEventInfo(@event, parameter.ParameterInfo.Name, requiredSize, memory, out actualSize));
                return parameter.ParameterInfo.Deserialize(actualSize, memory);
            }
            finally
            {
                Marshal.FreeHGlobal(memory);
            }
        }

        public static class EventInfo
        {
            /// <summary>
            /// Return the command-queue associated with event. For user event objects, <see cref="IntPtr.Zero"/> is returned.
            /// </summary>
            public static readonly EventParameterInfo<IntPtr> CommandQueue = (EventParameterInfo<IntPtr>)new ParameterInfoIntPtr(0x11D0);

            /// <summary>
            /// Return the context associated with event.
            /// </summary>
            public static readonly EventParameterInfo<IntPtr> Context = (EventParameterInfo<IntPtr>)new ParameterInfoIntPtr(0x11D4);

            /// <summary>
            /// Return the command associated with event. Can be one of the following values:
            ///
            /// <list type="bullet">
            /// <item><see cref="NOpenCL.CommandType.NdrangeKernel"/></item>
            /// <item><see cref="NOpenCL.CommandType.Task"/></item>
            /// <item><see cref="NOpenCL.CommandType.NativeKernel"/></item>
            /// <item><see cref="NOpenCL.CommandType.ReadBuffer"/></item>
            /// <item><see cref="NOpenCL.CommandType.WriteBuffer"/></item>
            /// <item><see cref="NOpenCL.CommandType.CopyBuffer"/></item>
            /// <item><see cref="NOpenCL.CommandType.ReadImage"/></item>
            /// <item><see cref="NOpenCL.CommandType.WriteImage"/></item>
            /// <item><see cref="NOpenCL.CommandType.CopyImage"/></item>
            /// <item><see cref="NOpenCL.CommandType.CopyBufferToImage"/></item>
            /// <item><see cref="NOpenCL.CommandType.CopyImageToBuffer"/></item>
            /// <item><see cref="NOpenCL.CommandType.MapBuffer"/></item>
            /// <item><see cref="NOpenCL.CommandType.MapImage"/></item>
            /// <item><see cref="NOpenCL.CommandType.UnmapMemObject"/></item>
            /// <item><see cref="NOpenCL.CommandType.Marker"/></item>
            /// <item><see cref="NOpenCL.CommandType.AcquireGlObjects"/></item>
            /// <item><see cref="NOpenCL.CommandType.ReleaseGlObjects"/></item>
            /// <item><see cref="NOpenCL.CommandType.ReadBufferRect"/></item>
            /// <item><see cref="NOpenCL.CommandType.WriteBufferRect"/></item>
            /// <item><see cref="NOpenCL.CommandType.CopyBufferRect"/></item>
            /// <item><see cref="NOpenCL.CommandType.User"/></item>
            /// <item><see cref="NOpenCL.CommandType.Barrier"/></item>
            /// <item><see cref="NOpenCL.CommandType.MigrateMemObjects"/></item>
            /// <item><see cref="NOpenCL.CommandType.FillBuffer"/></item>
            /// <item><see cref="NOpenCL.CommandType.FillImage"/></item>
            /// <item><see cref="NOpenCL.CommandType.GlFenceSyncObjectKhr"/> (if cl_khr_gl_event is enabled)</item>
            /// <item><see cref="NOpenCL.CommandType.AcquireD3d10ObjectsKhr"/> (if cl_khr_d3d10_sharing is enabled)</item>
            /// <item><see cref="NOpenCL.CommandType.ReleaseD3d10ObjectsKhr"/> (if cl_khr_d3d10_sharing is enabled)</item>
            /// <item><see cref="NOpenCL.CommandType.AcquireDx9MediaSurfacesKhr"/> (if cl_khr_dx9_media_sharing is enabled)</item>
            /// <item><see cref="NOpenCL.CommandType.ReleaseDx9MediaSurfacesKhr"/> (if cl_khr_dx9_media_sharing is enabled)</item>
            /// <item><see cref="NOpenCL.CommandType.AcquireD3d11ObjectsKhr"/> (if cl_khr_d3d11_sharing is enabled)</item>
            /// <item><see cref="NOpenCL.CommandType.ReleaseD3d11ObjectsKhr"/> (if cl_khr_d3d11_sharing is enabled)</item>
            /// </list>
            /// </summary>
            public static readonly EventParameterInfo<uint> CommandType = (EventParameterInfo<uint>)new ParameterInfoUInt32(0x11D1);

            /// <summary>
            /// Return the execution status of the command identified by event. The valid values are:
            ///
            /// <list type="bullet">
            /// <item><see cref="ExecutionStatus.Queued"/> (command has been enqueued in the command-queue),</item>
            /// <item><see cref="ExecutionStatus.Submitted"/> (enqueued command has been submitted by the host to the device associated with the command-queue),</item>
            /// <item><see cref="ExecutionStatus.Running"/> (device is currently executing this command),</item>
            /// <item><see cref="ExecutionStatus.Complete"/> (the command has completed), or</item>
            /// <item>Error code given by a negative integer value. (command was abnormally terminated – this may be caused by a bad memory access etc.) These error codes come from the same set of error codes that are returned from the platform or runtime API calls as return values or errcode_ret values.</item>
            /// </list>
            ///
            /// The error code values are negative, and event state values are positive. The
            /// event state values are ordered from the largest value (<see cref="ExecutionStatus.Queued"/>) for the first
            /// or initial state to the smallest value (<see cref="ExecutionStatus.Complete"/> or negative integer value)
            /// for the last or complete state. The value of <see cref="ExecutionStatus.Complete"/> and <see cref="ErrorCode.Success"/> are the same.
            /// </summary>
            public static readonly EventParameterInfo<uint> CommandExecutionStatus = (EventParameterInfo<uint>)new ParameterInfoUInt32(0x11D3);

            /// <summary>
            /// Return the event reference count. The reference count returned should be
            /// considered immediately stale. It is unsuitable for general use in applications.
            /// This feature is provided for identifying memory leaks.
            /// </summary>
            public static readonly EventParameterInfo<uint> ReferenceCount = (EventParameterInfo<uint>)new ParameterInfoUInt32(0x11D2);
        }

        public sealed class EventParameterInfo<T>
        {
            private readonly ParameterInfo<T> _parameterInfo;

            public EventParameterInfo(ParameterInfo<T> parameterInfo)
            {
                if (parameterInfo == null)
                    throw new ArgumentNullException("parameterInfo");

                _parameterInfo = parameterInfo;
            }

            public static explicit operator EventParameterInfo<T>(ParameterInfo<T> parameterInfo)
            {
                EventParameterInfo<T> result = parameterInfo as EventParameterInfo<T>;
                if (result != null)
                    return result;

                return new EventParameterInfo<T>(parameterInfo);
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
        private static extern ErrorCode clSetEventCallback(EventSafeHandle @event, ExecutionStatus executionCallbackType, EventCallback eventNotify, IntPtr userData);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate void EventCallback(EventSafeHandle @event, ExecutionStatus eventCommandExecutionStatus, IntPtr userData);

        [DllImport(ExternDll.OpenCL)]
        private static extern ErrorCode clRetainEvent(EventSafeHandle @event);

        [DllImport(ExternDll.OpenCL)]
        public static extern ErrorCode clReleaseEvent(IntPtr @event);

        #endregion

        #region Markers, Barriers, and Waiting

        [DllImport(ExternDll.OpenCL)]
        private static extern ErrorCode clEnqueueMarkerWithWaitList(
            CommandQueueSafeHandle commandQueue,
            uint numEventsInWaitList,
            [In, MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(SafeHandleArrayMarshaler))] EventSafeHandle[] eventWaitList,
            out EventSafeHandle @event);

        public static EventSafeHandle EnqueueMarkerWithWaitList(CommandQueueSafeHandle commandQueue, EventSafeHandle[] eventWaitList)
        {
            if (commandQueue == null)
                throw new ArgumentNullException("commandQueue");

            EventSafeHandle result;
            ErrorHandler.ThrowOnFailure(clEnqueueMarkerWithWaitList(commandQueue, eventWaitList != null ? (uint)eventWaitList.Length : 0, eventWaitList != null && eventWaitList.Length > 0 ? eventWaitList : null, out result));
            return result;
        }

        [DllImport(ExternDll.OpenCL)]
        private static extern ErrorCode clEnqueueBarrierWithWaitList(
            CommandQueueSafeHandle commandQueue,
            uint numEventsInWaitList,
            [In, MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(SafeHandleArrayMarshaler))] EventSafeHandle[] eventWaitList,
            out EventSafeHandle @event);

        public static EventSafeHandle EnqueueBarrierWithWaitList(CommandQueueSafeHandle commandQueue, EventSafeHandle[] eventWaitList)
        {
            if (commandQueue == null)
                throw new ArgumentNullException("commandQueue");

            EventSafeHandle result;
            ErrorHandler.ThrowOnFailure(clEnqueueBarrierWithWaitList(commandQueue, eventWaitList != null ? (uint)eventWaitList.Length : 0, eventWaitList != null && eventWaitList.Length > 0 ? eventWaitList : null, out result));
            return result;
        }

        #endregion

        #region Profiling Operations on Memory Objects and Kernels

        [DllImport(ExternDll.OpenCL)]
        private static extern ErrorCode clGetEventProfilingInfo(EventSafeHandle @event, int paramName, UIntPtr paramValueSize, IntPtr paramValue, out UIntPtr paramValueSizeRet);

        public static T GetEventProfilingInfo<T>(EventSafeHandle @event, EventProfilingParameterInfo<T> parameter)
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
                ErrorHandler.ThrowOnFailure(clGetEventProfilingInfo(@event, parameter.ParameterInfo.Name, UIntPtr.Zero, IntPtr.Zero, out requiredSize));

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
                ErrorHandler.ThrowOnFailure(clGetEventProfilingInfo(@event, parameter.ParameterInfo.Name, requiredSize, memory, out actualSize));
                return parameter.ParameterInfo.Deserialize(actualSize, memory);
            }
            finally
            {
                Marshal.FreeHGlobal(memory);
            }
        }

        public static class EventProfilingInfo
        {
            /// <summary>
            /// A 64-bit value that describes the current device time counter in nanoseconds when
            /// the command identified by <em>event</em> is enqueued in a command-queue by the host.
            /// </summary>
            public static readonly EventProfilingParameterInfo<ulong> CommandQueued =
                (EventProfilingParameterInfo<ulong>)new ParameterInfoUInt64(0x1280);

            /// <summary>
            /// A 64-bit value that describes the current device time counter in nanoseconds when
            /// the command identified by <em>event</em> that has been enqueued is submitted by the
            /// host to the device associated with the command-queue.
            /// </summary>
            public static readonly EventProfilingParameterInfo<ulong> CommandSubmit =
                (EventProfilingParameterInfo<ulong>)new ParameterInfoUInt64(0x1281);

            /// <summary>
            /// A 64-bit value that describes the current device time counter in nanoseconds when
            /// the command identified by <em>event</em> starts execution on the device.
            /// </summary>
            public static readonly EventProfilingParameterInfo<ulong> CommandStart =
                (EventProfilingParameterInfo<ulong>)new ParameterInfoUInt64(0x1282);

            /// <summary>
            /// A 64-bit value that describes the current device time counter in nanoseconds when
            /// the command identified by <em>event</em> has finished execution on the device.
            /// </summary>
            public static readonly EventProfilingParameterInfo<ulong> CommandEnd =
                (EventProfilingParameterInfo<ulong>)new ParameterInfoUInt64(0x1283);
        }

        public sealed class EventProfilingParameterInfo<T>
        {
            private readonly ParameterInfo<T> _parameterInfo;

            public EventProfilingParameterInfo(ParameterInfo<T> parameterInfo)
            {
                if (parameterInfo == null)
                    throw new ArgumentNullException("parameterInfo");

                _parameterInfo = parameterInfo;
            }

            public static explicit operator EventProfilingParameterInfo<T>(ParameterInfo<T> parameterInfo)
            {
                EventProfilingParameterInfo<T> result = parameterInfo as EventProfilingParameterInfo<T>;
                if (result != null)
                    return result;

                return new EventProfilingParameterInfo<T>(parameterInfo);
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

        #region Flush and Finish

        [DllImport(ExternDll.OpenCL)]
        private static extern ErrorCode clFlush(CommandQueueSafeHandle commandQueue);

        public static void Flush(CommandQueueSafeHandle commandQueue)
        {
            ErrorHandler.ThrowOnFailure(clFlush(commandQueue));
        }

        [DllImport(ExternDll.OpenCL)]
        private static extern ErrorCode clFinish(CommandQueueSafeHandle commandQueue);

        public static void Finish(CommandQueueSafeHandle commandQueue)
        {
            ErrorHandler.ThrowOnFailure(clFinish(commandQueue));
        }

        #endregion

        public abstract class ParameterInfo<T>
        {
            private readonly int _name;

            protected ParameterInfo(int name)
            {
                _name = name;
            }

            public int Name
            {
                get
                {
                    return _name;
                }
            }

            public virtual int? FixedSize
            {
                get
                {
                    return null;
                }
            }

            public abstract T Deserialize(UIntPtr memorySize, IntPtr memory);
        }

        public sealed class ParameterInfoString : ParameterInfo<string>
        {
            public ParameterInfoString(int name)
                : base(name)
            {
            }

            public override string Deserialize(UIntPtr memorySize, IntPtr memory)
            {
                return Marshal.PtrToStringAnsi(memory, (int)memorySize.ToUInt32() - 1);
            }
        }

        public sealed class ParameterInfoBoolean : ParameterInfo<bool>
        {
            public ParameterInfoBoolean(int name)
                : base(name)
            {
            }

            public override int? FixedSize
            {
                get
                {
                    return sizeof(int);
                }
            }

            public override bool Deserialize(UIntPtr memorySize, IntPtr memory)
            {
                if ((int)memorySize.ToUInt32() != FixedSize)
                    throw new InvalidOperationException();

                return Marshal.ReadInt32(memory) != 0;
            }
        }

        public sealed class ParameterInfoInt32 : ParameterInfo<int>
        {
            public ParameterInfoInt32(int name)
                : base(name)
            {
            }

            public override int? FixedSize
            {
                get
                {
                    return sizeof(int);
                }
            }

            public override int Deserialize(UIntPtr memorySize, IntPtr memory)
            {
                if ((int)memorySize.ToUInt32() != FixedSize)
                    throw new InvalidOperationException();

                return Marshal.ReadInt32(memory);
            }
        }

        public sealed class ParameterInfoInt64 : ParameterInfo<long>
        {
            public ParameterInfoInt64(int name)
                : base(name)
            {
            }

            public override int? FixedSize
            {
                get
                {
                    return sizeof(long);
                }
            }

            public override long Deserialize(UIntPtr memorySize, IntPtr memory)
            {
                if ((int)memorySize.ToUInt32() != FixedSize)
                    throw new InvalidOperationException();

                return Marshal.ReadInt64(memory);
            }
        }

        public sealed class ParameterInfoIntPtr : ParameterInfo<IntPtr>
        {
            public ParameterInfoIntPtr(int name)
                : base(name)
            {
            }

            public override int? FixedSize
            {
                get
                {
                    return IntPtr.Size;
                }
            }

            public override IntPtr Deserialize(UIntPtr memorySize, IntPtr memory)
            {
                if ((int)memorySize.ToUInt32() != FixedSize)
                    throw new InvalidOperationException();

                return Marshal.ReadIntPtr(memory);
            }
        }

        public sealed class ParameterInfoIntPtrArray : ParameterInfo<IntPtr[]>
        {
            public ParameterInfoIntPtrArray(int name)
                : base(name)
            {
            }

            public override IntPtr[] Deserialize(UIntPtr memorySize, IntPtr memory)
            {
                IntPtr[] array = new IntPtr[(int)((long)memorySize.ToUInt64() / IntPtr.Size)];
                Marshal.Copy(memory, array, 0, array.Length);
                return array;
            }
        }

        public sealed class ParameterInfoUInt32 : ParameterInfo<uint>
        {
            public ParameterInfoUInt32(int name)
                : base(name)
            {
            }

            public override int? FixedSize
            {
                get
                {
                    return sizeof(uint);
                }
            }

            public override uint Deserialize(UIntPtr memorySize, IntPtr memory)
            {
                if ((int)memorySize.ToUInt32() != FixedSize)
                    throw new InvalidOperationException();

                return (uint)Marshal.ReadInt32(memory);
            }
        }

        public sealed class ParameterInfoUInt64 : ParameterInfo<ulong>
        {
            public ParameterInfoUInt64(int name)
                : base(name)
            {
            }

            public override int? FixedSize
            {
                get
                {
                    return sizeof(ulong);
                }
            }

            public override ulong Deserialize(UIntPtr memorySize, IntPtr memory)
            {
                if ((int)memorySize.ToUInt32() != FixedSize)
                    throw new InvalidOperationException();

                return (ulong)Marshal.ReadInt64(memory);
            }
        }

        public sealed class ParameterInfoUIntPtr : ParameterInfo<UIntPtr>
        {
            public ParameterInfoUIntPtr(int name)
                : base(name)
            {
            }

            public override int? FixedSize
            {
                get
                {
                    return UIntPtr.Size;
                }
            }

            public override UIntPtr Deserialize(UIntPtr memorySize, IntPtr memory)
            {
                if ((int)memorySize.ToUInt32() != FixedSize)
                    throw new InvalidOperationException();

                return (UIntPtr)(ulong)(long)Marshal.ReadIntPtr(memory);
            }
        }

        public sealed class ParameterInfoUIntPtrArray : ParameterInfo<UIntPtr[]>
        {
            public ParameterInfoUIntPtrArray(int name)
                : base(name)
            {
            }

            public override UIntPtr[] Deserialize(UIntPtr memorySize, IntPtr memory)
            {
                IntPtr[] array = new IntPtr[(int)((long)memorySize.ToUInt64() / IntPtr.Size)];
                Marshal.Copy(memory, array, 0, array.Length);
                UIntPtr[] result = new UIntPtr[array.Length];
                System.Buffer.BlockCopy(array, 0, result, 0, array.Length * IntPtr.Size);
                return result;
            }
        }

        public enum ErrorCode
        {
            Success = 0,

            DeviceNotFound = -1,
            DeviceNotAvailable = -2,
            CompilerNotAvailable = -3,
            MemObjectAllocationFailure = -4,
            OutOfResources = -5,
            OutOfHostMemory = -6,
            ProfilingInfoNotAvailable = -7,
            MemCopyOverlap = -8,
            ImageFormatMismatch = -9,
            ImageFormatNotSupported = -10,
            BuildProgramFailure = -11,
            MapFailure = -12,
            MisalignedSubBufferOffset = -13,
            ExecStatusErrorForEventsInWaitList = -14,
            CompileProgramFailure = -15,
            LinkerNotAvailable = -16,
            LinkProgramFailure = -17,
            DevicePartitionFailed = -18,
            KernelArgInfoNotAvailable = -19,

            InvalidValue = -30,
            InvalidDeviceType = -31,
            InvalidPlatform = -32,
            InvalidDevice = -33,
            InvalidContext = -34,
            InvalidQueueProperties = -35,
            InvalidCommandQueue = -36,
            InvalidHostPtr = -37,
            InvalidMemObject = -38,
            InvalidImageFormatDescriptor = -39,
            InvalidImageSize = -40,
            InvalidSampler = -41,
            InvalidBinary = -42,
            InvalidBuildOptions = -43,
            InvalidProgram = -44,
            InvalidProgramExecutable = -45,
            InvalidKernelName = -46,
            InvalidKernelDefinition = -47,
            InvalidKernel = -48,
            InvalidArgIndex = -49,
            InvalidArgValue = -50,
            InvalidArgSize = -51,
            InvalidKernelArgs = -52,
            InvalidWorkDimension = -53,
            InvalidWorkGroupSize = -54,
            InvalidWorkItemSize = -55,
            InvalidGlobalOffset = -56,
            InvalidEventWaitList = -57,
            InvalidEvent = -58,
            InvalidOperation = -59,
            InvalidGlObject = -60,
            InvalidBufferSize = -61,
            InvalidMipLevel = -62,
            InvalidGlobalWorkSize = -63,
            InvalidProperty = -64,
            InvalidImageDescriptor = -65,
            InvalidCompilerOptions = -66,
            InvalidLinkerOptions = -67,
            InvalidDevicePartitionCount = -68,
        }
    }
}
