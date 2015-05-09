/*
 * Copyright (c) 2013 Sam Harwell, Tunnel Vision Laboratories LLC
 * All rights reserved.
 */

namespace NOpenCL
{
    /// <summary>
    /// Specifies the type of an OpenCL memory object.
    /// </summary>
    public enum MemObjectType
    {
        /// <summary>
        /// A buffer. (aka MEM_OBJECT_IMAGE1D_BUFFER)
        /// </summary>
        /// <seealso cref="NOpenCL.Mem"/>
        Buffer = 0x10F0,

        /// <summary>
        /// A 2D image. (aka MEM_OBJECT_IMAGE2D )
        /// </summary>
        /// <seealso cref="NOpenCL.Image"/>
        Image2D = 0x10F1,

        /// <summary>
        /// A 3D image. (aka MEM_OBJECT_IMAGE3D )
        /// </summary>
        /// <seealso cref="NOpenCL.Image"/>
        Image3D = 0x10F2,

        /// <summary>
        /// A 2D image array. (aka MEM_OBJECT_IMAGE2D_ARRAY )
        /// </summary>
        /// <seealso cref="NOpenCL.Image"/>
        Image2DArray = 0x10F3,

        /// <summary>
        /// A 1D image. (aka MEM_OBJECT_IMAGE1D )
        /// </summary>
        /// <seealso cref="NOpenCL.Image"/>
        Image1D = 0x10F4,

        /// <summary>
        /// A 1D image array. (aka MEM_OBJECT_IMAGE1D_ARRAY )
        /// </summary>
        /// <seealso cref="NOpenCL.Image"/>
        Image1DArray = 0x10F5,

        /// <summary>
        /// A 1D image created from a buffer object. (aka MEM_OBJECT_IMAGE1D_BUFFER)
        /// </summary>
        /// <seealso cref="NOpenCL.Image"/>
        Image1DBuffer = 0x10F6,
    }
}
