// Copyright (c) Tunnel Vision Laboratories, LLC. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

#pragma warning disable IDE1006 // Naming Styles

namespace NOpenCL
{
    using System;
    using System.Runtime.InteropServices;

    /// <content>
    /// Devices.
    /// </content>
    internal partial class UnsafeNativeMethods
    {
        [DllImport(ExternDll.OpenCL)]
        private static extern ErrorCode clGetDeviceIDs(
            ClPlatformID platform,
            DeviceType deviceType,
            uint numEntries,
            [Out, MarshalAs(UnmanagedType.LPArray)] ClDeviceID[] devices,
            out uint numDevices);

        public static ClDeviceID[] GetDeviceIDs(ClPlatformID platform, DeviceType deviceType)
        {
            uint required;
            ErrorCode result = clGetDeviceIDs(platform, deviceType, 0, null, out required);
            if (result == ErrorCode.DeviceNotFound && required == 0)
                return new ClDeviceID[0];

            ErrorHandler.ThrowOnFailure(result);

            ClDeviceID[] devices = new ClDeviceID[required];
            uint actual;
            ErrorHandler.ThrowOnFailure(clGetDeviceIDs(platform, deviceType, required, devices, out actual));
            Array.Resize(ref devices, (int)actual);
            return devices;
        }

        [DllImport(ExternDll.OpenCL)]
        private static extern ErrorCode clGetDeviceInfo(
            ClDeviceID device,
            int paramName,
            UIntPtr paramValueSize,
            IntPtr paramValue,
            out UIntPtr paramValueSizeRet);

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
                    throw new ArgumentNullException(nameof(parameterInfo));

                _parameterInfo = parameterInfo;
            }

            public static explicit operator DeviceParameterInfo<T>(ParameterInfo<T> parameterInfo)
            {
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
    }
}
