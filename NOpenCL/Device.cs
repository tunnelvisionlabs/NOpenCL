namespace NOpenCL
{
    using System;
    using System.Collections.Generic;
    using DeviceInfo = NOpenCL.UnsafeNativeMethods.DeviceInfo;

    public sealed class Device : IEquatable<Device>
    {
        private readonly UnsafeNativeMethods.ClDeviceID _device;

        private Device(UnsafeNativeMethods.ClDeviceID device)
        {
            _device = device;
        }

        public uint AddressBits
        {
            get
            {
                return UnsafeNativeMethods.GetDeviceInfo(_device, DeviceInfo.AddressBits);
            }
        }

        public bool Available
        {
            get
            {
                return UnsafeNativeMethods.GetDeviceInfo(_device, DeviceInfo.Available);
            }
        }

        public IReadOnlyList<string> BuiltInKernels
        {
            get
            {
                return UnsafeNativeMethods.GetDeviceInfo(_device, DeviceInfo.BuiltInKernels).Split(new [] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            }
        }

        public bool CompilerAvailable
        {
            get
            {
                return UnsafeNativeMethods.GetDeviceInfo(_device, DeviceInfo.CompilerAvailable);
            }
        }

        public FloatingPointConfiguration DoubleFloatingPointConfiguration
        {
            get
            {
                return (FloatingPointConfiguration)UnsafeNativeMethods.GetDeviceInfo(_device, DeviceInfo.DoubleFloatingPointConfiguration);
            }
        }

        public bool LittleEndian
        {
            get
            {
                return UnsafeNativeMethods.GetDeviceInfo(_device, DeviceInfo.LittleEndian);
            }
        }

        public bool ErrorCorrectionSupport
        {
            get
            {
                return UnsafeNativeMethods.GetDeviceInfo(_device, DeviceInfo.ErrorCorrectionSupport);
            }
        }

        public ExecutionCapabilities ExecutionCapabilities
        {
            get
            {
                return (ExecutionCapabilities)UnsafeNativeMethods.GetDeviceInfo(_device, DeviceInfo.ExecutionCapabilities);
            }
        }

        public IReadOnlyList<string> Extensions
        {
            get
            {
                return UnsafeNativeMethods.GetDeviceInfo(_device, DeviceInfo.Extensions).Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            }
        }

        public ulong GlobalCacheSize
        {
            get
            {
                return UnsafeNativeMethods.GetDeviceInfo(_device, DeviceInfo.GlobalCacheSize);
            }
        }

        public CacheType GlobalCacheType
        {
            get
            {
                return (CacheType)UnsafeNativeMethods.GetDeviceInfo(_device, DeviceInfo.GlobalCacheType);
            }
        }

        public uint GlobalCacheLineSize
        {
            get
            {
                return UnsafeNativeMethods.GetDeviceInfo(_device, DeviceInfo.GlobalCacheLineSize);
            }
        }

        public ulong GlobalMemorySize
        {
            get
            {
                return UnsafeNativeMethods.GetDeviceInfo(_device, DeviceInfo.GlobalMemorySize);
            }
        }

        public FloatingPointConfiguration HalfFloatingPointConfiguration
        {
            get
            {
                return (FloatingPointConfiguration)UnsafeNativeMethods.GetDeviceInfo(_device, DeviceInfo.HalfFloatingPointConfiguration);
            }
        }

        public bool HostUnifiedMemory
        {
            get
            {
                return UnsafeNativeMethods.GetDeviceInfo(_device, DeviceInfo.HostUnifiedMemory);
            }
        }

        public bool ImageSupport
        {
            get
            {
                return UnsafeNativeMethods.GetDeviceInfo(_device, DeviceInfo.ImageSupport);
            }
        }

        public UIntPtr Image2DMaxHeight
        {
            get
            {
                return UnsafeNativeMethods.GetDeviceInfo(_device, DeviceInfo.Image2DMaxHeight);
            }
        }

        public UIntPtr Image2DMaxWidth
        {
            get
            {
                return UnsafeNativeMethods.GetDeviceInfo(_device, DeviceInfo.Image2DMaxWidth);
            }
        }

        public UIntPtr Image3DMaxDepth
        {
            get
            {
                return UnsafeNativeMethods.GetDeviceInfo(_device, DeviceInfo.Image3DMaxDepth);
            }
        }

        public UIntPtr Image3DMaxHeight
        {
            get
            {
                return UnsafeNativeMethods.GetDeviceInfo(_device, DeviceInfo.Image3DMaxHeight);
            }
        }

        public UIntPtr Image3DMaxWidth
        {
            get
            {
                return UnsafeNativeMethods.GetDeviceInfo(_device, DeviceInfo.Image3DMaxWidth);
            }
        }

        public UIntPtr ImageMaxBufferSize
        {
            get
            {
                return UnsafeNativeMethods.GetDeviceInfo(_device, DeviceInfo.ImageMaxBufferSize);
            }
        }

        public UIntPtr ImageMaxArraySize
        {
            get
            {
                return UnsafeNativeMethods.GetDeviceInfo(_device, DeviceInfo.ImageMaxArraySize);
            }
        }

        public bool LinkerAvailable
        {
            get
            {
                return UnsafeNativeMethods.GetDeviceInfo(_device, DeviceInfo.LinkerAvailable);
            }
        }

        public ulong LocalMemorySize
        {
            get
            {
                return UnsafeNativeMethods.GetDeviceInfo(_device, DeviceInfo.LocalMemorySize);
            }
        }

        public LocalMemoryType LocalMemoryType
        {
            get
            {
                return (LocalMemoryType)UnsafeNativeMethods.GetDeviceInfo(_device, DeviceInfo.LocalMemoryType);
            }
        }

        public uint MaxClockFrequency
        {
            get
            {
                return UnsafeNativeMethods.GetDeviceInfo(_device, DeviceInfo.MaxClockFrequency);
            }
        }

        public uint MaxComputeUnits
        {
            get
            {
                return UnsafeNativeMethods.GetDeviceInfo(_device, DeviceInfo.MaxComputeUnits);
            }
        }

        public uint MaxConstantArguments
        {
            get
            {
                return UnsafeNativeMethods.GetDeviceInfo(_device, DeviceInfo.MaxConstantArguments);
            }
        }

        public ulong MaxConstantBufferSize
        {
            get
            {
                return UnsafeNativeMethods.GetDeviceInfo(_device, DeviceInfo.MaxConstantBufferSize);
            }
        }

        public ulong MaxMemoryAllocationSize
        {
            get
            {
                return UnsafeNativeMethods.GetDeviceInfo(_device, DeviceInfo.MaxMemoryAllocationSize);
            }
        }

        public UIntPtr MaxParameterSize
        {
            get
            {
                return UnsafeNativeMethods.GetDeviceInfo(_device, DeviceInfo.MaxParameterSize);
            }
        }

        public uint MaxReadImageArguments
        {
            get
            {
                return UnsafeNativeMethods.GetDeviceInfo(_device, DeviceInfo.MaxReadImageArguments);
            }
        }

        public uint MaxSamplers
        {
            get
            {
                return UnsafeNativeMethods.GetDeviceInfo(_device, DeviceInfo.MaxSamplers);
            }
        }

        public UIntPtr MaxWorkGroupSize
        {
            get
            {
                return UnsafeNativeMethods.GetDeviceInfo(_device, DeviceInfo.MaxWorkGroupSize);
            }
        }

        public uint MaxWorkItemDimensions
        {
            get
            {
                return UnsafeNativeMethods.GetDeviceInfo(_device, DeviceInfo.MaxWorkItemDimensions);
            }
        }

        public IReadOnlyList<UIntPtr> MaxWorkItemSizes
        {
            get
            {
                return UnsafeNativeMethods.GetDeviceInfo(_device, DeviceInfo.MaxWorkItemSizes);
            }
        }

        public uint MaxWriteImageArguments
        {
            get
            {
                return UnsafeNativeMethods.GetDeviceInfo(_device, DeviceInfo.MaxWriteImageArguments);
            }
        }

        public uint MemoryBaseAddressAlignment
        {
            get
            {
                return UnsafeNativeMethods.GetDeviceInfo(_device, DeviceInfo.MemoryBaseAddressAlignment);
            }
        }

        public uint MinDataTypeAlignmentSize
        {
            get
            {
                return UnsafeNativeMethods.GetDeviceInfo(_device, DeviceInfo.MinDataTypeAlignment);
            }
        }

        public string Name
        {
            get
            {
                return UnsafeNativeMethods.GetDeviceInfo(_device, DeviceInfo.Name);
            }
        }

        public uint NativeVectorWidthChar
        {
            get
            {
                return UnsafeNativeMethods.GetDeviceInfo(_device, DeviceInfo.NativeVectorWidthChar);
            }
        }

        public uint NativeVectorWidthShort
        {
            get
            {
                return UnsafeNativeMethods.GetDeviceInfo(_device, DeviceInfo.NativeVectorWidthShort);
            }
        }

        public uint NativeVectorWidthInt
        {
            get
            {
                return UnsafeNativeMethods.GetDeviceInfo(_device, DeviceInfo.NativeVectorWidthInt);
            }
        }

        public uint NativeVectorWidthLong
        {
            get
            {
                return UnsafeNativeMethods.GetDeviceInfo(_device, DeviceInfo.NativeVectorWidthLong);
            }
        }

        public uint NativeVectorWidthFloat
        {
            get
            {
                return UnsafeNativeMethods.GetDeviceInfo(_device, DeviceInfo.NativeVectorWidthFloat);
            }
        }

        public uint NativeVectorWidthDouble
        {
            get
            {
                return UnsafeNativeMethods.GetDeviceInfo(_device, DeviceInfo.NativeVectorWidthDouble);
            }
        }

        public uint NativeVectorWidthHalf
        {
            get
            {
                return UnsafeNativeMethods.GetDeviceInfo(_device, DeviceInfo.NativeVectorWidthHalf);
            }
        }

        public string OpenCLVersion
        {
            get
            {
                return UnsafeNativeMethods.GetDeviceInfo(_device, DeviceInfo.OpenCLVersion);
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
                return UnsafeNativeMethods.GetDeviceInfo(_device, DeviceInfo.PartitionMaxSubDevices);
            }
        }

        public IReadOnlyList<PartitionProperty> PartitionProperties
        {
            get
            {
                IntPtr[] result = UnsafeNativeMethods.GetDeviceInfo(_device, DeviceInfo.PartitionProperties);
                return Array.ConvertAll(result, partitionProperty => (PartitionProperty)partitionProperty);
            }
        }

        public AffinityDomain PartitionAffinityDomain
        {
            get
            {
                return (AffinityDomain)UnsafeNativeMethods.GetDeviceInfo(_device, DeviceInfo.PartitionAffinityDomain);
            }
        }

        public IReadOnlyList<PartitionProperty> PartitionType
        {
            get
            {
                IntPtr[] result = UnsafeNativeMethods.GetDeviceInfo(_device, DeviceInfo.PartitionType);
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
                return UnsafeNativeMethods.GetDeviceInfo(_device, DeviceInfo.PreferredVectorWidthChar);
            }
        }

        public uint PreferredVectorWidthShort
        {
            get
            {
                return UnsafeNativeMethods.GetDeviceInfo(_device, DeviceInfo.PreferredVectorWidthShort);
            }
        }

        public uint PreferredVectorWidthInt
        {
            get
            {
                return UnsafeNativeMethods.GetDeviceInfo(_device, DeviceInfo.PreferredVectorWidthInt);
            }
        }

        public uint PreferredVectorWidthLong
        {
            get
            {
                return UnsafeNativeMethods.GetDeviceInfo(_device, DeviceInfo.PreferredVectorWidthLong);
            }
        }

        public uint PreferredVectorWidthFloat
        {
            get
            {
                return UnsafeNativeMethods.GetDeviceInfo(_device, DeviceInfo.PreferredVectorWidthFloat);
            }
        }

        public uint PreferredVectorWidthDouble
        {
            get
            {
                return UnsafeNativeMethods.GetDeviceInfo(_device, DeviceInfo.PreferredVectorWidthDouble);
            }
        }

        public uint PreferredVectorWidthHalf
        {
            get
            {
                return UnsafeNativeMethods.GetDeviceInfo(_device, DeviceInfo.PreferredVectorWidthHalf);
            }
        }

        public UIntPtr PrintfBufferSize
        {
            get
            {
                return UnsafeNativeMethods.GetDeviceInfo(_device, DeviceInfo.PrintfBufferSize);
            }
        }

        public bool PreferredInteropUserSync
        {
            get
            {
                return UnsafeNativeMethods.GetDeviceInfo(_device, DeviceInfo.PreferredInteropUserSync);
            }
        }

        public string Profile
        {
            get
            {
                return UnsafeNativeMethods.GetDeviceInfo(_device, DeviceInfo.Profile);
            }
        }

        public UIntPtr ProfilingTimerResolution
        {
            get
            {
                return UnsafeNativeMethods.GetDeviceInfo(_device, DeviceInfo.ProfilingTimerResolution);
            }
        }

        public CommandQueueProperties QueueProperties
        {
            get
            {
                return (CommandQueueProperties)UnsafeNativeMethods.GetDeviceInfo(_device, DeviceInfo.QueueProperties);
            }
        }

        public uint ReferenceCount
        {
            get
            {
                return UnsafeNativeMethods.GetDeviceInfo(_device, DeviceInfo.ReferenceCount);
            }
        }

        public FloatingPointConfiguration SingleFloatingPointConfiguration
        {
            get
            {
                return (FloatingPointConfiguration)UnsafeNativeMethods.GetDeviceInfo(_device, DeviceInfo.SingleFloatingPointConfiguration);
            }
        }

        public DeviceType DeviceType
        {
            get
            {
                return (DeviceType)UnsafeNativeMethods.GetDeviceInfo(_device, DeviceInfo.DeviceType);
            }
        }

        public string Vendor
        {
            get
            {
                return UnsafeNativeMethods.GetDeviceInfo(_device, DeviceInfo.Vendor);
            }
        }

        public uint VendorID
        {
            get
            {
                return UnsafeNativeMethods.GetDeviceInfo(_device, DeviceInfo.VendorID);
            }
        }

        public string Version
        {
            get
            {
                return UnsafeNativeMethods.GetDeviceInfo(_device, DeviceInfo.Version);
            }
        }

        public string DriverVersion
        {
            get
            {
                return UnsafeNativeMethods.GetDeviceInfo(_device, DeviceInfo.DriverVersion);
            }
        }

        public static Device[] GetDevices(Platform platform, DeviceType deviceType)
        {
            if (platform == null)
                throw new ArgumentNullException("platform");

            UnsafeNativeMethods.ClDeviceID[] devices = UnsafeNativeMethods.GetDeviceIDs(platform.ID, deviceType);
            return Array.ConvertAll(devices, device => new Device(device));
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
    }
}
