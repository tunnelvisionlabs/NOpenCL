namespace NOpenCL
{
    using System;
    using System.Runtime.InteropServices;

    internal static partial class UnsafeNativeMethods
    {
        #region Platforms

        [DllImport(ExternDll.OpenCL)]
        private static extern ErrorCode clGetPlatformIDs(uint numEntries, [In, Out, MarshalAs(UnmanagedType.LPArray)] ClPlatformID[] platforms, out uint numPlatforms);

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
        public static extern ErrorCode clGetPlatformInfo(ClPlatformID platform, int paramName, UIntPtr paramValueSize, IntPtr paramValue, out UIntPtr paramValueSizeRet);

        public static T GetPlatformInfo<T>(ClPlatformID platform, PlatformParameterInfo<T> parameter)
        {
            int? fixedSize = parameter.FixedSize;
            UIntPtr requiredSize;
            if (fixedSize.HasValue)
                requiredSize = (UIntPtr)fixedSize;
            else
                ErrorHandler.ThrowOnFailure(clGetPlatformInfo(platform, parameter.Name, UIntPtr.Zero, IntPtr.Zero, out requiredSize));

            IntPtr memory = IntPtr.Zero;
            try
            {
                memory = Marshal.AllocHGlobal((int)requiredSize.ToUInt32());
                UIntPtr actualSize;
                ErrorHandler.ThrowOnFailure(clGetPlatformInfo(platform, parameter.Name, requiredSize, memory, out actualSize));
                return parameter.Deserialize(actualSize, memory);
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

        public sealed class PlatformParameterInfo<T> : ParameterInfo<T>
        {
            private readonly ParameterInfo<T> _parameterInfo;

            public PlatformParameterInfo(ParameterInfo<T> parameterInfo)
                : base(parameterInfo.Name)
            {
                _parameterInfo = parameterInfo;
            }

            public static explicit operator PlatformParameterInfo<T>(ParameterInfo<T> parameterInfo)
            {
                if (parameterInfo == null)
                    throw new ArgumentNullException("parameterInfo");

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

            public override int? FixedSize
            {
                get
                {
                    return _parameterInfo.FixedSize;
                }
            }

            public override T Deserialize(UIntPtr memorySize, IntPtr memory)
            {
                return _parameterInfo.Deserialize(memorySize, memory);
            }
        }

        #endregion

        #region Devices

        [DllImport(ExternDll.OpenCL)]
        private static extern ErrorCode clGetDeviceIDs(ClPlatformID platform, DeviceType deviceType, uint numEntries, [In, Out, MarshalAs(UnmanagedType.LPArray)] ClDeviceID[] devices, out uint numDevices);

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
        public static extern ErrorCode clGetDeviceInfo(ClDeviceID device, int paramName, UIntPtr paramValueSize, IntPtr paramValue, out UIntPtr paramValueSizeRet);

        public static T GetDeviceInfo<T>(ClDeviceID device, DeviceParameterInfo<T> parameter)
        {
            int? fixedSize = parameter.FixedSize;
            UIntPtr requiredSize;
            if (fixedSize.HasValue)
                requiredSize = (UIntPtr)fixedSize;
            else
                ErrorHandler.ThrowOnFailure(clGetDeviceInfo(device, parameter.Name, UIntPtr.Zero, IntPtr.Zero, out requiredSize));

            IntPtr memory = IntPtr.Zero;
            try
            {
                memory = Marshal.AllocHGlobal((int)requiredSize.ToUInt32());
                UIntPtr actualSize;
                ErrorHandler.ThrowOnFailure(clGetDeviceInfo(device, parameter.Name, requiredSize, memory, out actualSize));
                return parameter.Deserialize(actualSize, memory);
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
        }

        public static class DeviceInfo
        {
            public static readonly DeviceParameterInfo<uint> AddressBits = (DeviceParameterInfo<uint>)new ParameterInfoUInt32(0x100D);
            public static readonly DeviceParameterInfo<bool> Available = (DeviceParameterInfo<bool>)new ParameterInfoBoolean(0x1027);
            public static readonly DeviceParameterInfo<string> BuiltInKernels = (DeviceParameterInfo<string>)new ParameterInfoString(0x103F);
            public static readonly DeviceParameterInfo<bool> CompilerAvailable = (DeviceParameterInfo<bool>)new ParameterInfoBoolean(0x1028);
            public static readonly DeviceParameterInfo<IntPtr> DoubleFloatingPointConfiguration = (DeviceParameterInfo<IntPtr>)new ParameterInfoIntPtr(0x1032);
            public static readonly DeviceParameterInfo<bool> LittleEndian = (DeviceParameterInfo<bool>)new ParameterInfoBoolean(0x1026);
            public static readonly DeviceParameterInfo<bool> ErrorCorrectionSupport = (DeviceParameterInfo<bool>)new ParameterInfoBoolean(0x1024);
            public static readonly DeviceParameterInfo<IntPtr> ExecutionCapabilities = (DeviceParameterInfo<IntPtr>)new ParameterInfoIntPtr(0x1029);
            public static readonly DeviceParameterInfo<string> Extensions = (DeviceParameterInfo<string>)new ParameterInfoString(0x1030);
            public static readonly DeviceParameterInfo<ulong> GlobalCacheSize = (DeviceParameterInfo<ulong>)new ParameterInfoUInt64(0x101E);
            public static readonly DeviceParameterInfo<uint> GlobalCacheType = (DeviceParameterInfo<uint>)new ParameterInfoUInt32(0x101C);
            public static readonly DeviceParameterInfo<uint> GlobalCacheLineSize = (DeviceParameterInfo<uint>)new ParameterInfoUInt32(0x101D);
            public static readonly DeviceParameterInfo<ulong> GlobalMemorySize = (DeviceParameterInfo<ulong>)new ParameterInfoUInt64(0x101F);
            public static readonly DeviceParameterInfo<IntPtr> HalfFloatingPointConfiguration = (DeviceParameterInfo<IntPtr>)new ParameterInfoIntPtr(0x1033);
            public static readonly DeviceParameterInfo<bool> HostUnifiedMemory = (DeviceParameterInfo<bool>)new ParameterInfoBoolean(0x1035);
            public static readonly DeviceParameterInfo<bool> ImageSupport = (DeviceParameterInfo<bool>)new ParameterInfoBoolean(0x1016);
            public static readonly DeviceParameterInfo<UIntPtr> Image2DMaxHeight = (DeviceParameterInfo<UIntPtr>)new ParameterInfoUIntPtr(0x1012);
            public static readonly DeviceParameterInfo<UIntPtr> Image2DMaxWidth = (DeviceParameterInfo<UIntPtr>)new ParameterInfoUIntPtr(0x1011);
            public static readonly DeviceParameterInfo<UIntPtr> Image3DMaxDepth = (DeviceParameterInfo<UIntPtr>)new ParameterInfoUIntPtr(0x1015);
            public static readonly DeviceParameterInfo<UIntPtr> Image3DMaxHeight = (DeviceParameterInfo<UIntPtr>)new ParameterInfoUIntPtr(0x1014);
            public static readonly DeviceParameterInfo<UIntPtr> Image3DMaxWidth = (DeviceParameterInfo<UIntPtr>)new ParameterInfoUIntPtr(0x1013);
            public static readonly DeviceParameterInfo<UIntPtr> ImageMaxBufferSize = (DeviceParameterInfo<UIntPtr>)new ParameterInfoUIntPtr(0x1040);
            public static readonly DeviceParameterInfo<UIntPtr> ImageMaxArraySize = (DeviceParameterInfo<UIntPtr>)new ParameterInfoUIntPtr(0x1041);
            public static readonly DeviceParameterInfo<bool> LinkerAvailable = (DeviceParameterInfo<bool>)new ParameterInfoBoolean(0x103E);
            public static readonly DeviceParameterInfo<ulong> LocalMemorySize = (DeviceParameterInfo<ulong>)new ParameterInfoUInt64(0x1023);
            public static readonly DeviceParameterInfo<uint> LocalMemoryType = (DeviceParameterInfo<uint>)new ParameterInfoUInt32(0x1022);
            public static readonly DeviceParameterInfo<uint> MaxClockFrequency = (DeviceParameterInfo<uint>)new ParameterInfoUInt32(0x100C);
            public static readonly DeviceParameterInfo<uint> MaxComputeUnits = (DeviceParameterInfo<uint>)new ParameterInfoUInt32(0x1002);
            public static readonly DeviceParameterInfo<uint> MaxConstantArguments = (DeviceParameterInfo<uint>)new ParameterInfoUInt32(0x1021);
            public static readonly DeviceParameterInfo<ulong> MaxConstantBufferSize = (DeviceParameterInfo<ulong>)new ParameterInfoUInt64(0x1020);
            public static readonly DeviceParameterInfo<ulong> MaxMemoryAllocationSize = (DeviceParameterInfo<ulong>)new ParameterInfoUInt64(0x1010);
            public static readonly DeviceParameterInfo<UIntPtr> MaxParameterSize = (DeviceParameterInfo<UIntPtr>)new ParameterInfoUIntPtr(0x1017);
            public static readonly DeviceParameterInfo<uint> MaxReadImageArguments = (DeviceParameterInfo<uint>)new ParameterInfoUInt32(0x100E);
            public static readonly DeviceParameterInfo<uint> MaxSamplers = (DeviceParameterInfo<uint>)new ParameterInfoUInt32(0x1018);
            public static readonly DeviceParameterInfo<UIntPtr> MaxWorkGroupSize = (DeviceParameterInfo<UIntPtr>)new ParameterInfoUIntPtr(0x1004);
            public static readonly DeviceParameterInfo<uint> MaxWorkItemDimensions = (DeviceParameterInfo<uint>)new ParameterInfoUInt32(0x1003);
            public static readonly DeviceParameterInfo<UIntPtr[]> MaxWorkItemSizes = (DeviceParameterInfo<UIntPtr[]>)new ParameterInfoUIntPtrArray(0x1005);
            public static readonly DeviceParameterInfo<uint> MaxWriteImageArguments = (DeviceParameterInfo<uint>)new ParameterInfoUInt32(0x100F);
            public static readonly DeviceParameterInfo<uint> MemoryBaseAddressAlignment = (DeviceParameterInfo<uint>)new ParameterInfoUInt32(0x1019);
            public static readonly DeviceParameterInfo<uint> MinDataTypeAlignment = (DeviceParameterInfo<uint>)new ParameterInfoUInt32(0x101A);
            public static readonly DeviceParameterInfo<string> Name = (DeviceParameterInfo<string>)new ParameterInfoString(0x102B);
            public static readonly DeviceParameterInfo<uint> NativeVectorWidthChar = (DeviceParameterInfo<uint>)new ParameterInfoUInt32(0x1036);
            public static readonly DeviceParameterInfo<uint> NativeVectorWidthShort = (DeviceParameterInfo<uint>)new ParameterInfoUInt32(0x1037);
            public static readonly DeviceParameterInfo<uint> NativeVectorWidthInt = (DeviceParameterInfo<uint>)new ParameterInfoUInt32(0x1038);
            public static readonly DeviceParameterInfo<uint> NativeVectorWidthLong = (DeviceParameterInfo<uint>)new ParameterInfoUInt32(0x1039);
            public static readonly DeviceParameterInfo<uint> NativeVectorWidthFloat = (DeviceParameterInfo<uint>)new ParameterInfoUInt32(0x103A);
            public static readonly DeviceParameterInfo<uint> NativeVectorWidthDouble = (DeviceParameterInfo<uint>)new ParameterInfoUInt32(0x103B);
            public static readonly DeviceParameterInfo<uint> NativeVectorWidthHalf = (DeviceParameterInfo<uint>)new ParameterInfoUInt32(0x103C);
            public static readonly DeviceParameterInfo<string> OpenCLVersion = (DeviceParameterInfo<string>)new ParameterInfoString(0x103D);
            public static readonly DeviceParameterInfo<IntPtr> ParentDevice = (DeviceParameterInfo<IntPtr>)new ParameterInfoIntPtr(0x1042);
            public static readonly DeviceParameterInfo<uint> PartitionMaxSubDevices = (DeviceParameterInfo<uint>)new ParameterInfoUInt32(0x1043);
            public static readonly DeviceParameterInfo<IntPtr[]> PartitionProperties = (DeviceParameterInfo<IntPtr[]>)new ParameterInfoIntPtrArray(0x1044);
            public static readonly DeviceParameterInfo<IntPtr> PartitionAffinityDomain = (DeviceParameterInfo<IntPtr>)new ParameterInfoIntPtr(0x1045);
            public static readonly DeviceParameterInfo<IntPtr[]> PartitionType = (DeviceParameterInfo<IntPtr[]>)new ParameterInfoIntPtrArray(0x1046);
            public static readonly DeviceParameterInfo<IntPtr> Platform = (DeviceParameterInfo<IntPtr>)new ParameterInfoIntPtr(0x1031);
            public static readonly DeviceParameterInfo<uint> PreferredVectorWidthChar = (DeviceParameterInfo<uint>)new ParameterInfoUInt32(0x1006);
            public static readonly DeviceParameterInfo<uint> PreferredVectorWidthShort = (DeviceParameterInfo<uint>)new ParameterInfoUInt32(0x1007);
            public static readonly DeviceParameterInfo<uint> PreferredVectorWidthInt = (DeviceParameterInfo<uint>)new ParameterInfoUInt32(0x1008);
            public static readonly DeviceParameterInfo<uint> PreferredVectorWidthLong = (DeviceParameterInfo<uint>)new ParameterInfoUInt32(0x1009);
            public static readonly DeviceParameterInfo<uint> PreferredVectorWidthFloat = (DeviceParameterInfo<uint>)new ParameterInfoUInt32(0x100A);
            public static readonly DeviceParameterInfo<uint> PreferredVectorWidthDouble = (DeviceParameterInfo<uint>)new ParameterInfoUInt32(0x100B);
            public static readonly DeviceParameterInfo<uint> PreferredVectorWidthHalf = (DeviceParameterInfo<uint>)new ParameterInfoUInt32(0x1034);
            public static readonly DeviceParameterInfo<UIntPtr> PrintfBufferSize = (DeviceParameterInfo<UIntPtr>)new ParameterInfoUIntPtr(0x1049);
            public static readonly DeviceParameterInfo<bool> PreferredInteropUserSync = (DeviceParameterInfo<bool>)new ParameterInfoBoolean(0x1048);
            public static readonly DeviceParameterInfo<string> Profile = (DeviceParameterInfo<string>)new ParameterInfoString(0x102E);
            public static readonly DeviceParameterInfo<UIntPtr> ProfilingTimerResolution = (DeviceParameterInfo<UIntPtr>)new ParameterInfoUIntPtr(0x1025);
            public static readonly DeviceParameterInfo<IntPtr> QueueProperties = (DeviceParameterInfo<IntPtr>)new ParameterInfoIntPtr(0x102A);
            public static readonly DeviceParameterInfo<uint> ReferenceCount = (DeviceParameterInfo<uint>)new ParameterInfoUInt32(0x1047);
            public static readonly DeviceParameterInfo<IntPtr> SingleFloatingPointConfiguration = (DeviceParameterInfo<IntPtr>)new ParameterInfoIntPtr(0x101B);
            public static readonly DeviceParameterInfo<IntPtr> DeviceType = (DeviceParameterInfo<IntPtr>)new ParameterInfoIntPtr(0x1000);
            public static readonly DeviceParameterInfo<string> Vendor = (DeviceParameterInfo<string>)new ParameterInfoString(0x102C);
            public static readonly DeviceParameterInfo<uint> VendorID = (DeviceParameterInfo<uint>)new ParameterInfoUInt32(0x1001);
            public static readonly DeviceParameterInfo<string> Version = (DeviceParameterInfo<string>)new ParameterInfoString(0x102F);
            public static readonly DeviceParameterInfo<string> DriverVersion = (DeviceParameterInfo<string>)new ParameterInfoString(0x102D);
        }

        public sealed class DeviceParameterInfo<T> : ParameterInfo<T>
        {
            private readonly ParameterInfo<T> _parameterInfo;

            public DeviceParameterInfo(ParameterInfo<T> parameterInfo)
                : base(parameterInfo.Name)
            {
                _parameterInfo = parameterInfo;
            }

            public static explicit operator DeviceParameterInfo<T>(ParameterInfo<T> parameterInfo)
            {
                if (parameterInfo == null)
                    throw new ArgumentNullException("parameterInfo");

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

            public override int? FixedSize
            {
                get
                {
                    return _parameterInfo.FixedSize;
                }
            }

            public override T Deserialize(UIntPtr memorySize, IntPtr memory)
            {
                return _parameterInfo.Deserialize(memorySize, memory);
            }
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
                Buffer.BlockCopy(array, 0, result, 0, array.Length * IntPtr.Size);
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
