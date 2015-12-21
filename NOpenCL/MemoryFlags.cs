// Copyright (c) Tunnel Vision Laboratories, LLC. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace NOpenCL
{
    using System;

    /// <summary>
    /// A bit-field that is used to specify allocation and usage information such as the memory
    /// arena that should be used to allocate the buffer object and how it will be used.
    /// </summary>
    [Flags]
    public enum MemoryFlags : ulong
    {
        /// <summary>
        /// No flags are specified.
        /// </summary>
        None = 0,

        /// <summary>
        /// This flag specifies that the memory object will be read and written by a kernel.
        /// </summary>
        ReadWrite = 1 << 0,

        /// <summary>
        /// This flags specifies that the memory object will be written but not read by a kernel.
        /// </summary>
        /// <remarks>
        /// Reading from a buffer or image object created with <see cref="WriteOnly"/> inside a kernel is undefined.
        ///
        /// <para><see cref="ReadWrite"/> and <see cref="WriteOnly"/> are mutually exclusive.</para>
        /// </remarks>
        WriteOnly = 1 << 1,

        /// <summary>
        /// This flag specifies that the memory object is a read-only memory object when used inside a kernel.
        /// </summary>
        /// <remarks>
        /// Writing to a buffer or image object created with <see cref="ReadOnly"/> inside a kernel is undefined.
        ///
        /// <para><see cref="ReadWrite"/> or <see cref="WriteOnly"/> and <see cref="ReadOnly"/> are mutually exclusive.</para>
        /// </remarks>
        ReadOnly = 1 << 2,

        /// <summary>
        /// This flag indicates that the application wants the OpenCL implementation to
        /// use memory referenced by a specific host pointer as the storage bits for the
        /// memory object.
        /// </summary>
        /// <remarks>
        /// OpenCL implementations are allowed to cache the buffer contents pointed
        /// to by the host pointer in device memory. This cached copy can be used
        /// when kernels are executed on a device.
        ///
        /// <para>The result of OpenCL commands that operate on multiple buffer objects
        /// created with the same host pointer or overlapping host regions is considered
        /// to be undefined.</para>
        ///
        /// <para>Refer to the
        /// <a href="http://www.khronos.org/registry/cl/sdk/1.2/docs/man/xhtml/dataTypes.html">description of the alignment rules</a>
        /// for the host pointer for memory objects (buffer and images) created using
        /// <see cref="UseHostPointer"/>.</para>
        /// </remarks>
        UseHostPointer = 1 << 3,

        /// <summary>
        /// This flag specifies that the application wants the OpenCL implementation to
        /// allocate memory from host accessible memory.
        /// </summary>
        /// <remarks>
        /// <see cref="AllocateHostPointer"/> and <see cref="UseHostPointer"/> are
        /// mutually exclusive.
        /// </remarks>
        AllocateHostPointer = 1 << 4,

        /// <summary>
        /// This flag indicates that the application wants the OpenCL implementation to
        /// allocate memory for the memory object and copy the data from memory referenced
        /// by a specified host pointer.
        /// </summary>
        /// <remarks>
        /// <see cref="CopyHostPointer"/> and <see cref="UseHostPointer"/> are mutually exclusive.
        ///
        /// <para><see cref="CopyHostPointer"/> can be used with <see cref="AllocateHostPointer"/>
        /// to initialize the contents of the memory object allocated using host-accessible
        /// (e.g. PCIe) memory.</para>
        /// </remarks>
        CopyHostPointer = 1 << 5,

        ////Reserved = 1 << 6,

        /// <summary>
        /// This flag specifies that the host will only write to the memory object (using
        /// OpenCL APIs that enqueue a write or a map for write). This can be used to
        /// optimize write access from the host (e.g. enable write combined allocations for
        /// memory objects for devices that communicate with the host over a system bus
        /// such as PCIe).
        /// </summary>
        HostWriteOnly = 1 << 7,

        /// <summary>
        /// This flag specifies that the host will only read the memory object (using OpenCL
        /// APIs that enqueue a read or a map for read).
        /// </summary>
        /// <remarks>
        /// <see cref="HostWriteOnly"/> and <see cref="HostReadOnly"/> are mutually exclusive.
        /// </remarks>
        HostReadOnly = 1 << 8,

        /// <summary>
        /// This flag specifies that the host will not read or write the memory object.
        /// </summary>
        /// <remarks>
        /// <see cref="HostWriteOnly"/> or <see cref="HostReadOnly"/> and
        /// <see cref="HostNoAccess"/> are mutually exclusive.
        /// </remarks>
        HostNoAccess = 1 << 9,
    }
}
