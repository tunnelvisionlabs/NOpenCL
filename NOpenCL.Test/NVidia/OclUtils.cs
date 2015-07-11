// Copyright (c) Tunnel Vision Laboratories, LLC. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace NOpenCL.Test.NVidia
{
    using System;
    using System.Collections.Generic;
    using NOpenCL.Extensions;

    /// <summary>
    /// Utilities specific to OpenCL samples in NVIDIA GPU Computing SDK.
    /// </summary>
    public static class OclUtils
    {
        /// <summary>
        /// Gets the platform ID for NVIDIA if available, otherwise default.
        /// </summary>
        /// <returns>The OpenCL platform.</returns>
        public static Platform GetPlatform()
        {
            Platform[] platforms = Platform.GetPlatforms();
            if (platforms.Length == 0)
                throw new PlatformNotSupportedException("No OpenCL platform found.");

            foreach (Platform platform in platforms)
            {
                if (platform.Name.Contains("NVIDIA"))
                    return platform;
            }

            // default to first platform if NVIDIA not found
            return platforms[0];
        }

        public static void PrintDeviceInfo(Device device)
        {
            if (device == null)
                throw new ArgumentNullException("device");

            Console.WriteLine("  CL_DEVICE_NAME: \t\t\t{0}", device.Name);
            Console.WriteLine("  CL_DEVICE_VENDOR: \t\t\t{0}", device.Vendor);
            Console.WriteLine("  CL_DRIVER_VERSION: \t\t\t{0}", device.DriverVersion);
            Console.WriteLine("  CL_DEVICE_VERSION: \t\t\t{0}", device.Version);

            if (!device.Version.StartsWith("OpenCL 1.0"))
            {
                Console.WriteLine("  CL_DEVICE_OPENCL_C_VERSION: \t\t{0}", device.OpenCLVersion);
            }

            DeviceType deviceType = device.DeviceType;
            if ((deviceType & DeviceType.Cpu) != 0)
                Console.WriteLine("  CL_DEVICE_TYPE:\t\t\t{0}", "CL_DEVICE_TYPE_CPU");
            if ((deviceType & DeviceType.Gpu) != 0)
                Console.WriteLine("  CL_DEVICE_TYPE:\t\t\t{0}", "CL_DEVICE_TYPE_GPU");
            if ((deviceType & DeviceType.Accelerator) != 0)
                Console.WriteLine("  CL_DEVICE_TYPE:\t\t\t{0}", "CL_DEVICE_TYPE_ACCELERATOR");
            if ((deviceType & DeviceType.Default) != 0)
                Console.WriteLine("  CL_DEVICE_TYPE:\t\t\t{0}", "CL_DEVICE_TYPE_DEFAULT");

            Console.WriteLine("  CL_DEVICE_MAX_COMPUTE_UNITS:\t\t{0}", device.MaxComputeUnits);
            Console.WriteLine("  CL_DEVICE_MAX_WORK_ITEM_DIMENSIONS:\t{0}", device.MaxWorkItemDimensions);
            Console.WriteLine("  CL_DEVICE_MAX_WORK_ITEM_SIZES:\t{0}", string.Join(" / ", device.MaxWorkItemSizes));
            Console.WriteLine("  CL_DEVICE_MAX_WORK_GROUP_SIZE:\t{0}", device.MaxWorkGroupSize);
            Console.WriteLine("  CL_DEVICE_MAX_CLOCK_FREQUENCY:\t{0} MHz", device.MaxClockFrequency);
            Console.WriteLine("  CL_DEVICE_ADDRESS_BITS:\t\t{0}", device.AddressBits);
            Console.WriteLine("  CL_DEVICE_MAX_MEM_ALLOC_SIZE:\t\t{0} MByte", device.MaxMemoryAllocationSize / (1024 * 1024));
            Console.WriteLine("  CL_DEVICE_GLOBAL_MEM_SIZE:\t\t{0} MByte", device.GlobalMemorySize / (1024 * 1024));
            Console.WriteLine("  CL_DEVICE_ERROR_CORRECTION_SUPPORT:\t{0}", device.ErrorCorrectionSupport ? "yes" : "no");
            Console.WriteLine("  CL_DEVICE_LOCAL_MEM_TYPE:\t\t{0}", device.LocalMemoryType.ToString().ToLowerInvariant());
            Console.WriteLine("  CL_DEVICE_LOCAL_MEM_SIZE:\t\t{0} KByte", device.LocalMemorySize / 1024);
            Console.WriteLine("  CL_DEVICE_MAX_CONSTANT_BUFFER_SIZE:\t{0} KByte", device.MaxConstantBufferSize / 1024);

            CommandQueueProperties commandQueueProperties = device.QueueProperties;
            if ((commandQueueProperties & CommandQueueProperties.OutOfOrderExecutionModeEnable) != 0)
                Console.WriteLine("  CL_DEVICE_QUEUE_PROPERTIES:\t\t{0}", "CL_QUEUE_OUT_OF_ORDER_EXEC_MODE_ENABLE");
            if ((commandQueueProperties & CommandQueueProperties.ProfilingEnable) != 0)
                Console.WriteLine("  CL_DEVICE_QUEUE_PROPERTIES:\t\t{0}", "CL_QUEUE_PROFILING_ENABLE");

            Console.WriteLine("  CL_DEVICE_IMAGE_SUPPORT:\t\t{0}", device.ImageSupport ? "yes" : "no");
            Console.WriteLine("  CL_DEVICE_MAX_READ_IMAGE_ARGS:\t\t{0}", device.MaxReadImageArguments);
            Console.WriteLine("  CL_DEVICE_MAX_WRITE_IMAGE_ARGS:\t\t{0}", device.MaxWriteImageArguments);

            FloatingPointConfiguration fpconfig = device.SingleFloatingPointConfiguration;
            Console.WriteLine("  CL_DEVICE_SINGLE_FP_CONFIG:\t\t{0}{1}{2}{3}{4}{5}",
                (fpconfig & FloatingPointConfiguration.Denorm) != 0 ? "denorms " : "",
                (fpconfig & FloatingPointConfiguration.InfNaN) != 0 ? "INF-quietNaNs " : "",
                (fpconfig & FloatingPointConfiguration.RoundToNearest) != 0 ? "round-to-nearest " : "",
                (fpconfig & FloatingPointConfiguration.RoundToZero) != 0 ? "round-to-zero " : "",
                (fpconfig & FloatingPointConfiguration.RoundToInf) != 0 ? "round-to-inf " : "",
                (fpconfig & FloatingPointConfiguration.Fma) != 0 ? "fma " : "");

            Console.Write("  CL_DEVICE_IMAGE <dim>");
            Console.WriteLine("\t\t\t2D_MAX_WIDTH\t {0}", device.Image2DMaxWidth);
            Console.WriteLine("\t\t\t\t\t2D_MAX_HEIGHT\t {0}", device.Image2DMaxHeight);
            Console.WriteLine("\t\t\t\t\t3D_MAX_WIDTH\t {0}", device.Image3DMaxWidth);
            Console.WriteLine("\t\t\t\t\t3D_MAX_HEIGHT\t {0}", device.Image3DMaxHeight);
            Console.WriteLine("\t\t\t\t\t3D_MAX_DEPTH\t {0}", device.Image3DMaxDepth);

            IReadOnlyList<string> extensions = device.Extensions;
            if (extensions.Count > 0)
            {
                Console.WriteLine();
                Console.Write("  CL_DEVICE_EXTENSIONS:");
                for (int i = 0; i < extensions.Count; i++)
                {
                    if (i > 0)
                        Console.Write("\t\t");

                    Console.WriteLine("\t\t\t{0}", extensions[i]);
                }
            }
            else
            {
                Console.WriteLine("  CL_DEVICE_EXTENSIONS: None");
            }

            if (NvDeviceAttributeQuery.IsSupported(device))
            {
                int computeCapabilityMajor = (int)device.GetComputeCapabilityMajorNv();
                int computeCapabilityMinor = (int)device.GetComputeCapabilityMinorNv();

                Console.WriteLine("  CL_DEVICE_COMPUTE_CAPABILITY_NV:\t{0}.{1}", computeCapabilityMajor, computeCapabilityMinor);

                Console.WriteLine("  NUMBER OF MULTIPROCESSORS:\t\t{0}", device.MaxComputeUnits);
                Console.WriteLine("  NUMBER OF CUDA CORES:\t\t\t{0}", ConvertSMVersion2Cores(computeCapabilityMajor, computeCapabilityMinor) * device.MaxComputeUnits);

                Console.WriteLine("  CL_DEVICE_REGISTERS_PER_BLOCK_NV:\t{0}", device.GetRegistersPerBlockNv());
                Console.WriteLine("  CL_DEVICE_WARP_SIZE_NV:\t{0}", device.GetWarpSizeNv());
                Console.WriteLine("  CL_DEVICE_GPU_OVERLAP_NV:\t{0}", device.GetGpuOverlapNv());
                Console.WriteLine("  CL_DEVICE_KERNEL_EXEC_TIMEOUT_NV:\t{0}", device.GetKernelExecTimeoutNv());
                Console.WriteLine("  CL_DEVICE_INTEGRATED_MEMORY_NV:\t{0}", device.GetIntegratedMemoryNv());
            }

            Console.Write("  CL_DEVICE_PREFERRED_VECTOR_WIDTH_<t>\t");
            Console.Write("CHAR {0}, SHORT {1}, INT {2}, LONG {3}, FLOAT {4}, DOUBLE {5}",
                device.PreferredVectorWidthChar,
                device.PreferredVectorWidthShort,
                device.PreferredVectorWidthInt,
                device.PreferredVectorWidthLong,
                device.PreferredVectorWidthFloat,
                device.PreferredVectorWidthDouble);
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine();
        }

        public static int ConvertSMVersion2Cores(int major, int minor)
        {
            switch ((major << 4) + minor)
            {
            case 0x10:
                // Tesla Generation (SM 1.0) G80 class
                return 8;

            case 0x11:
                // Tesla Generation (SM 1.1) G8x class
                return 8;

            case 0x12:
                // Tesla Generation (SM 1.2) G9x class
                return 8;

            case 0x13:
                // Tesla Generation (SM 1.3) GT200 class
                return 8;

            case 0x20:
                // Fermi Generation (SM 2.0) GF100 class
                return 32;

            case 0x21:
                // Fermi Generation (SM 2.1) GF10x class
                return 48;

            case 0x30:
                // Fermi Generation (SM 3.0) GK10x class
                return 192;

            default:
                return -1;
            }
        }
    }
}
