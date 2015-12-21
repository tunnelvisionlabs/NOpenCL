// Copyright (c) Tunnel Vision Laboratories, LLC. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace NOpenCL
{
    /// <summary>
    /// Specifies the access qualifier for a kernel argument.
    /// </summary>
    public enum AccessQualifier
    {
        /// <summary>
        /// The access qualifier for arguments which are not an image type.
        /// </summary>
        None = 0x11A3,

        /// <summary>
        /// Specifies that an image argument can only be read by a kernel.
        /// </summary>
        /// <remarks>
        /// <para>This is the default access qualifier if one is not explicitly specified.</para>
        /// </remarks>
        ReadOnly = 0x11A0,

        /// <summary>
        /// Specifies that an image argument can only be written by a kernel.
        /// </summary>
        WriteOnly = 0x11A1,

        /// <summary>
        /// Specifies that an image argument can be read or written by a kernel.
        /// </summary>
        ReadWrite = 0x11A2,
    }
}
