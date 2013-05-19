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
        #region Buffer Objects

        [DllImport(ExternDll.OpenCL)]
        private static extern BufferSafeHandle clCreateBuffer(
            ContextSafeHandle context,
            MemoryFlags flags,
            IntPtr size,
            IntPtr hostPointer,
            out ErrorCode errorCode);

        public static BufferSafeHandle CreateBuffer(ContextSafeHandle context, MemoryFlags flags, IntPtr size, IntPtr hostPointer)
        {
            if (context == null)
                throw new ArgumentNullException("context");

            ErrorCode errorCode;
            BufferSafeHandle handle = clCreateBuffer(context, flags, size, hostPointer, out errorCode);
            ErrorHandler.ThrowOnFailure(errorCode);
            return handle;
        }

        [DllImport(ExternDll.OpenCL)]
        private static extern BufferSafeHandle clCreateSubBuffer(
            BufferSafeHandle buffer,
            MemoryFlags flags,
            BufferCreateType mustBeRegion,
            [In] ref BufferRegion regionInfo,
            out ErrorCode errorCode);

        public static BufferSafeHandle CreateSubBuffer(BufferSafeHandle buffer, MemoryFlags flags, BufferRegion regionInfo)
        {
            if (buffer == null)
                throw new ArgumentNullException("buffer");

            ErrorCode errorCode;
            BufferSafeHandle handle = clCreateSubBuffer(buffer, flags, BufferCreateType.Region, ref regionInfo, out errorCode);
            ErrorHandler.ThrowOnFailure(errorCode);
            return handle;
        }

        [DllImport(ExternDll.OpenCL)]
        private static extern ErrorCode clEnqueueReadBuffer(
            CommandQueueSafeHandle commandQueue,
            BufferSafeHandle buffer,
            [MarshalAs(UnmanagedType.Bool)] bool blockingRead,
            IntPtr offset,
            IntPtr size,
            IntPtr destination,
            uint numEventsInWaitList,
            [In, MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(SafeHandleArrayMarshaler))] EventSafeHandle[] eventWaitList,
            out EventSafeHandle @event);

        public static EventSafeHandle EnqueueReadBuffer(CommandQueueSafeHandle commandQueue, BufferSafeHandle buffer, bool blocking, IntPtr offset, IntPtr size, IntPtr destination, EventSafeHandle[] eventWaitList)
        {
            if (commandQueue == null)
                throw new ArgumentNullException("commandQueue");
            if (buffer == null)
                throw new ArgumentNullException("buffer");
            if (destination == IntPtr.Zero)
                throw new ArgumentNullException("destination");

            EventSafeHandle result;
            ErrorHandler.ThrowOnFailure(clEnqueueReadBuffer(commandQueue, buffer, blocking, offset, size, destination, GetNumEventsInWaitList(eventWaitList), GetEventWaitList(eventWaitList), out result));
            return result;
        }

        [DllImport(ExternDll.OpenCL)]
        private static extern ErrorCode clEnqueueWriteBuffer(
            CommandQueueSafeHandle commandQueue,
            BufferSafeHandle buffer,
            [MarshalAs(UnmanagedType.Bool)] bool blockingWrite,
            IntPtr offset,
            IntPtr size,
            IntPtr source,
            uint numEventsInWaitList,
            [In, MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(SafeHandleArrayMarshaler))] EventSafeHandle[] eventWaitList,
            out EventSafeHandle @event);

        public static EventSafeHandle EnqueueWriteBuffer(CommandQueueSafeHandle commandQueue, BufferSafeHandle buffer, bool blocking, IntPtr offset, IntPtr size, IntPtr source, EventSafeHandle[] eventWaitList)
        {
            if (commandQueue == null)
                throw new ArgumentNullException("commandQueue");
            if (buffer == null)
                throw new ArgumentNullException("buffer");
            if (source == IntPtr.Zero)
                throw new ArgumentNullException("destination");

            EventSafeHandle result;
            ErrorHandler.ThrowOnFailure(clEnqueueWriteBuffer(commandQueue, buffer, blocking, offset, size, source, GetNumEventsInWaitList(eventWaitList), GetEventWaitList(eventWaitList), out result));
            return result;
        }

        [DllImport(ExternDll.OpenCL)]
        private static extern ErrorCode clEnqueueReadBufferRect(
            CommandQueueSafeHandle commandQueue,
            BufferSafeHandle buffer,
            [MarshalAs(UnmanagedType.Bool)] bool blockingRead,
            [In] ref BufferCoordinates bufferOrigin,
            [In] ref BufferCoordinates hostOrigin,
            [In] ref BufferSize region,
            IntPtr bufferRowPitch,
            IntPtr bufferSlicePitch,
            IntPtr hostRowPitch,
            IntPtr hostSlicePitch,
            IntPtr destination,
            uint numEventsInWaitList,
            [In, MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(SafeHandleArrayMarshaler))] EventSafeHandle[] eventWaitList,
            out EventSafeHandle @event);

        public static EventSafeHandle EnqueueReadBufferRect(
            CommandQueueSafeHandle commandQueue,
            BufferSafeHandle buffer,
            bool blocking,
            ref BufferCoordinates bufferOrigin,
            ref BufferCoordinates hostOrigin,
            ref BufferSize region,
            IntPtr bufferRowPitch,
            IntPtr bufferSlicePitch,
            IntPtr hostRowPitch,
            IntPtr hostSlicePitch,
            IntPtr destination,
            EventSafeHandle[] eventWaitList)
        {
            if (commandQueue == null)
                throw new ArgumentNullException("commandQueue");
            if (buffer == null)
                throw new ArgumentNullException("buffer");
            if (destination == IntPtr.Zero)
                throw new ArgumentNullException("destination");

            EventSafeHandle result;
            ErrorHandler.ThrowOnFailure(clEnqueueReadBufferRect(commandQueue, buffer, blocking, ref bufferOrigin, ref hostOrigin, ref region, bufferRowPitch, bufferSlicePitch, hostRowPitch, hostSlicePitch, destination, GetNumEventsInWaitList(eventWaitList), GetEventWaitList(eventWaitList), out result));
            return result;
        }

        [DllImport(ExternDll.OpenCL)]
        private static extern ErrorCode clEnqueueWriteBufferRect(
            CommandQueueSafeHandle commandQueue,
            BufferSafeHandle buffer,
            [MarshalAs(UnmanagedType.Bool)] bool blockingWrite,
            [In] ref BufferCoordinates bufferOrigin,
            [In] ref BufferCoordinates hostOrigin,
            [In] ref BufferSize region,
            IntPtr bufferRowPitch,
            IntPtr bufferSlicePitch,
            IntPtr hostRowPitch,
            IntPtr hostSlicePitch,
            IntPtr source,
            uint numEventsInWaitList,
            [In, MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(SafeHandleArrayMarshaler))] EventSafeHandle[] eventWaitList,
            out EventSafeHandle @event);

        public static EventSafeHandle EnqueueWriteBufferRect(
            CommandQueueSafeHandle commandQueue,
            BufferSafeHandle buffer,
            bool blocking,
            ref BufferCoordinates bufferOrigin,
            ref BufferCoordinates hostOrigin,
            ref BufferSize region,
            IntPtr bufferRowPitch,
            IntPtr bufferSlicePitch,
            IntPtr hostRowPitch,
            IntPtr hostSlicePitch,
            IntPtr source,
            EventSafeHandle[] eventWaitList)
        {
            if (commandQueue == null)
                throw new ArgumentNullException("commandQueue");
            if (buffer == null)
                throw new ArgumentNullException("buffer");
            if (source == IntPtr.Zero)
                throw new ArgumentNullException("source");

            EventSafeHandle result;
            ErrorHandler.ThrowOnFailure(clEnqueueWriteBufferRect(commandQueue, buffer, blocking, ref bufferOrigin, ref hostOrigin, ref region, bufferRowPitch, bufferSlicePitch, hostRowPitch, hostSlicePitch, source, GetNumEventsInWaitList(eventWaitList), GetEventWaitList(eventWaitList), out result));
            return result;
        }

        [DllImport(ExternDll.OpenCL)]
        private static extern ErrorCode clEnqueueFillBuffer(
            CommandQueueSafeHandle commandQueue,
            BufferSafeHandle buffer,
            IntPtr pattern,
            IntPtr patternSize,
            IntPtr offset,
            IntPtr size,
            uint numEventsInWaitList,
            [In, MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(SafeHandleArrayMarshaler))] EventSafeHandle[] eventWaitList,
            out EventSafeHandle @event);

        [DllImport(ExternDll.OpenCL)]
        private static extern ErrorCode clEnqueueCopyBuffer(
            CommandQueueSafeHandle commandQueue,
            BufferSafeHandle sourceBuffer,
            BufferSafeHandle destinationBuffer,
            IntPtr sourceOffset,
            IntPtr destinationOffset,
            IntPtr size,
            uint numEventsInWaitList,
            [In, MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(SafeHandleArrayMarshaler))] EventSafeHandle[] eventWaitList,
            out EventSafeHandle @event);

        public static EventSafeHandle EnqueueCopyBuffer(CommandQueueSafeHandle commandQueue, BufferSafeHandle sourceBuffer, BufferSafeHandle destinationBuffer, IntPtr sourceOffset, IntPtr destinationOffset, IntPtr size, EventSafeHandle[] eventWaitList)
        {
            if (commandQueue == null)
                throw new ArgumentNullException("commandQueue");
            if (sourceBuffer == null)
                throw new ArgumentNullException("sourceBuffer");
            if (destinationBuffer == null)
                throw new ArgumentNullException("destinationBuffer");

            EventSafeHandle result;
            ErrorHandler.ThrowOnFailure(clEnqueueCopyBuffer(commandQueue, sourceBuffer, destinationBuffer, sourceOffset, destinationOffset, size, GetNumEventsInWaitList(eventWaitList), GetEventWaitList(eventWaitList), out result));
            return result;
        }

        [DllImport(ExternDll.OpenCL)]
        private static extern ErrorCode clEnqueueCopyBufferRect(
            CommandQueueSafeHandle commandQueue,
            BufferSafeHandle sourceBuffer,
            BufferSafeHandle destinationBuffer,
            [In] ref BufferCoordinates sourceOrigin,
            [In] ref BufferCoordinates destinationOrigin,
            [In] ref BufferSize region,
            IntPtr sourceRowPitch,
            IntPtr sourceSlicePitch,
            IntPtr destinationRowPitch,
            IntPtr destinationSlicePitch,
            uint numEventsInWaitList,
            [In, MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(SafeHandleArrayMarshaler))] EventSafeHandle[] eventWaitList,
            out EventSafeHandle @event);

        public static EventSafeHandle EnqueueCopyBufferRect(
            CommandQueueSafeHandle commandQueue,
            BufferSafeHandle sourceBuffer,
            BufferSafeHandle destinationBuffer,
            [In] ref BufferCoordinates sourceOrigin,
            [In] ref BufferCoordinates destinationOrigin,
            [In] ref BufferSize region,
            IntPtr sourceRowPitch,
            IntPtr sourceSlicePitch,
            IntPtr destinationRowPitch,
            IntPtr destinationSlicePitch,
            EventSafeHandle[] eventWaitList)
        {
            if (commandQueue == null)
                throw new ArgumentNullException("commandQueue");
            if (sourceBuffer == null)
                throw new ArgumentNullException("sourceBuffer");
            if (destinationBuffer == null)
                throw new ArgumentNullException("destinationBuffer");

            EventSafeHandle result;
            ErrorHandler.ThrowOnFailure(clEnqueueCopyBufferRect(commandQueue, sourceBuffer, destinationBuffer, ref sourceOrigin, ref destinationOrigin, ref region, sourceRowPitch, sourceSlicePitch, destinationRowPitch, destinationSlicePitch, GetNumEventsInWaitList(eventWaitList), GetEventWaitList(eventWaitList), out result));
            return result;
        }

        [DllImport(ExternDll.OpenCL)]
        private static extern IntPtr clEnqueueMapBuffer(
            CommandQueueSafeHandle commandQueue,
            BufferSafeHandle buffer,
            [MarshalAs(UnmanagedType.Bool)] bool blockingMap,
            MapFlags mapFlags,
            IntPtr offset,
            IntPtr size,
            uint numEventsInWaitList,
            [In, MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(SafeHandleArrayMarshaler))] EventSafeHandle[] eventWaitList,
            out EventSafeHandle @event,
            out ErrorCode errorCode);

        public static EventSafeHandle EnqueueMapBuffer(
            CommandQueueSafeHandle commandQueue,
            BufferSafeHandle buffer,
            bool blocking,
            MapFlags mapFlags,
            IntPtr offset,
            IntPtr size,
            out IntPtr mappedPointer,
            EventSafeHandle[] eventWaitList)
        {
            if (commandQueue == null)
                throw new ArgumentNullException("commandQueue");
            if (buffer == null)
                throw new ArgumentNullException("buffer");

            EventSafeHandle result;
            ErrorCode errorCode;
            mappedPointer = clEnqueueMapBuffer(commandQueue, buffer, blocking, mapFlags, offset, size, GetNumEventsInWaitList(eventWaitList), GetEventWaitList(eventWaitList), out result, out errorCode);
            ErrorHandler.ThrowOnFailure(errorCode);
            return result;
        }

        [DllImport(ExternDll.OpenCL)]
        private static extern ImageSafeHandle clCreateImage(
            ContextSafeHandle context,
            MemoryFlags flags,
            [In] ref ImageFormat imageFormat,
            [In] ref ImageDescriptor imageDescriptor,
            IntPtr hostAddress,
            out ErrorCode errorCode);

        public static ImageSafeHandle CreateImage(ContextSafeHandle context, MemoryFlags flags, ref ImageFormat imageFormat, ref ImageDescriptor imageDescriptor, IntPtr hostAddress)
        {
            if (context == null)
                throw new ArgumentNullException("context");

            ErrorCode errorCode;
            ImageSafeHandle result = clCreateImage(context, flags, ref imageFormat, ref imageDescriptor, hostAddress, out errorCode);
            ErrorHandler.ThrowOnFailure(errorCode);
            return result;
        }

        [DllImport(ExternDll.OpenCL)]
        private static extern ErrorCode clGetSupportedImageFormats(ContextSafeHandle context, MemoryFlags flags, MemObjectType imageType, uint numEntries, [Out] ImageFormat[] imageFormats, out uint numImageFormats);

        [DllImport(ExternDll.OpenCL)]
        private static extern ErrorCode clEnqueueReadImage(
            CommandQueueSafeHandle commandQueue,
            ImageSafeHandle image,
            [MarshalAs(UnmanagedType.Bool)] bool blockingRead,
            [In] ref BufferCoordinates origin,
            [In] ref BufferSize region,
            IntPtr rowPitch,
            IntPtr slicePitch,
            IntPtr destination,
            uint numEventsInWaitList,
            [In, MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(SafeHandleArrayMarshaler))] EventSafeHandle[] eventWaitList,
            out EventSafeHandle @event);

        [DllImport(ExternDll.OpenCL)]
        private static extern ErrorCode clEnqueueWriteImage(
            CommandQueueSafeHandle commandQueue,
            ImageSafeHandle image,
            [MarshalAs(UnmanagedType.Bool)] bool blockingWrite,
            [In] ref BufferCoordinates origin,
            [In] ref BufferSize region,
            IntPtr inputRowPtch,
            IntPtr inputSlicePitch,
            IntPtr source,
            uint numEventsInWaitList,
            [In, MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(SafeHandleArrayMarshaler))] EventSafeHandle[] eventWaitList,
            out EventSafeHandle @event);

        [DllImport(ExternDll.OpenCL)]
        private static extern ErrorCode clEnqueueFillImage(
            CommandQueueSafeHandle commandQueue,
            ImageSafeHandle image,
            IntPtr fillColor,
            [In] ref BufferCoordinates origin,
            [In] ref BufferSize region,
            uint numEventsInWaitList,
            [In, MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(SafeHandleArrayMarshaler))] EventSafeHandle[] eventWaitList,
            out EventSafeHandle @event);

        [DllImport(ExternDll.OpenCL)]
        private static extern ErrorCode clEnqueueCopyImage(
            CommandQueueSafeHandle commandQueue,
            ImageSafeHandle sourceImage,
            ImageSafeHandle destinationImage,
            [In] ref BufferCoordinates sourceOrigin,
            [In] ref BufferCoordinates destinationOrigin,
            [In] ref BufferSize region,
            uint numEventsInWaitList,
            [In, MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(SafeHandleArrayMarshaler))] EventSafeHandle[] eventWaitList,
            out EventSafeHandle @event);

        [DllImport(ExternDll.OpenCL)]
        private static extern ErrorCode clEnqueueCopyImageToBuffer(
            CommandQueueSafeHandle commandQueue,
            ImageSafeHandle sourceImage,
            BufferSafeHandle destinationBuffer,
            [In] ref BufferCoordinates sourceOrigin,
            [In] ref BufferSize region,
            IntPtr destinationOffset,
            uint numEventsInWaitList,
            [In, MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(SafeHandleArrayMarshaler))] EventSafeHandle[] eventWaitList,
            out EventSafeHandle @event);

        [DllImport(ExternDll.OpenCL)]
        private static extern ErrorCode clEnqueueCopyBufferToImage(
            CommandQueueSafeHandle commandQueue,
            BufferSafeHandle sourceBuffer,
            ImageSafeHandle destinationImage,
            IntPtr sourceOffset,
            [In] ref BufferCoordinates destinationOrigin,
            [In] ref BufferSize region,
            uint numEventsInWaitList,
            [In, MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(SafeHandleArrayMarshaler))] EventSafeHandle[] eventWaitList,
            out EventSafeHandle @event);

        [DllImport(ExternDll.OpenCL)]
        private static extern IntPtr clEnqueueMapImage(
            CommandQueueSafeHandle commandQueue,
            ImageSafeHandle image,
            [MarshalAs(UnmanagedType.Bool)] bool blockingMap,
            MapFlags mapFlags,
            [In] ref BufferCoordinates origin,
            [In] ref BufferSize region,
            out IntPtr imageRowPitch,
            out IntPtr imageSlicePitch,
            uint numEventsInWaitList,
            [In, MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(SafeHandleArrayMarshaler))] EventSafeHandle[] eventWaitList,
            out EventSafeHandle @event,
            out ErrorCode errorCode);

        [DllImport(ExternDll.OpenCL)]
        private static extern ErrorCode clEnqueueUnmapMemObject(
            CommandQueueSafeHandle commandQueue,
            MemObjectSafeHandle memObject,
            IntPtr mappedPointer,
            uint numEventsInWaitList,
            [In, MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(SafeHandleArrayMarshaler))] EventSafeHandle[] eventWaitList,
            out EventSafeHandle @event);

        public static EventSafeHandle EnqueueUnmapMemObject(
            CommandQueueSafeHandle commandQueue,
            MemObjectSafeHandle memObject,
            IntPtr mappedPointer,
            EventSafeHandle[] eventWaitList)
        {
            if (commandQueue == null)
                throw new ArgumentNullException("commandQueue");
            if (memObject == null)
                throw new ArgumentNullException("memObject");

            EventSafeHandle result;
            ErrorHandler.ThrowOnFailure(clEnqueueUnmapMemObject(commandQueue, memObject, mappedPointer, GetNumEventsInWaitList(eventWaitList), GetEventWaitList(eventWaitList), out result));
            return result;
        }

        [DllImport(ExternDll.OpenCL)]
        private static extern ErrorCode clEnqueueMigrateMemObjects(
            CommandQueueSafeHandle commandQueue,
            uint numMemObjects,
            [In, MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(SafeHandleArrayMarshaler))] MemObjectSafeHandle[] memObjects,
            MigrationFlags flags,
            uint numEventsInWaitList,
            [In, MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(SafeHandleArrayMarshaler))] EventSafeHandle[] eventWaitList,
            out EventSafeHandle @event);

        public static EventSafeHandle EnqueueMigrateMemObjects(
            CommandQueueSafeHandle commandQueue,
            MemObjectSafeHandle[] memObjects,
            MigrationFlags flags,
            EventSafeHandle[] eventWaitList)
        {
            if (commandQueue == null)
                throw new ArgumentNullException("commandQueue");
            if (memObjects == null)
                throw new ArgumentNullException("memObjects");

            EventSafeHandle result;
            ErrorHandler.ThrowOnFailure(clEnqueueMigrateMemObjects(commandQueue, (uint)memObjects.Length, memObjects, flags, GetNumEventsInWaitList(eventWaitList), GetEventWaitList(eventWaitList), out result));
            return result;
        }

        [DllImport(ExternDll.OpenCL)]
        private static extern ErrorCode clGetMemObjectInfo(MemObjectSafeHandle memObject, int paramName, UIntPtr paramValueSize, IntPtr paramValue, out UIntPtr paramValueSizeRet);

        public static T GetMemObjectInfo<T>(MemObjectSafeHandle memObject, MemObjectParameterInfo<T> parameter)
        {
            if (memObject == null)
                throw new ArgumentNullException("memObject");
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
                ErrorHandler.ThrowOnFailure(clGetMemObjectInfo(memObject, parameter.ParameterInfo.Name, UIntPtr.Zero, IntPtr.Zero, out requiredSize));

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
                ErrorHandler.ThrowOnFailure(clGetMemObjectInfo(memObject, parameter.ParameterInfo.Name, requiredSize, memory, out actualSize));
                return parameter.ParameterInfo.Deserialize(actualSize, memory);
            }
            finally
            {
                Marshal.FreeHGlobal(memory);
            }
        }

        public static class MemObjectInfo
        {
            public static readonly MemObjectParameterInfo<uint> Type =
                (MemObjectParameterInfo<uint>)new ParameterInfoUInt32(0x1100);
            public static readonly MemObjectParameterInfo<ulong> Flags =
                (MemObjectParameterInfo<ulong>)new ParameterInfoUInt64(0x1101);
            public static readonly MemObjectParameterInfo<UIntPtr> Size =
                (MemObjectParameterInfo<UIntPtr>)new ParameterInfoUIntPtr(0x1102);
            public static readonly MemObjectParameterInfo<IntPtr> HostAddress =
                (MemObjectParameterInfo<IntPtr>)new ParameterInfoIntPtr(0x1103);
            public static readonly MemObjectParameterInfo<uint> MapCount =
                (MemObjectParameterInfo<uint>)new ParameterInfoUInt32(0x1104);
            public static readonly MemObjectParameterInfo<uint> ReferenceCount =
                (MemObjectParameterInfo<uint>)new ParameterInfoUInt32(0x1105);
            public static readonly MemObjectParameterInfo<IntPtr> Context =
                (MemObjectParameterInfo<IntPtr>)new ParameterInfoIntPtr(0x1106);
            public static readonly MemObjectParameterInfo<IntPtr> AssociatedMemObject =
                (MemObjectParameterInfo<IntPtr>)new ParameterInfoIntPtr(0x1107);
            public static readonly MemObjectParameterInfo<UIntPtr> Offset =
                (MemObjectParameterInfo<UIntPtr>)new ParameterInfoUIntPtr(0x1108);
        }

        public sealed class MemObjectParameterInfo<T>
        {
            private readonly ParameterInfo<T> _parameterInfo;

            public MemObjectParameterInfo(ParameterInfo<T> parameterInfo)
            {
                if (parameterInfo == null)
                    throw new ArgumentNullException("parameterInfo");

                _parameterInfo = parameterInfo;
            }

            public static explicit operator MemObjectParameterInfo<T>(ParameterInfo<T> parameterInfo)
            {
                return new MemObjectParameterInfo<T>(parameterInfo);
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
        private static extern ErrorCode clGetImageInfo(ImageSafeHandle image, int paramName, UIntPtr paramValueSize, IntPtr paramValue, out UIntPtr paramValueSizeRet);

        public static T GetImageInfo<T>(ImageSafeHandle image, ImageParameterInfo<T> parameter)
        {
            if (image == null)
                throw new ArgumentNullException("image");
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
                ErrorHandler.ThrowOnFailure(clGetImageInfo(image, parameter.ParameterInfo.Name, UIntPtr.Zero, IntPtr.Zero, out requiredSize));

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
                ErrorHandler.ThrowOnFailure(clGetImageInfo(image, parameter.ParameterInfo.Name, requiredSize, memory, out actualSize));
                return parameter.ParameterInfo.Deserialize(actualSize, memory);
            }
            finally
            {
                Marshal.FreeHGlobal(memory);
            }
        }

        public static class ImageInfo
        {
            public static ImageParameterInfo<IntPtr[]> Format = (ImageParameterInfo<IntPtr[]>)new ParameterInfoIntPtrArray(0x1110);
            public static ImageParameterInfo<UIntPtr> ElementSize = (ImageParameterInfo<UIntPtr>)new ParameterInfoUIntPtr(0x1111);
            public static ImageParameterInfo<UIntPtr> RowPitch = (ImageParameterInfo<UIntPtr>)new ParameterInfoUIntPtr(0x1112);
            public static ImageParameterInfo<UIntPtr> SlicePitch = (ImageParameterInfo<UIntPtr>)new ParameterInfoUIntPtr(0x1113);
            public static ImageParameterInfo<UIntPtr> Width = (ImageParameterInfo<UIntPtr>)new ParameterInfoUIntPtr(0x1114);
            public static ImageParameterInfo<UIntPtr> Height = (ImageParameterInfo<UIntPtr>)new ParameterInfoUIntPtr(0x1115);
            public static ImageParameterInfo<UIntPtr> Depth = (ImageParameterInfo<UIntPtr>)new ParameterInfoUIntPtr(0x1116);
            public static ImageParameterInfo<UIntPtr> ArraySize = (ImageParameterInfo<UIntPtr>)new ParameterInfoUIntPtr(0x1117);
            public static ImageParameterInfo<IntPtr> Buffer = (ImageParameterInfo<IntPtr>)new ParameterInfoIntPtr(0x1118);
            public static ImageParameterInfo<uint> NumMipLevels = (ImageParameterInfo<uint>)new ParameterInfoUInt32(0x1119);
            public static ImageParameterInfo<uint> NumSamples = (ImageParameterInfo<uint>)new ParameterInfoUInt32(0x111A);
        }

        public sealed class ImageParameterInfo<T>
        {
            private readonly ParameterInfo<T> _parameterInfo;

            public ImageParameterInfo(ParameterInfo<T> parameterInfo)
            {
                if (parameterInfo == null)
                    throw new ArgumentNullException("parameterInfo");

                _parameterInfo = parameterInfo;
            }

            public static explicit operator ImageParameterInfo<T>(ParameterInfo<T> parameterInfo)
            {
                return new ImageParameterInfo<T>(parameterInfo);
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
        private static extern ErrorCode clRetainMemObject(MemObjectSafeHandle memObject);

        [DllImport(ExternDll.OpenCL)]
        public static extern ErrorCode clReleaseMemObject(IntPtr memObject);

        [DllImport(ExternDll.OpenCL)]
        private static extern ErrorCode clSetMemObjectDestructorCallback(MemObjectSafeHandle memObject, MemObjectDestructorCallback notify, IntPtr userData);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate void MemObjectDestructorCallback(MemObjectSafeHandle memObject, IntPtr userData);

        #endregion
    }
}
