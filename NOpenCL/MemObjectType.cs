// Copyright (c) Tunnel Vision Laboratories, LLC. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace NOpenCL
{
    /// <summary>
    /// Specifies the type of an OpenCL memory object.
    /// </summary>
    public enum MemObjectType
    {
        /// <summary>
        /// A buffer.
        /// </summary>
        /// <seealso cref="NOpenCL.Buffer"/>
        Buffer = 0x10F0,

        /// <summary>
        /// A 2D image.
        /// </summary>
        /// <seealso cref="NOpenCL.Image"/>
        Image2D = 0x10F1,

        /// <summary>
        /// A 3D image.
        /// </summary>
        /// <seealso cref="NOpenCL.Image"/>
        Image3D = 0x10F2,

        /// <summary>
        /// A 2D image array.
        /// </summary>
        /// <seealso cref="NOpenCL.Image"/>
        Image2DArray = 0x10F3,

        /// <summary>
        /// A 1D image.
        /// </summary>
        /// <seealso cref="NOpenCL.Image"/>
        Image1D = 0x10F4,

        /// <summary>
        /// A 1D image array.
        /// </summary>
        /// <seealso cref="NOpenCL.Image"/>
        Image1DArray = 0x10F5,

        /// <summary>
        /// A 1D image created from a buffer object.
        /// </summary>
        /// <seealso cref="NOpenCL.Image"/>
        Image1DBuffer = 0x10F6,
    }
}
