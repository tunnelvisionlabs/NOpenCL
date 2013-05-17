namespace NOpenCL
{
    using System;
    using System.Collections.Generic;
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
                throw new ArgumentNullException("handle");

            _device = device;
            _handle = handle;
        }

        public uint AddressBits
        {
            get
            {
                return UnsafeNativeMethods.GetDeviceInfo(ID, DeviceInfo.AddressBits);
            }
        }

        public bool Available
        {
            get
            {
                return UnsafeNativeMethods.GetDeviceInfo(ID, DeviceInfo.Available);
            }
        }

        public IReadOnlyList<string> BuiltInKernels
        {
            get
            {
                return UnsafeNativeMethods.GetDeviceInfo(ID, DeviceInfo.BuiltInKernels).Split(new [] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            }
        }

        public bool CompilerAvailable
        {
            get
            {
                return UnsafeNativeMethods.GetDeviceInfo(ID, DeviceInfo.CompilerAvailable);
            }
        }

        public FloatingPointConfiguration DoubleFloatingPointConfiguration
        {
            get
            {
                return (FloatingPointConfiguration)UnsafeNativeMethods.GetDeviceInfo(ID, DeviceInfo.DoubleFloatingPointConfiguration);
            }
        }

        public bool LittleEndian
        {
            get
            {
                return UnsafeNativeMethods.GetDeviceInfo(ID, DeviceInfo.LittleEndian);
            }
        }

        public bool ErrorCorrectionSupport
        {
            get
            {
                return UnsafeNativeMethods.GetDeviceInfo(ID, DeviceInfo.ErrorCorrectionSupport);
            }
        }

        public ExecutionCapabilities ExecutionCapabilities
        {
            get
            {
                return (ExecutionCapabilities)UnsafeNativeMethods.GetDeviceInfo(ID, DeviceInfo.ExecutionCapabilities);
            }
        }

        public IReadOnlyList<string> Extensions
        {
            get
            {
                return UnsafeNativeMethods.GetDeviceInfo(ID, DeviceInfo.Extensions).Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            }
        }

        public ulong GlobalCacheSize
        {
            get
            {
                return UnsafeNativeMethods.GetDeviceInfo(ID, DeviceInfo.GlobalCacheSize);
            }
        }

        public CacheType GlobalCacheType
        {
            get
            {
                return (CacheType)UnsafeNativeMethods.GetDeviceInfo(ID, DeviceInfo.GlobalCacheType);
            }
        }

        public uint GlobalCacheLineSize
        {
            get
            {
                return UnsafeNativeMethods.GetDeviceInfo(ID, DeviceInfo.GlobalCacheLineSize);
            }
        }

        public ulong GlobalMemorySize
        {
            get
            {
                return UnsafeNativeMethods.GetDeviceInfo(ID, DeviceInfo.GlobalMemorySize);
            }
        }

        public FloatingPointConfiguration HalfFloatingPointConfiguration
        {
            get
            {
                return (FloatingPointConfiguration)UnsafeNativeMethods.GetDeviceInfo(ID, DeviceInfo.HalfFloatingPointConfiguration);
            }
        }

        public bool HostUnifiedMemory
        {
            get
            {
                return UnsafeNativeMethods.GetDeviceInfo(ID, DeviceInfo.HostUnifiedMemory);
            }
        }

        public bool ImageSupport
        {
            get
            {
                return UnsafeNativeMethods.GetDeviceInfo(ID, DeviceInfo.ImageSupport);
            }
        }

        public UIntPtr Image2DMaxHeight
        {
            get
            {
                return UnsafeNativeMethods.GetDeviceInfo(ID, DeviceInfo.Image2DMaxHeight);
            }
        }

        public UIntPtr Image2DMaxWidth
        {
            get
            {
                return UnsafeNativeMethods.GetDeviceInfo(ID, DeviceInfo.Image2DMaxWidth);
            }
        }

        public UIntPtr Image3DMaxDepth
        {
            get
            {
                return UnsafeNativeMethods.GetDeviceInfo(ID, DeviceInfo.Image3DMaxDepth);
            }
        }

        public UIntPtr Image3DMaxHeight
        {
            get
            {
                return UnsafeNativeMethods.GetDeviceInfo(ID, DeviceInfo.Image3DMaxHeight);
            }
        }

        public UIntPtr Image3DMaxWidth
        {
            get
            {
                return UnsafeNativeMethods.GetDeviceInfo(ID, DeviceInfo.Image3DMaxWidth);
            }
        }

        public UIntPtr ImageMaxBufferSize
        {
            get
            {
                return UnsafeNativeMethods.GetDeviceInfo(ID, DeviceInfo.ImageMaxBufferSize);
            }
        }

        public UIntPtr ImageMaxArraySize
        {
            get
            {
                return UnsafeNativeMethods.GetDeviceInfo(ID, DeviceInfo.ImageMaxArraySize);
            }
        }

        public bool LinkerAvailable
        {
            get
            {
                return UnsafeNativeMethods.GetDeviceInfo(ID, DeviceInfo.LinkerAvailable);
            }
        }

        public ulong LocalMemorySize
        {
            get
            {
                return UnsafeNativeMethods.GetDeviceInfo(ID, DeviceInfo.LocalMemorySize);
            }
        }

        public LocalMemoryType LocalMemoryType
        {
            get
            {
                return (LocalMemoryType)UnsafeNativeMethods.GetDeviceInfo(ID, DeviceInfo.LocalMemoryType);
            }
        }

        public uint MaxClockFrequency
        {
            get
            {
                return UnsafeNativeMethods.GetDeviceInfo(ID, DeviceInfo.MaxClockFrequency);
            }
        }

        public uint MaxComputeUnits
        {
            get
            {
                return UnsafeNativeMethods.GetDeviceInfo(ID, DeviceInfo.MaxComputeUnits);
            }
        }

        public uint MaxConstantArguments
        {
            get
            {
                return UnsafeNativeMethods.GetDeviceInfo(ID, DeviceInfo.MaxConstantArguments);
            }
        }

        public ulong MaxConstantBufferSize
        {
            get
            {
                return UnsafeNativeMethods.GetDeviceInfo(ID, DeviceInfo.MaxConstantBufferSize);
            }
        }

        public ulong MaxMemoryAllocationSize
        {
            get
            {
                return UnsafeNativeMethods.GetDeviceInfo(ID, DeviceInfo.MaxMemoryAllocationSize);
            }
        }

        public UIntPtr MaxParameterSize
        {
            get
            {
                return UnsafeNativeMethods.GetDeviceInfo(ID, DeviceInfo.MaxParameterSize);
            }
        }

        public uint MaxReadImageArguments
        {
            get
            {
                return UnsafeNativeMethods.GetDeviceInfo(ID, DeviceInfo.MaxReadImageArguments);
            }
        }

        public uint MaxSamplers
        {
            get
            {
                return UnsafeNativeMethods.GetDeviceInfo(ID, DeviceInfo.MaxSamplers);
            }
        }

        public UIntPtr MaxWorkGroupSize
        {
            get
            {
                return UnsafeNativeMethods.GetDeviceInfo(ID, DeviceInfo.MaxWorkGroupSize);
            }
        }

        public uint MaxWorkItemDimensions
        {
            get
            {
                return UnsafeNativeMethods.GetDeviceInfo(ID, DeviceInfo.MaxWorkItemDimensions);
            }
        }

        public IReadOnlyList<UIntPtr> MaxWorkItemSizes
        {
            get
            {
                return UnsafeNativeMethods.GetDeviceInfo(ID, DeviceInfo.MaxWorkItemSizes);
            }
        }

        public uint MaxWriteImageArguments
        {
            get
            {
                return UnsafeNativeMethods.GetDeviceInfo(ID, DeviceInfo.MaxWriteImageArguments);
            }
        }

        public uint MemoryBaseAddressAlignment
        {
            get
            {
                return UnsafeNativeMethods.GetDeviceInfo(ID, DeviceInfo.MemoryBaseAddressAlignment);
            }
        }

        [Obsolete("Deprecated in OpenCL 1.2")]
        public uint MinDataTypeAlignmentSize
        {
            get
            {
                return UnsafeNativeMethods.GetDeviceInfo(ID, DeviceInfo.MinDataTypeAlignment);
            }
        }

        public string Name
        {
            get
            {
                return UnsafeNativeMethods.GetDeviceInfo(ID, DeviceInfo.Name);
            }
        }

        public uint NativeVectorWidthChar
        {
            get
            {
                return UnsafeNativeMethods.GetDeviceInfo(ID, DeviceInfo.NativeVectorWidthChar);
            }
        }

        public uint NativeVectorWidthShort
        {
            get
            {
                return UnsafeNativeMethods.GetDeviceInfo(ID, DeviceInfo.NativeVectorWidthShort);
            }
        }

        public uint NativeVectorWidthInt
        {
            get
            {
                return UnsafeNativeMethods.GetDeviceInfo(ID, DeviceInfo.NativeVectorWidthInt);
            }
        }

        public uint NativeVectorWidthLong
        {
            get
            {
                return UnsafeNativeMethods.GetDeviceInfo(ID, DeviceInfo.NativeVectorWidthLong);
            }
        }

        public uint NativeVectorWidthFloat
        {
            get
            {
                return UnsafeNativeMethods.GetDeviceInfo(ID, DeviceInfo.NativeVectorWidthFloat);
            }
        }

        public uint NativeVectorWidthDouble
        {
            get
            {
                return UnsafeNativeMethods.GetDeviceInfo(ID, DeviceInfo.NativeVectorWidthDouble);
            }
        }

        public uint NativeVectorWidthHalf
        {
            get
            {
                return UnsafeNativeMethods.GetDeviceInfo(ID, DeviceInfo.NativeVectorWidthHalf);
            }
        }

        public string OpenCLVersion
        {
            get
            {
                return UnsafeNativeMethods.GetDeviceInfo(ID, DeviceInfo.OpenCLVersion);
            }
        }

        public Device Parent
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public uint PartitionMaxSubDevices
        {
            get
            {
                return UnsafeNativeMethods.GetDeviceInfo(ID, DeviceInfo.PartitionMaxSubDevices);
            }
        }

        public IReadOnlyList<PartitionProperty> PartitionProperties
        {
            get
            {
                IntPtr[] result = UnsafeNativeMethods.GetDeviceInfo(ID, DeviceInfo.PartitionProperties);
                return Array.ConvertAll(result, partitionProperty => (PartitionProperty)partitionProperty);
            }
        }

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

        public Platform Platform
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public uint PreferredVectorWidthChar
        {
            get
            {
                return UnsafeNativeMethods.GetDeviceInfo(ID, DeviceInfo.PreferredVectorWidthChar);
            }
        }

        public uint PreferredVectorWidthShort
        {
            get
            {
                return UnsafeNativeMethods.GetDeviceInfo(ID, DeviceInfo.PreferredVectorWidthShort);
            }
        }

        public uint PreferredVectorWidthInt
        {
            get
            {
                return UnsafeNativeMethods.GetDeviceInfo(ID, DeviceInfo.PreferredVectorWidthInt);
            }
        }

        public uint PreferredVectorWidthLong
        {
            get
            {
                return UnsafeNativeMethods.GetDeviceInfo(ID, DeviceInfo.PreferredVectorWidthLong);
            }
        }

        public uint PreferredVectorWidthFloat
        {
            get
            {
                return UnsafeNativeMethods.GetDeviceInfo(ID, DeviceInfo.PreferredVectorWidthFloat);
            }
        }

        public uint PreferredVectorWidthDouble
        {
            get
            {
                return UnsafeNativeMethods.GetDeviceInfo(ID, DeviceInfo.PreferredVectorWidthDouble);
            }
        }

        public uint PreferredVectorWidthHalf
        {
            get
            {
                return UnsafeNativeMethods.GetDeviceInfo(ID, DeviceInfo.PreferredVectorWidthHalf);
            }
        }

        public UIntPtr PrintfBufferSize
        {
            get
            {
                return UnsafeNativeMethods.GetDeviceInfo(ID, DeviceInfo.PrintfBufferSize);
            }
        }

        public bool PreferredInteropUserSync
        {
            get
            {
                return UnsafeNativeMethods.GetDeviceInfo(ID, DeviceInfo.PreferredInteropUserSync);
            }
        }

        public string Profile
        {
            get
            {
                return UnsafeNativeMethods.GetDeviceInfo(ID, DeviceInfo.Profile);
            }
        }

        public UIntPtr ProfilingTimerResolution
        {
            get
            {
                return UnsafeNativeMethods.GetDeviceInfo(ID, DeviceInfo.ProfilingTimerResolution);
            }
        }

        public CommandQueueProperties QueueProperties
        {
            get
            {
                return (CommandQueueProperties)UnsafeNativeMethods.GetDeviceInfo(ID, DeviceInfo.QueueProperties);
            }
        }

        public uint ReferenceCount
        {
            get
            {
                return UnsafeNativeMethods.GetDeviceInfo(ID, DeviceInfo.ReferenceCount);
            }
        }

        public FloatingPointConfiguration SingleFloatingPointConfiguration
        {
            get
            {
                return (FloatingPointConfiguration)UnsafeNativeMethods.GetDeviceInfo(ID, DeviceInfo.SingleFloatingPointConfiguration);
            }
        }

        public DeviceType DeviceType
        {
            get
            {
                return (DeviceType)UnsafeNativeMethods.GetDeviceInfo(ID, DeviceInfo.DeviceType);
            }
        }

        public string Vendor
        {
            get
            {
                return UnsafeNativeMethods.GetDeviceInfo(ID, DeviceInfo.Vendor);
            }
        }

        public uint VendorID
        {
            get
            {
                return UnsafeNativeMethods.GetDeviceInfo(ID, DeviceInfo.VendorID);
            }
        }

        public string Version
        {
            get
            {
                return UnsafeNativeMethods.GetDeviceInfo(ID, DeviceInfo.Version);
            }
        }

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
        /// The number of non-zero count entries in the list may not exceed <see cref="MaxSubDevices"/>.
        /// The total number of compute units specified may not exceed <see cref="MaxComputeUnits"/>.
        /// </summary>
        /// <param name="partitionSizes">The size of the partitions to be created.</param>
        /// <returns>A collection of sub-devices representing the created partitions.</returns>
        public DisposableCollection<Device> Partition(params int[] partitionSizes)
        {
            if (partitionSizes == null)
                throw new ArgumentNullException("partitionSizes");
            if (partitionSizes.Length == 0)
                throw new ArgumentException();

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
        /// <param name="affinityDomain"></param>
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
