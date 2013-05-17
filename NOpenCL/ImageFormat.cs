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
