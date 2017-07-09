// Copyright (c) Tunnel Vision Laboratories, LLC. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

#pragma warning disable IDE1006 // Naming Styles

namespace NOpenCL
{
    using System;
    using System.Runtime.InteropServices;
    using NOpenCL.SafeHandles;

    /// <content>
    /// Sampler objects.
    /// </content>
    internal partial class UnsafeNativeMethods
    {
        /// <summary>
        /// Creates a sampler object.
        /// http://www.khronos.org/registry/cl/sdk/1.2/docs/man/xhtml/clCreateSampler.html
        /// http://www.khronos.org/registry/cl/specs/opencl-1.2.pdf#page=128
        /// </summary>
        /// <remarks>
        /// A sampler object describes how to sample an image when the image is read in the kernel.
        /// The built-in functions to read from an image in a kernel take a sampler as an argument.
        /// The sampler arguments to the image read function can be sampler objects created using
        /// OpenCL functions and passed as argument values to the kernel or can be samplers declared
        /// inside a kernel. In this section we discuss how sampler objects are created using OpenCL
        /// functions.
        ///
        /// <para>For more information about working with samplers, see <see cref="Sampler"/>.</para>
        /// </remarks>
        /// <param name="context">Must be a valid OpenCL context.</param>
        /// <param name="normalizedCoordinates">Determines if the image coordinates specified are normalized (if <paramref name="normalizedCoordinates"/> is <c>true</c>) or not (if <paramref name="normalizedCoordinates"/> is <c>false</c>).</param>
        /// <param name="addressingMode">Specifies how out-of-range image coordinates are handled when
        /// reading from an image. This can be set to <see cref="AddressingMode.MirroredRepeat"/>,
        /// <see cref="AddressingMode.Repeat"/>, <see cref="AddressingMode.ClampToEdge"/>,
        /// <see cref="AddressingMode.Clamp"/>, and <see cref="AddressingMode.None"/>.</param>
        /// <param name="filterMode">Specifies the type of filter that must be applied when reading an image.
        /// This can be <see cref="FilterMode.Nearest"/> or <see cref="FilterMode.Linear"/>.</param>
        /// <param name="errorCode">Returns an appropriate error code.</param>
        /// <returns>Returns a valid non-zero sampler object and <paramref name="errorCode"/> is
        /// set to <see cref="ErrorCode.Success"/> if the sampler object is created successfully.
        /// Otherwise, it returns an invalid handle value with one of the following error values
        /// returned in <paramref name="errorCode"/>:
        ///
        /// <list type="bullet">
        /// <item><see cref="ErrorCode.InvalidContext"/> if <paramref name="context"/> is not a valid context.</item>
        /// <item><see cref="ErrorCode.InvalidValue"/> if <paramref name="addressingMode"/>, <paramref name="filterMode"/>, or <paramref name="normalizedCoordinates"/> or a combination of these argument values are not valid.</item>
        /// <item><see cref="ErrorCode.InvalidOperation"/> if images are not supported by any device associated with <paramref name="context"/> (i.e. <see cref="DeviceInfo.ImageSupport"/> specified in the table of OpenCL Device Queries for <see cref="GetDeviceInfo"/> is <c>false</c>).</item>
        /// <item><see cref="ErrorCode.OutOfResources"/> if there is a failure to allocate resources required by the OpenCL implementation on the device.</item>
        /// <item><see cref="ErrorCode.OutOfHostMemory"/> if there is a failure to allocate resources required by the OpenCL implementation on the host.</item>
        /// </list>
        /// </returns>
        [DllImport(ExternDll.OpenCL)]
        private static extern SamplerSafeHandle clCreateSampler(
            ContextSafeHandle context,
            [MarshalAs(UnmanagedType.Bool)] bool normalizedCoordinates,
            AddressingMode addressingMode,
            FilterMode filterMode,
            out ErrorCode errorCode);

        internal static SamplerSafeHandle CreateSampler(ContextSafeHandle context, bool normalizedCoordinates, AddressingMode addressingMode, FilterMode filterMode)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            ErrorCode errorCode;
            SamplerSafeHandle handle = clCreateSampler(context, normalizedCoordinates, addressingMode, filterMode, out errorCode);
            ErrorHandler.ThrowOnFailure(errorCode);
            return handle;
        }

        /// <summary>
        /// Increments the sampler reference count.
        /// http://www.khronos.org/registry/cl/sdk/1.2/docs/man/xhtml/clRetainSampler.html
        /// http://www.khronos.org/registry/cl/specs/opencl-1.2.pdf#page=129
        /// </summary>
        /// <remarks>
        /// <see cref="clCreateSampler"/> performs an implicit retain.
        /// </remarks>
        /// <param name="sampler">Specifies the sampler being retained.</param>
        /// <returns>
        /// Returns <see cref="ErrorCode.Success"/> if the function is executed successfully.
        /// Otherwise, it returns one of the following errors:
        ///
        /// <list type="bullet">
        /// <item><see cref="ErrorCode.InvalidSampler"/> if sampler is not a valid sampler object.</item>
        /// <item><see cref="ErrorCode.OutOfResources"/> if there is a failure to allocate resources required by the OpenCL implementation on the device.</item>
        /// <item><see cref="ErrorCode.OutOfHostMemory"/> if there is a failure to allocate resources required by the OpenCL implementation on the host.</item>
        /// </list>
        /// </returns>
        [DllImport(ExternDll.OpenCL)]
        private static extern ErrorCode clRetainSampler(SamplerSafeHandle sampler);

        /// <summary>
        /// Decrements the sampler reference count.
        /// http://www.khronos.org/registry/cl/sdk/1.2/docs/man/xhtml/clReleaseSampler.html
        /// http://www.khronos.org/registry/cl/specs/opencl-1.2.pdf#page=129
        /// </summary>
        /// <remarks>
        /// The sampler object is deleted after the reference count
        /// becomes zero and commands queued for execution on a command-queue(s) that
        /// use <paramref name="sampler"/> have finished.
        /// </remarks>
        /// <param name="sampler">Specifies the sampler being retained.</param>
        /// <returns>
        /// Returns <see cref="ErrorCode.Success"/> if the function is executed successfully.
        /// Otherwise, it returns one of the following errors:
        ///
        /// <list type="bullet">
        /// <item><see cref="ErrorCode.InvalidSampler"/> if sampler is not a valid sampler object.</item>
        /// <item><see cref="ErrorCode.OutOfResources"/> if there is a failure to allocate resources required by the OpenCL implementation on the device.</item>
        /// <item><see cref="ErrorCode.OutOfHostMemory"/> if there is a failure to allocate resources required by the OpenCL implementation on the host.</item>
        /// </list>
        /// </returns>
        [DllImport(ExternDll.OpenCL)]
        internal static extern ErrorCode clReleaseSampler(IntPtr sampler);

        /// <summary>
        /// Returns information about the sampler object.
        /// http://www.khronos.org/registry/cl/sdk/1.2/docs/man/xhtml/clGetSamplerInfo.html
        /// http://www.khronos.org/registry/cl/specs/opencl-1.2.pdf#page=130
        /// </summary>
        /// <param name="sampler">Specifies the sampler being queried.</param>
        /// <param name="paramName">Specifies the information to query. See the members of <see cref="SamplerInfo"/> for the supported values.</param>
        /// <param name="paramValueSize">Specifies the size in bytes of memory pointed to by <paramref name="paramValue"/>. This size must be ≥ size of return type as described in the table above.</param>
        /// <param name="paramValue">A pointer to memory where the appropriate result being queried is returned. If <paramref name="paramValue"/> is <see cref="IntPtr.Zero"/>, it is ignored.</param>
        /// <param name="paramValueSizeRet">Returns the actual size in bytes of data copied to <paramref name="paramValue"/>.</param>
        /// <returns>
        /// Returns <see cref="ErrorCode.Success"/> if the function is executed successfully.
        /// Otherwise, it returns one of the following errors:
        ///
        /// <list type="bullet">
        /// <item><see cref="ErrorCode.InvalidValue"/> if <paramref name="paramName"/> is not valid, or if size in bytes specified by <paramref name="paramValueSize"/> is &lt; size of return type as described in the <see cref="SamplerInfo"/> documentation and <paramref name="paramValue"/> is not <see cref="IntPtr.Zero"/>.</item>
        /// <item><see cref="ErrorCode.InvalidSampler"/> if <paramref name="sampler"/> is a not a valid sampler object.</item>
        /// <item><see cref="ErrorCode.OutOfResources"/> if there is a failure to allocate resources required by the OpenCL implementation on the device.</item>
        /// <item><see cref="ErrorCode.OutOfHostMemory"/> if there is a failure to allocate resources required by the OpenCL implementation on the host.</item>
        /// </list>
        /// </returns>
        [DllImport(ExternDll.OpenCL)]
        private static extern ErrorCode clGetSamplerInfo(
            SamplerSafeHandle sampler,
            int paramName,
            UIntPtr paramValueSize,
            IntPtr paramValue,
            out UIntPtr paramValueSizeRet);

        internal static T GetSamplerInfo<T>(SamplerSafeHandle sampler, SamplerParameterInfo<T> parameter)
        {
            if (sampler == null)
                throw new ArgumentNullException(nameof(sampler));
            if (parameter == null)
                throw new ArgumentNullException(nameof(parameter));

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
            /// <seealso cref="Sampler.ReferenceCount"/>
            public static readonly SamplerParameterInfo<uint> ReferenceCount = (SamplerParameterInfo<uint>)new ParameterInfoUInt32(0x1150);

            /// <summary>
            /// Return the context specified when the sampler is created.
            /// </summary>
            /// <seealso cref="Sampler.Context"/>
            public static readonly SamplerParameterInfo<IntPtr> Context = (SamplerParameterInfo<IntPtr>)new ParameterInfoIntPtr(0x1151);

            /// <summary>
            /// Return the normalized coords value associated with sampler.
            /// </summary>
            /// <seealso cref="Sampler.NormalizedCoordinates"/>
            public static readonly SamplerParameterInfo<bool> NormalizedCoordinates = (SamplerParameterInfo<bool>)new ParameterInfoBoolean(0x1152);

            /// <summary>
            /// Return the addressing mode value associated with sampler.
            /// </summary>
            /// <seealso cref="Sampler.AddressingMode"/>
            public static readonly SamplerParameterInfo<uint> AddressingMode = (SamplerParameterInfo<uint>)new ParameterInfoUInt32(0x1153);

            /// <summary>
            /// Return the filter mode value associated with sampler.
            /// </summary>
            /// <seealso cref="Sampler.FilterMode"/>
            public static readonly SamplerParameterInfo<uint> FilterMode = (SamplerParameterInfo<uint>)new ParameterInfoUInt32(0x1154);
        }

        public sealed class SamplerParameterInfo<T>
        {
            private readonly ParameterInfo<T> _parameterInfo;

            public SamplerParameterInfo(ParameterInfo<T> parameterInfo)
            {
                if (parameterInfo == null)
                    throw new ArgumentNullException(nameof(parameterInfo));

                _parameterInfo = parameterInfo;
            }

            public static explicit operator SamplerParameterInfo<T>(ParameterInfo<T> parameterInfo)
            {
                var result = parameterInfo as SamplerParameterInfo<T>;
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
    }
}
