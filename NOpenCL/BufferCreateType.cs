// Copyright (c) Tunnel Vision Laboratories, LLC. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace NOpenCL
{
    /// <summary>
    /// Describes the type of buffer object to be created by <see cref="Buffer.CreateSubBuffer"/>.
    /// </summary>
    public enum BufferCreateType
    {
        /// <summary>
        /// A sub-buffer that represents a specific region in the parent buffer.
        /// </summary>
        Region = 0x1220,
    }
}
