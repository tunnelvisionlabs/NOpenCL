// Copyright (c) Tunnel Vision Laboratories, LLC. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

#pragma warning disable IDE1006 // Naming Styles

namespace NOpenCL
{
    using System;
    using System.Runtime.InteropServices;
    using NOpenCL.SafeHandles;

    /// <content>
    /// Profiling operations on memory objects and kernels.
    /// </content>
    internal partial class UnsafeNativeMethods
    {
        [DllImport(ExternDll.OpenCL)]
        private static extern ErrorCode clGetEventProfilingInfo(
            EventSafeHandle @event,
            int paramName,
            UIntPtr paramValueSize,
            IntPtr paramValue,
            out UIntPtr paramValueSizeRet);

        public static T GetEventProfilingInfo<T>(EventSafeHandle @event, EventProfilingParameterInfo<T> parameter)
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
                ErrorHandler.ThrowOnFailure(clGetEventProfilingInfo(@event, parameter.ParameterInfo.Name, UIntPtr.Zero, IntPtr.Zero, out requiredSize));

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
                ErrorHandler.ThrowOnFailure(clGetEventProfilingInfo(@event, parameter.ParameterInfo.Name, requiredSize, memory, out actualSize));
                return parameter.ParameterInfo.Deserialize(actualSize, memory);
            }
            finally
            {
                Marshal.FreeHGlobal(memory);
            }
        }

        public static class EventProfilingInfo
        {
            /// <summary>
            /// A 64-bit value that describes the current device time counter in nanoseconds when
            /// the command identified by <em>event</em> is enqueued in a command-queue by the host.
            /// </summary>
            public static readonly EventProfilingParameterInfo<ulong> CommandQueued =
                (EventProfilingParameterInfo<ulong>)new ParameterInfoUInt64(0x1280);

            /// <summary>
            /// A 64-bit value that describes the current device time counter in nanoseconds when
            /// the command identified by <em>event</em> that has been enqueued is submitted by the
            /// host to the device associated with the command-queue.
            /// </summary>
            public static readonly EventProfilingParameterInfo<ulong> CommandSubmit =
                (EventProfilingParameterInfo<ulong>)new ParameterInfoUInt64(0x1281);

            /// <summary>
            /// A 64-bit value that describes the current device time counter in nanoseconds when
            /// the command identified by <em>event</em> starts execution on the device.
            /// </summary>
            public static readonly EventProfilingParameterInfo<ulong> CommandStart =
                (EventProfilingParameterInfo<ulong>)new ParameterInfoUInt64(0x1282);

            /// <summary>
            /// A 64-bit value that describes the current device time counter in nanoseconds when
            /// the command identified by <em>event</em> has finished execution on the device.
            /// </summary>
            public static readonly EventProfilingParameterInfo<ulong> CommandEnd =
                (EventProfilingParameterInfo<ulong>)new ParameterInfoUInt64(0x1283);
        }

        public sealed class EventProfilingParameterInfo<T>
        {
            private readonly ParameterInfo<T> _parameterInfo;

            public EventProfilingParameterInfo(ParameterInfo<T> parameterInfo)
            {
                if (parameterInfo == null)
                    throw new ArgumentNullException(nameof(parameterInfo));

                _parameterInfo = parameterInfo;
            }

            public static explicit operator EventProfilingParameterInfo<T>(ParameterInfo<T> parameterInfo)
            {
                return new EventProfilingParameterInfo<T>(parameterInfo);
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
