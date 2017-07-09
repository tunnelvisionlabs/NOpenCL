// Copyright (c) Tunnel Vision Laboratories, LLC. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

#pragma warning disable IDE1006 // Naming Styles

namespace NOpenCL
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using NOpenCL.SafeHandles;

    /// <content>
    /// Partitioning a device.
    /// </content>
    internal partial class UnsafeNativeMethods
    {
        [DllImport(ExternDll.OpenCL)]
        private static extern ErrorCode clCreateSubDevices(
            ClDeviceID device,
            [In, MarshalAs(UnmanagedType.LPArray)] IntPtr[] properties,
            uint numDevices,
            [Out, MarshalAs(UnmanagedType.LPArray)] ClDeviceID[] devices,
            out uint numDevicesRet);

        private static DisposableCollection<Device> CreateSubDevices(ClDeviceID device, IntPtr[] properties)
        {
            uint required;
            ErrorHandler.ThrowOnFailure(clCreateSubDevices(device, properties, 0, null, out required));

            ClDeviceID[] devices = new ClDeviceID[required];
            uint actual;
            ErrorHandler.ThrowOnFailure(clCreateSubDevices(device, properties, required, devices, out actual));

            var result = new DisposableCollection<Device>(false);
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
                throw new ArgumentNullException(nameof(partitionSizes));

            var propertiesList = new List<IntPtr>();
            propertiesList.Add((IntPtr)PartitionProperty.PartitionByCounts);
            foreach (int partitionSize in partitionSizes)
            {
                if (partitionSize < 0)
                    throw new ArgumentOutOfRangeException(nameof(partitionSizes), "Partition size cannot be negative.");

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
    }
}
