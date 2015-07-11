// Copyright (c) Tunnel Vision Laboratories, LLC. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace NOpenCL
{
    /// <summary>
    /// Specifies the channels contained in each image element, as well as their
    /// order.
    /// </summary>
    public enum ChannelOrder
    {
        /// <summary>
        /// Each image element consists of a single alpha channel.
        /// </summary>
        A = 0x10B1,

        /// <summary>
        /// Each image element consists of a single red channel.
        /// </summary>
        R = 0x10B0,

        /// <summary>
        /// Each image element consists of two channels: red, green.
        /// </summary>
        RG = 0x10B2,

        /// <summary>
        /// Each image element consists of two channels: red, alpha.
        /// </summary>
        RA = 0x10B3,

        /// <summary>
        /// Each image element consists of three channels: red, green, blue.
        /// </summary>
        /// <remarks>
        /// This format can only be used with <see cref="ChannelType.UnormShort565"/>,
        /// <see cref="ChannelType.UnormShort555"/>, or <see cref="ChannelType.UnormInt101010"/>.
        /// </remarks>
        RGB = 0x10B4,

        /// <summary>
        /// Each image element consists of four channels: red, green, blue, alpha.
        /// </summary>
        RGBA = 0x10B5,

        /// <summary>
        /// Each image element consists of four channels: blue, green, red, alpha.
        /// </summary>
        BGRA = 0x10B6,

        /// <summary>
        /// Each image element consists of four channels: alpha, red, green, blue.
        /// </summary>
        /// <remarks>
        /// This format can only be used with <see cref="ChannelType.UnormInt8"/>,
        /// <see cref="ChannelType.SnormInt8"/>, <see cref="ChannelType.SignedInt8"/>,
        /// or <see cref="ChannelType.UnsignedInt8"/>.
        /// </remarks>
        ARGB = 0x10B7,

        /// <summary>
        /// Each image element consists of a single intensity channel.
        /// </summary>
        /// <remarks>
        /// This format can only be used with <see cref="ChannelType.UnormInt8"/>,
        /// <see cref="ChannelType.UnormInt16"/>, <see cref="ChannelType.SnormInt8"/>,
        /// <see cref="ChannelType.SnormInt16"/>, <see cref="ChannelType.HalfFloat"/>,
        /// or <see cref="ChannelType.Float"/>.
        /// </remarks>
        Intensity = 0x10B8,

        /// <summary>
        /// Each image element consists of a single luminance channel.
        /// </summary>
        /// <remarks>
        /// This format can only be used with <see cref="ChannelType.UnormInt8"/>,
        /// <see cref="ChannelType.UnormInt16"/>, <see cref="ChannelType.SnormInt8"/>,
        /// <see cref="ChannelType.SnormInt16"/>, <see cref="ChannelType.HalfFloat"/>,
        /// or <see cref="ChannelType.Float"/>.
        /// </remarks>
        Luminance = 0x10B9,

        /// <summary>
        /// Each image element consists of a single red channel.
        /// Similar to <see cref="R"/>, except the alpha value at the border is 0.
        /// </summary>
        Rx = 0x10BA,

        /// <summary>
        /// Each image element consists of two channels: red, green.
        /// Similar to <see cref="RG"/>, except the alpha value at the border is 0.
        /// </summary>
        RGx = 0x10BB,

        /// <summary>
        /// Each image element consists of three channels: red, green, blue.
        /// Similar to <see cref="RGB"/>, except the alpha value at the border is 0.
        /// </summary>
        /// <remarks>
        /// This format can only be used with <see cref="ChannelType.UnormShort565"/>,
        /// <see cref="ChannelType.UnormShort555"/>, or <see cref="ChannelType.UnormInt101010"/>.
        /// </remarks>
        RGBx = 0x10BC,

        /// <summary>
        /// Each image element consists of a single depth channel.
        /// </summary>
        Depth = 0x10BD,

        /// <summary>
        /// Each image element consists of a single depth stencil channel.
        /// </summary>
        DepthStencil = 0x10BE,
    }
}
