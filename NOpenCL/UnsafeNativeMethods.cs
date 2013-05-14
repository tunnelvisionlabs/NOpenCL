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

        public static T GetPlatformInfo<T>(ClPlatformID platform, ParameterInfo<T> parameter)
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
            public static readonly ParameterInfo<string> Profile = new ParameterInfoString(0x0900);
            public static readonly ParameterInfo<string> Version = new ParameterInfoString(0x0901);
            public static readonly ParameterInfo<string> Name = new ParameterInfoString(0x0902);
            public static readonly ParameterInfo<string> Vendor = new ParameterInfoString(0x0903);
            public static readonly ParameterInfo<string> Extensions = new ParameterInfoString(0x0904);
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
