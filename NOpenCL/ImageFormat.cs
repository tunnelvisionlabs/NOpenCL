// Copyright (c) Tunnel Vision Laboratories, LLC. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace NOpenCL
{
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    public struct ImageFormat
    {
        public readonly ChannelOrder ChannelOrder;
        public readonly ChannelType ChannelType;

        public ImageFormat(ChannelOrder channelOrder, ChannelType channelType)
        {
            ChannelOrder = channelOrder;
            ChannelType = channelType;
        }
    }
}
