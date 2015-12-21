// Copyright (c) Tunnel Vision Laboratories, LLC. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace NOpenCL
{
    public enum DeviceType : ulong
    {
        None = 0,
        Default = 1 << 0,
        Cpu = 1 << 1,
        Gpu = 1 << 2,
        Accelerator = 1 << 3,
        Custom = 1 << 4,
        All = ~0UL,
    }
}
