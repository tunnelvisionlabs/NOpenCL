﻿// Copyright (c) Tunnel Vision Laboratories, LLC. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace NOpenCL
{
    using System;
    using System.Runtime.InteropServices;
    using NOpenCL.SafeHandles;

    partial class UnsafeNativeMethods
    {
        #region Kernel Objects

        [DllImport(ExternDll.OpenCL)]
        private static extern KernelSafeHandle clCreateKernel(
            ProgramSafeHandle program,
            [MarshalAs(UnmanagedType.LPStr)] string kernelName,
            out ErrorCode errorCode);

        public static KernelSafeHandle CreateKernel(ProgramSafeHandle program, string kernelName)
        {
            if (program == null)
                throw new ArgumentNullException("program");
            if (kernelName == null)
                throw new ArgumentNullException("kernelName");
            if (string.IsNullOrEmpty(kernelName))
                throw new ArgumentException();

            ErrorCode errorCode;
            KernelSafeHandle kernel = clCreateKernel(program, kernelName, out errorCode);
            ErrorHandler.ThrowOnFailure(errorCode);
            return kernel;
        }

        [DllImport(ExternDll.OpenCL)]
        private static extern ErrorCode clCreateKernelsInProgram(
            ProgramSafeHandle program,
            uint numKernels,
            [Out, MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(SafeHandleArrayMarshaler))] KernelSafeHandle[] kernels,
            out uint numKernelsRet);

        [DllImport(ExternDll.OpenCL)]
        private static extern ErrorCode clRetainKernel(KernelSafeHandle kernel);

        [DllImport(ExternDll.OpenCL)]
        public static extern ErrorCode clReleaseKernel(IntPtr kernel);

        [DllImport(ExternDll.OpenCL)]
        private static extern ErrorCode clSetKernelArg(
            KernelSafeHandle kernel,
            int argumentIndex,
            UIntPtr argSize,
            IntPtr argValue);

        public static void SetKernelArg(KernelSafeHandle kernel, int argumentIndex, UIntPtr argSize, IntPtr argValue)
        {
            if (kernel == null)
                throw new ArgumentNullException("kernel");
            if (argumentIndex < 0)
                throw new ArgumentOutOfRangeException("argumentIndex");

            ErrorHandler.ThrowOnFailure(clSetKernelArg(kernel, argumentIndex, argSize, argValue));
        }

        [DllImport(ExternDll.OpenCL)]
        private static extern ErrorCode clGetKernelArgInfo(
            KernelSafeHandle kernel,
            int argumentIndex,
            int paramName,
            UIntPtr paramValueSize,
            IntPtr paramValue,
            out UIntPtr paramValueSizeRet);

        public static T GetKernelArgInfo<T>(KernelSafeHandle kernel, int argumentIndex, KernelArgParameterInfo<T> parameter)
        {
            if (kernel == null)
                throw new ArgumentNullException("kernel");
            if (parameter == null)
                throw new ArgumentNullException("parameter");
            if (argumentIndex < 0)
                throw new ArgumentOutOfRangeException("argumentIndex");

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
                ErrorHandler.ThrowOnFailure(clGetKernelArgInfo(kernel, argumentIndex, parameter.ParameterInfo.Name, UIntPtr.Zero, IntPtr.Zero, out requiredSize));

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
                ErrorHandler.ThrowOnFailure(clGetKernelArgInfo(kernel, argumentIndex, parameter.ParameterInfo.Name, requiredSize, memory, out actualSize));
                return parameter.ParameterInfo.Deserialize(actualSize, memory);
            }
            finally
            {
                Marshal.FreeHGlobal(memory);
            }
        }

        public static class KernelArgInfo
        {
            public static readonly KernelArgParameterInfo<uint> AddressQualifier =
                (KernelArgParameterInfo<uint>)new ParameterInfoUInt32(0x1196);
            public static readonly KernelArgParameterInfo<uint> AccessQualifier =
                (KernelArgParameterInfo<uint>)new ParameterInfoUInt32(0x1197);
            public static readonly KernelArgParameterInfo<string> TypeName =
                (KernelArgParameterInfo<string>)new ParameterInfoString(0x1198);
            public static readonly KernelArgParameterInfo<ulong> TypeQualifier =
                (KernelArgParameterInfo<ulong>)new ParameterInfoUInt64(0x1199);
            public static readonly KernelArgParameterInfo<string> Name =
                (KernelArgParameterInfo<string>)new ParameterInfoString(0x119A);
        }

        public sealed class KernelArgParameterInfo<T>
        {
            private readonly ParameterInfo<T> _parameterInfo;

            public KernelArgParameterInfo(ParameterInfo<T> parameterInfo)
            {
                if (parameterInfo == null)
                    throw new ArgumentNullException("parameterInfo");

                _parameterInfo = parameterInfo;
            }

            public static explicit operator KernelArgParameterInfo<T>(ParameterInfo<T> parameterInfo)
            {
                return new KernelArgParameterInfo<T>(parameterInfo);
            }

            public ParameterInfo<T> ParameterInfo
            {
                get
                {
                    return _parameterInfo;
                }
            }
        }

        [DllImport(ExternDll.OpenCL)]
        private static extern ErrorCode clGetKernelInfo(
            KernelSafeHandle kernel,
            int paramName,
            UIntPtr paramValueSize,
            IntPtr paramValue,
            out UIntPtr paramValueSizeRet);

        public static T GetKernelInfo<T>(KernelSafeHandle kernel, KernelParameterInfo<T> parameter)
        {
            if (kernel == null)
                throw new ArgumentNullException("kernel");
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
                ErrorHandler.ThrowOnFailure(clGetKernelInfo(kernel, parameter.ParameterInfo.Name, UIntPtr.Zero, IntPtr.Zero, out requiredSize));

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
                ErrorHandler.ThrowOnFailure(clGetKernelInfo(kernel, parameter.ParameterInfo.Name, requiredSize, memory, out actualSize));
                return parameter.ParameterInfo.Deserialize(actualSize, memory);
            }
            finally
            {
                Marshal.FreeHGlobal(memory);
            }
        }

        public static class KernelInfo
        {
            /// <summary>
            /// Return the kernel function name.
            /// </summary>
            public static readonly KernelParameterInfo<string> FunctionName = (KernelParameterInfo<string>)new ParameterInfoString(0x1190);

            /// <summary>
            /// Return the number of arguments to kernel.
            /// </summary>
            public static readonly KernelParameterInfo<uint> NumArgs = (KernelParameterInfo<uint>)new ParameterInfoUInt32(0x1191);

            /// <summary>
            /// Return the kernel reference count.
            /// <para/>
            /// The reference count returned should be considered immediately stale. It
            /// is unsuitable for general use in applications. This feature is provided
            /// for identifying memory leaks.
            /// </summary>
            public static readonly KernelParameterInfo<uint> ReferenceCount = (KernelParameterInfo<uint>)new ParameterInfoUInt32(0x1192);

            /// <summary>
            /// Return the context associated with kernel.
            /// </summary>
            public static readonly KernelParameterInfo<IntPtr> Context = (KernelParameterInfo<IntPtr>)new ParameterInfoIntPtr(0x1193);

            /// <summary>
            /// Return the program object associated with kernel.
            /// </summary>
            public static readonly KernelParameterInfo<IntPtr> Program = (KernelParameterInfo<IntPtr>)new ParameterInfoIntPtr(0x1194);

            /// <summary>
            /// Returns any attributes specified using the <c>__attribute__</c> qualifier
            /// with the kernel function declaration in the program source. These attributes
            /// include those on the <c>__attribute__</c> page and other attributes supported
            /// by an implementation.
            /// <para/>
            /// Attributes are returned as they were declared inside <c>__attribute__((...))</c>,
            /// with any surrounding whitespace and embedded newlines removed. When multiple
            /// attributes are present, they are returned as a single, space delimited string.
            /// </summary>
            public static readonly KernelParameterInfo<string> Attributes = (KernelParameterInfo<string>)new ParameterInfoString(0x1195);
        }

        public sealed class KernelParameterInfo<T>
        {
            private readonly ParameterInfo<T> _parameterInfo;

            public KernelParameterInfo(ParameterInfo<T> parameterInfo)
            {
                if (parameterInfo == null)
                    throw new ArgumentNullException("parameterInfo");

                _parameterInfo = parameterInfo;
            }

            public static explicit operator KernelParameterInfo<T>(ParameterInfo<T> parameterInfo)
            {
                return new KernelParameterInfo<T>(parameterInfo);
            }

            public ParameterInfo<T> ParameterInfo
            {
                get
                {
                    return _parameterInfo;
                }
            }
        }

        [DllImport(ExternDll.OpenCL)]
        private static extern ErrorCode clGetKernelWorkGroupInfo(
            KernelSafeHandle kernel,
            ClDeviceID device,
            int paramName,
            UIntPtr paramValueSize,
            IntPtr paramValue,
            out UIntPtr paramValueSizeRet);

        public static ulong[] GetKernelWorkGroupInfo(KernelSafeHandle kernel, ClDeviceID device, KernelWorkGroupInfo kernelWorkGroupInfo)
        {
            if (kernel == null)
                throw new ArgumentNullException("kernel");

            int size = 0;
            int arrayCt = 0;
            switch (kernelWorkGroupInfo)
            {
                case KernelWorkGroupInfo.GlobalWorkSize:
                    size = UIntPtr.Size;
                    arrayCt = 3;
                    break;
                case KernelWorkGroupInfo.WorkGroupSize:
                    size = UIntPtr.Size;
                    arrayCt = 1;
                    break;
                case KernelWorkGroupInfo.CompileWorkGroupSize:
                    size = UIntPtr.Size;
                    arrayCt = 3;
                    break;
                case KernelWorkGroupInfo.LocalMemorySize:
                    size = sizeof(ulong);
                    arrayCt = 1;
                    break;
                case KernelWorkGroupInfo.PreferredWorkGroupSizeMultiple:
                    size = UIntPtr.Size;
                    arrayCt = 1;
                    break;
                case KernelWorkGroupInfo.PrivateMemorySize:
                    size = sizeof(ulong);
                    arrayCt = 1;
                    break;
            }
            int fixedSize = size * arrayCt;

#if DEBUG
            UIntPtr requiredSize;
            ErrorHandler.ThrowOnFailure(clGetKernelWorkGroupInfo(kernel, device, (int)kernelWorkGroupInfo, UIntPtr.Zero, IntPtr.Zero, out requiredSize));
            if (requiredSize.ToUInt64() != (ulong)fixedSize)
                throw new ArgumentException("The parameter definition includes a fixed size that does not match the required size according to the runtime.");
#endif

            IntPtr memory = IntPtr.Zero;
            try
            {
                memory = Marshal.AllocHGlobal(fixedSize);
                UIntPtr actualSize;
                ErrorHandler.ThrowOnFailure(clGetKernelWorkGroupInfo(kernel, device, (int)kernelWorkGroupInfo, (UIntPtr)fixedSize, memory, out actualSize));
                IntPtr[] array = new IntPtr[(int)((long)actualSize.ToUInt64() / IntPtr.Size)];
                Marshal.Copy(memory, array, 0, array.Length);

                ulong[] outArray = new ulong[arrayCt];
                for (int i = 0; i < arrayCt; i++)
                    outArray[i] = (ulong)array[i];

                return outArray;
            }
            finally
            {
                Marshal.FreeHGlobal(memory);
            }
        }

        public enum KernelWorkGroupInfo
        {
            /// <summary>
            /// This provides a mechanism for the application to query the maximum global size
            /// that can be used to execute a kernel (i.e. <em>globalWorkSize</em> argument to
            /// <see cref="clEnqueueNDRangeKernel"/>) on a custom device given by device or a
            /// built-in kernel on an OpenCL device given by device.
            /// <para/>
            /// If device is not a custom device or kernel is not a built-in kernel,
            /// <see cref="clGetKernelArgInfo"/> returns the error <see cref="ErrorCode.InvalidValue"/>.
            /// </summary>
            GlobalWorkSize = 0x11B5,

            /// <summary>
            /// This provides a mechanism for the application to query the maximum work-group
            /// size that can be used to execute a kernel on a specific device given by device.
            /// The OpenCL implementation uses the resource requirements of the kernel (register
            /// usage etc.) to determine what this work-group size should be.
            /// </summary>
            WorkGroupSize = 0x11B0,

            /// <summary>
            /// Returns the work-group size specified by the <c>__attribute__((reqd_work_group_size(X, Y, Z)))</c>
            /// qualifier. See Function Qualifiers. If the work-group size is not specified
            /// using the above attribute qualifier (0, 0, 0) is returned.
            /// </summary>
            CompileWorkGroupSize = 0x11B1,

            /// <summary>
            /// Returns the amount of local memory in bytes being used by a kernel. This includes
            /// local memory that may be needed by an implementation to execute the kernel,
            /// variables declared inside the kernel with the <c>__local</c> address qualifier and
            /// local memory to be allocated for arguments to the kernel declared as pointers
            /// with the <c>__local</c> address qualifier and whose size is specified with
            /// <see cref="clSetKernelArg"/>.
            /// <para/>
            /// If the local memory size, for any pointer argument to the kernel declared with the
            /// <c>__local</c> address qualifier, is not specified, its size is assumed to be 0.
            /// </summary>
            LocalMemorySize = 0x11B2,

            /// <summary>
            /// Returns the preferred multiple of workgroup size for launch. This is a performance
            /// hint. Specifying a workgroup size that is not a multiple of the value returned by
            /// this query as the value of the local work size argument to <see cref="clEnqueueNDRangeKernel"/> will
            /// not fail to enqueue the kernel for execution unless the work-group size specified
            /// is larger than the device maximum.
            /// </summary>
            PreferredWorkGroupSizeMultiple = 0x11B3,


            /// <summary>
            /// Returns the minimum amount of private memory, in bytes, used by each workitem in
            /// the kernel. This value may include any private memory needed by an implementation
            /// to execute the kernel, including that used by the language built-ins and variable
            /// declared inside the kernel with the <c>__private</c> qualifier.
            /// </summary>
            PrivateMemorySize = 0x11B4,
        }

        #endregion

        #region Executing Kernels

        [DllImport(ExternDll.OpenCL)]
        private static extern ErrorCode clEnqueueNDRangeKernel(
            CommandQueueSafeHandle commandQueue,
            KernelSafeHandle kernel,
            uint workDim,
            [In, MarshalAs(UnmanagedType.LPArray)] IntPtr[] globalWorkOffset,
            [In, MarshalAs(UnmanagedType.LPArray)] IntPtr[] globalWorkSize,
            [In, MarshalAs(UnmanagedType.LPArray)] IntPtr[] localWorkSize,
            uint numEventsInWaitList,
            [In, MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(SafeHandleArrayMarshaler))] EventSafeHandle[] eventWaitList,
            out EventSafeHandle @event);

        public static EventSafeHandle EnqueueNDRangeKernel(
            CommandQueueSafeHandle commandQueue,
            KernelSafeHandle kernel,
            IntPtr[] globalWorkOffset,
            IntPtr[] globalWorkSize,
            IntPtr[] localWorkSize,
            EventSafeHandle[] eventWaitList)
        {
            if (commandQueue == null)
                throw new ArgumentNullException("commandQueue");
            if (kernel == null)
                throw new ArgumentNullException("kernel");
            if (globalWorkSize == null)
                throw new ArgumentNullException("globalWorkSize");
            if (localWorkSize == null)
                throw new ArgumentNullException("localWorkSize");

            uint workDim = (uint)globalWorkSize.Length;
            if (globalWorkOffset != null && globalWorkOffset.Length != workDim)
                throw new ArgumentException();
            if (localWorkSize != null && localWorkSize.Length != workDim)
                throw new ArgumentException();

            EventSafeHandle result;
            ErrorHandler.ThrowOnFailure(clEnqueueNDRangeKernel(commandQueue, kernel, (uint)workDim, globalWorkOffset, globalWorkSize, localWorkSize, GetNumItems(eventWaitList), GetItems(eventWaitList), out result));
            return result;
        }

        [DllImport(ExternDll.OpenCL)]
        private static extern ErrorCode clEnqueueTask(
            CommandQueueSafeHandle commandQueue,
            KernelSafeHandle kernel,
            uint numEventsInWaitList,
            [In, MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(SafeHandleArrayMarshaler))] EventSafeHandle[] eventWaitList,
            out EventSafeHandle @event);

        public static EventSafeHandle EnqueueTask(
            CommandQueueSafeHandle commandQueue,
            KernelSafeHandle kernel,
            EventSafeHandle[] eventWaitList)
        {
            if (commandQueue == null)
                throw new ArgumentNullException("commandQueue");
            if (kernel == null)
                throw new ArgumentNullException("kernel");

            EventSafeHandle result;
            ErrorHandler.ThrowOnFailure(clEnqueueTask(commandQueue, kernel, GetNumItems(eventWaitList), GetItems(eventWaitList), out result));
            return result;
        }

        [DllImport(ExternDll.OpenCL)]
        private static extern ErrorCode clEnqueueNativeKernel(
            CommandQueueSafeHandle commandQueue,
            NativeKernelFunction userFunction,
            IntPtr args,
            IntPtr cbArgs,
            uint numMemObjects,
            [In, MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(SafeHandleArrayMarshaler))] MemObjectSafeHandle[] memList,
            [In, MarshalAs(UnmanagedType.LPArray)] IntPtr[] argsMemLoc,
            uint numEventsInWaitList,
            [In, MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(SafeHandleArrayMarshaler))] EventSafeHandle[] eventWaitList,
            out EventSafeHandle @event);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate void NativeKernelFunction(IntPtr args);

        #endregion
    }
}
