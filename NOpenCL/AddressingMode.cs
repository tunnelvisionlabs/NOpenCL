// Copyright (c) Tunnel Vision Laboratories, LLC. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace NOpenCL
{
    /// <summary>
    /// Specifies the image addressing-mode i.e. how out-of-range image coordinates are handled.
    /// </summary>
    /// <remarks>
    /// <para>
    /// For 1D and 2D image arrays, the addressing mode applies only to the x and (x, y)
    /// coordinates. The addressing mode for the coordinate which specifies the array index
    /// is always <see cref="ClampToEdge"/>.
    /// </para>
    /// </remarks>
    public enum AddressingMode
    {
        /// <summary>
        /// For this addressing mode the programmer guarantees that the image coordinates
        /// used to sample elements of the image refer to a location inside the image;
        /// otherwise the results are undefined.
        /// </summary>
        None = 0x1130,

        /// <summary>
        /// Out-of-range image coordinates are clamped to the extent.
        /// </summary>
        ClampToEdge = 0x1131,

        /// <summary>
        /// Out-of-range image coordinates will return a border color. This is similar to
        /// the GL_ADDRESS_CLAMP_TO_BORDER addressing mode.
        /// </summary>
        Clamp = 0x1132,

        /// <summary>
        /// Out-of-range image coordinates are wrapped to the valid range. This address mode
        /// can only be used with normalized coordinates. If normalized coordinates are not
        /// used, this addressing mode may generate image coordinates that are undefined.
        /// </summary>
        Repeat = 0x1133,

        /// <summary>
        /// Flip the image coordinate at every integer junction. This addressing mode can
        /// only be used with normalized coordinates. If normalized coordinates are not
        /// used, this addressing mode may generate image coordinates that are undefined.
        /// </summary>
        MirroredRepeat = 0x1134,
    }
}
