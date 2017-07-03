// Copyright (c) Tunnel Vision Laboratories, LLC. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace NOpenCL
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using NOpenCL.SafeHandles;
    using DeviceInfo = NOpenCL.UnsafeNativeMethods.DeviceInfo;

    public sealed class Device : IEquatable<Device>, IDisposable
    {
        private readonly UnsafeNativeMethods.ClDeviceID _device;
        private readonly DeviceSafeHandle _handle;

        private bool _disposed;

        internal Device(UnsafeNativeMethods.ClDeviceID device)
        {
            _device = device;
        }

        internal Device(UnsafeNativeMethods.ClDeviceID device, DeviceSafeHandle handle)
        {
            if (handle == null)
                throw new ArgumentNullException(nameof(handle));

            _device = device;
            _handle = handle;
        }

        /// <summary>
        /// The default compute device address space size specified as an unsigned integer
        /// value in bits. Currently supported values are 32 or 64 bits.
        /// </summary>
        public uint AddressBits
        {
            get
            {
                return UnsafeNativeMethods.GetDeviceInfo(ID, DeviceInfo.AddressBits);
            }
        }

        /// <summary>
        /// Is true if the device is available and false if the device is not available.
        /// </summary>
        public bool Available
        {
            get
            {
                return UnsafeNativeMethods.GetDeviceInfo(ID, DeviceInfo.Available);
            }
        }

        /// <summary>
        /// A list of built-in kernels supported by the device. An empty list is
        /// returned if no built-in kernels are supported by the device.
        /// </summary>
        public IReadOnlyList<string> BuiltInKernels
        {
            get
            {
                return UnsafeNativeMethods.GetDeviceInfo(ID, DeviceInfo.BuiltInKernels).Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            }
        }

        /// <summary>
        /// Is false if the implementation does not have a compiler available to compile
        /// the program source. Is true if the compiler is available. This can be false
        /// for the embedded platform profile only.
        /// </summary>
        public bool CompilerAvailable
        {
            get
            {
                return UnsafeNativeMethods.GetDeviceInfo(ID, DeviceInfo.CompilerAvailable);
            }
        }

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
        public FloatingPointConfiguration DoubleFloatingPointConfiguration
        {
            get
            {
                return (FloatingPointConfiguration)UnsafeNativeMethods.GetDeviceInfo(ID, DeviceInfo.DoubleFloatingPointConfiguration);
            }
        }

        /// <summary>
        /// Is true if the OpenCL device is a little endian device and false otherwise.
        /// </summary>
        public bool LittleEndian
        {
            get
            {
                return UnsafeNativeMethods.GetDeviceInfo(ID, DeviceInfo.LittleEndian);
            }
        }

        /// <summary>
        /// Is true if the device implements error correction for all accesses to compute
        /// device memory (global and constant). Is false if the device does not implement
        /// such error correction.
        /// </summary>
        public bool ErrorCorrectionSupport
        {
            get
            {
                return UnsafeNativeMethods.GetDeviceInfo(ID, DeviceInfo.ErrorCorrectionSupport);
            }
        }

        /// <summary>
        /// Describes the execution capabilities of the device.
        /// The mandated minimum capability is <see cref="NOpenCL.ExecutionCapabilities.Kernel"/>.
        /// </summary>
        public ExecutionCapabilities ExecutionCapabilities
        {
            get
            {
                return (ExecutionCapabilities)UnsafeNativeMethods.GetDeviceInfo(ID, DeviceInfo.ExecutionCapabilities);
            }
        }

        /// <summary>
        /// Returns a list of extension names supported by the device. The list
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
        public IReadOnlyList<string> Extensions
        {
            get
            {
                return UnsafeNativeMethods.GetDeviceInfo(ID, DeviceInfo.Extensions).Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            }
        }

        /// <summary>
        /// Size of global memory cache in bytes.
        /// </summary>
        public ulong GlobalCacheSize
        {
            get
            {
                return UnsafeNativeMethods.GetDeviceInfo(ID, DeviceInfo.GlobalCacheSize);
            }
        }

        /// <summary>
        /// Type of global memory cache supported.
        /// </summary>
        public CacheType GlobalCacheType
        {
            get
            {
                return (CacheType)UnsafeNativeMethods.GetDeviceInfo(ID, DeviceInfo.GlobalCacheType);
            }
        }

        /// <summary>
        /// Size of global memory cache line in bytes.
        /// </summary>
        public uint GlobalCacheLineSize
        {
            get
            {
                return UnsafeNativeMethods.GetDeviceInfo(ID, DeviceInfo.GlobalCacheLineSize);
            }
        }

        /// <summary>
        /// Size of global device memory in bytes.
        /// </summary>
        public ulong GlobalMemorySize
        {
            get
            {
                return UnsafeNativeMethods.GetDeviceInfo(ID, DeviceInfo.GlobalMemorySize);
            }
        }

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
        public FloatingPointConfiguration HalfFloatingPointConfiguration
        {
            get
            {
                return (FloatingPointConfiguration)UnsafeNativeMethods.GetDeviceInfo(ID, DeviceInfo.HalfFloatingPointConfiguration);
            }
        }

        /// <summary>
        /// Is true if the device and the host have a unified memory subsystem and is false otherwise.
        /// </summary>
        public bool HostUnifiedMemory
        {
            get
            {
                return UnsafeNativeMethods.GetDeviceInfo(ID, DeviceInfo.HostUnifiedMemory);
            }
        }

        /// <summary>
        /// Is true if images are supported by the OpenCL device and false otherwise.
        /// </summary>
        public bool ImageSupport
        {
            get
            {
                return UnsafeNativeMethods.GetDeviceInfo(ID, DeviceInfo.ImageSupport);
            }
        }

        /// <summary>
        /// Max height of 2D image in pixels. The minimum value is 8192 if <see cref="ImageSupport"/> is true.
        /// </summary>
        public UIntPtr Image2DMaxHeight
        {
            get
            {
                return UnsafeNativeMethods.GetDeviceInfo(ID, DeviceInfo.Image2DMaxHeight);
            }
        }

        /// <summary>
        /// Max width of 2D image or 1D image not created from a buffer object in pixels. The minimum value is 8192 if <see cref="ImageSupport"/> is true.
        /// </summary>
        public UIntPtr Image2DMaxWidth
        {
            get
            {
                return UnsafeNativeMethods.GetDeviceInfo(ID, DeviceInfo.Image2DMaxWidth);
            }
        }

        /// <summary>
        /// Max depth of 3D image in pixels. The minimum value is 2048 if <see cref="ImageSupport"/> is true.
        /// </summary>
        public UIntPtr Image3DMaxDepth
        {
            get
            {
                return UnsafeNativeMethods.GetDeviceInfo(ID, DeviceInfo.Image3DMaxDepth);
            }
        }

        /// <summary>
        /// Max height of 3D image in pixels. The minimum value is 2048 if <see cref="ImageSupport"/> is true.
        /// </summary>
        public UIntPtr Image3DMaxHeight
        {
            get
            {
                return UnsafeNativeMethods.GetDeviceInfo(ID, DeviceInfo.Image3DMaxHeight);
            }
        }

        /// <summary>
        /// Max width of 3D image in pixels. The minimum value is 2048 if <see cref="ImageSupport"/> is true.
        /// </summary>
        public UIntPtr Image3DMaxWidth
        {
            get
            {
                return UnsafeNativeMethods.GetDeviceInfo(ID, DeviceInfo.Image3DMaxWidth);
            }
        }

        /// <summary>
        /// Max number of pixels for a 1D image created from a buffer object. The minimum value is 65536 if <see cref="ImageSupport"/> is true.
        /// </summary>
        public UIntPtr ImageMaxBufferSize
        {
            get
            {
                return UnsafeNativeMethods.GetDeviceInfo(ID, DeviceInfo.ImageMaxBufferSize);
            }
        }

        /// <summary>
        /// Max number of images in a 1D or 2D image array. The minimum value is 2048 if <see cref="ImageSupport"/> is true.
        /// </summary>
        public UIntPtr ImageMaxArraySize
        {
            get
            {
                return UnsafeNativeMethods.GetDeviceInfo(ID, DeviceInfo.ImageMaxArraySize);
            }
        }

        /// <summary>
        /// Is false if the implementation does not have a linker available. Is true
        /// if the linker is available. This can be false for the embedded platform
        /// profile only. This must be true if <see cref="CompilerAvailable"/> is true.
        /// </summary>
        public bool LinkerAvailable
        {
            get
            {
                return UnsafeNativeMethods.GetDeviceInfo(ID, DeviceInfo.LinkerAvailable);
            }
        }

        /// <summary>
        /// Size of local memory arena in bytes. The minimum value is 32 KB for devices
        /// that are not of type <see cref="NOpenCL.DeviceType.Custom"/>.
        /// </summary>
        public ulong LocalMemorySize
        {
            get
            {
                return UnsafeNativeMethods.GetDeviceInfo(ID, DeviceInfo.LocalMemorySize);
            }
        }

        /// <summary>
        /// Type of local memory supported. This can be set to <see cref="NOpenCL.LocalMemoryType.Local"/>
        /// implying dedicated local memory storage such as SRAM, or <see cref="NOpenCL.LocalMemoryType.Global"/>.
        /// For custom devices, <see cref="NOpenCL.LocalMemoryType.None"/> can also be returned indicating
        /// no local memory support.
        /// </summary>
        public LocalMemoryType LocalMemoryType
        {
            get
            {
                return (LocalMemoryType)UnsafeNativeMethods.GetDeviceInfo(ID, DeviceInfo.LocalMemoryType);
            }
        }

        /// <summary>
        /// Maximum configured clock frequency of the device in MHz.
        /// </summary>
        public uint MaxClockFrequency
        {
            get
            {
                return UnsafeNativeMethods.GetDeviceInfo(ID, DeviceInfo.MaxClockFrequency);
            }
        }

        /// <summary>
        /// The number of parallel compute units on the OpenCL device. A work-group
        /// executes on a single compute unit. The minimum value is 1.
        /// </summary>
        public uint MaxComputeUnits
        {
            get
            {
                return UnsafeNativeMethods.GetDeviceInfo(ID, DeviceInfo.MaxComputeUnits);
            }
        }

        /// <summary>
        /// Max number of arguments declared with the <c>__constant</c> qualifier in
        /// a kernel. The minimum value is 8 for devices that are not of type <see cref="NOpenCL.DeviceType.Custom"/>.
        /// </summary>
        public uint MaxConstantArguments
        {
            get
            {
                return UnsafeNativeMethods.GetDeviceInfo(ID, DeviceInfo.MaxConstantArguments);
            }
        }

        /// <summary>
        /// Max size in bytes of a constant buffer allocation. The minimum value is 64
        /// KB for devices that are not of type <see cref="NOpenCL.DeviceType.Custom"/>.
        /// </summary>
        public ulong MaxConstantBufferSize
        {
            get
            {
                return UnsafeNativeMethods.GetDeviceInfo(ID, DeviceInfo.MaxConstantBufferSize);
            }
        }

        /// <summary>
        /// Max size of memory object allocation in bytes. The minimum value is max
        /// (1/4th of <see cref="GlobalMemorySize"/>, 128*1024*1024) for devices that are
        /// not of type <see cref="NOpenCL.DeviceType.Custom"/>.
        /// </summary>
        public ulong MaxMemoryAllocationSize
        {
            get
            {
                return UnsafeNativeMethods.GetDeviceInfo(ID, DeviceInfo.MaxMemoryAllocationSize);
            }
        }

        /// <summary>
        /// Max size in bytes of the arguments that can be passed to a kernel. The
        /// minimum value is 1024 for devices that are not of type <see cref="NOpenCL.DeviceType.Custom"/>.
        /// For this minimum value, only a maximum of 128 arguments can be passed to a kernel.
        /// </summary>
        public UIntPtr MaxParameterSize
        {
            get
            {
                return UnsafeNativeMethods.GetDeviceInfo(ID, DeviceInfo.MaxParameterSize);
            }
        }

        /// <summary>
        /// Max number of simultaneous image objects that can be read by a kernel.
        /// The minimum value is 128 if <see cref="ImageSupport"/> is true.
        /// </summary>
        public uint MaxReadImageArguments
        {
            get
            {
                return UnsafeNativeMethods.GetDeviceInfo(ID, DeviceInfo.MaxReadImageArguments);
            }
        }

        /// <summary>
        /// Maximum number of samplers that can be used in a kernel. The minimum value
        /// is 16 if <see cref="ImageSupport"/> is true.
        /// </summary>
        /// <seealso cref="Sampler"/>
        public uint MaxSamplers
        {
            get
            {
                return UnsafeNativeMethods.GetDeviceInfo(ID, DeviceInfo.MaxSamplers);
            }
        }

        /// <summary>
        /// Maximum number of work-items in a work-group executing a kernel on a single
        /// compute unit, using the data parallel execution model.
        /// (Refer to <see cref="CommandQueue.EnqueueNDRangeKernel(Kernel, IntPtr, IntPtr, Event[])" autoUpgrade="true"/>).
        /// The minimum value is 1.
        /// </summary>
        public UIntPtr MaxWorkGroupSize
        {
            get
            {
                return UnsafeNativeMethods.GetDeviceInfo(ID, DeviceInfo.MaxWorkGroupSize);
            }
        }

        /// <summary>
        /// Maximum dimensions that specify the global and local work-item IDs used by
        /// the data parallel execution model. (Refer to
        /// <see cref="CommandQueue.EnqueueNDRangeKernel(Kernel, IntPtr, IntPtr, Event[])" autoUpgrade="true"/>). The
        /// minimum value is 3 for devices that are not of type <see cref="NOpenCL.DeviceType.Custom"/>.
        /// </summary>
        public uint MaxWorkItemDimensions
        {
            get
            {
                return UnsafeNativeMethods.GetDeviceInfo(ID, DeviceInfo.MaxWorkItemDimensions);
            }
        }

        /// <summary>
        /// Maximum number of work-items that can be specified in each dimension of
        /// the work-group to <see cref="CommandQueue.EnqueueNDRangeKernel(Kernel, IntPtr, IntPtr, Event[])" autoUpgrade="true"/>.
        /// <para/>
        /// Returns <em>n</em> <see cref="IntPtr"/> entries, where <em>n</em> is the
        /// value returned by the query for <see cref="MaxWorkItemDimensions"/>.
        /// <para/>
        /// The minimum value is (1, 1, 1) for devices that are not of type <see cref="NOpenCL.DeviceType.Custom"/>.
        /// </summary>
        public IReadOnlyList<UIntPtr> MaxWorkItemSizes
        {
            get
            {
                return UnsafeNativeMethods.GetDeviceInfo(ID, DeviceInfo.MaxWorkItemSizes);
            }
        }

        /// <summary>
        /// Max number of simultaneous image objects that can be written to by a
        /// kernel. The minimum value is 8 if <see cref="ImageSupport"/> is true.
        /// </summary>
        public uint MaxWriteImageArguments
        {
            get
            {
                return UnsafeNativeMethods.GetDeviceInfo(ID, DeviceInfo.MaxWriteImageArguments);
            }
        }

        /// <summary>
        /// The minimum value is the size (in bits) of the largest OpenCL built-in
        /// data type supported by the device (long16 in FULL profile, long16 or
        /// int16 in EMBEDDED profile) for devices that are not of type
        /// <see cref="NOpenCL.DeviceType.Custom"/>.
        /// </summary>
        public uint MemoryBaseAddressAlignment
        {
            get
            {
                return UnsafeNativeMethods.GetDeviceInfo(ID, DeviceInfo.MemoryBaseAddressAlignment);
            }
        }

        /// <summary>
        /// The smallest alignment in bytes which can be used for any data type.
        /// </summary>
        [Obsolete("Deprecated in OpenCL 1.2")]
        public uint MinDataTypeAlignmentSize
        {
            get
            {
                return UnsafeNativeMethods.GetDeviceInfo(ID, DeviceInfo.MinDataTypeAlignment);
            }
        }

        /// <summary>
        /// The device name.
        /// </summary>
        public string Name
        {
            get
            {
                return UnsafeNativeMethods.GetDeviceInfo(ID, DeviceInfo.Name);
            }
        }

        /// <summary>
        /// Returns the native ISA vector width. The vector width is defined as
        /// the number of scalar elements that can be stored in the vector.
        /// </summary>
        public uint NativeVectorWidthChar
        {
            get
            {
                return UnsafeNativeMethods.GetDeviceInfo(ID, DeviceInfo.NativeVectorWidthChar);
            }
        }

        /// <summary>
        /// Returns the native ISA vector width. The vector width is defined as
        /// the number of scalar elements that can be stored in the vector.
        /// </summary>
        public uint NativeVectorWidthShort
        {
            get
            {
                return UnsafeNativeMethods.GetDeviceInfo(ID, DeviceInfo.NativeVectorWidthShort);
            }
        }

        /// <summary>
        /// Returns the native ISA vector width. The vector width is defined as
        /// the number of scalar elements that can be stored in the vector.
        /// </summary>
        public uint NativeVectorWidthInt
        {
            get
            {
                return UnsafeNativeMethods.GetDeviceInfo(ID, DeviceInfo.NativeVectorWidthInt);
            }
        }

        /// <summary>
        /// Returns the native ISA vector width. The vector width is defined as
        /// the number of scalar elements that can be stored in the vector.
        /// </summary>
        public uint NativeVectorWidthLong
        {
            get
            {
                return UnsafeNativeMethods.GetDeviceInfo(ID, DeviceInfo.NativeVectorWidthLong);
            }
        }

        /// <summary>
        /// Returns the native ISA vector width. The vector width is defined as
        /// the number of scalar elements that can be stored in the vector.
        /// </summary>
        public uint NativeVectorWidthFloat
        {
            get
            {
                return UnsafeNativeMethods.GetDeviceInfo(ID, DeviceInfo.NativeVectorWidthFloat);
            }
        }

        /// <summary>
        /// Returns the native ISA vector width. The vector width is defined as
        /// the number of scalar elements that can be stored in the vector.
        ///
        /// <para>If double precision is not supported, this must return 0.</para>
        /// </summary>
        public uint NativeVectorWidthDouble
        {
            get
            {
                return UnsafeNativeMethods.GetDeviceInfo(ID, DeviceInfo.NativeVectorWidthDouble);
            }
        }

        /// <summary>
        /// Returns the native ISA vector width. The vector width is defined as
        /// the number of scalar elements that can be stored in the vector.
        ///
        /// <para>If the <c>cl_khr_fp16</c> extension is not supported, this must return 0.</para>
        /// </summary>
        public uint NativeVectorWidthHalf
        {
            get
            {
                return UnsafeNativeMethods.GetDeviceInfo(ID, DeviceInfo.NativeVectorWidthHalf);
            }
        }

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
        public string OpenCLVersion
        {
            get
            {
                return UnsafeNativeMethods.GetDeviceInfo(ID, DeviceInfo.OpenCLVersion);
            }
        }

        /// <summary>
        /// Returns the the parent <see cref="Device"/> to which this sub-device belongs.
        /// If device is a root-level device, this property is <c>null</c>.
        /// </summary>
        public Device Parent
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Returns the maximum number of sub-devices that can be created when a
        /// device is partitioned. The value returned cannot exceed <see cref="MaxComputeUnits"/>.
        /// </summary>
        public uint PartitionMaxSubDevices
        {
            get
            {
                return UnsafeNativeMethods.GetDeviceInfo(ID, DeviceInfo.PartitionMaxSubDevices);
            }
        }

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
        public IReadOnlyList<PartitionProperty> PartitionProperties
        {
            get
            {
                IntPtr[] result = UnsafeNativeMethods.GetDeviceInfo(ID, DeviceInfo.PartitionProperties);
                return Array.ConvertAll(result, partitionProperty => (PartitionProperty)partitionProperty);
            }
        }

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
        public AffinityDomain PartitionAffinityDomain
        {
            get
            {
                return (AffinityDomain)UnsafeNativeMethods.GetDeviceInfo(ID, DeviceInfo.PartitionAffinityDomain);
            }
        }

        public IReadOnlyList<PartitionProperty> PartitionType
        {
            get
            {
                IntPtr[] result = UnsafeNativeMethods.GetDeviceInfo(ID, DeviceInfo.PartitionType);
                return Array.ConvertAll(result, partitionProperty => (PartitionProperty)partitionProperty);
            }
        }

        /// <summary>
        /// The platform associated with this device.
        /// </summary>
        public Platform Platform
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Preferred native vector width size for built-in scalar types that can be put
        /// into vectors. The vector width is defined as the number of scalar elements
        /// that can be stored in the vector.
        /// </summary>
        public uint PreferredVectorWidthChar
        {
            get
            {
                return UnsafeNativeMethods.GetDeviceInfo(ID, DeviceInfo.PreferredVectorWidthChar);
            }
        }

        /// <summary>
        /// Preferred native vector width size for built-in scalar types that can be put
        /// into vectors. The vector width is defined as the number of scalar elements
        /// that can be stored in the vector.
        /// </summary>
        public uint PreferredVectorWidthShort
        {
            get
            {
                return UnsafeNativeMethods.GetDeviceInfo(ID, DeviceInfo.PreferredVectorWidthShort);
            }
        }

        /// <summary>
        /// Preferred native vector width size for built-in scalar types that can be put
        /// into vectors. The vector width is defined as the number of scalar elements
        /// that can be stored in the vector.
        /// </summary>
        public uint PreferredVectorWidthInt
        {
            get
            {
                return UnsafeNativeMethods.GetDeviceInfo(ID, DeviceInfo.PreferredVectorWidthInt);
            }
        }

        /// <summary>
        /// Preferred native vector width size for built-in scalar types that can be put
        /// into vectors. The vector width is defined as the number of scalar elements
        /// that can be stored in the vector.
        /// </summary>
        public uint PreferredVectorWidthLong
        {
            get
            {
                return UnsafeNativeMethods.GetDeviceInfo(ID, DeviceInfo.PreferredVectorWidthLong);
            }
        }

        /// <summary>
        /// Preferred native vector width size for built-in scalar types that can be put
        /// into vectors. The vector width is defined as the number of scalar elements
        /// that can be stored in the vector.
        /// </summary>
        public uint PreferredVectorWidthFloat
        {
            get
            {
                return UnsafeNativeMethods.GetDeviceInfo(ID, DeviceInfo.PreferredVectorWidthFloat);
            }
        }

        /// <summary>
        /// Preferred native vector width size for built-in scalar types that can be put
        /// into vectors. The vector width is defined as the number of scalar elements
        /// that can be stored in the vector.
        ///
        /// <para>If double precision is not supported, this must return 0.</para>
        /// </summary>
        public uint PreferredVectorWidthDouble
        {
            get
            {
                return UnsafeNativeMethods.GetDeviceInfo(ID, DeviceInfo.PreferredVectorWidthDouble);
            }
        }

        /// <summary>
        /// Preferred native vector width size for built-in scalar types that can be put
        /// into vectors. The vector width is defined as the number of scalar elements
        /// that can be stored in the vector.
        ///
        /// <para>If the <c>cl_khr_fp16</c> extension is not supported, this must return 0.</para>
        /// </summary>
        public uint PreferredVectorWidthHalf
        {
            get
            {
                return UnsafeNativeMethods.GetDeviceInfo(ID, DeviceInfo.PreferredVectorWidthHalf);
            }
        }

        /// <summary>
        /// Maximum size of the internal buffer that holds the output of printf calls from
        /// a kernel. The minimum value for the FULL profile is 1 MB.
        /// </summary>
        public UIntPtr PrintfBufferSize
        {
            get
            {
                return UnsafeNativeMethods.GetDeviceInfo(ID, DeviceInfo.PrintfBufferSize);
            }
        }

        /// <summary>
        /// Is true if the device's preference is for the user to be responsible for
        /// synchronization, when sharing memory objects between OpenCL and other APIs
        /// such as DirectX, false if the device / implementation has a performant path
        /// for performing synchronization of memory object shared between OpenCL and
        /// other APIs such as DirectX.
        /// </summary>
        public bool PreferredInteropUserSync
        {
            get
            {
                return UnsafeNativeMethods.GetDeviceInfo(ID, DeviceInfo.PreferredInteropUserSync);
            }
        }

        /// <summary>
        /// OpenCL profile string. Returns the profile name supported by the device
        /// (see note). The profile name returned can be one of the following strings:
        ///
        /// <list type="bullet">
        /// <item>FULL_PROFILE - if the device supports the OpenCL specification (functionality defined as part of the core specification and does not require any extensions to be supported).</item>
        /// <item>EMBEDDED_PROFILE - if the device supports the OpenCL embedded profile.</item>
        /// </list>
        /// </summary>
        public string Profile
        {
            get
            {
                return UnsafeNativeMethods.GetDeviceInfo(ID, DeviceInfo.Profile);
            }
        }

        /// <summary>
        /// Describes the resolution of device timer. This is measured in nanoseconds.
        /// </summary>
        public UIntPtr ProfilingTimerResolution
        {
            get
            {
                return UnsafeNativeMethods.GetDeviceInfo(ID, DeviceInfo.ProfilingTimerResolution);
            }
        }

        /// <summary>
        /// Describes the command-queue properties supported by the device.
        /// </summary>
        public CommandQueueProperties QueueProperties
        {
            get
            {
                return (CommandQueueProperties)UnsafeNativeMethods.GetDeviceInfo(ID, DeviceInfo.QueueProperties);
            }
        }

        /// <summary>
        /// Returns the device reference count. If the device is a root-level device,
        /// a reference count of one is returned.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public uint ReferenceCount
        {
            get
            {
                return UnsafeNativeMethods.GetDeviceInfo(ID, DeviceInfo.ReferenceCount);
            }
        }

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
        public FloatingPointConfiguration SingleFloatingPointConfiguration
        {
            get
            {
                return (FloatingPointConfiguration)UnsafeNativeMethods.GetDeviceInfo(ID, DeviceInfo.SingleFloatingPointConfiguration);
            }
        }

        /// <summary>
        /// The OpenCL device type. Currently supported values are one of or a
        /// combination of: <see cref="NOpenCL.DeviceType.Cpu"/>, <see cref="NOpenCL.DeviceType.Gpu"/>,
        /// <see cref="NOpenCL.DeviceType.Accelerator"/>, <see cref="NOpenCL.DeviceType.Default"/>, a
        /// combination of the above types, or <see cref="NOpenCL.DeviceType.Custom"/>.
        /// </summary>
        public DeviceType DeviceType
        {
            get
            {
                return (DeviceType)UnsafeNativeMethods.GetDeviceInfo(ID, DeviceInfo.DeviceType);
            }
        }

        /// <summary>
        /// The vendor name.
        /// </summary>
        public string Vendor
        {
            get
            {
                return UnsafeNativeMethods.GetDeviceInfo(ID, DeviceInfo.Vendor);
            }
        }

        /// <summary>
        /// A unique device vendor identifier. An example of a unique device identifier
        /// could be the PCIe ID.
        /// </summary>
        public uint VendorID
        {
            get
            {
                return UnsafeNativeMethods.GetDeviceInfo(ID, DeviceInfo.VendorID);
            }
        }

        /// <summary>
        /// OpenCL version string. Returns the OpenCL version supported by the device.
        /// This version string has the following format:
        /// <para/>
        /// <em>OpenCL&lt;space&gt;&lt;major_version.minor_version&gt;&lt;space&gt;&lt;vendor-specific information&gt;</em>
        /// <para/>
        /// The <em>major_version.minor_version</em> value returned will be 1.1.
        /// </summary>
        public string Version
        {
            get
            {
                return UnsafeNativeMethods.GetDeviceInfo(ID, DeviceInfo.Version);
            }
        }

        /// <summary>
        /// OpenCL software driver version string in the form <em>major_number.minor_number</em>.
        /// </summary>
        public string DriverVersion
        {
            get
            {
                return UnsafeNativeMethods.GetDeviceInfo(ID, DeviceInfo.DriverVersion);
            }
        }

        internal UnsafeNativeMethods.ClDeviceID ID
        {
            get
            {
                ThrowIfDisposed();
                return _device;
            }
        }

        /// <summary>
        /// Split the aggregate device into as many smaller aggregate devices as can
        /// be created, each containing <paramref name="partitionSize"/> compute units.
        /// If <paramref name="partitionSize"/> does not divide evenly into
        /// <see cref="MaxComputeUnits"/>, then the remaining compute units are not used.
        /// </summary>
        /// <param name="partitionSize">The size of the partitions to be created.</param>
        /// <returns>A collection of sub-devices representing the created partitions.</returns>
        public DisposableCollection<Device> PartitionEqually(int partitionSize)
        {
            return UnsafeNativeMethods.PartitionEqually(ID, partitionSize);
        }

        /// <summary>
        /// Split the aggregate device into smaller aggregate devices according to the
        /// specified <paramref name="partitionSizes"/>. For each nonzero count <em>m</em>
        /// in the list, a sub-device is created with <em>m</em> compute units in it.
        /// <para/>
        /// The number of non-zero count entries in the list may not exceed <see cref="PartitionMaxSubDevices"/>.
        /// The total number of compute units specified may not exceed <see cref="MaxComputeUnits"/>.
        /// </summary>
        /// <param name="partitionSizes">The size of the partitions to be created.</param>
        /// <returns>A collection of sub-devices representing the created partitions.</returns>
        public DisposableCollection<Device> Partition(params int[] partitionSizes)
        {
            if (partitionSizes == null)
                throw new ArgumentNullException(nameof(partitionSizes));
            if (partitionSizes.Length == 0)
                throw new ArgumentException($"{nameof(partitionSizes)} cannot be empty", nameof(partitionSizes));

            return UnsafeNativeMethods.PartitionByCounts(ID, partitionSizes);
        }

        /// <summary>
        /// Split the device into smaller aggregate devices containing one or more compute
        /// units that all share part of a cache hierarchy. The value accompanying this
        /// property may be drawn from the following list:
        ///
        /// <list type="bullet">
        /// <item><see cref="AffinityDomain.Numa"/> - Split the device into sub-devices comprised of compute units that share a NUMA node.</item>
        /// <item><see cref="AffinityDomain.L4Cache"/> - Split the device into sub-devices comprised of compute units that share a level 4 data cache.</item>
        /// <item><see cref="AffinityDomain.L3Cache"/> - Split the device into sub-devices comprised of compute units that share a level 3 data cache.</item>
        /// <item><see cref="AffinityDomain.L2Cache"/> - Split the device into sub-devices comprised of compute units that share a level 2 data cache.</item>
        /// <item><see cref="AffinityDomain.L1Cache"/> - Split the device into sub-devices comprised of compute units that share a level 1 data cache.</item>
        /// <item><see cref="AffinityDomain.NextPartitionable"/> - Split the device along the next partitionable affinity domain. The implementation shall find the first level along which the device or sub-device may be further subdivided in the order NUMA, L4, L3, L2, L1, and partition the device into sub-devices comprised of compute units that share memory subsystems at this level.</item>
        /// </list>
        ///
        /// The user may determine what happened by checking <see cref="PartitionType"/> on the sub-devices.
        /// </summary>
        /// <param name="affinityDomain">Specifies the cache hierarchy shared by the partitioned compute units.</param>
        /// <returns>A collection of sub-devices representing the created partitions.</returns>
        public DisposableCollection<Device> PartitionByAffinityDomain(AffinityDomain affinityDomain)
        {
            return UnsafeNativeMethods.PartitionByAffinityDomain(ID, affinityDomain);
        }

        public void Dispose()
        {
            if (_handle != null)
                _handle.Dispose();

            _disposed = true;
            GC.SuppressFinalize(this);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as Device);
        }

        public bool Equals(Device other)
        {
            if (other == this)
                return true;
            else if (other == null)
                return false;

            return object.Equals(_device, other._device);
        }

        public override int GetHashCode()
        {
            return _device.GetHashCode();
        }

        private void ThrowIfDisposed()
        {
            if (_disposed)
                throw new ObjectDisposedException(GetType().FullName);
        }
    }
}
