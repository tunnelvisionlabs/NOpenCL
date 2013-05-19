/*
 * Copyright (c) 2013 Sam Harwell, Tunnel Vision Laboratories LLC
 * All rights reserved.
 */

namespace NOpenCL
{
    using System;
    using System.Runtime.InteropServices;
    using NOpenCL.SafeHandles;

    partial class UnsafeNativeMethods
    {
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
    }
}
