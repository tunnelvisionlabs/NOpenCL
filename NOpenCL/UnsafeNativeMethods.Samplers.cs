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
        #region Sampler Objects

        [DllImport(ExternDll.OpenCL)]
        private static extern SamplerSafeHandle clCreateSampler(ContextSafeHandle context, [MarshalAs(UnmanagedType.Bool)] bool normalizedCoordinates, AddressingMode addressingMode, FilterMode filterMode, out ErrorCode errorCode);

        public static SamplerSafeHandle CreateSampler(ContextSafeHandle context, bool normalizedCoordinates, AddressingMode addressingMode, FilterMode filterMode)
        {
            if (context == null)
                throw new ArgumentNullException("context");

            ErrorCode errorCode;
            SamplerSafeHandle handle = clCreateSampler(context, normalizedCoordinates, addressingMode, filterMode, out errorCode);
            ErrorHandler.ThrowOnFailure(errorCode);
            return handle;
        }

        [DllImport(ExternDll.OpenCL)]
        private static extern ErrorCode clRetainSampler(SamplerSafeHandle sampler);

        [DllImport(ExternDll.OpenCL)]
        public static extern ErrorCode clReleaseSampler(IntPtr sampler);

        [DllImport(ExternDll.OpenCL)]
        private static extern ErrorCode clGetSamplerInfo(SamplerSafeHandle sampler, int paramName, UIntPtr paramValueSize, IntPtr paramValue, out UIntPtr paramValueSizeRet);

        public static T GetSamplerInfo<T>(SamplerSafeHandle sampler, SamplerParameterInfo<T> parameter)
        {
            if (sampler == null)
                throw new ArgumentNullException("sampler");
            if (parameter == null)
                throw new ArgumentNullException("parameter");

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
                ErrorHandler.ThrowOnFailure(clGetSamplerInfo(sampler, parameter.ParameterInfo.Name, UIntPtr.Zero, IntPtr.Zero, out requiredSize));

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
                ErrorHandler.ThrowOnFailure(clGetSamplerInfo(sampler, parameter.ParameterInfo.Name, requiredSize, memory, out actualSize));
                return parameter.ParameterInfo.Deserialize(actualSize, memory);
            }
            finally
            {
                Marshal.FreeHGlobal(memory);
            }
        }

        public static class SamplerInfo
        {
            /// <summary>
            /// Return the sampler reference count. The reference count returned should be
            /// considered immediately stale. It is unsuitable for general use in applications.
            /// This feature is provided for identifying memory leaks.
            /// </summary>
            public static readonly SamplerParameterInfo<uint> ReferenceCount = (SamplerParameterInfo<uint>)new ParameterInfoUInt32(0x1150);

            /// <summary>
            /// Return the context specified when the sampler is created.
            /// </summary>
            public static readonly SamplerParameterInfo<IntPtr> Context = (SamplerParameterInfo<IntPtr>)new ParameterInfoIntPtr(0x1151);

            /// <summary>
            /// Return the normalized coords value associated with sampler.
            /// </summary>
            public static readonly SamplerParameterInfo<bool> NormalizedCoordinates = (SamplerParameterInfo<bool>)new ParameterInfoBoolean(0x1152);

            /// <summary>
            /// Return the addressing mode value associated with sampler.
            /// </summary>
            public static readonly SamplerParameterInfo<uint> AddressingMode = (SamplerParameterInfo<uint>)new ParameterInfoUInt32(0x1153);

            /// <summary>
            /// Return the filter mode value associated with sampler.
            /// </summary>
            public static readonly SamplerParameterInfo<uint> FilterMode = (SamplerParameterInfo<uint>)new ParameterInfoUInt32(0x1154);
        }

        public sealed class SamplerParameterInfo<T>
        {
            private readonly ParameterInfo<T> _parameterInfo;

            public SamplerParameterInfo(ParameterInfo<T> parameterInfo)
            {
                if (parameterInfo == null)
                    throw new ArgumentNullException("parameterInfo");

                _parameterInfo = parameterInfo;
            }

            public static explicit operator SamplerParameterInfo<T>(ParameterInfo<T> parameterInfo)
            {
                SamplerParameterInfo<T> result = parameterInfo as SamplerParameterInfo<T>;
                if (result != null)
                    return result;

                return new SamplerParameterInfo<T>(parameterInfo);
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
