/*
 * Copyright (c) 2013 Sam Harwell, Tunnel Vision Laboratories LLC
 * All rights reserved.
 */

namespace NOpenCL
{
    /// <summary>
    /// Describes the type of buffer object to be created by <see cref="Mem.CreateSubBuffer"/>.
    /// </summary>
    public enum BufferCreateType
    {
        /// <summary>
        /// A sub-buffer that represents a specific region in the parent buffer.
        /// </summary>
        Region = 0x1220,
    }
}
