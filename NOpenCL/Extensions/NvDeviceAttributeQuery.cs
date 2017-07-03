// Copyright (c) Tunnel Vision Laboratories, LLC. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace NOpenCL.Extensions
{
    using System;
    using System.Linq;

    public static class NvDeviceAttributeQuery
    {
        public static readonly string ExtensionName = "cl_nv_device_attribute_query";

        private static readonly UnsafeNativeMethods.DeviceParameterInfo<uint> ComputeCapabilityMajorNv
            = (UnsafeNativeMethods.DeviceParameterInfo<uint>)new UnsafeNativeMethods.ParameterInfoUInt32(0x4000);

        private static readonly UnsafeNativeMethods.DeviceParameterInfo<uint> ComputeCapabilityMinorNv
            = (UnsafeNativeMethods.DeviceParameterInfo<uint>)new UnsafeNativeMethods.ParameterInfoUInt32(0x4001);

        private static readonly UnsafeNativeMethods.DeviceParameterInfo<uint> RegistersPerBlockNv
            = (UnsafeNativeMethods.DeviceParameterInfo<uint>)new UnsafeNativeMethods.ParameterInfoUInt32(0x4002);

        private static readonly UnsafeNativeMethods.DeviceParameterInfo<uint> WarpSizeNv
            = (UnsafeNativeMethods.DeviceParameterInfo<uint>)new UnsafeNativeMethods.ParameterInfoUInt32(0x4003);

        private static readonly UnsafeNativeMethods.DeviceParameterInfo<bool> GpuOverlapNv
            = (UnsafeNativeMethods.DeviceParameterInfo<bool>)new UnsafeNativeMethods.ParameterInfoBoolean(0x4004);

        private static readonly UnsafeNativeMethods.DeviceParameterInfo<bool> KernelExecTimeoutNv
            = (UnsafeNativeMethods.DeviceParameterInfo<bool>)new UnsafeNativeMethods.ParameterInfoBoolean(0x4005);

        private static readonly UnsafeNativeMethods.DeviceParameterInfo<bool> IntegratedMemoryNv
            = (UnsafeNativeMethods.DeviceParameterInfo<bool>)new UnsafeNativeMethods.ParameterInfoBoolean(0x4006);

        public static bool IsSupported(Device device)
        {
            if (device == null)
                throw new ArgumentNullException(nameof(device));

            return device.Extensions.Contains(ExtensionName);
        }

        public static uint GetComputeCapabilityMajorNv(this Device device)
        {
            if (device == null)
                throw new ArgumentNullException(nameof(device));

            return UnsafeNativeMethods.GetDeviceInfo(device.ID, ComputeCapabilityMajorNv);
        }

        public static uint GetComputeCapabilityMinorNv(this Device device)
        {
            if (device == null)
                throw new ArgumentNullException(nameof(device));

            return UnsafeNativeMethods.GetDeviceInfo(device.ID, ComputeCapabilityMinorNv);
        }

        public static uint GetRegistersPerBlockNv(this Device device)
        {
            if (device == null)
                throw new ArgumentNullException(nameof(device));

            return UnsafeNativeMethods.GetDeviceInfo(device.ID, RegistersPerBlockNv);
        }

        public static uint GetWarpSizeNv(this Device device)
        {
            if (device == null)
                throw new ArgumentNullException(nameof(device));

            return UnsafeNativeMethods.GetDeviceInfo(device.ID, WarpSizeNv);
        }

        public static bool GetGpuOverlapNv(this Device device)
        {
            if (device == null)
                throw new ArgumentNullException(nameof(device));

            return UnsafeNativeMethods.GetDeviceInfo(device.ID, GpuOverlapNv);
        }

        public static bool GetKernelExecTimeoutNv(this Device device)
        {
            if (device == null)
                throw new ArgumentNullException(nameof(device));

            return UnsafeNativeMethods.GetDeviceInfo(device.ID, KernelExecTimeoutNv);
        }

        public static bool GetIntegratedMemoryNv(this Device device)
        {
            if (device == null)
                throw new ArgumentNullException(nameof(device));

            return UnsafeNativeMethods.GetDeviceInfo(device.ID, IntegratedMemoryNv);
        }
    }
}
