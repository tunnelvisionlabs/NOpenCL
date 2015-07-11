// Copyright (c) Tunnel Vision Laboratories, LLC. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace NOpenCL
{
    /// <summary>
    /// Describes the size of the channel data type. The number of bits per element
    /// determined by the <see cref="ChannelType"/> and <see cref="ChannelOrder"/>
    /// must be a power of two.
    /// </summary>
    public enum ChannelType
    {
        /// <summary>
        /// Each channel component is a normalized signed 8-bit integer value.
        /// </summary>
        SnormInt8 = 0x10D0,

        /// <summary>
        /// Each channel component is a normalized signed 16-bit integer value.
        /// </summary>
        SnormInt16 = 0x10D1,

        /// <summary>
        /// Each channel component is a normalized unsigned 8-bit integer value.
        /// </summary>
        UnormInt8 = 0x10D2,

        /// <summary>
        /// Each channel component is a normalized unsigned 16-bit integer value.
        /// </summary>
        UnormInt16 = 0x10D3,

        /// <summary>
        /// Represents a normalized 5-6-5 3-channel RGB image. The channel order
        /// must be <see cref="ChannelOrder.RGB"/> or <see cref="ChannelOrder.RGBx"/>.
        /// </summary>
        UnormShort565 = 0x10D4,

        /// <summary>
        /// Represents a normalized x-5-5-5 4-channel xRGB image. The channel order
        /// must be <see cref="ChannelOrder.RGB"/> or <see cref="ChannelOrder.RGBx"/>.
        /// </summary>
        UnormShort555 = 0x10D5,

        /// <summary>
        /// Represents a normalized x-10-10-10 4-channel xRGB image. The channel
        /// order must be <see cref="ChannelOrder.RGB"/> or <see cref="ChannelOrder.RGBx"/>.
        /// </summary>
        UnormInt101010 = 0x10D6,

        /// <summary>
        /// Each channel component is an unnormalized signed 8-bit integer value.
        /// </summary>
        SignedInt8 = 0x10D7,

        /// <summary>
        /// Each channel component is an unnormalized signed 16-bit integer value.
        /// </summary>
        SignedInt16 = 0x10D8,

        /// <summary>
        /// Each channel component is an unnormalized signed 32-bit integer value.
        /// </summary>
        SignedInt32 = 0x10D9,

        /// <summary>
        /// Each channel component is an unnormalized unsigned 8-bit integer value.
        /// </summary>
        UnsignedInt8 = 0x10DA,

        /// <summary>
        /// Each channel component is an unnormalized unsigned 16-bit integer value.
        /// </summary>
        UnsignedInt16 = 0x10DB,

        /// <summary>
        /// Each channel component is an unnormalized unsigned 32-bit integer value.
        /// </summary>
        UnsignedInt32 = 0x10DC,

        /// <summary>
        /// Each channel component is a 16-bit half-float value.
        /// </summary>
        HalfFloat = 0x10DD,

        /// <summary>
        /// Each channel component is a single precision floating-point value.
        /// </summary>
        Float = 0x10DE,

        /// <summary>
        /// Each channel component is a normalized unsigned 24-bit integer value.
        /// </summary>
        UnormInt24 = 0x10DF,
    }
}
